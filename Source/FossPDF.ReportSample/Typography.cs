﻿using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.ReportSample
{
    public static class Typography
    {
        public static TextStyle Title => TextStyle.Default.FontFamily(Fonts.Lato).FontColor(Colors.Blue.Darken3).FontSize(26).Black();
        public static TextStyle Headline => TextStyle.Default.FontFamily(Fonts.Lato).FontColor(Colors.Blue.Medium).FontSize(16).SemiBold();
        public static TextStyle Normal => TextStyle.Default.FontFamily(Fonts.Lato).FontColor(Colors.Black).FontSize(10).LineHeight(1.2f);
    }
}
