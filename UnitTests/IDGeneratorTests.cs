using NUnit.Framework;
using Saucery.Util;
using Shouldly;

namespace UnitTests
{
    [TestFixture]
    [Order(4)]
    class IDGeneratorTests
    {
        [Test]
        public void SingleRunTest()
        {
            var generator = IDGenerator.Instance;
            generator.ShouldNotBeNull();
            generator.Id.ShouldNotBeNull();
        }

        [Test]
        public void MultipleRunTest()
        {
            var generator = IDGenerator.Instance;
            generator.ShouldNotBeNull();
            generator.Id.ShouldNotBeNull();

            var generator2 = IDGenerator.Instance;
            generator2.ShouldNotBeNull();
            generator2.Id.ShouldNotBeNull();
            generator.Id.ShouldBeEquivalentTo(generator2.Id);
        }
    }
}
