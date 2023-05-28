using System;
using FossPDF.Elements.Text.Calculation;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Text.Items
{
    internal class TextBlockPageNumber : TextBlockSpan
    {
        public const string PageNumberPlaceholder = "123";
        public Func<IPageContext, string> Source { get; set; } = _ => PageNumberPlaceholder;
        protected override bool EnableTextCache => false;
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            UpdatePageNumberText(request.PageContext);
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            UpdatePageNumberText(request.PageContext);
            base.Draw(request);
        }

        private void UpdatePageNumberText(IPageContext context)
        {
            Text = Source(context) ?? string.Empty;
        }
    }
}
