using NUnit.Framework;
using Saucery.Dojo;
using Saucery.RestAPI;
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
            
            platforms.ShouldNotBeNull(); //3317


            //To Keep
            //windows_platforms           (898)
            //mac1010_webdriver_platforms (70)
            //mac1011_webdriver_platforms (151)
            //mac1012_webdriver_platforms (190)
            //mac1013_webdriver_platforms (191)
            //mac1014_webdriver_platforms (189)
            //mac1015_webdriver_platforms (149)
            //mac11_webdriver_platforms   (93)
            //mac12_webdriver_platforms   (93)
            //ios_appium_platforms        (460)   //IOS versions is short_version. All have recommended backend version.
            //android_appium_platforms    (159)
            //TOTAL                       2494


            var windows_platforms = platforms.FindAll(p => p.os.Contains("Windows") && p.automation_backend.Equals("webdriver"));

            var mac1010_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.10") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac1011_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.11") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac1012_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.12") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac1013_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.13") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac1014_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.14") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac1015_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.15") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac11_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 11") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            var mac12_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 12") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));

            var ios_appium_platforms = platforms.FindAll(p => (p.api_name.Equals("iphone") || p.api_name.Equals("ipad")) && p.automation_backend.Equals("appium"));
            var ios_appium_platforms_byversion = ios_appium_platforms.GroupBy(g => g.recommended_backend_version);

            var android_webdriver_platforms = platforms.FindAll(p => p.api_name.Equals("android") && p.automation_backend.Equals("webdriver"));
            var android_webdriver_platforms_byversion = android_webdriver_platforms.GroupBy(g => g.recommended_backend_version);
            var android_appium_platforms = platforms.FindAll(p => p.api_name.Equals("android") && p.automation_backend.Equals("appium"));
            var android_appium_platforms_byversion = android_appium_platforms.GroupBy(g => g.recommended_backend_version);

            var filteredPlatforms = new List<SupportedPlatform>();
            filteredPlatforms.AddRange(windows_platforms);           //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1010_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1011_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1012_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1013_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1014_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac1015_webdriver_platforms); //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac11_webdriver_platforms);   //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(mac12_webdriver_platforms);   //Not filtered for Min and Max Versions
            filteredPlatforms.AddRange(ios_appium_platforms);
            filteredPlatforms.AddRange(android_appium_platforms);

            var configurator = new PlatformConfigurator(filteredPlatforms);
            var availablePlatforms = configurator.AvailablePlatforms;

            //IOS
            //var iosp1 = availablePlatforms[2];
            //var allNames = iosp1.Browsers.Select(n => n.DeviceName);
            //var distinctNames = iosp1.Browsers.Select(n => n.DeviceName).Distinct();

            //var iosp2 = availablePlatforms[6];
            //var allNames2 = iosp2.Browsers.Select(n => n.DeviceName);
            //var distinctNames2 = iosp2.Browsers.Select(n => n.DeviceName).Distinct();

            //Join Lists
            //distinctNames.ToList().AddRange(distinctNames2.ToList());
            //var fullDistinct = distinctNames.Distinct();

            //Android
            //var androidplatforms = availablePlatforms[5];

            //var allAndroidNames = androidplatforms.Browsers.Select(n => n.DeviceName);
            //var distinctAndroidNames = androidplatforms.Browsers.Select(n => n.DeviceName).Distinct();
        }
    }
}