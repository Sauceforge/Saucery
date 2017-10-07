using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SauceryX.DataSources;
using SauceryX.Driver;
using SauceryX.OnDemand;
using SauceryX.RestAPI.TestStatus;
using SauceryX.Util;

namespace SauceryX.Tests {
    //[TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryAndroidBase : SauceryRoot, IDisposable {
        protected SauceryAndroidDriver Driver;

        public SauceryAndroidBase(SaucePlatform platform) : base(platform) {
        }
        
        static SauceryAndroidBase() {
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        }

        public override void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
            try {
                Driver = new SauceryAndroidDriver(new Uri(SauceryConstants.SAUCELABS_HUB), caps);
                Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitSecs));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        //[TearDown]
        public void Dispose() {
            if (Driver != null) {
                //var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                //SauceLabsStatusNotifier.NotifyStatus(Driver.GetSessionId(), passed);
                //PrintSessionDetails();
                Driver.Quit();
            }
        }

        public void PrintSessionDetails() {
            try {
                var sessionId = Driver.GetSessionId();
                Console.WriteLine(@"SauceOnDemandSessionID={0} job-name={1}", sessionId, TestName);
            } catch(WebDriverException) {
                Console.WriteLine(@"Caught WebDriverException, quitting driver.");
                Driver.Quit();
            }
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */