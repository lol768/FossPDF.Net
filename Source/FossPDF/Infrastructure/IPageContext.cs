using System.Collections.Generic;
using FossPDF.Drawing;

namespace FossPDF.Infrastructure
{
    internal class DocumentLocation
    {
        public string Name { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int Length => PageEnd - PageStart + 1;
    }

    internal interface IPageContext
    {
        int CurrentPage { get; }
        void SetSectionPage(string name);
        DocumentLocation? GetLocation(string name);
        DocumentSpecificFontManager FontManager { get; }
    }
}
