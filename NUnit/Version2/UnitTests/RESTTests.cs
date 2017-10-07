using System;
using NUnit.Framework;
using Shouldly;
using UnitTests.RestAPI.FlowControl;
using UnitTests.RestAPI.RecommendedAppiumVersion;

namespace UnitTests {
    public class RestTests {
        [Test]
        public void FlowControlTest() {
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME, SauceryConstants.MY_USERNAME);
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY, "c7cc7b3c-773a-47a3-93f6-b3d005f13786");
            var flowController = new SauceLabsFlowController();
            flowController.ControlFlow();
        }

        [Test]
        public void AppiumRecommendTest() {
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME, SauceryConstants.MY_USERNAME);
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY, "c7cc7b3c-773a-47a3-93f6-b3d005f13786");
            var statusNotifier = new SauceLabsAppiumRecommender();
            var version = statusNotifier.RecommendAppium();
            var components = version.Split('.');
            components.Length.ShouldBe(3);
            components[0].ShouldBeGreaterThan("0");
            components[1].ShouldBeGreaterThan("3");
            components[2].ShouldBeGreaterThan("13");
        }
    }
}