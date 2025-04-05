using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FossPDF.Elements;
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Examples.Engine
{
    public enum RenderingTestResult
    {
        Pdf,
        Images,
        Svg
    }
    
    public class RenderingTest
    {
        private string FileNamePrefix = "test";
        private Size Size { get; set; }
        private int? MaxPagesThreshold { get; set; }
        private bool ShowResult { get; set; }
        private bool ApplyCaching { get; set; }
        private bool ApplyDebugging { get; set; }
        private RenderingTestResult ResultType { get; set; } = RenderingTestResult.Images;

        private bool ShowingResultsEnabled = true;
        
        private RenderingTest()
        {
            
        }

        public static RenderingTest Create([CallerMemberName] string fileName = "test")
        {
            return new RenderingTest().FileName(fileName);
        }

        public RenderingTest FileName(string fileName)
        {
            FileNamePrefix = fileName;
            return this;
        }
        
        public RenderingTest PageSize(Size size)
        {
            Size = size;
            return this;
        }
        
        public RenderingTest PageSize(int width, int height)
        {
            return PageSize(new Size(width, height));
        }

        public RenderingTest ProducePdf()
        {
            ResultType = RenderingTestResult.Pdf;
            return this;
        }
        
        public RenderingTest ProduceImages()
        {
            ResultType = RenderingTestResult.Images;
            return this;
        }

        public RenderingTest ProduceSvg()
        {
            ResultType = RenderingTestResult.Svg;
            return this;
        }

        public RenderingTest ShowResults()
        {
            ShowResult = true;
            return this;
        }
        
        public RenderingTest MaxPages(int value)
        {
            MaxPagesThreshold = value;
            return this;
        }
        
        public RenderingTest EnableCaching(bool value = true)
        {
            ApplyCaching = value;
            return this;
        }
        
        public RenderingTest EnableDebugging(bool value = true)
        {
            ApplyDebugging = value;
            return this;
        }
        
        public void Render(Action<IContainer> content)
        {
            RenderDocument(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(Size.Width, Size.Height));
                    page.Content().Container().Background(Colors.White).Element(content);
                });
            });
        }

        public void RenderDocument(Action<IDocumentContainer> content)
        {
            MaxPagesThreshold ??= ResultType == RenderingTestResult.Pdf ? 1000 : 10;
            var document = new SimpleDocument(content, MaxPagesThreshold.Value, ApplyCaching, ApplyDebugging);

            Render(document);
        }
        
        private void Render(IDocument document)
        {
            var fileExplorer = PlatformUtils.PlatformUtils.GetFileExplorerForPlatform();

            if (ResultType == RenderingTestResult.Images)
            {
                Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-${i}.png";
                document.GenerateImages(fileNameSchema);
                
                if (ShowResult && ShowingResultsEnabled)
                    Process.Start(fileExplorer, fileNameSchema(0));
            }

            if (ResultType == RenderingTestResult.Svg)
            {
                Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-{i}.svg";
                document.GenerateSvg(fileNameSchema);

                if (ShowResult && ShowingResultsEnabled)
                    Process.Start(fileExplorer, fileNameSchema(0));
            }

            if (ResultType == RenderingTestResult.Pdf)
            {
                var fileName = $"{FileNamePrefix}.pdf";
                document.GeneratePdf(fileName);
                
                if (ShowResult && ShowingResultsEnabled)
                    Process.Start(fileExplorer, fileName);
            }
        }
    }
}
