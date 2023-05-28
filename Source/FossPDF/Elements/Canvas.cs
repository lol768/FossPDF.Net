using FossPDF.Drawing;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using SkiaSharp;

namespace FossPDF.Elements
{
    public delegate void DrawOnCanvas(SKCanvas canvas, Size availableSpace);
    
    internal class Canvas : Element, ICacheable
    {
        public DrawOnCanvas Handler { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
            if (Handler == null || skiaCanvas == null)
                return;

            var originalMatrix = skiaCanvas.TotalMatrix;
            Handler.Invoke(skiaCanvas, availableSpace);
            skiaCanvas.SetMatrix(originalMatrix);
        }
    }
}
