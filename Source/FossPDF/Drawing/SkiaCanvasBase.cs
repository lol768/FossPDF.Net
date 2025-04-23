using System;
using FossPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace FossPDF.Drawing
{
    internal abstract class SkiaCanvasBase : ICanvas, IRenderingCanvas
    {
        internal SKCanvas? Canvas { get; set; }

        public abstract void BeginDocument();
        public abstract void EndDocument();

        public abstract void BeginPage(Size size);
        public abstract void EndPage();

        public void Translate(Position vector)
        {
            Canvas.Translate(vector.X, vector.Y);
        }

        public void DrawRectangle(Position vector, Size size, string color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var paint = color.ColorToPaint();
            Canvas.DrawRect(vector.X, vector.Y, size.Width, size.Height, paint);
        }

        public void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style, DocumentSpecificFontManager fontManager)
        {
            Canvas.DrawText(skTextBlob, position.X, position.Y, fontManager.ToPaint(style));
        }

        public void DrawImage(SKImage image, Position vector, Size size)
        {
            Canvas.DrawImage(image, new SKRect(vector.X, vector.Y, size.Width, size.Height));
        }

        public void DrawHyperlink(string url, Size size)
        {
            Canvas.DrawUrlAnnotation(new SKRect(0, 0, size.Width, size.Height), url).Dispose();
        }

        public void DrawSectionLink(string sectionName, Size size)
        {
            Canvas.DrawLinkDestinationAnnotation(new SKRect(0, 0, size.Width, size.Height), sectionName).Dispose();
        }

        public void DrawSection(string sectionName)
        {
            Canvas.DrawNamedDestinationAnnotation(new SKPoint(0, 0), sectionName).Dispose();
        }

        public void Rotate(float angle)
        {
            Canvas.RotateDegrees(angle);
        }

        public void Scale(float scaleX, float scaleY)
        {
            Canvas.Scale(scaleX, scaleY);
        }
    }
}
