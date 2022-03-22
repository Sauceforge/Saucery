using NUnit.Framework;
using Saucery.Util;
using Shouldly;

namespace UnitTests {
    [TestFixture]
    class IDGeneratorTests
    {
        [Test]
        public void SingleRunTest()
        {
            var generator = IDGenerator.Instance;
            generator.ShouldNotBeNull();
            IDGenerator.Id.ShouldNotBeNull();
        }

        [Test]
        public void MultipleRunTest()
        {
            var generator = IDGenerator.Instance;
            generator.ShouldNotBeNull();
            IDGenerator.Id.ShouldNotBeNull();

            var generator2 = IDGenerator.Instance;
            generator2.ShouldNotBeNull();
            IDGenerator.Id.ShouldNotBeNull();
            IDGenerator.Id.ShouldBeEquivalentTo(IDGenerator.Id);
        }
    }
}
