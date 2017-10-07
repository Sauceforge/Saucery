using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Saucery3.OnDemand;
using Saucery3.Tests;
using Saucery3Tester.PageObjects;
using Shouldly;

namespace Saucery3Tester.Tests {
    public class OpenSauceFixture : SauceryBase {
        public OpenSauceFixture(SaucePlatform platform) : base(platform) {
            //Console.WriteLine("In GuineaPigFixture constructor");
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        [TestCase(5)]
        [TestCase(4)]
        //[Ignore("Need OpenSauce")]
        public void DoSomethingOnAWebPageWithSelenium(int data) {
            //Console.WriteLine("In DoSomethingOnAWebPageWithSelenium");
            //Saucery supports NUnit data-driven tests 
            Console.WriteLine(@"My data is: " + data);
            //Console.WriteLine(Driver== null? "Driver is null": "Driver good");
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        //[Ignore("Need OpenSauce")]
        public void DoSomethingElseOnAWebPageWithSelenium() {
            //Console.WriteLine("In DoSomethingElseOnAWebPageWithSelenium");
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
        public void DoSomethingElseAgainOnAWebPageWithSelenium() {
            //Console.WriteLine("In DoSomethingElseAgainOnAWebPageWithSelenium");
            //Console.WriteLine(Driver == null ? "Driver is null" : "Driver good");
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent();

            useragent.ShouldNotBeNull();
        }
    }
}