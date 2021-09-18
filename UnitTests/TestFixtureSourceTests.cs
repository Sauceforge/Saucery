using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnitTests.Issue1118;

namespace UnitTests {
    public class TestFixtureSourceTests {
        [Test]
        public void Issue1118() {
            var suite = TestBuilder.MakeFixture(typeof(Issue1118_Fixture));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests.Count, Is.EqualTo(3));
            Assert.That(suite.TestCaseCount, Is.EqualTo(6));

            var suite2 = TestBuilder.MakeFixture(typeof(Issue1118_Fixture2));
            Assert.That(suite2.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite2.Tests.Count, Is.EqualTo(3));
            Assert.That(suite2.TestCaseCount, Is.EqualTo(6));
        }
    }
}