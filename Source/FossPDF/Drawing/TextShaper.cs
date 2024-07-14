using System;
using System.Linq;
using HarfBuzzSharp;
using FossPDF.Infrastructure;
using SkiaSharp;
using Buffer = HarfBuzzSharp.Buffer;

namespace FossPDF.Drawing
{
    public class TextShaper
    {
        private readonly DocumentSpecificFontManager _fontManager;
        public const int FontShapingScale = 512;

        private TextStyle TextStyle { get; }

        private SKFont Font => _fontManager.ToFont(TextStyle);
        private Font ShaperFont => _fontManager.ToShaperFont(TextStyle);
        private SKPaint Paint => _fontManager.ToPaint(TextStyle);

        public TextShaper(TextStyle textStyle, DocumentSpecificFontManager fontManager)
        {
            _fontManager = fontManager;
            TextStyle = textStyle;
        }

        public TextShapingResult Shape(string text)
        {
            // Console.WriteLine("Hello from Shape! The string is: \""+text+"\"");
            using var buffer = new Buffer();

            PopulateBufferWithText(buffer, text);
            buffer.GuessSegmentProperties();

            if (TextStyle.Direction == TextDirection.LeftToRight)
                buffer.Direction = Direction.LeftToRight;

            if (TextStyle.Direction == TextDirection.RightToLeft)
                buffer.Direction = Direction.RightToLeft;

            ShaperFont.Shape(buffer);

            var length = buffer.Length;
            var glyphInfos = buffer.GlyphInfos;
            var glyphPositions = buffer.GlyphPositions;

            var scaleY = Paint.TextSize / FontShapingScale;
            var scaleX = scaleY * Paint.TextScaleX;

            var xOffset = 0f;
            var yOffset = 0f;

            // used for letter spacing calculation
            var lastCluster = glyphInfos.LastOrDefault().Cluster;
            var letterSpacing = (TextStyle.LetterSpacing ?? 0) * (TextStyle.Size ?? 16);

            var glyphs = new ShapedGlyph[length];

            bool isFirstGlyphCluster = true;

            for (var i = 0; i < length; i++)
            {
                var hasExtents = ShaperFont.TryGetGlyphExtents(glyphInfos[i].Codepoint, out var extents);
                float? lBearing = hasExtents ? scaleX * extents.XBearing : null;
                float? rBearing = hasExtents
                    ? scaleX * (glyphPositions[i].XAdvance - (extents.Width + extents.XBearing))
                    : null;
                var widthAdjustment = 0f;

                if (isFirstGlyphCluster)
                {
                    xOffset -= lBearing ?? 0;
                    widthAdjustment = lBearing ?? 0;
                }

                // letter spacing should be applied between glyph clusters, not between individual glyphs,
                // different cluster id indicates the end of the glyph cluster
                if (lastCluster != glyphInfos[i].Cluster)
                {
                    isFirstGlyphCluster = false;
                    lastCluster = glyphInfos[i].Cluster;
                    xOffset += letterSpacing;
                }

                bool isLastGlyphCluster = glyphInfos[i].Cluster == glyphInfos[length - 1].Cluster;


                if (isLastGlyphCluster)
                {
                    // Console.WriteLine("\t[!] Last cluster with identifier: " + glyphInfos[i].Cluster);
                    widthAdjustment += rBearing ?? 0;
                }

                glyphs[i] = new ShapedGlyph
                {
                    Codepoint = (ushort)glyphInfos[i].Codepoint,
                    Position = new SKPoint(xOffset + glyphPositions[i].XOffset * scaleX,
                        yOffset - glyphPositions[i].YOffset * scaleY),
                    Width = glyphPositions[i].XAdvance * scaleX - widthAdjustment,
                    LBearing = lBearing,
                    RBearing = rBearing,
                };

                // Console.WriteLine(@"		Glyph index: {0}", glyphInfos[i].Codepoint.ToString("X"));
                // Console.WriteLine("\t" + "\tX-offset: {0} (scaled: {1})", glyphPositions[i].XOffset,
                //     glyphPositions[i].XOffset * scaleX);
                // Console.WriteLine("\t" + "\tY-offset: {0} (scaled: {1})", glyphPositions[i].YOffset,
                //     glyphPositions[i].YOffset * scaleY);
                // // isFirstGlyphCluster
                // Console.WriteLine("\t" + "\tIsFirstGlyphCluster: {0}", isFirstGlyphCluster);
                // // isLastGlyphCluster
                // Console.WriteLine("\t" + "\tIsLastGlyphCluster: {0}", isLastGlyphCluster);
                // // xOffset modifier
                // Console.WriteLine("\t" + "\tX-offset modifier [scaled]: {0}", xOffset);
                // Console.WriteLine("\t" + "\tX-advance: {0} (scaled: {1})", glyphPositions[i].XAdvance,
                //     glyphPositions[i].XAdvance * scaleX);
                // Console.WriteLine("\t" + "\tY-advance: {0} (scaled: {1})", glyphPositions[i].YAdvance,
                //     glyphPositions[i].YAdvance * scaleY);
                if (hasExtents)
                {
                    // Console.WriteLine("\t" + "\tExtents height: {0} (scaled: {1})", extents.Height,
                    //     extents.Height * scaleY);
                    // Console.WriteLine("\t" + "\tExtents width: {0} (scaled: {1})", extents.Width,
                    //     extents.Width * scaleX);
                    // // print lBearing and rBearing (both scaled)
                    // Console.WriteLine("\t" + "\tExtents lBearing [scaled] {0}", lBearing);
                    // Console.WriteLine("\t" + "\tExtents rBearing [scaled] {0}", rBearing);
                }

                // Console.WriteLine("\n");

                xOffset += glyphPositions[i].XAdvance * scaleX;
                yOffset += glyphPositions[i].YAdvance * scaleY;
            }

            if (glyphs.Length > 0)
            {
                // normalise all the glyphs so they start at no further to the left than X=0
                var minX = glyphs.Min(g => g.Position.X);
                for (var i = 0; i < glyphs.Length; i++)
                {
                    glyphs[i].Position.X -= minX;
                }
            }

            return new TextShapingResult(buffer.Direction, glyphs, scaleX, scaleY, _fontManager);
        }

        void PopulateBufferWithText(Buffer buffer, string text)
        {
            var encoding = Paint.TextEncoding;

            if (encoding == SKTextEncoding.Utf8)
                buffer.AddUtf8(text);

            else if (encoding == SKTextEncoding.Utf16)
                buffer.AddUtf16(text);

            else if (encoding == SKTextEncoding.Utf32)
                buffer.AddUtf32(text);

            else
                throw new NotSupportedException("TextEncoding of type GlyphId is not supported.");
        }
    }

    public struct ShapedGlyph
    {
        public ushort Codepoint;
        public SKPoint Position;
        public float Width;
        public float? RBearing { get; set; }
        public float? LBearing { get; set; }
    }

    public struct DrawTextCommand
    {
        public SKTextBlob SkTextBlob;
        public float TextOffsetX;
    }

    public class TextShapingResult
    {
        private Direction Direction { get; }
        private ShapedGlyph[] Glyphs { get; }
        public float ScaleX { get; }
        public float ScaleY { get; }

        public int Length => Glyphs.Length;

        private DocumentSpecificFontManager FontManager { get; }

        public ShapedGlyph this[int index] =>
            Direction == Direction.LeftToRight
                ? Glyphs[index]
                : Glyphs[Glyphs.Length - 1 - index];

        public TextShapingResult(Direction direction, ShapedGlyph[] glyphs, float scaleX, float scaleY, DocumentSpecificFontManager fontManager)
        {
            Direction = direction;
            Glyphs = glyphs;
            ScaleX = scaleX;
            ScaleY = scaleY;
            FontManager = fontManager;
        }

        public int BreakText(int startIndex, float maxWidth)
        {
            return Direction switch
            {
                Direction.LeftToRight => BreakTextLeftToRight(),
                Direction.RightToLeft => BreakTextRightToLeft(),
                _ => throw new ArgumentOutOfRangeException()
            };

            int BreakTextLeftToRight()
            {
                // Console.WriteLine("Hello from BreakTextLeftToRight! We're breaking text from index: " + startIndex + " with max width: " + maxWidth);
                var index = startIndex;
                maxWidth += Glyphs[startIndex].Position.X;

                while (index < Glyphs.Length)
                {
                    var glyph = Glyphs[index];
                    var leftAdjust = (index == startIndex ? (glyph.LBearing ?? 0f) : 0f);
                    var rightAdjust = glyph.RBearing ?? 0f; // we won't know if we'll need this or not until we hit the end of the line
                    if (glyph.Position.X + glyph.Width + leftAdjust + rightAdjust >
                        maxWidth + Size.Epsilon)
                        break;

                    index++;
                }

                return index - 1;
            }

            int BreakTextRightToLeft()
            {
                var index = startIndex;

                var startOffset = this[startIndex].Position.X + this[startIndex].Width;

                while (index < Glyphs.Length)
                {
                    var glyph = Glyphs[index];
                    var leftAdjust = glyph.LBearing ?? 0f; // we won't know if we'll need this or not until we hit the end of the line
                    var rightAdjust = (index == Glyphs.Length-1 ? (glyph.RBearing ?? 0f) : 0f);


                    if (startOffset - this[index].Position.X - leftAdjust - rightAdjust > maxWidth + Size.Epsilon)
                        break;

                    index++;
                }

                return index - 1;
            }
        }

        public float MeasureWidth(int startIndex, int endIndex)
        {
            if (Glyphs.Length == 0)
                return 0;

            var start = this[startIndex];
            var end = this[endIndex];


            var adjustX = 0;
            if (start.LBearing != null)
                adjustX -= (int)start.LBearing.Value;

            if (end.RBearing != null)
                adjustX -= (int)(end.RBearing);

            // sum the widths of each glyph
            var sum = 0f;
            for (var i = startIndex; i <= endIndex; i++)
            {
                sum += this[i].Width;
            }

            return sum + adjustX;
        }

        public DrawTextCommand? PositionText(int startIndex, int endIndex, TextStyle textStyle)
        {
            if (Glyphs.Length == 0)
                return null;

            if (startIndex > endIndex)
                return null;

            using var skTextBlobBuilder = new SKTextBlobBuilder();

            var positionedRunBuffer =
                skTextBlobBuilder.AllocatePositionedRun(FontManager.ToFont(textStyle), endIndex - startIndex + 1);
            var glyphSpan = positionedRunBuffer.GetGlyphSpan();
            var positionSpan = positionedRunBuffer.GetPositionSpan();

            for (var sourceIndex = startIndex; sourceIndex <= endIndex; sourceIndex++)
            {
                var runIndex = sourceIndex - startIndex;

                glyphSpan[runIndex] = this[sourceIndex].Codepoint;
                positionSpan[runIndex] = this[sourceIndex].Position;
            }

            var firstVisualCharacterIndex = Direction == Direction.LeftToRight
                ? startIndex
                : endIndex;

            var lastVisualCharacterIndex = Direction == Direction.LeftToRight
                ? endIndex
                : startIndex;

            var offsetAdjustX = this[firstVisualCharacterIndex].LBearing ?? 0;

            // if we're in RTL mode, use the RBearing of the last char instead
            if (Direction == Direction.RightToLeft)
                offsetAdjustX = this[lastVisualCharacterIndex].RBearing ?? 0;

            var cmd = new DrawTextCommand
            {
                SkTextBlob = skTextBlobBuilder.Build(),
                TextOffsetX = -this[firstVisualCharacterIndex].Position.X - offsetAdjustX
            };

            // cmd.TextOffsetX = 0;


            return cmd;
        }
    }
}
