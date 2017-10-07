using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Saucery2.Driver;
using Saucery2.Util;

namespace Saucery2.Tests {
    [TestFixture]
    public class SauceryIOSBase : SauceryRoot {
        protected SauceryIOSDriver Driver;

        public override void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
            try {
                Driver = new SauceryIOSDriver(caps);
                Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitSecs));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [TearDown]
        public void Cleanup() {
            if(Driver != null) {
                var passed = TestContext.CurrentContext.Result.Status == TestStatus.Passed;
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