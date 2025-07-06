using System.Collections.Generic;
using HarfBuzzSharp;
using SkiaSharp;

namespace FossPDF.Drawing;

public record FontToBeSubset
{
    public SKTypeface Typeface { get; init; }
    public Font ShaperFont { get; init; }
    public HashSet<uint> Glyphs { get; init; }
}
