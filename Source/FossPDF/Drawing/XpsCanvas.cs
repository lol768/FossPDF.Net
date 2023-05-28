using System;
using System.IO;
using FossPDF.Drawing.Exceptions;
using FossPDF.Helpers;
using SkiaSharp;

namespace FossPDF.Drawing
{
    internal class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(CreateXps(stream, documentMetadata))
        {
            
        }
        
        private static SKDocument CreateXps(Stream stream, DocumentMetadata documentMetadata)
        {
            try
            {
                return SKDocument.CreateXps(stream, documentMetadata.RasterDpi);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("XPS", exception);
            }
        }
    }
}
