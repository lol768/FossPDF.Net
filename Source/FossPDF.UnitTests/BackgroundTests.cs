using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.Helpers;
using FossPDF.Infrastructure;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class BackgroundTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Background>();
        
        [Test]
        public void Draw_ShouldHandleNullChild()
        {
            TestPlan
                .For(x => new Background
                {
                    Color = Colors.Red.Medium
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), Colors.Red.Medium)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_ShouldHandleChild()
        {
            TestPlan
                .For(x => new Background
                {
                    Color = Colors.Red.Medium,
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), Colors.Red.Medium)
                .ExpectChildDraw(new Size(400, 300))
                .CheckDrawResult();
        }
    }
}
