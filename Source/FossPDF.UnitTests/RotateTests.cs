using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.Infrastructure;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class RotateTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Rotate>();

        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new Rotate
                {
                    Child = x.CreateChild(),
                    Angle = 123
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasRotate(123)
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasRotate(-123)
                .CheckDrawResult();
        } 
    }
}
