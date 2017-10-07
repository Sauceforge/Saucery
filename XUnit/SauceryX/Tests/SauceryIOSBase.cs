using System;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SauceryX.Convertors;
using SauceryX.Driver;
using SauceryX.OnDemand;
using SauceryX.RestAPI.TestStatus;
using SauceryX.Util;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SauceryX.Tests {
    //[TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryIOSBase : SauceryRoot {
        protected SauceryIOSDriver Driver;

        public SauceryIOSBase(SaucePlatform platform) : base(platform) {
        }

        static SauceryIOSBase() {
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        }

        public override void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
            try {
                Driver = new SauceryIOSDriver(new Uri(SauceryConstants.SAUCELABS_HUB), caps);
                Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitSecs));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose() {
            if (Driver != null) {

                var passed =  TestContext.CurrentContext.Result.Status == TestStatus.Passed;
                // log the result to SauceLabs
                SauceLabsStatusNotifier.NotifyStatus(Driver.GetSessionId(), passed);
                PrintSessionDetails();
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