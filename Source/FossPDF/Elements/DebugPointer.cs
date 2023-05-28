using System.ComponentModel;

namespace FossPDF.Elements
{
    internal class DebugPointer : Container
    {
        public string Target { get; set; }
        public bool Highlight { get; set; }
    }
}
