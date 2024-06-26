using FossPDF.Drawing;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

namespace FossPDF.Elements
{
    internal class Empty : Element
    {
        internal static Empty Instance { get; } = new Empty();
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(0, 0);
        }

        internal override void Draw(Size availableSpace)
        {
            
        }
    }
}
