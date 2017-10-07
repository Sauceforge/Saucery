using NUnit.Framework;
using OpenQA.Selenium.Remote;
using Saucery2.Capabilities;
using Saucery2.OnDemand;
using Saucery2.RestAPI.FlowControl;
using Saucery2.RestAPI.RecommendedAppiumVersion;
using Saucery2.RestAPI.TestStatus;

namespace Saucery2.Tests {
    [TestFixture]
    public abstract class SauceryRoot {
        protected string TestName;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;
        protected static SauceLabsAppiumRecommender SauceLabsAppiumRecommender;

        static SauceryRoot() {
            //Console.WriteLine("In SauceryRoot static initialiser");
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        public void Setup(SaucePlatform platform) {
            //Console.WriteLine("In Setup");
            TestName = platform.GetTestName(TestContext.CurrentContext.Test.Name);

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired capabilities
            var caps = CapabilityFactory.CreateCapabilities(platform, TestName);
            InitialiseDriver(caps, 30);
        }

        public abstract void InitialiseDriver(DesiredCapabilities caps, int waitSecs);
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */