using FossPDF.Drawing;
using SkiaSharp;

namespace FossPDF.Infrastructure
{
    internal interface ICanvas
    {
        void Translate(Position vector);

        void DrawRectangle(Position vector, Size size, string color);
        void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style, DocumentSpecificFontManager fontManager);
        void DrawImage(SKImage image, Position position, Size size);

        void DrawHyperlink(string url, Size size);
        void DrawSectionLink(string sectionName, Size size);
        void DrawSection(string sectionName);

        void Rotate(float angle);
        void Scale(float scaleX, float scaleY);
    }
}
