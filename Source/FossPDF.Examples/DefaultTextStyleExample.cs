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
        private const string SubsetEndpoint = "http://localhost:8080/subset";

        private record SubsetConfiguration(uint[] GlyphIds);
        
        [Test]
        public void DefaultTextStyle()
        {
            var httpClient = new HttpClient();
            FontManager.RegisterSubsetCallback(subsets =>
            {
                FontManager.ClearCacheReadyForSubsets();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var tasks = subsets.Select(fontToBeSubset => Task.Run(async () =>
                {
                    var subset = fontToBeSubset.Glyphs;
                    using var blob = fontToBeSubset.Typeface.OpenStream(out _);
                    var buffer = new byte[blob.Length];
                    blob.Read(buffer, buffer.Length);
                    var configuration = new SubsetConfiguration(subset.ToArray());
                    var httpContent = new MultipartFormDataContent
                    {
                        {new ByteArrayContent(buffer), "file", "font.ttf"},
                        {JsonContent.Create(configuration, options:options), "configuration"}
                    };
                    var response = await httpClient.PostAsync(SubsetEndpoint, httpContent);
                    var subsetBuffer = await response.Content.ReadAsByteArrayAsync();
                    FontManager.RegisterFont(new MemoryStream(subsetBuffer));
                }));
                // wait
                Task.WaitAll(tasks.ToArray());
            });
            
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
                            column.Item().Text("deHll loorw");
                        
                            column.Item().Text(text =>
                            {
                                // text in this block is additionally semibold
                                text.DefaultTextStyle(TextStyle.Default.SemiBold());
        
                                text.Line("012345");
                            });
                        });
                    });
                });
        }
    }
}
