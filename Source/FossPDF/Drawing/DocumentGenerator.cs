using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FossPDF.Drawing.Exceptions;
using FossPDF.Drawing.Proxy;
using FossPDF.Elements;
using FossPDF.Elements.Text;
using FossPDF.Elements.Text.Items;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using HarfBuzzSharp;
using SkiaSharp;

namespace FossPDF.Drawing
{
    static class DocumentGenerator
    {
        static DocumentGenerator()
        {
            NativeDependencyCompatibilityChecker.Test();
        }

        internal static void GeneratePdf(Stream stream, IDocument document)
        {
            CheckIfStreamIsCompatible(stream);

            var metadata = document.GetMetadata();
            var canvas = new PdfCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }

        internal static void GenerateXps(Stream stream, IDocument document)
        {
            CheckIfStreamIsCompatible(stream);

            var metadata = document.GetMetadata();
            var canvas = new XpsCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }

        private static void CheckIfStreamIsCompatible(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("The library requires a Stream object with the 'write' capability available (the CanWrite flag). Please consider using the MemoryStream class.");

            if (!stream.CanSeek)
                throw new ArgumentException("The library requires a Stream object with the 'seek' capability available (the CanSeek flag). Please consider using the MemoryStream class.");
        }

        internal static ICollection<byte[]> GenerateImages(IDocument document)
        {
            var metadata = document.GetMetadata();
            var canvas = new ImageCanvas(metadata);
            RenderDocument(canvas, document);

            return canvas.Images;
        }

        internal static ICollection<string> GenerateSvg(IDocument document)
        {
            using var canvas = new SvgCanvas();
            RenderDocument(canvas, document);

            return canvas.Images;
        }

        internal static ICollection<PreviewerPicture> GeneratePreviewerPictures(IDocument document)
        {
            using var canvas = new SkiaPictureCanvas();
            RenderDocument(canvas, document);
            return canvas.Pictures;
        }

        internal static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            ApplyInheritedAndGlobalTexStyle(content, TextStyle.Default);
            ApplyContentDirection(content, ContentDirection.LeftToRight);

            var debuggingState = Settings.EnableDebugging ? ApplyDebugging(content) : null;

            if (Settings.EnableCaching)
                ApplyCaching(content);

            var documentSpecificFontManager = FontManager.MakeDocumentSpecific();
            documentSpecificFontManager.Tag = document.GetMetadata().Title ?? string.Empty;
            var pageContext = new PageContext
            {
                FontManager = documentSpecificFontManager
            };
            RenderPass(pageContext, new FreeCanvas(), content, debuggingState);

            // interrogate all TextBlockSpans
            var allMyFuckingGlyphs = new Dictionary<Font, HashSet<uint>>();
            var typeFaceToGlyphs = new Dictionary<SKTypeface, HashSet<uint>>();

            content.VisitChildren(el =>
            {
                ProcessElementPrepareForSubsetting(el, allMyFuckingGlyphs, typeFaceToGlyphs, pageContext.FontManager);
            });

            var reqs = typeFaceToGlyphs.Select(entry => new FontToBeSubset { Glyphs = entry.Value, Typeface = entry.Key }).ToList();
            pageContext.FontManager.FireSubsetCallback(reqs);
            RenderPass(pageContext, new FreeCanvas(), content, debuggingState);

            RenderPass(pageContext, canvas, content, debuggingState);
            pageContext.FontManager.DisposeAll();
        }

        private static void ProcessElementPrepareForSubsetting(Element? el, IDictionary<Font, HashSet<uint>> allMyFuckingGlyphs,
            IDictionary<SKTypeface, HashSet<uint>> typeFaceToGlyphs, DocumentSpecificFontManager fontManager)
        {
            if (el is DynamicHost dh)
            {
                // these are rather annoying "black boxes", but we can reach
                // inside them and use the TextBlocks cached on the DynamicHost
                foreach (var textBlock in dh.GetTextBlocks())
                {
                    ProcessElementPrepareForSubsetting(textBlock, allMyFuckingGlyphs, typeFaceToGlyphs, fontManager);
                }
            }
            if (el is not TextBlock tb) return;
            foreach (var textBlockItem in tb.Items)
            {
                if (textBlockItem is not TextBlockSpan span) continue;
                var glyphCodepoints = span.GetGlyphCodepoints();
                if (glyphCodepoints == null) continue;

                var shaperFont = fontManager.ToShaperFont(span.Style);
                var existingList = allMyFuckingGlyphs.TryGetValue(shaperFont, out var glyphList)
                    ? glyphList
                    : new HashSet<uint>();
                foreach (var glyphCodepoint in glyphCodepoints)
                {
                    existingList.Add(glyphCodepoint);
                }

                var existingTfList = typeFaceToGlyphs.TryGetValue(fontManager.ToFont(span.Style).Typeface, out var glyphListTf)
                    ? glyphListTf
                    : new HashSet<uint>();
                foreach (var glyphCodepoint in glyphCodepoints)
                {
                    existingTfList.Add(glyphCodepoint);
                }

                typeFaceToGlyphs[fontManager.ToFont(span.Style).Typeface] = existingTfList;
                span.ClearTextShapingResult();

                allMyFuckingGlyphs[shaperFont] = existingList;
            }
        }

        internal static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DebuggingState? debuggingState)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            InjectDependencies(content, pageContext, canvas);
            content.VisitChildren(x => (x as IStateResettable)?.ResetState());

            canvas.BeginDocument();

            var currentPage = 1;

            while(true)
            {
                pageContext.SetPageNumber(currentPage);
                debuggingState?.Reset();

                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }

                try
                {
                    canvas.BeginPage(spacePlan);
                    content.Draw(spacePlan);
                }
                catch (Exception exception)
                {
                    canvas.EndDocument();
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                canvas.EndPage();

                if (currentPage >= Settings.DocumentLayoutExceptionThreshold)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }

                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;

                currentPage++;
            }

            canvas.EndDocument();

            void ThrowLayoutException()
            {
                var message = $"Composed layout generates infinite document. This may happen in two cases. " +
                              $"1) Your document and its layout configuration is correct but the content takes more than {Settings.DocumentLayoutExceptionThreshold} pages. " +
                              $"In this case, please increase the value {nameof(FossPDF)}.{nameof(Settings)}.{nameof(Settings.DocumentLayoutExceptionThreshold)} static property. " +
                              $"2) The layout configuration of your document is invalid. Some of the elements require more space than is provided." +
                              $"Please analyze your documents structure to detect this element and fix its size constraints.";

                var elementTrace = debuggingState?.BuildTrace() ?? "Debug trace is available only in the DEBUG mode.";

                throw new DocumentLayoutException(message, elementTrace);
            }
        }

        internal static void InjectDependencies(this Element content, IPageContext pageContext, ICanvas canvas)
        {
            content.VisitChildren(x =>
            {
                if (x == null)
                    return;

                x.PageContext = pageContext;
                x.Canvas = canvas;
            });
        }

        private static void ApplyCaching(Container content)
        {
            content.VisitChildren(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        private static DebuggingState ApplyDebugging(Container content)
        {
            var debuggingState = new DebuggingState();

            content.VisitChildren(x =>
            {
                x.CreateProxy(y => new DebuggingProxy(debuggingState, y));
            });

            return debuggingState;
        }

        internal static void ApplyContentDirection(this Element? content, ContentDirection direction)
        {
            if (content == null)
                return;

            if (content is ContentDirectionSetter contentDirectionSetter)
            {
                ApplyContentDirection(contentDirectionSetter.Child, contentDirectionSetter.ContentDirection);
                return;
            }

            if (content is IContentDirectionAware contentDirectionAware)
                contentDirectionAware.ContentDirection = direction;

            foreach (var child in content.GetChildren())
                ApplyContentDirection(child, direction);
        }

        internal static void ApplyInheritedAndGlobalTexStyle(this Element? content, TextStyle documentDefaultTextStyle)
        {
            if (content == null)
                return;

            if (content is TextBlock textBlock)
            {
                foreach (var textBlockItem in textBlock.Items)
                {
                    if (textBlockItem is TextBlockSpan textSpan)
                        textSpan.Style = textSpan.Style.ApplyInheritedStyle(documentDefaultTextStyle).ApplyGlobalStyle();

                    if (textBlockItem is TextBlockElement textElement)
                        ApplyInheritedAndGlobalTexStyle(textElement.Element, documentDefaultTextStyle);
                }

                return;
            }

            if (content is DynamicHost dynamicHost)
                dynamicHost.TextStyle = dynamicHost.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);

            if (content is DefaultTextStyle defaultTextStyleElement)
               documentDefaultTextStyle = defaultTextStyleElement.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);

            foreach (var child in content.GetChildren())
                ApplyInheritedAndGlobalTexStyle(child, documentDefaultTextStyle);
        }

    }
}
