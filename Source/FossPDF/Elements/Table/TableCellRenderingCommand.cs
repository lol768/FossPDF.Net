using FossPDF.Drawing;
using FossPDF.Infrastructure;

namespace FossPDF.Elements.Table
{
    internal class TableCellRenderingCommand
    {
        public TableCell Cell { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }
}
