using System;
using Gallio.Framework;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities;
using Saucery.Driver;
using Saucery.Extensions;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.TestStatus;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Tests {
    //[Parallelizable(TestScope.All)]
    [TestFixture, Factory(typeof(PlatformTestData), "GetPlatforms"), Parallelizable]
    public class SauceryBase {
        protected readonly PlatformTestData PlatformTestData;
        protected SauceryRemoteWebDriver Driver;
        protected string TestName;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;

        static SauceryBase() {
            DiagnosticLog.WriteLine("SauceryBase static called");
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        public SauceryBase(PlatformTestData platformTestData) {
            DiagnosticLog.WriteLine("SauceryBase constructor called");
            PlatformTestData = platformTestData;
        }

        [SetUp]
        //public void Setup(PlatformTestData platform, bool useChromeOnAndroid = false, string nativeApp = null) {
        public void Init() {
            TestName = PlatformTestData.GetTestName(TestContext.CurrentContext.Test.Name);
            DiagnosticLog.WriteLine("TestName: {0}", TestName);

            DebugMessages.PrintPlatformDetails(PlatformTestData);
            // set up the desired capabilities
            //var caps = CapabilityFactory.CreateCapabilities(PlatformTestData, TestName, nativeApp, useChromeOnAndroid);
            var caps = CapabilityFactory.CreateCapabilities(PlatformTestData, TestName);
            InitialiseDriver(caps, 30);
        }

        public void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
            SauceLabsFlowController.ControlFlow();
            try {
                Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), caps);
                DiagnosticLog.WriteLine("Driver just initialised with sessionId " + Driver.GetSessionId() +
                                        " and TestName " + TestContext.CurrentContext.Test.Name);
                var timespan = TimeSpan.FromSeconds(waitSecs);
                Driver.Manage().Timeouts().ImplicitlyWait(timespan);
                //Driver.Manage().Timeouts().SetPageLoadTimeout(timespan);
                //Driver.Manage().Timeouts().SetScriptTimeout(timespan);
            }
            catch (Exception ex) {
                DiagnosticLog.WriteLine(ex.Message);
            }
        }

        [TearDown]
        public void Cleanup() {
            PrintSessionDetails();
            SafelyQuit();
        }

        public void PrintSessionDetails() {
            try {
                if (Driver != null) {
                    var sessionId = Driver.GetSessionId();
                    // log the result to SauceLabs
                    DiagnosticLog.WriteLine("sessionID: " + sessionId);
                    //DiagnosticLog.WriteLine("TestStatus: " + TestContext.CurrentContext.Outcome.Status);
                    //var passed = TestContext.CurrentContext.Outcome.Status == TestStatus.Passed;
                    //DiagnosticLog.WriteLine("passed: " + passed);
                    //SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                    DiagnosticLog.WriteLine(@"SauceOnDemandSessionID={0} job-name={1}", sessionId, TestName);
                }
            }
            catch (WebDriverException) {
                DiagnosticLog.WriteLine(@"Caught WebDriverException, quitting driver.");
                SafelyQuit();
            }
        }

        private void SafelyQuit() {
            if (Driver != null) {
                DiagnosticLog.WriteLine("Quitting Driver for {0}!", TestContext.CurrentContext.Test.Name);
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