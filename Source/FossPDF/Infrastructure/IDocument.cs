using FossPDF.Drawing;

namespace FossPDF.Infrastructure
{
    public interface IDocument
    {
        DocumentMetadata GetMetadata();
        void Compose(IDocumentContainer container);
    }
}
