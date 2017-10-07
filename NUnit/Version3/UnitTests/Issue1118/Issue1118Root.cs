using NUnit.Framework;

namespace UnitTests.Issue1118 {
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public abstract class Issue1118_Root {
        protected readonly string Browser;

        protected Issue1118_Root(string browser) {
            Browser = browser;
        }

        [SetUp]
        public void Setup() {
        }
    }
}