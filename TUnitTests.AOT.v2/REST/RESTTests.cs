using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests.REST;

public class RestTests {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FlowControlTest(bool isRealDevice) {
        var flowController = new SauceLabsFlowController();
        await flowController.ControlFlowAsync(isRealDevice);
    }

    private static void AssertSupportedPlatform<TPlatform>()
        where TPlatform : PlatformBase {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        availablePlatforms.ShouldNotBeNull();

        var platformList = availablePlatforms.GetPlatform<TPlatform>();

        platformList.ShouldNotBeNull();
        platformList.Count.ShouldBe(1);
        platformList.GetType().ShouldBe(typeof(List<TPlatform>));
    }

    private static void AssertSupportedRealDevice<TPlatform>()
        where TPlatform : PlatformBase {
        var realDevices = _fixture.PlatformConfigurator!.AvailableRealDevices;
        realDevices.ShouldNotBeNull();

        var platformList = realDevices.GetPlatform<TPlatform>();

        platformList.ShouldNotBeNull();
        platformList.Count.ShouldBe(1);
        platformList.GetType().ShouldBe(typeof(List<TPlatform>));
    }

    private static void AssertBrowserCount<TPlatform>()
        where TPlatform : PlatformBase {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        availablePlatforms.ShouldNotBeNull();

        var platformList = availablePlatforms.GetPlatform<TPlatform>();
        platformList.Count.ShouldBe(1);

        var first = platformList[0];
        first.ShouldNotBeNull();
        first.Browsers.Count.ShouldBeEquivalentTo(first.Selenium4BrowserNames!.Count);
    }

    [Test] public void SupportedPlatform_Linux() => AssertSupportedPlatform<LinuxPlatform>();
    [Test] public void SupportedPlatform_Windows11() => AssertSupportedPlatform<Windows11Platform>();
    [Test] public void SupportedPlatform_Windows10() => AssertSupportedPlatform<Windows10Platform>();
    [Test] public void SupportedPlatform_Windows81() => AssertSupportedPlatform<Windows81Platform>();
    [Test] public void SupportedPlatform_Windows8() => AssertSupportedPlatform<Windows8Platform>();
    [Test] public void SupportedPlatform_Mac15() => AssertSupportedPlatform<Mac15Platform>();
    [Test] public void SupportedPlatform_Mac14() => AssertSupportedPlatform<Mac14Platform>();
    [Test] public void SupportedPlatform_Mac13() => AssertSupportedPlatform<Mac13Platform>();
    [Test] public void SupportedPlatform_Mac12() => AssertSupportedPlatform<Mac12Platform>();
    [Test] public void SupportedPlatform_Mac11() => AssertSupportedPlatform<Mac11Platform>();
    [Test] public void SupportedPlatform_IOS261() => AssertSupportedPlatform<IOS261Platform>();
    [Test] public void SupportedPlatform_IOS186() => AssertSupportedPlatform<IOS186Platform>();
    [Test] public void SupportedPlatform_IOS18() => AssertSupportedPlatform<IOS18Platform>();
    [Test] public void SupportedPlatform_IOS175() => AssertSupportedPlatform<IOS175Platform>();
    [Test] public void SupportedPlatform_IOS17() => AssertSupportedPlatform<IOS17Platform>();
    [Test] public void SupportedPlatform_IOS164() => AssertSupportedPlatform<IOS164Platform>();
    [Test] public void SupportedPlatform_IOS162() => AssertSupportedPlatform<IOS162Platform>();
    [Test] public void SupportedPlatform_IOS161() => AssertSupportedPlatform<IOS161Platform>();
    [Test] public void SupportedPlatform_IOS16() => AssertSupportedPlatform<IOS16Platform>();
    [Test] public void SupportedPlatform_IOS155() => AssertSupportedPlatform<IOS155Platform>();
    [Test] public void SupportedPlatform_IOS154() => AssertSupportedPlatform<IOS154Platform>();
    [Test] public void SupportedPlatform_IOS152() => AssertSupportedPlatform<IOS152Platform>();
    [Test] public void SupportedPlatform_IOS15() => AssertSupportedPlatform<IOS15Platform>();
    [Test] public void SupportedPlatform_IOS145() => AssertSupportedPlatform<IOS145Platform>();
    [Test] public void SupportedPlatform_IOS144() => AssertSupportedPlatform<IOS144Platform>();
    [Test] public void SupportedPlatform_IOS143() => AssertSupportedPlatform<IOS143Platform>();
    [Test] public void SupportedPlatform_IOS14() => AssertSupportedPlatform<IOS14Platform>();
    [Test] public void SupportedPlatform_Android16() => AssertSupportedPlatform<Android16Platform>();
    [Test] public void SupportedPlatform_Android15() => AssertSupportedPlatform<Android15Platform>();
    [Test] public void SupportedPlatform_Android14() => AssertSupportedPlatform<Android14Platform>();
    [Test] public void SupportedPlatform_Android13() => AssertSupportedPlatform<Android13Platform>();
    [Test] public void SupportedPlatform_Android12() => AssertSupportedPlatform<Android12Platform>();
    [Test] public void SupportedPlatform_Android11() => AssertSupportedPlatform<Android11Platform>();
    [Test] public void SupportedPlatform_Android10() => AssertSupportedPlatform<Android10Platform>();
    [Test] public void SupportedPlatform_Android9() => AssertSupportedPlatform<Android9Platform>();
    [Test] public void SupportedPlatform_Android81() => AssertSupportedPlatform<Android81Platform>();
    [Test] public void SupportedPlatform_Android8() => AssertSupportedPlatform<Android8Platform>();
    [Test] public void SupportedPlatform_Android71() => AssertSupportedPlatform<Android71Platform>();
    [Test] public void SupportedPlatform_Android7() => AssertSupportedPlatform<Android7Platform>();
    [Test] public void SupportedPlatform_Android6() => AssertSupportedPlatform<Android6Platform>();
    [Test] public void SupportedPlatform_Android51() => AssertSupportedPlatform<Android51Platform>();

    [Test] public void SupportedRealDevice_IOS26() => AssertSupportedRealDevice<IOS26Platform>();
    [Test] public void SupportedRealDevice_IOS18() => AssertSupportedRealDevice<IOS18Platform>();
    [Test] public void SupportedRealDevice_IOS17() => AssertSupportedRealDevice<IOS17Platform>();
    [Test] public void SupportedRealDevice_IOS16() => AssertSupportedRealDevice<IOS16Platform>();
    [Test] public void SupportedRealDevice_IOS15() => AssertSupportedRealDevice<IOS15Platform>();
    [Test] public void SupportedRealDevice_IOS14() => AssertSupportedRealDevice<IOS14Platform>();
    [Test] public void SupportedRealDevice_IOS13() => AssertSupportedRealDevice<IOS13Platform>();
    [Test] public void SupportedRealDevice_Android16() => AssertSupportedRealDevice<Android16Platform>();
    [Test] public void SupportedRealDevice_Android15() => AssertSupportedRealDevice<Android15Platform>();
    [Test] public void SupportedRealDevice_Android14() => AssertSupportedRealDevice<Android14Platform>();
    [Test] public void SupportedRealDevice_Android13() => AssertSupportedRealDevice<Android13Platform>();
    [Test] public void SupportedRealDevice_Android12() => AssertSupportedRealDevice<Android12Platform>();
    [Test] public void SupportedRealDevice_Android11() => AssertSupportedRealDevice<Android11Platform>();
    [Test] public void SupportedRealDevice_Android10() => AssertSupportedRealDevice<Android10Platform>();
    [Test] public void SupportedRealDevice_Android9() => AssertSupportedRealDevice<Android9Platform>();

    [Test] public void BrowserCount_Linux() => AssertBrowserCount<LinuxPlatform>();
    [Test] public void BrowserCount_Windows11() => AssertBrowserCount<Windows11Platform>();
    [Test] public void BrowserCount_Windows10() => AssertBrowserCount<Windows10Platform>();
    [Test] public void BrowserCount_Windows81() => AssertBrowserCount<Windows81Platform>();
    [Test] public void BrowserCount_Windows8() => AssertBrowserCount<Windows8Platform>();
    [Test] public void BrowserCount_Mac15() => AssertBrowserCount<Mac15Platform>();
    [Test] public void BrowserCount_Mac14() => AssertBrowserCount<Mac14Platform>();
    [Test] public void BrowserCount_Mac13() => AssertBrowserCount<Mac13Platform>();
    [Test] public void BrowserCount_Mac12() => AssertBrowserCount<Mac12Platform>();
    [Test] public void BrowserCount_Mac11() => AssertBrowserCount<Mac11Platform>();
}