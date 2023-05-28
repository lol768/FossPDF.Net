using System;
using FossPDF.Drawing;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Elements
{
    internal class DynamicImage : Element
    {
        public Func<Size, byte[]>? Source { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var imageData = Source?.Invoke(availableSpace);
            
            if (imageData == null)
                return;

            using var image = SKImage.FromEncodedData(imageData);
            Canvas.DrawImage(image, Position.Zero, availableSpace);
        }
    }
}
