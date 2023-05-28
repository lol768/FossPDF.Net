using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;
using Svg.Skia;

namespace FossPDF.Examples
{
    public class SvgImageExample
    {
        [Test]
        public void BorderRadius()
        {
            RenderingTest
                .Create()
                .PageSize(175, 100)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.Grey.Lighten2)
                        .Padding(25)
                        .Canvas((canvas, space) =>
                        {
                            using var svg = new SKSvg();
                            svg.Load("pdf-icon.svg");
                            
                            canvas.DrawPicture(svg.Picture);
                        });
                });
        }
    }
}
