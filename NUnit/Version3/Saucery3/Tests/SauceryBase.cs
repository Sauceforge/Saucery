using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Saucery3.DataSources;
using Saucery3.Driver;
using Saucery3.OnDemand;
using Saucery3.Util;

namespace Saucery3.Tests {
    [Parallelizable(ParallelScope.Fixtures)]
    [TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryBase : SauceryRoot {
        protected SauceryRemoteWebDriver Driver;

        public SauceryBase(SaucePlatform platform) : base(platform) {
            //Console.WriteLine(@"In SauceryBase constructor");
        }

        public override void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                //Console.WriteLine("About to create Driver");
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), caps);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitSecs);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [TearDown]
        public void Cleanup() {
            if(Driver != null) {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
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