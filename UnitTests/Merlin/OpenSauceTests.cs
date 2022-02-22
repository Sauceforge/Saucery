﻿using System;
using NUnit.Framework;
using Saucery.OnDemand;
using Saucery.Tests;
using Merlin.PageObjects;
using Shouldly;

namespace Merlin {
    public class OpenSauceTests : SauceryBase {
        public OpenSauceTests(SaucePlatform platform) : base(platform) {
        }

        [Test]
        [TestCase(5)]
        [TestCase(4)]
        //[Ignore("Need OpenSauce")]
        public void DataDrivenTitleTest(int data) {
            Console.WriteLine(@"My data is: " + data);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        [Test]
        //[Ignore("Need OpenSauce")]
        public void ClickLinkTest() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            
            // find and click the link on the page
            guineaPigPage.ClickLink(Driver);

            // verify the browser was navigated to the correct page
            Driver.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
        }

        [Test]
        //[Ignore("Need OpenSauce")]
        public void UserAgentTest() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent(Driver);

            useragent.ShouldNotBeNull();
        }
    }
}