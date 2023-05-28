using NUnit.Framework;
using FossPDF.Drawing;
using FossPDF.Elements;
using FossPDF.Infrastructure;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class PageBreakTests
    {
        [Test]
        public void Measure()
        {
            TestPlan
                .For(x => new PageBreak())
                
                .MeasureElement(new Size(400, 300))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero))
                
                .DrawElement(new Size(400, 300))
                .CheckDrawResult()
                
                .MeasureElement(new Size(500, 400))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0));
        }
    }
}
