using NUnit.Framework;
using FossPDF.Drawing;
using FossPDF.Elements;
using FossPDF.Infrastructure;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class ShowEntireTests
    {
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsWrap()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsPartialRender()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenElementReturnsFullRender()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.FullRender(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw() => SimpleContainerTests.Draw<ShowEntire>();
    }
}
