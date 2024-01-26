using System.Collections.Generic;
using SkiaSharp;

namespace FossPDF.Drawing;

public record FontToBeSubset
{
    public SKTypeface Typeface { get; init; }
    public HashSet<uint> Glyphs { get; init; }
}