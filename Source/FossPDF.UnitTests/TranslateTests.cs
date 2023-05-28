using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.Infrastructure;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class TranslateTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Translate>();
        
        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new Translate
                {
                    Child = x.CreateChild(),
                    TranslateX = 50,
                    TranslateY = 75
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasTranslate(50, 75)
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasTranslate(-50, -75)
                .CheckDrawResult();
        }
    }
}
