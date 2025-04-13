using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Drawing
{
    public class PreviewerPicture
    {
        public SKPicture Picture { get; set; }
        public Size Size { get; set; }

        public PreviewerPicture(SKPicture picture, Size size)
        {
            Picture = picture;
            Size = size;
        }
    }

    internal class SkiaPictureCanvas : SkiaCanvasBase, IDisposable
    {
        private bool _disposed;

        private SKPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }

        public ICollection<PreviewerPicture> Pictures { get; } = new List<PreviewerPicture>();

        public override void BeginDocument()
        {
            Pictures.Clear();
        }

        public override void BeginPage(Size size)
        {
            CurrentPageSize = size;
            PictureRecorder?.Dispose();
            PictureRecorder = new SKPictureRecorder();
            Canvas?.Dispose();
            Canvas = PictureRecorder.BeginRecording(new SKRect(0, 0, size.Width, size.Height));
        }

        public override void EndPage()
        {
            using var picture = PictureRecorder?.EndRecording();

            if (picture != null && CurrentPageSize.HasValue)
                Pictures.Add(new PreviewerPicture(picture, CurrentPageSize.Value));

            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument()
        {
            Canvas?.Dispose();
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            PictureRecorder?.Dispose();
            _disposed = true;
        }

        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
