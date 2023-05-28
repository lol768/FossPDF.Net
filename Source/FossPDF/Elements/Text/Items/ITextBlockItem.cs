using FossPDF.Elements.Text.Calculation;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Text.Items
{
    internal interface ITextBlockItem
    {
        TextMeasurementResult? Measure(TextMeasurementRequest request);
        void Draw(TextDrawingRequest request);
    }
}
