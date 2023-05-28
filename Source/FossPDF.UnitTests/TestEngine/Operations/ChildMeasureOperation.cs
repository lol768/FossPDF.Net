using FossPDF.Drawing;
using FossPDF.Infrastructure;

namespace FossPDF.UnitTests.TestEngine.Operations
{
    internal class ChildMeasureOperation : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }
        public SpacePlan Output { get; }

        public ChildMeasureOperation(string childId, Size input, SpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Output = output;
        }
    }
}
