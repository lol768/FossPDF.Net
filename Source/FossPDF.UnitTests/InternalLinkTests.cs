using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class InternalLinkTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<SectionLink>();
        
        // TODO: consider tests for the Draw method
    }
}
