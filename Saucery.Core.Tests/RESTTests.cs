using NUnit.Framework;
using NUnit.Framework.Constraints;
using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.Tests.Util;
using Shouldly;

namespace Saucery.Core.Tests;

[TestFixture]
public class RestTests 
{
    private PlatformConfigurator? _configurator;

    [OneTimeSetUp]
    public void Setup()
    {
        _configurator = new PlatformConfigurator(PlatformFilter.ALL);
    }

    [Test]
    public void FlowControlTest() {
        var flowController = new SauceLabsFlowController();
        //Console.WriteLine(@"RESTTests: About to call ControlFlow()");
        flowController.ControlFlow(false);
    }

    [Test]
    public void FlowControlTestRealDevice()
    {
        var flowController = new SauceLabsFlowController();
        //Console.WriteLine(@"RESTTests: About to call ControlFlow()");
        flowController.ControlFlow(true);
    }

    [Test]
    public void SupportedRealDevicePlatformTest()
    {
        PlatformConfigurator configurator = new(PlatformFilter.REALDEVICE);
        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldBeEmpty();
        realDevices.ShouldNotBeNull();
    }

    [Test]
    public void SupportedEmulatedPlatformTest()
    {
        PlatformConfigurator configurator = new(PlatformFilter.EMULATED);
        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldNotBeNull();
        realDevices.ShouldBeEmpty();
    }

    [Test]
    [GenericTestCase(typeof(LinuxPlatform), TestName = "LinuxSupportedPlatformTest")]
    [GenericTestCase(typeof(Windows11Platform), TestName = "Windows11SupportedPlatformTest")]
    [GenericTestCase(typeof(Windows10Platform), TestName = "Windows10SupportedPlatformTest")]
    [GenericTestCase(typeof(Windows81Platform), TestName = "Windows81SupportedPlatformTest")]
    [GenericTestCase(typeof(Windows8Platform), TestName = "Windows8SupportedPlatformTest")]
    [GenericTestCase(typeof(Windows7Platform), TestName = "Windows7SupportedPlatformTest")]

    [GenericTestCase(typeof(Mac1010Platform), TestName = "Mac1010SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac1011Platform), TestName = "Mac1011SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac1012Platform), TestName = "Mac1012SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac1013Platform), TestName = "Mac1013SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac1014Platform), TestName = "Mac1014SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac1015Platform), TestName = "Mac1015SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac11Platform), TestName = "Mac11SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac12Platform), TestName = "Mac12SupportedPlatformTest")]
    [GenericTestCase(typeof(Mac13Platform), TestName = "Mac13SupportedPlatformTest")]

    [GenericTestCase(typeof(IOS103Platform), TestName = "IOS103SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS111Platform), TestName = "IOS111SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS112Platform), TestName = "IOS112SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS113Platform), TestName = "IOS113SupportedPlatformv")]
    [GenericTestCase(typeof(IOS11Platform), TestName = "IOS11SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS122Platform), TestName = "IOS122SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS124Platform), TestName = "IOS124SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS12Platform), TestName = "IOS12SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS132Platform), TestName = "IOS132SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS134Platform), TestName = "IOS134SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS13Platform), TestName = "IOS13SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS143Platform), TestName = "IOS143SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS144Platform), TestName = "IOS144SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS145Platform), TestName = "IOS145SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS14Platform), TestName = "IOS14SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS15Platform), TestName = "IOS15SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS152Platform), TestName = "IOS152SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS154Platform), TestName = "IOS154SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS16Platform), TestName = "IOS16SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS161Platform), TestName = "IOS161SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS162Platform), TestName = "IOS162SupportedPlatformTest")]
    [GenericTestCase(typeof(IOS17Platform), TestName = "IOS17SupportedPlatformTest")]

    [GenericTestCase(typeof(Android15Platform), TestName = "Android14SupportedPlatformTest")]
    [GenericTestCase(typeof(Android14Platform), TestName = "Android14SupportedPlatformTest")]
    [GenericTestCase(typeof(Android13Platform), TestName = "Android13SupportedPlatformTest")]
    [GenericTestCase(typeof(Android12Platform), TestName = "Android12SupportedPlatformTest")]
    [GenericTestCase(typeof(Android11Platform), TestName = "Android11SupportedPlatformTest")]
    [GenericTestCase(typeof(Android10Platform), TestName = "Android10SupportedPlatformTest")]
    [GenericTestCase(typeof(Android9Platform), TestName = "Android9SupportedPlatformTest")]
    [GenericTestCase(typeof(Android81Platform), TestName = "Android81SupportedPlatformTest")]
    [GenericTestCase(typeof(Android8Platform), TestName = "Android8SupportedPlatformTest")]
    [GenericTestCase(typeof(Android71Platform), TestName = "Android71SupportedPlatformTest")]
    [GenericTestCase(typeof(Android7Platform), TestName = "Android7SupportedPlatformTest")]
    [GenericTestCase(typeof(Android6Platform), TestName = "Android6SupportedPlatformTest")]
    [GenericTestCase(typeof(Android51Platform), TestName = "Android51SupportedPlatformTest")]
    public void SupportedPlatformTheory<T>() where T : PlatformBase
    {
        var availablePlatforms = _configurator!.AvailablePlatforms;
        var realDevices = _configurator.AvailableRealDevices;

        availablePlatforms.ShouldNotBeNull();
        realDevices.ShouldNotBeNull();

        var platform = availablePlatforms.GetPlatform<T>();

        //Null Check
        platform.ShouldNotBeNull();

        //Count Checks
        platform.Count.ShouldBe(1);

        //TypeOf Checks
        platform.ShouldBeAssignableTo(typeof(List<T>));
    }

    [Test]
    [GenericTestCase(typeof(LinuxPlatform), TestName = "LinuxBrowserCountTest")]
    [GenericTestCase(typeof(Windows11Platform), TestName = "Windows11BrowserCountTest")]
    [GenericTestCase(typeof(Windows10Platform), TestName = "Windows10BrowserCountTest")]
    [GenericTestCase(typeof(Windows81Platform), TestName = "Windows81BrowserCountTest")]
    [GenericTestCase(typeof(Windows8Platform), TestName = "Windows8BrowserCountTest")]
    [GenericTestCase(typeof(Windows7Platform), TestName = "Windows7BrowserCountTest")]

    [GenericTestCase(typeof(Mac1010Platform), TestName = "Mac1010BrowserCountTest")]
    [GenericTestCase(typeof(Mac1011Platform), TestName = "Mac1011BrowserCountTest")]
    [GenericTestCase(typeof(Mac1012Platform), TestName = "Mac1012BrowserCountTest")]
    [GenericTestCase(typeof(Mac1013Platform), TestName = "Mac1013BrowserCountTest")]
    [GenericTestCase(typeof(Mac1014Platform), TestName = "Mac1014BrowserCountTest")]
    [GenericTestCase(typeof(Mac1015Platform), TestName = "Mac1015BrowserCountTest")]
    [GenericTestCase(typeof(Mac11Platform), TestName = "Mac11BrowserCountTest")]
    [GenericTestCase(typeof(Mac12Platform), TestName = "Mac12BrowserCountTest")]
    [GenericTestCase(typeof(Mac13Platform), TestName = "Mac13BrowserCountTest")]
    //[Ignore("Account has no minutes")]
    public void BrowserCountTest<T>() where T : PlatformBase
    {
        //var windows10platforms = platforms.FindAll(w => w.os.Equals(SauceryConstants.PLATFORM_WINDOWS_10));
        //var windows10platformbrowsers = windows10platforms.GroupBy(w => w.api_name);

        var availablePlatforms = _configurator!.AvailablePlatforms;
                
        //var browsers = availablePlatforms.SelectMany(i => i.Browsers).Distinct().ToList();
        //var iosBrowsers = browsers.FindAll(x => x.DeviceName.Equals("iPhone XS Max Simulator") || x.DeviceName.Equals("iPhone 5s Simulator")).OrderBy(o=>o.DeviceName).ThenBy(o=>o.PlatformVersion);

        var platform = availablePlatforms.GetPlatform<T>();

        //Browser Count Checks
        platform[0].Browsers.Count.ShouldBeEquivalentTo(platform[0].Selenium4BrowserNames!.Count);
    }
}