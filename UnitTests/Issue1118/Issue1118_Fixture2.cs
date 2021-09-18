using NUnit.Framework;

namespace UnitTests.Issue1118 {
    public class Issue1118_Fixture2 : Issue1118_Base {
        public Issue1118_Fixture2(string browser) : base(browser) {
        }

        [Test]
        //[Ignore("Only needs to exist for Issue1118 Test")]
        public void DoSomethingOnAWebPageWithSelenium() {
        }

        [Test]
        //[Ignore("Only needs to exist for Issue1118 Test")]
        public void DoSomethingElseOnAWebPageWithSelenium() {
        }
    }
}