﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarfBuzzSharp;
using FossPDF.Fluent;
using FossPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace FossPDF.Drawing
{
    public class DocumentSpecificFontManager
    {
        public string Tag { get; set; }
        public ConcurrentDictionary<string, FontStyleSet> StyleSets { get; init; }
        public ConcurrentDictionary<TextStyle, FontMetrics> FontMetrics { get; init; }
        public ConcurrentDictionary<TextStyle, SKPaint> FontPaints { get; init; }
        public ConcurrentDictionary<string, SKPaint> ColorPaints { get; init; }
        public ConcurrentDictionary<TextStyle, Font> ShaperFonts { get; init; }
        public ConcurrentDictionary<TextStyle, SKFont> Fonts { get; init; }
        public ConcurrentDictionary<TextStyle, TextShaper> TextShapers { get; init; }
        public Action<DocumentSpecificFontManager, IEnumerable<FontToBeSubset>>? SubsetCallback { get; init; }

        public void ClearCacheReadyForSubsets()
        {
            StyleSets.Clear();
            FontMetrics.Clear();
            FontPaints.Clear();
            ShaperFonts.Clear();
            Fonts.Clear();
            TextShapers.Clear();
            RegisterLibraryDefaultFonts();
        }

        private void RegisterFontType(SKData fontData, string? customName = null)
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

        internal void FireSubsetCallback(IEnumerable<FontToBeSubset> input)
        {
            SubsetCallback?.Invoke(this, input);
        }

        public void RegisterFont(Stream stream)
        {
            using var fontData = SKData.Create(stream);
            RegisterFontType(fontData);
        }

        public void RegisterFontFromEmbeddedResource(string pathName)
        {
            using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(pathName);

            if (stream == null)
                throw new ArgumentException(
                    $"Cannot load font file from an embedded resource. Please make sure that the resource is available or the path is correct: {pathName}");

            RegisterFont(stream);
        }

        private void RegisterLibraryDefaultFonts()
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


        internal SKPaint ToPaint(TextStyle style)
        {
            return FontPaints.GetOrAdd(style, Convert);

            SKPaint Convert(TextStyle style)
            {
                var targetTypeface = GetTypeface(style);

                return new SKPaint
                {
                    Color = SKColor.Parse(style.Color),
                    Typeface = targetTypeface,
                    TextSize = (style.Size ?? 12) * GetTextScale(style),
                    IsAntialias = true,
                    TextSkewX = GetTextSkew(style, targetTypeface),
                    FakeBoldText = UseFakeBoldText(style, targetTypeface)
                };
            }

            SKTypeface GetTypeface(TextStyle style)
            {
                var weight = (SKFontStyleWeight)(style.FontWeight ?? FontWeight.Normal);

                // superscript and subscript use slightly bolder font to match visually line thickness
                if (style.FontPosition is FontPosition.Superscript or FontPosition.Subscript)
                {
                    var weightValue = (int)weight;
                    weightValue = Math.Min(weightValue + 100, 1000);

                    weight = (SKFontStyleWeight)(weightValue);
                }

                var slant = (style.IsItalic ?? false) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

                var fontStyle = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

                if (StyleSets.TryGetValue(style.FontFamily, out var fontStyleSet))
                    return fontStyleSet.Match(fontStyle);

                var fontFromDefaultSource = SKFontManager.Default.MatchFamily(style.FontFamily, fontStyle);

                if (fontFromDefaultSource != null)
                    return fontFromDefaultSource;

                var availableFontNames = string.Join(", ", SKFontManager.Default.GetFontFamilies());

                throw new ArgumentException(
                    $"The typeface '{style.FontFamily}' could not be found. " +
                    $"Please consider the following options: " +
                    $"1) install the font on your operating system or execution environment. " +
                    $"2) load a font file specifically for FossPDF usage via the FossPDF.Drawing.FontManager.RegisterFontType(Stream fileContentStream) static method. " +
                    $"Available font family names: [{availableFontNames}]");
            }

            static float GetTextScale(TextStyle style)
            {
                return style.FontPosition switch
                {
                    FontPosition.Normal => 1f,
                    FontPosition.Subscript => 0.625f,
                    FontPosition.Superscript => 0.625f,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            static float GetTextSkew(TextStyle originalTextStyle, SKTypeface targetTypeface)
            {
                // requested italic text but got typeface that is not italic
                var useObliqueText = originalTextStyle.IsItalic == true && !targetTypeface.IsItalic;

                return useObliqueText ? -0.25f : 0;
            }

            static bool UseFakeBoldText(TextStyle originalTextStyle, SKTypeface targetTypeface)
            {
                // requested bold text but got typeface that is not bold
                return originalTextStyle.FontWeight > FontWeight.Medium && !targetTypeface.IsBold;
            }
        }

        internal FontMetrics ToFontMetrics(TextStyle style)
        {
            return FontMetrics.GetOrAdd(style, key =>
            {
                var skiaFontMetrics = ToPaint(key.NormalPosition()).FontMetrics;

                return new FontMetrics
                {
                    Ascent = skiaFontMetrics.Ascent,
                    Descent = skiaFontMetrics.Descent,

                    UnderlineThickness = GetUnderlineThickness(),
                    UnderlinePosition = GetUnderlinePosition(),

                    StrikeoutThickness = GetStrikeoutThickness(),
                    StrikeoutPosition = GetStrikeoutPosition()
                };

                // HACK: On MacOS, certain font metrics are not determined accurately.
                // Provide defaults based on other metrics.

                float GetUnderlineThickness()
                {
                    return skiaFontMetrics.UnderlineThickness ?? (skiaFontMetrics.XHeight * 0.15f);
                }

                float GetUnderlinePosition()
                {
                    return skiaFontMetrics.UnderlinePosition ?? (skiaFontMetrics.XHeight * 0.2f);
                }

                float GetStrikeoutThickness()
                {
                    return skiaFontMetrics.StrikeoutThickness ?? (skiaFontMetrics.XHeight * 0.15f);
                }

                float GetStrikeoutPosition()
                {
                    return skiaFontMetrics.StrikeoutPosition ?? (-skiaFontMetrics.XHeight * 0.6f);
                }
            });
        }

        internal Font ToShaperFont(TextStyle style)
        {
            return ShaperFonts.GetOrAdd(style, key =>
            {
                var typeface = ToPaint(key).Typeface;

                using var harfBuzzBlob = typeface.OpenStream(out var ttcIndex).ToHarfBuzzBlob();

                using var face = new Face(harfBuzzBlob, ttcIndex)
                {
                    Index = ttcIndex,
                    UnitsPerEm = typeface.UnitsPerEm,
                    GlyphCount = typeface.GlyphCount
                };

                var font = new Font(face);
                font.SetScale(TextShaper.FontShapingScale, TextShaper.FontShapingScale);
                font.SetFunctionsOpenType();

                return font;
            });
        }

        internal TextShaper ToTextShaper(TextStyle style)
        {
            return TextShapers.GetOrAdd(style, key => new TextShaper(key, this));
        }

        public SKFont ToFont(TextStyle style)
        {
            return Fonts.GetOrAdd(style, key => ToPaint(key).ToFont());
        }

        public void DisposeAll()
        {
            foreach (var paint in FontPaints.Values)
                paint.Dispose();

            foreach (var font in Fonts.Values)
            {
                font.Typeface.Dispose();
                font.Dispose();
            }
        }
    }
}
