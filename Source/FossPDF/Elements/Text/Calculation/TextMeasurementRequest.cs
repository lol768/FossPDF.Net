﻿using FossPDF.Drawing;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Text.Calculation
{
    internal class TextMeasurementRequest
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }

        public DocumentSpecificFontManager FontManager { get; set; }

        public int StartIndex { get; set; }
        public float AvailableWidth { get; set; }

        public bool IsFirstElementInBlock { get; set; }
        public bool IsFirstElementInLine { get; set; }
    }
}
