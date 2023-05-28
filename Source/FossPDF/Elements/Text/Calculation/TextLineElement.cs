using FossPDF.Elements.Text.Items;

namespace FossPDF.Elements.Text.Calculation
{
    internal class TextLineElement
    {
        public ITextBlockItem Item { get; set; }
        public TextMeasurementResult Measurement { get; set; }
    }
}
