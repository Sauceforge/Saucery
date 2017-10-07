using System.Runtime.CompilerServices;
using Gallio.Framework;
using MbUnit.Framework;
using Saucery.TestDataSources;
using Saucery.Tests;
using SauceryTester.PageObjects;

namespace SauceryTester.Tests {
    /// <summary>tests for the sauce labs guinea pig page</summary>
    //[Parallelizable(TestScope.All)]
    [Parallelizable]
    public class GuineaPigTests : SauceryBase {
        private const string UrlRoot = "https://saucelabs.com/test/guinea-pig";
        public GuineaPigTests(PlatformTestData platformTestData) : base(platformTestData) {
        }

        #region Tests
        [Test]
        [Parallelizable]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        
        public void PageTitle() {
            var guineaPigPage = new GuineaPigPage(Driver, UrlRoot);
            DiagnosticLog.WriteLine("Title: " + Driver.Title);
            // verify the page title is correct
            Assert.IsTrue(Driver.Title.Contains("I am a page title - Sauce Labs"));
        }

        [Test]
        [Parallelizable]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void LinkWorks() {
            var guineaPigPage = new GuineaPigPage(Driver, UrlRoot);
            //Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            //DiagnosticLog.WriteLine("**PAGE START**" + Driver.PageSource + "**PAGE END**");

            // find and click the link on the page
            guineaPigPage.ClickLink();
             
            // verify the browser was navigated to the correct page
            Assert.IsTrue(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [Test]
        [Parallelizable]
        [Ignore]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void UserAgentPresent() {
            var guineaPigPage = new GuineaPigPage(Driver, UrlRoot);
            //Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");
            Assert.IsNotNull(guineaPigPage.GetUserAgent());
        }

        #endregion
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */