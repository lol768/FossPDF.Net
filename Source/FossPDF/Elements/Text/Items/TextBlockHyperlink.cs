using FossPDF.Elements.Text.Calculation;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Text.Items
{
    internal class TextBlockHyperlink : TextBlockSpan
    {
        public string Url { get; set; }
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawHyperlink(Url, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            base.Draw(request);
        }
    }
}
