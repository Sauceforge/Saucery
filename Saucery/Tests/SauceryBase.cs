using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Saucery.DataSources;
using Saucery.Driver;
using Saucery.OnDemand;
using Saucery.Util;

namespace Saucery.Tests {
    //[Parallelizable(ParallelScope.Children)]
    [TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryBase : SauceryRoot {
        protected SauceryRemoteWebDriver Driver;

        public SauceryBase(SaucePlatform platform) : base(platform) {
            
        }

        public override void InitialiseDriver(DriverOptions opts, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                //Console.WriteLine("About to create Driver");
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts.ToCapabilities());
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

        public override void InitialiseDriver(ICapabilities driver, int waitSecs)
        {
            throw new NotImplementedException();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */