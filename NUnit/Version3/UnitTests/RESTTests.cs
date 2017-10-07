using System;
using NuGet;
using NUnit.Framework;
using Shouldly;
using UnitTests.RestAPI.FlowControl;
using UnitTests.RestAPI.RecommendedAppiumVersion;

namespace UnitTests {
    public class RestTests {
        static RestTests() {
            //Console.WriteLine(@"In RestTests static");
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME, SauceryConstants.MY_USERNAME_LOWER);
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY, "b5f9d6c8-2ec8-4ea5-a593-74db0184f3b5");
        }

        [Test]
        [Ignore("Need OpenSauce")]
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
            var components = version.Split('.');
            components.Length.ShouldBe(3);
            components[0].ShouldBeGreaterThanOrEqualTo("1");
            components[1].ShouldBeGreaterThanOrEqualTo("6");
            components[2].ShouldBeGreaterThanOrEqualTo("0");
        }
    }
}