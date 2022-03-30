using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.Dojo;
using Saucery.Driver;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.TestStatus;
using Saucery.Util;
using System;

namespace Saucery.Tests
{
    public class SauceryBase {
        protected string TestName;
        protected SauceryRemoteWebDriver Driver;
        protected readonly BrowserVersion BrowserVersion;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;

        static SauceryBase() {
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        protected SauceryBase(BrowserVersion browserVersion) {
            BrowserVersion = browserVersion;
        }

        public void InitialiseDriver(DriverOptions opts, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, waitSecs);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [SetUp]
        public void Setup() {
            BrowserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
            TestName = BrowserVersion.TestName;

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired options
            var factory = new OptionFactory(BrowserVersion);
            var opts = factory.CreateOptions(TestName);
            InitialiseDriver(opts, 400);
        }

        [TearDown]
        public void Cleanup() {
            try {
                if (Driver != null) {
                    var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                    // log the result to SauceLabs
                    var sessionId = Driver.GetSessionId();
                    SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                    Console.WriteLine(@"SessionID={0} job-name={1}", sessionId, TestName);
                    Driver.Quit();
                }
            } catch (WebDriverException) {
                Console.WriteLine(@"Caught WebDriverException, quitting driver.");
                Driver.Quit();
            }
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */