using System;
using System.Collections.Generic;
using FossPDF.Drawing;
using FossPDF.Infrastructure;
using FossPDF.Helpers;

namespace FossPDF.Fluent
{
    public class Document : IDocument
    {
        static Document()
        {
            NativeDependencyCompatibilityChecker.Test();
        }
        
        private Action<IDocumentContainer> ContentSource { get; }
        private DocumentMetadata Metadata { get; set; } = DocumentMetadata.Default;

        private Document(Action<IDocumentContainer> contentSource)
        {
            ContentSource = contentSource;
        }
        
        public static Document Create(Action<IDocumentContainer> handler)
        {
            return new Document(handler);
        }

        public Document WithMetadata(DocumentMetadata metadata)
        {
            Metadata = metadata ?? Metadata;
            return this;
        }
        
        #region IDocument

        public DocumentMetadata GetMetadata() => Metadata;
        public void Compose(IDocumentContainer container) => ContentSource(container);

        #endregion
    }
}
