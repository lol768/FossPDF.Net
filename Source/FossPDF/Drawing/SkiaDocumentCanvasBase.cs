﻿using System.IO;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Drawing
{
    internal class SkiaDocumentCanvasBase : SkiaCanvasBase
    {
        private SKDocument? Document { get; }

        protected SkiaDocumentCanvasBase(SKDocument document)
        {
            Document = document;
        }

        ~SkiaDocumentCanvasBase()
        {
            Document?.Dispose();
        }

        public override void BeginDocument()
        {

        }

        public override void EndDocument()
        {
            Canvas?.Dispose();

            Document.Close();
            Document.Dispose();
        }

        public override void BeginPage(Size size)
        {
            Canvas?.Dispose();
            Canvas = Document.BeginPage(size.Width, size.Height);
        }

        public override void EndPage()
        {
            Document.EndPage();
            Canvas?.Dispose();
            Canvas = null;
        }
    }
}
