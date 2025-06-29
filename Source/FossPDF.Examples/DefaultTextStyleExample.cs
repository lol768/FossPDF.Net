using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FossPDF.Drawing;
using NUnit.Framework;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace FossPDF.Examples
{
    public class DefaultTextStyleExample
    {
        [Test]
        public void DefaultTextStyle()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        // all text in this set of pages has size 20
                        page.DefaultTextStyle(TextStyle.Default.Size(20));

                        page.Margin(20);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);

                        page.Content().Column(column =>
                        {
                            column.Item().Text(Placeholders.Sentence());

                            column.Item().Text(text =>
                            {
                                // text in this block is additionally semibold
                                text.DefaultTextStyle(TextStyle.Default.SemiBold());

                                text.Line(Placeholders.Sentence());

                                // this text has size 20 but also semibold and red
                                text.Span(Placeholders.Sentence()).FontColor(Colors.Red.Medium);
                            });
                        });
                    });
                });
        }
    }
}
