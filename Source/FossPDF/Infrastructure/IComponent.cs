using FossPDF.Elements;

namespace FossPDF.Infrastructure
{
    interface ISlot
    {
        
    }

    class Slot : Container, ISlot
    {
        
    }
    
    public interface IComponent
    {
        void Compose(IContainer container);
    }
}
