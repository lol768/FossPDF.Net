using System;
using System.Linq;
using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples
{
    public class PageBackgroundForegroundExample
    {
        [Test]
        public void PageBackgroundForeground()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .MaxPages(100)
                .ShowResults()
                .RenderDocument(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Inch);
                        page.DefaultTextStyle(TextStyle.Default.FontSize(16));
                        page.PageColor(Colors.White);

                        const string transparentBlue = "#662196f3";

                        page.Background()
                            .AlignTop()
                            .ExtendHorizontal()
                            .Height(200)
                            .Background(transparentBlue);
                        
                        page.Foreground()
                            .AlignBottom()
                            .ExtendHorizontal()
                            .Height(250)
                            .Background(transparentBlue);
                        
                        page.Header().Text("Background and foreground").Bold().FontColor(Colors.Blue.Darken2).FontSize(36);
                        
                        page.Content().PaddingVertical(25).Column(column =>
                        {
                            column.Spacing(25);

                            foreach (var i in Enumerable.Range(0, 100))
                                column.Item().Background(Colors.Grey.Lighten2).Height(75);
                        });
                    });
                });
        }
    }
}
