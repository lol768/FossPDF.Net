using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class FooterWithAlternatingAlignment : IDynamicComponent<int>
    {
        public int State { get; set; }
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(element =>
            {
                element
                    .Element(x => context.PageNumber % 2 == 0 ? x.AlignLeft() : x.AlignRight())
                    .Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
            
            return new DynamicComponentComposeResult()
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicPageNumberLeftRightExamples
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .MaxPages(100)
                .ShowResults()
                .ProduceImages()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.PageColor(Colors.White);
                        page.Margin(1, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(18));

                        page.Content().Column(column =>
                        {
                            foreach (var i in Enumerable.Range(0, 50))
                                column.Item().PaddingTop(25).Background(Colors.Grey.Lighten2).Height(50);
                        });
                        
                        page.Footer().Dynamic(new FooterWithAlternatingAlignment());
                    });
                });
        }
    }
}
