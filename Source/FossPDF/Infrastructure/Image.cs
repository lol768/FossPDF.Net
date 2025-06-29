using System;
using System.IO;
using SkiaSharp;

namespace FossPDF.Infrastructure;

public class Image : IDisposable
{
    internal SKImage SkImage { get; }

    private Image(SKImage image)
    {
        SkImage = image;
    }

    public void Dispose()
    {
        SkImage.Dispose();
    }

    public static Image FromBinaryData(byte[] imageBytes)
    {
        var skImage = SKImage.FromEncodedData(imageBytes);
        var createdImage = new Image(skImage);
        return createdImage;
    }

    public static Image FromFile(string filePath)
    {
        var skImage = SKImage.FromEncodedData(filePath);
        var createdImage = new Image(skImage);
        return createdImage;
    }

    public static Image FromStream(Stream stream)
    {
        var skImage = SKImage.FromEncodedData(stream);
        var createdImage = new Image(skImage);
        return createdImage;
    }
}
