using System.Linq;
using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class ScaleToFitExamples
    {
        [Test]
        public void ScaleToFit()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container.Padding(25).Column(column =>
                    {
                        var text = Placeholders.Paragraph();

                        foreach (var i in Enumerable.Range(2, 5))
                        {
                            column
                                .Item()
                                .MinimalBox()
                                .Border(1)
                                .Padding(5)
                                .Width(i * 40)
                                .Height(i * 20)
                                .ScaleToFit()
                                .Text(text);
                        }
                    });
                });
        }
    }
}
