using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Elements
{
    internal class Background : ContainerElement
    {
        public string Color { get; set; } = Colors.Black;
        
        internal override void Draw(Size availableSpace)
        {
            Canvas.DrawRectangle(Position.Zero, availableSpace, Color);
            base.Draw(availableSpace);
        }
    }
}
