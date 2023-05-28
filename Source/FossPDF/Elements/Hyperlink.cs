using FossPDF.Drawing;
using FossPDF.Infrastructure;

namespace FossPDF.Elements
{
    internal class Hyperlink : ContainerElement
    {
        public string Url { get; set; } = "https://www.FossPDF.com";
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;

            Canvas.DrawHyperlink(Url, targetSize);
            base.Draw(availableSpace);
        }
    }
}
