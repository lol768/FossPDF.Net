using System;
using System.IO;
using FossPDF.Drawing.Exceptions;
using FossPDF.Elements;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Fluent
{
    public static class ImageExtensions
    {
        public static void Image(this IContainer parent, byte[] imageData, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(imageData);
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, string filePath, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(filePath);
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, Stream fileStream, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(fileStream);
            parent.Image(image, scaling);
        }

        public static void Image(this IContainer parent, Infrastructure.Image image, ImageScaling scaling = ImageScaling.FitWidth)
        {
            parent.Image(image.SkImage, scaling);
        }
        
        private static void Image(this IContainer parent, SKImage image, ImageScaling scaling = ImageScaling.FitWidth)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");
            
            var imageElement = new Elements.Image
            {
                InternalImage = image
            };

            if (scaling != ImageScaling.Resize)
            {
                var aspectRatio = image.Width / (float)image.Height;
                parent = parent.AspectRatio(aspectRatio, Map(scaling));
            }

            parent.Element(imageElement);

            static AspectRatioOption Map(ImageScaling scaling)
            {
                return scaling switch
                {
                    ImageScaling.FitWidth => AspectRatioOption.FitWidth,
                    ImageScaling.FitHeight => AspectRatioOption.FitHeight,
                    ImageScaling.FitArea => AspectRatioOption.FitArea,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static void Image(this IContainer element, Func<Size, byte[]> imageSource)
        {
            element.Element(new DynamicImage
            {
                Source = imageSource
            });
        }
    }
}
