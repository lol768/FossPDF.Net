using System.Collections.Generic;
using FossPDF.Infrastructure;

namespace FossPDF.Drawing.Proxy
{
    public class DebugStackItem
    {
        public IElement Element { get; internal set; }
        public Size AvailableSpace { get; internal set; }
        public SpacePlan SpacePlan { get; internal set; }

        public ICollection<DebugStackItem> Stack { get; internal set; } = new List<DebugStackItem>();
    }
}
