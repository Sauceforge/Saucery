using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.Dojo;
using Saucery.Driver;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.RestAPI.TestStatus;
using Saucery.Util;
using System;

namespace Saucery.Tests
{
    public class SauceryBase { //: SauceryRoot {
        protected SauceryRemoteWebDriver Driver;
        protected string TestName;
        protected readonly BrowserVersion BrowserVersion;
        protected static PlatformConfigurator PlatformConfigurator;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;
        protected static SauceLabsAppiumRecommender SauceLabsAppiumRecommender;

        protected SauceryBase(BrowserVersion browserVersion) { //: base(browserVersion) {
            BrowserVersion = browserVersion;
        }

        public void InitialiseDriver(DriverOptions opts, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                //Console.WriteLine("About to create Driver");
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, waitSecs);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [SetUp]
        public void Setup()
        {
            //Console.WriteLine("In Setup");
            BrowserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
            TestName = BrowserVersion.TestName;

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired options
            var factory = new OptionFactory(BrowserVersion);
            var opts = factory.CreateOptions(TestName);
            InitialiseDriver(opts, 180);
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

        //public static void SetRequestedPlatforms(List<SaucePlatform> platforms)
        //{
        //    if (Enviro.SauceOnDemandBrowsers == null)
        //    {
        //        //Not Unit Tests: Should only be executed once.
        //        //Unit Tests: Will not be executed (as the BuiltInCompositor will set it).
        //        var json = JsonConvert.SerializeObject(platforms);
        //        Enviro.SetVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS, json);
        //    }
        //}
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */