using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class StopPaging
    {
        [Test]
        public void Example()
        {
            RenderingTest
                .Create()
                .PageSize(300, 250)
                .ProduceImages()
                .ShowResults()
                .Render(container => 
                {
                    container
                        .Padding(25)
                        .DefaultTextStyle(TextStyle.Default.Size(14))
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .Text(text =>
                                {
                                    text.DefaultTextStyle(TextStyle.Default.SemiBold().Color(Colors.Blue.Medium));
                                    
                                    text.Span("Page ");
                                    text.CurrentPageNumber();
                                });
                            
                            decoration
                                .Content()
                                .Column(column =>
                                {
                                    column.Spacing(25);
                                    column.Item().StopPaging().Text(Placeholders.LoremIpsum());
                                    column.Item().ExtendHorizontal().Height(75).Background(Colors.Grey.Lighten2);
                                });
                        });
                });
        }
    }
}
