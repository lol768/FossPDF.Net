using System.Diagnostics;
using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class MinimalApiExamples
    {
        [Test]
        public void MinimalApi()
        {
            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));
                        
                        page.Header()
                            .Text("Hello PDF!")
                            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
                        
                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Spacing(20);
                                
                                x.Item().Text(Placeholders.LoremIpsum());
                                x.Item().Image(Placeholders.Image(200, 100));
                            });
                        
                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                })
                .GeneratePdf("hello.pdf");

            Process.Start("explorer.exe", "hello.pdf");
        }
    }
}
