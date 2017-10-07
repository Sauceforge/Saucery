using NUnit.Framework;
using Saucery3.OnDemand;
using Saucery3.Tests;
using Saucery3Tester.PageObjects;
using Shouldly;

namespace Saucery3Tester.Tests {
    public class OpenSauceFixture2 : SauceryBase {
        public OpenSauceFixture2(SaucePlatform platform) : base(platform) {
            //Console.WriteLine("In GuineaPigFixture2 constructor");
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        //[Ignore("Need OpenSauce")]
        public void DoSomethingOnAWebPageWithSelenium2() {
            //Console.WriteLine("In DoSomethingOnAWebPageWithSelenium2");
            //Console.WriteLine(Driver == null ? "Driver is null" : "Driver good");
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        //[Ignore("Need OpenSauce")]
        public void DoSomethingElseOnAWebPageWithSelenium2() {
            //Console.WriteLine("In DoSomethingElseOnAWebPageWithSelenium2");
            //Console.WriteLine(Driver == null ? "Driver is null" : "Driver good");
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            // find and click the link on the page
            guineaPigPage.ClickLink();

            // verify the browser was navigated to the correct page
            Driver.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        //[Ignore("Need OpenSauce")]
        public void DoSomethingElseAgainOnAWebPageWithSelenium2() {
            //Console.WriteLine("In DoSomethingElseAgainOnAWebPageWithSelenium2");
            //Console.WriteLine(Driver == null ? "Driver is null" : "Driver good");
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent();

            useragent.ShouldNotBeNull();
        }
    }
}