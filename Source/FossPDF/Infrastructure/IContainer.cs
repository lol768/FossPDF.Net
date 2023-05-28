namespace FossPDF.Infrastructure
{
    public interface IContainer
    {
        IElement? Child { get; set; }
    }
}
