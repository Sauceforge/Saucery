using NUnit.Framework;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.Util;
using Shouldly;

namespace UnitTests
{
    public class RestTests {
        [Test]
        //[Ignore("Need OpenSauce")]
        public void FlowControlTest() {
            var flowController = new SauceLabsFlowController();
            //Console.WriteLine(@"RESTTests: About to call ControlFlow()");
            flowController.ControlFlow();
        }

        [Test]
        //[Ignore("Account has no minutes")]
        public void AppiumRecommendTest() {
            var statusNotifier = new SauceLabsAppiumRecommender();
            var version = statusNotifier.RecommendAppium();
            var components = version.Split(SauceryConstants.DOT);
            components.Length.ShouldBe(3);

            var latestAppiumComponents = SauceryConstants.LATEST_APPIUM_VERSION.Split(SauceryConstants.DOT);
            latestAppiumComponents.Length.ShouldBe(3);

            components[0].ShouldBeGreaterThanOrEqualTo(latestAppiumComponents[0]);
            components[1].ShouldBeGreaterThanOrEqualTo(latestAppiumComponents[1]);
            components[2].ShouldBeGreaterThanOrEqualTo(latestAppiumComponents[2]);
        }
    }
}