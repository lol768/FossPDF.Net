using System.Collections.Generic;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Drawing
{
    internal class ImageCanvas : SkiaCanvasBase
    {
        private DocumentMetadata Metadata { get; }
        private SKSurface Surface { get; set; }

        internal ICollection<byte[]> Images { get; } = new List<byte[]>();

        public ImageCanvas(DocumentMetadata metadata)
        {
            Metadata = metadata;
        }

        public override void BeginDocument()
        {

        }

        public override void EndDocument()
        {
            Canvas?.Dispose();
            Surface?.Dispose();
        }

        public override void BeginPage(Size size)
        {
            var scalingFactor = Metadata.RasterDpi / (float) PageSizes.PointsPerInch;
            var imageInfo = new SKImageInfo((int) (size.Width * scalingFactor), (int) (size.Height * scalingFactor));

            Surface = SKSurface.Create(imageInfo);
            Canvas = Surface.Canvas;

            Canvas.Scale(scalingFactor);
        }

        public override void EndPage()
        {
            Canvas.Save();
            using var skImage = Surface.Snapshot();
            using var skData = skImage.Encode(SKEncodedImageFormat.Png, 100);
            var image = skData.ToArray();
            Images.Add(image);

            Canvas.Dispose();
            Surface.Dispose();
        }
    }
}
