using FossPDF.Infrastructure;

namespace FossPDF.UnitTests.TestEngine.Operations
{
    internal class CanvasTranslateOperation : OperationBase
    {
        public Position Position { get; }

        public CanvasTranslateOperation(Position position)
        {
            Position = position;
        }
    }
}
