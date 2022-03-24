using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.DataSources;
using Saucery.Dojo;
using Saucery.Driver;
using Saucery.OnDemand;
using Saucery.Util;
using System;
using System.Collections.Generic;

namespace Saucery.Tests
{
    [TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryBase : SauceryRoot {
        protected SauceryRemoteWebDriver Driver;

        public SauceryBase(BrowserVersion browserVersion) : base(browserVersion) {
            
        }

        public override void InitialiseDriver(DriverOptions opts, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                //Console.WriteLine("About to create Driver");
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts);
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

        public static void SetRequestedPlatforms(List<SaucePlatform> platforms)
        {
            if (Enviro.SauceOnDemandBrowsers == null)
            {
                //Not Unit Tests: Should only be executed once.
                //Unit Tests: Will not be executed (as the BuiltInCompositor will set it).
                var json = JsonConvert.SerializeObject(platforms);
                Enviro.SetVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS, json);
            }
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */