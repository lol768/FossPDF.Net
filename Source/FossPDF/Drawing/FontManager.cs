using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarfBuzzSharp;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Drawing
{
    public static class FontManager
    {
        private static readonly ConcurrentDictionary<string, FontStyleSet> StyleSets = new();
        private static readonly ConcurrentDictionary<TextStyle, FontMetrics> FontMetrics = new();
        private static readonly ConcurrentDictionary<TextStyle, SKPaint> FontPaints = new();
        private static readonly ConcurrentDictionary<string, SKPaint> ColorPaints = new();
        private static readonly ConcurrentDictionary<TextStyle, Font> ShaperFonts = new();
        private static readonly ConcurrentDictionary<TextStyle, SKFont> Fonts = new();
        private static readonly ConcurrentDictionary<TextStyle, TextShaper> TextShapers = new();
        private static Action<DocumentSpecificFontManager, IEnumerable<FontToBeSubset>>? _subsetCallback;

        static FontManager()
        {
            NativeDependencyCompatibilityChecker.Test();
            RegisterLibraryDefaultFonts();
        }

        public static void RegisterSubsetCallback(Action<DocumentSpecificFontManager, IEnumerable<FontToBeSubset>> callback)
        {
            _subsetCallback = callback;
        }

        private static void RegisterFontType(SKData fontData, string? customName = null)
        {
            foreach (var index in Enumerable.Range(0, 256))
            {
                var typeface = SKTypeface.FromData(fontData, index);

                if (typeface == null)
                    break;

                var typefaceName = customName ?? typeface.FamilyName;

                var fontStyleSet = StyleSets.GetOrAdd(typefaceName, _ => new FontStyleSet());
                fontStyleSet.Add(typeface);
            }
        }

        [Obsolete(
            "Since version 2022.8 this method has been renamed. Please use the RegisterFontWithCustomName method.")]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            RegisterFontWithCustomName(fontName, stream);
        }

        public static void RegisterFontWithCustomName(string fontName, Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
            RegisterFontType(fontData, customName: fontName);
        }

        public static void RegisterFont(Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
        }

        public static void RegisterFontFromEmbeddedResource(string pathName)
        {
            using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(pathName);

            if (stream == null)
                throw new ArgumentException(
                    $"Cannot load font file from an embedded resource. Please make sure that the resource is available or the path is correct: {pathName}");

            RegisterFont(stream);
        }

        private static void RegisterLibraryDefaultFonts()
        {
            var fontFileNames = new[]
            {
                "Lato-Black.ttf",
                "Lato-BlackItalic.ttf",

                "Lato-Bold.ttf",
                "Lato-BoldItalic.ttf",

                "Lato-Regular.ttf",
                "Lato-Italic.ttf",

                "Lato-Light.ttf",
                "Lato-LightItalic.ttf",

                "Lato-Thin.ttf",
                "Lato-ThinItalic.ttf"
            };

            foreach (var fileName in fontFileNames)
            {
                var filePath = $"FossPDF.Resources.DefaultFont.{fileName}";

                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
                RegisterFont(stream);
            }
        }

        internal static SKPaint ColorToPaint(this string color)
        {
            return ColorPaints.GetOrAdd(color, Convert);

            static SKPaint Convert(string color)
            {
                return new SKPaint
                {
                    Color = SKColor.Parse(color),
                    IsAntialias = true
                };
            }
        }

        public static DocumentSpecificFontManager MakeDocumentSpecific()
        {
            // we must clone the dictionaries to avoid sharing the same instances between different documents
            // which might be processed in parallel

            // Note that for a single document, the same font manager instance is used for all pages
            // and the expectation is that this will happen synchronously.
            return new DocumentSpecificFontManager
            {
                StyleSets = new ConcurrentDictionary<string, FontStyleSet>(StyleSets),
                FontMetrics = new ConcurrentDictionary<TextStyle, FontMetrics>(FontMetrics),
                FontPaints = new ConcurrentDictionary<TextStyle, SKPaint>(FontPaints),
                ShaperFonts = new ConcurrentDictionary<TextStyle, Font>(ShaperFonts),
                Fonts = new ConcurrentDictionary<TextStyle, SKFont>(Fonts),
                TextShapers = new ConcurrentDictionary<TextStyle, TextShaper>(TextShapers),
                SubsetCallback = _subsetCallback
            };
        }
    }
}
