using FossPDF.Elements.Text.Calculation;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Text.Items
{
    internal class TextBlockSectionLink : TextBlockSpan
    {
        public string SectionName { get; set; }
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawSectionLink(SectionName, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            base.Draw(request);
        }
    }
}
