using NUnit.Framework;

namespace UnitTests.Issue1118 {
    [TestFixtureSource(typeof (Issue1118_SourceData))]
    public class Issue1118_Base : Issue1118_Root {
        public Issue1118_Base(string browser) : base(browser) {
        }

        [TearDown]
        public void Cleanup() {
        }
    }
}