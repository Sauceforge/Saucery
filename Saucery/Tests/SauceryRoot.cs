using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.RestAPI.TestStatus;
using Saucery.Util;
using System.Collections.Generic;

//[assembly: LevelOfParallelism(3)]

namespace Saucery.Tests
{
    [TestFixture]
    //[Parallelizable(ParallelScope.All)]
    public abstract class SauceryRoot {
        protected string TestName;
        protected readonly BrowserVersion BrowserVersion;
        protected static PlatformConfigurator PlatformConfigurator;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;
        protected static SauceLabsAppiumRecommender SauceLabsAppiumRecommender;

        protected SauceryRoot(BrowserVersion browserVersion) {
            //Console.WriteLine(@"In SauceryRoot constructor");
            BrowserVersion = browserVersion;
        }

        static SauceryRoot() {
            OnceOnlyMessages.TestingOn(JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers));
            OnceOnlyMessages.OnDemand();
            PlatformConfigurator = new PlatformConfigurator();
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        [SetUp]
        public void Setup() {
            //Console.WriteLine("In Setup");
            BrowserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
            TestName = BrowserVersion.TestName;

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired options
            var factory = new OptionFactory(BrowserVersion);
            var opts = factory.CreateOptions(TestName);
            InitialiseDriver(opts, 60);
        }

        public abstract void InitialiseDriver(DriverOptions opts, int waitSecs);
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */