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
    public class ProgressHeader : IDynamicComponent<int>
    {
        public int State { get; set; }
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(container =>
            {
                var width = context.AvailableSize.Width * context.PageNumber / context.TotalPages;
                
                container
                    .Background(Colors.Blue.Lighten2)
                    .Height(25)
                    .Width(width)
                    .Background(Colors.Blue.Darken1);
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicProgressHeader
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .ShowResults()
                .MaxPages(100)
                .ProduceImages()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        page.Header().Dynamic(new ProgressHeader());
                        
                        page.Content().Column(column =>
                        {
                            foreach (var i in Enumerable.Range(0, 100))
                                column.Item().PaddingTop(25).Background(Colors.Grey.Lighten2).Height(50);
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(20));
                            
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}
