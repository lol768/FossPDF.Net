using System;
using System.IO;
using NUnit.Framework;
using FossPDF.Drawing.Exceptions;
using FossPDF.Examples.Engine;
using FossPDF.Fluent;
using FossPDF.Helpers;

namespace FossPDF.Examples
{
    public class ImageExamples
    {
        [Test]
        public void LoadingImage()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ProducePdf()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(25).Column(column =>
                    {
                        column.Spacing(25);
                        
                        column.Item().Image("logo.png");

                        var binaryData = File.ReadAllBytes("logo.png");
                        column.Item().Image(binaryData);
                        
                        using var stream = new FileStream("logo.png", FileMode.Open);
                        column.Item().Image(stream);
                    });
                });
        }
        
        [Test]
        public void DynamicImage()
        {
            RenderingTest
                .Create()
                .PageSize(450, 350)
                .ProducePdf()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(25)
                        .Image(Placeholders.Image);
                });
        }
        
        [Test]
        public void Exception()
        {
            Assert.Throws<DocumentComposeException>(() =>
            {
                RenderingTest
                    .Create()
                    .PageSize(PageSizes.A2)
                    .ProducePdf()
                    .ShowResults()
                    .Render(page => page.Image("non_existent.png"));
            });
        }
        
        [Test]
        public void ReusingTheSameImageFileShouldBePossible()
        {
            var fileName = Path.GetTempFileName() + ".jpg";
            
            try
            {
                var image = Placeholders.Image(300, 100);
                
                using var file = File.Create(fileName);
                file.Write(image);
                file.Dispose();
                
                RenderingTest
                    .Create()
                    .ProducePdf()
                    .PageSize(PageSizes.A4)
                    .ShowResults()
                    .Render(container =>
                    {
                        container
                            .Padding(20)
                            .Column(column =>
                            {
                                column.Spacing(20);
                                
                                column.Item().Image(fileName);
                                column.Item().Image(fileName);
                                column.Item().Image(fileName);
                            });
                    });
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
