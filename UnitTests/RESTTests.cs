using NUnit.Framework;
using Saucery.Dojo;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.RestAPI.FlowControl;
using Saucery.RestAPI.RecommendedAppiumVersion;
using Saucery.Util;
using Shouldly;
using System.Collections.Generic;

namespace UnitTests {
    [TestFixture]
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
            //var windows10platforms = platforms.FindAll(w => w.os.Equals(SauceryConstants.PLATFORM_WINDOWS_10));
            //var windows10platformbrowsers = windows10platforms.GroupBy(w => w.api_name);

            var configurator = new PlatformConfigurator();
            var availablePlatforms = configurator.AvailablePlatforms;

            availablePlatforms.ShouldNotBeNull();

            //var browsers = availablePlatforms.SelectMany(i => i.Browsers).Distinct().ToList();
            //var iosBrowsers = browsers.FindAll(x => x.DeviceName.Equals("iPhone XS Max Simulator") || x.DeviceName.Equals("iPhone 5s Simulator")).OrderBy(o=>o.DeviceName).ThenBy(o=>o.PlatformVersion);

            //Reflection Tests
            var windows11platform = availablePlatforms.GetPlatform<Windows11Platform>();
            var windows10platform = availablePlatforms.GetPlatform<Windows10Platform>();
            var windows81platform = availablePlatforms.GetPlatform<Windows81Platform>();
            var windows8platform = availablePlatforms.GetPlatform<Windows8Platform>();
            var windows7platform = availablePlatforms.GetPlatform<Windows7Platform>();

            var mac1010platform = availablePlatforms.GetPlatform<Mac1010Platform>();
            var mac1011platform = availablePlatforms.GetPlatform<Mac1011Platform>();
            var mac1012platform = availablePlatforms.GetPlatform<Mac1012Platform>();
            var mac1013platform = availablePlatforms.GetPlatform<Mac1013Platform>();
            var mac1014platform = availablePlatforms.GetPlatform<Mac1014Platform>();
            var mac1015platform = availablePlatforms.GetPlatform<Mac1015Platform>();
            var mac12platform = availablePlatforms.GetPlatform<Mac12Platform>();
            var mac11platform = availablePlatforms.GetPlatform<Mac11Platform>();

            var ios103platform = availablePlatforms.GetPlatform<IOS103Platform>();
            var ios111platform = availablePlatforms.GetPlatform<IOS111Platform>();
            var ios112platform = availablePlatforms.GetPlatform<IOS112Platform>();
            var ios113platform = availablePlatforms.GetPlatform<IOS113Platform>();
            var ios11platform = availablePlatforms.GetPlatform<IOS11Platform>();
            var ios122platform = availablePlatforms.GetPlatform<IOS122Platform>();
            var ios124platform = availablePlatforms.GetPlatform<IOS124Platform>();
            var ios12platform = availablePlatforms.GetPlatform<IOS12Platform>();
            var ios132platform = availablePlatforms.GetPlatform<IOS132Platform>();
            var ios134platform = availablePlatforms.GetPlatform<IOS134Platform>();
            var ios13platform = availablePlatforms.GetPlatform<IOS13Platform>();
            var ios143platform = availablePlatforms.GetPlatform<IOS143Platform>();
            var ios144platform = availablePlatforms.GetPlatform<IOS144Platform>();
            var ios145platform = availablePlatforms.GetPlatform<IOS145Platform>();
            var ios14platform = availablePlatforms.GetPlatform<IOS14Platform>();
            var ios15platform = availablePlatforms.GetPlatform<IOS15Platform>();
            var ios152platform = availablePlatforms.GetPlatform<IOS152Platform>();

            var android12platform = availablePlatforms.GetPlatform<Android12Platform>();
            var android11platform = availablePlatforms.GetPlatform<Android11Platform>();
            var android10platform = availablePlatforms.GetPlatform<Android10Platform>();
            var android9platform = availablePlatforms.GetPlatform<Android9Platform>();
            var android81platform = availablePlatforms.GetPlatform<Android81Platform>();
            var android8platform = availablePlatforms.GetPlatform<Android8Platform>();
            var android71platform = availablePlatforms.GetPlatform<Android71Platform>();
            var android7platform = availablePlatforms.GetPlatform<Android7Platform>();
            var android6platform = availablePlatforms.GetPlatform<Android6Platform>();
            var android51platform = availablePlatforms.GetPlatform<Android51Platform>();

            //Null Checks
            windows11platform.ShouldNotBeNull();
            windows10platform.ShouldNotBeNull();
            windows81platform.ShouldNotBeNull();
            windows8platform.ShouldNotBeNull();
            windows7platform.ShouldNotBeNull();
            mac1010platform.ShouldNotBeNull();
            mac1011platform.ShouldNotBeNull();
            mac1012platform.ShouldNotBeNull();
            mac1013platform.ShouldNotBeNull();
            mac1014platform.ShouldNotBeNull();
            mac1015platform.ShouldNotBeNull();
            mac11platform.ShouldNotBeNull();
            mac12platform.ShouldNotBeNull();

            ios103platform.ShouldNotBeNull();
            ios111platform.ShouldNotBeNull();
            ios112platform.ShouldNotBeNull();
            ios113platform.ShouldNotBeNull();
            ios11platform.ShouldNotBeNull();
            ios122platform.ShouldNotBeNull();
            ios124platform.ShouldNotBeNull();
            ios12platform.ShouldNotBeNull();
            ios132platform.ShouldNotBeNull();
            ios134platform.ShouldNotBeNull();
            ios13platform.ShouldNotBeNull();
            ios143platform.ShouldNotBeNull();
            ios144platform.ShouldNotBeNull();
            ios145platform.ShouldNotBeNull();
            ios14platform.ShouldNotBeNull();
            ios15platform.ShouldNotBeNull();
            ios152platform.ShouldNotBeNull();

            android12platform.ShouldNotBeNull();
            android11platform.ShouldNotBeNull();
            android10platform.ShouldNotBeNull();
            android9platform.ShouldNotBeNull();
            android81platform.ShouldNotBeNull();
            android8platform.ShouldNotBeNull();
            android71platform.ShouldNotBeNull();
            android7platform.ShouldNotBeNull();
            android6platform.ShouldNotBeNull();
            android51platform.ShouldNotBeNull();

            //Count Checks
            windows11platform.Count.ShouldBe(1);
            windows10platform.Count.ShouldBe(1);
            windows81platform.Count.ShouldBe(1);
            windows8platform.Count.ShouldBe(1);
            windows7platform.Count.ShouldBe(1);
            mac1010platform.Count.ShouldBe(1);
            mac1011platform.Count.ShouldBe(1);
            mac1012platform.Count.ShouldBe(1);
            mac1013platform.Count.ShouldBe(1);
            mac1014platform.Count.ShouldBe(1);
            mac1015platform.Count.ShouldBe(1);
            mac11platform.Count.ShouldBe(1);
            mac12platform.Count.ShouldBe(1);

            ios103platform.Count.ShouldBe(1);
            ios111platform.Count.ShouldBe(1);
            ios112platform.Count.ShouldBe(1);
            ios113platform.Count.ShouldBe(1);
            ios11platform.Count.ShouldBe(1);
            ios122platform.Count.ShouldBe(1);
            ios124platform.Count.ShouldBe(1);
            ios12platform.Count.ShouldBe(1);
            ios132platform.Count.ShouldBe(1);
            ios134platform.Count.ShouldBe(1);
            ios13platform.Count.ShouldBe(1);
            ios143platform.Count.ShouldBe(1);
            ios144platform.Count.ShouldBe(1);
            ios145platform.Count.ShouldBe(1);
            ios14platform.Count.ShouldBe(1);
            ios15platform.Count.ShouldBe(1);
            ios152platform.Count.ShouldBe(1);

            android12platform.Count.ShouldBe(1);
            android11platform.Count.ShouldBe(1);
            android10platform.Count.ShouldBe(1);
            android9platform.Count.ShouldBe(1);
            android81platform.Count.ShouldBe(1);
            android8platform.Count.ShouldBe(1);
            android71platform.Count.ShouldBe(1);
            android7platform.Count.ShouldBe(1);
            android6platform.Count.ShouldBe(1);
            android51platform.Count.ShouldBe(1);

            //Browser Count Checks
            windows11platform[0].Browsers.Count.ShouldBeEquivalentTo(windows11platform[0].BrowserNames.Count);
            windows10platform[0].Browsers.Count.ShouldBeEquivalentTo(windows10platform[0].BrowserNames.Count);
            windows81platform[0].Browsers.Count.ShouldBeEquivalentTo(windows81platform[0].BrowserNames.Count);
            windows8platform[0].Browsers.Count.ShouldBeEquivalentTo(windows8platform[0].BrowserNames.Count);
            windows7platform[0].Browsers.Count.ShouldBeEquivalentTo(windows7platform[0].BrowserNames.Count);
            mac1010platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1010platform[0].BrowserNames.Count);
            mac1011platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1011platform[0].BrowserNames.Count);
            mac1012platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1012platform[0].BrowserNames.Count);
            mac1013platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1013platform[0].BrowserNames.Count);
            mac1014platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1014platform[0].BrowserNames.Count);
            mac1015platform[0].Browsers.Count.ShouldBeEquivalentTo(mac1015platform[0].BrowserNames.Count);
            mac11platform[0].Browsers.Count.ShouldBeEquivalentTo(mac11platform[0].BrowserNames.Count);
            mac12platform[0].Browsers.Count.ShouldBeEquivalentTo(mac12platform[0].BrowserNames.Count);

            //TypeOf Checks
            windows11platform.ShouldBeAssignableTo(typeof(List<Windows11Platform>));
            windows11platform.ShouldBeAssignableTo(typeof(List<Windows11Platform>));
            windows10platform.ShouldBeAssignableTo(typeof(List<Windows10Platform>));
            windows81platform.ShouldBeAssignableTo(typeof(List<Windows81Platform>));
            windows8platform.ShouldBeAssignableTo(typeof(List<Windows8Platform>));
            windows7platform.ShouldBeAssignableTo(typeof(List<Windows7Platform>));
            mac1010platform.ShouldBeAssignableTo(typeof(List<Mac1010Platform>));
            mac1011platform.ShouldBeAssignableTo(typeof(List<Mac1011Platform>));
            mac1012platform.ShouldBeAssignableTo(typeof(List<Mac1012Platform>));
            mac1013platform.ShouldBeAssignableTo(typeof(List<Mac1013Platform>));
            mac1014platform.ShouldBeAssignableTo(typeof(List<Mac1014Platform>));
            mac1015platform.ShouldBeAssignableTo(typeof(List<Mac1015Platform>));
            mac11platform.ShouldBeAssignableTo(typeof(List<Mac11Platform>));
            mac12platform.ShouldBeAssignableTo(typeof(List<Mac12Platform>));

            ios103platform.ShouldBeAssignableTo(typeof(List<IOS103Platform>));
            ios111platform.ShouldBeAssignableTo(typeof(List<IOS111Platform>));
            ios112platform.ShouldBeAssignableTo(typeof(List<IOS112Platform>));
            ios113platform.ShouldBeAssignableTo(typeof(List<IOS113Platform>));
            ios11platform.ShouldBeAssignableTo(typeof(List<IOS11Platform>));
            ios122platform.ShouldBeAssignableTo(typeof(List<IOS122Platform>));
            ios124platform.ShouldBeAssignableTo(typeof(List<IOS124Platform>));
            ios12platform.ShouldBeAssignableTo(typeof(List<IOS12Platform>));
            ios132platform.ShouldBeAssignableTo(typeof(List<IOS132Platform>));
            ios134platform.ShouldBeAssignableTo(typeof(List<IOS134Platform>));
            ios13platform.ShouldBeAssignableTo(typeof(List<IOS13Platform>));
            ios143platform.ShouldBeAssignableTo(typeof(List<IOS143Platform>));
            ios144platform.ShouldBeAssignableTo(typeof(List<IOS144Platform>));
            ios145platform.ShouldBeAssignableTo(typeof(List<IOS145Platform>));
            ios14platform.ShouldBeAssignableTo(typeof(List<IOS14Platform>));
            ios15platform.ShouldBeAssignableTo(typeof(List<IOS15Platform>));
            ios152platform.ShouldBeAssignableTo(typeof(List<IOS152Platform>));

            android12platform.ShouldBeAssignableTo(typeof(List<Android12Platform>));
            android11platform.ShouldBeAssignableTo(typeof(List<Android11Platform>));
            android10platform.ShouldBeAssignableTo(typeof(List<Android10Platform>));
            android9platform.ShouldBeAssignableTo(typeof(List<Android9Platform>));
            android81platform.ShouldBeAssignableTo(typeof(List<Android81Platform>));
            android8platform.ShouldBeAssignableTo(typeof(List<Android8Platform>));
            android71platform.ShouldBeAssignableTo(typeof(List<Android71Platform>));
            android7platform.ShouldBeAssignableTo(typeof(List<Android7Platform>));
            android6platform.ShouldBeAssignableTo(typeof(List<Android6Platform>));
            android51platform.ShouldBeAssignableTo(typeof(List<Android51Platform>));
        }
    }
}