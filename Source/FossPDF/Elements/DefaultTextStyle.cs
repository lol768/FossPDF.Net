﻿using FossPDF.Drawing;
using FossPDF.Infrastructure;

namespace FossPDF.Elements
{
    internal class DefaultTextStyle : ContainerElement
    {
        public TextStyle TextStyle { get; set; } = TextStyle.Default;
    }
}
