﻿using NUnit.Framework;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.Util;
using Shouldly;
using System;

namespace UnitTests
{
    public class RestTests {
        static RestTests() {
            //Console.WriteLine(@"In RestTests static");
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME, SauceryConstants.MY_USERNAME_LOWER);
            Environment.SetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY, "POPULATEME");
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
            components[1].ShouldBeGreaterThanOrEqualTo("22");
            components[2].ShouldBeGreaterThanOrEqualTo("0");
        }
    }
}