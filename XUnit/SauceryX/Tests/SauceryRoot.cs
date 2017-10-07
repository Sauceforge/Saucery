using OpenQA.Selenium.Remote;
using SauceryX.Activation;
using SauceryX.Capabilities;
using SauceryX.OnDemand;
using SauceryX.RestAPI.FlowControl;
using SauceryX.RestAPI.TestStatus;
using Xunit;

namespace SauceryX.Tests {
    //0[TestFixture]
    public abstract class SauceryRoot : IClassFixture<SauceryFixture> {
        protected string TestName;
        protected readonly SaucePlatform Platform;
        protected static SauceLabsStatusNotifier SauceLabsStatusNotifier;
        internal static SauceLabsFlowController SauceLabsFlowController;

        protected SauceryFixture Fixture;

        protected SauceryRoot(SaucePlatform platform) {
            Platform = platform;

            var validator = new ActivationValidator();
            validator.CheckActivation();
            //TestName = Platform.GetTestName(TestContext.CurrentContext.Test.Name);
            TestName = "XUnit doesn't support getting testName yet";

            //DebugMessages.PrintPlatformDetails(platform);
            // set up the desired capabilities
            var caps = CapabilityFactory.CreateCapabilities(Platform, TestName);
            InitialiseDriver(caps, 30);
        }
        //protected SauceryRoot() {
        //    var platforms = Enviro.SauceOnDemandBrowsers;
        //}

        static SauceryRoot() {
            SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
            SauceLabsFlowController = new SauceLabsFlowController();
        }

        public abstract void InitialiseDriver(DesiredCapabilities caps, int waitSecs);
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */