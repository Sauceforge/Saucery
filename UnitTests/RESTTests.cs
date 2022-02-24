using NUnit.Framework;
using Saucery.Dojo;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.RestAPI.SupportedPlatforms;
using Saucery.Util;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    [Order(6)]
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

        [Test]
        //[Ignore("Account has no minutes")]
        public void SupportedPlatformTest()
        {
            var platformAcquirer = new SauceLabsPlatformAcquirer();
            var platforms = platformAcquirer.AcquirePlatforms();
            
            platforms.ShouldNotBeNull();

            var configurator = new PlatformConfigurator(platforms);
            var availablePlatforms = configurator.AvailablePlatforms;

            //IOS
            var iosp1 = availablePlatforms[2];
            var allNames = iosp1.Browsers.Select(n => n.DeviceName);
            var distinctNames = iosp1.Browsers.Select(n => n.DeviceName).Distinct();

            var iosp2 = availablePlatforms[6];
            var allNames2 = iosp2.Browsers.Select(n => n.DeviceName);
            var distinctNames2 = iosp2.Browsers.Select(n => n.DeviceName).Distinct();

            //Join Lists
            distinctNames.ToList().AddRange(distinctNames2.ToList());
            var fullDistinct = distinctNames.Distinct();

            //Android
            var androidplatforms = availablePlatforms[5];

            var allAndroidNames = androidplatforms.Browsers.Select(n => n.DeviceName);
            var distinctAndroidNames = androidplatforms.Browsers.Select(n => n.DeviceName).Distinct();
        }
    }
}