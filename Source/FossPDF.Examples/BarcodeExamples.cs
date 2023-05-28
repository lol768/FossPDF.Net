using System.IO;
using NUnit.Framework;
using FossPDF.Drawing;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class BarcodeExamples
    {
        [Test]
        public void Example()
        {
            FontManager.RegisterFont(File.OpenRead("LibreBarcode39-Regular.ttf"));
            
            RenderingTest
                .Create()
                .PageSize(400, 200)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("*FossPDF*")
                        .FontFamily("Libre Barcode 39")
                        .FontSize(64);
                });
        }
    }
}
