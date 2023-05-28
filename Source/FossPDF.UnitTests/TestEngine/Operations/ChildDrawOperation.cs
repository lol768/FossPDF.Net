using FossPDF.Infrastructure;

namespace FossPDF.UnitTests.TestEngine.Operations
{
    public class ChildDrawOperation : OperationBase
    {
        public string ChildId { get; }
        public Size Input { get; }

        public ChildDrawOperation(string childId, Size input)
        {
            ChildId = childId;
            Input = input;
        }
    }
}
