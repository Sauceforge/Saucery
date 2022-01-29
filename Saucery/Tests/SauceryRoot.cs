using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.RestAPI.TestStatus;
using Saucery.Util;
using System;
using System.Collections.Generic;

namespace Saucery.Tests
{
    [TestFixture]
    //[Parallelizable(ParallelScope.Fixtures)]
    public abstract class SauceryRoot {
        protected string TestName;
        protected readonly SaucePlatform Platform;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;
        protected static SauceLabsAppiumRecommender SauceLabsAppiumRecommender;


        protected SauceryRoot(SaucePlatform platform) {
            //Console.WriteLine(@"In SauceryRoot constructor");
            Platform = platform;
        }

        static SauceryRoot() {
            OnceOnlyMessages.TestingOn(JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers));
            OnceOnlyMessages.OnDemand();
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        [SetUp]
        public void Setup() {
            //Console.WriteLine("In Setup");
            Platform.SetTestName(TestContext.CurrentContext.Test.Name);
            TestName = Platform.TestName;

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired capabilities
            var factory = new OptionFactory(Platform);
            if (factory.IsSupportedPlatform())
            {
                var opts = factory.CreateOptions(TestName);
                InitialiseDriver(opts, 60);
            }
            else
            {
                Console.WriteLine(SauceryConstants.NOT_SUPPORTED_MESSAGE);
            }
        }

        public abstract void InitialiseDriver(DriverOptions opts, int waitSecs);

        public abstract void InitialiseDriver(ICapabilities driver, int waitSecs);
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */