using NUnit.Framework;
using FossPDF.Elements;
using FossPDF.UnitTests.TestEngine;

namespace FossPDF.UnitTests
{
    [TestFixture]
    public class ExternalLinkTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Hyperlink>();
        
        // TODO: consider tests for the Draw method
    }
}
