using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

namespace Saucery.Core.Tests.XUnitv3.REST;

public static class PlatformTypes {
    public static IEnumerable<object[]> SupportedPlatformTypes =>
        Select(
            [
                typeof(LinuxPlatform),
                typeof(Windows11Platform),
                typeof(Windows10Platform),
                typeof(Windows81Platform),
                typeof(Windows8Platform),
                typeof(Mac15Platform),
                typeof(Mac14Platform),
                typeof(Mac13Platform),
                typeof(Mac12Platform),
                typeof(Mac11Platform),
                typeof(IOS261Platform),
                typeof(IOS186Platform),
                typeof(IOS18Platform),
                typeof(IOS175Platform),
                typeof(IOS17Platform),
                typeof(IOS164Platform),
                typeof(IOS162Platform),
                typeof(IOS161Platform),
                typeof(IOS16Platform),
                typeof(IOS155Platform),
                typeof(IOS154Platform),
                typeof(IOS152Platform),
                typeof(IOS15Platform),
                typeof(IOS145Platform),
                typeof(IOS144Platform),
                typeof(IOS143Platform),
                typeof(IOS14Platform),
                typeof(Android16Platform),
                typeof(Android15Platform),
                typeof(Android14Platform),
                typeof(Android13Platform),
                typeof(Android12Platform),
                typeof(Android11Platform),
                typeof(Android10Platform),
                typeof(Android9Platform),
                typeof(Android81Platform),
                typeof(Android8Platform),
                typeof(Android71Platform),
                typeof(Android7Platform),
                typeof(Android6Platform),
                typeof(Android51Platform),
        ]);

    public static IEnumerable<object[]> SupportedRealDeviceTypes =>
        Select(
            [
                typeof(IOS26Platform),
                typeof(IOS18Platform),
                typeof(IOS17Platform),
                typeof(IOS16Platform),
                typeof(IOS15Platform),
                typeof(IOS14Platform),
                typeof(IOS13Platform),
                typeof(Android16Platform),
                typeof(Android15Platform),
                typeof(Android14Platform),
                typeof(Android13Platform),
                typeof(Android12Platform),
                typeof(Android11Platform),
                typeof(Android10Platform),
                typeof(Android9Platform)
        ]);

    public static IEnumerable<object[]> PlatformsWithBrowsersTypes =>
        Select(
            [
                typeof(LinuxPlatform),
                typeof(Windows11Platform),
                typeof(Windows10Platform),
                typeof(Windows81Platform),
                typeof(Windows8Platform),
                typeof(Mac15Platform),
                typeof(Mac14Platform),
                typeof(Mac13Platform),
                typeof(Mac12Platform),
                typeof(Mac11Platform)
        ]);

    private static IEnumerable<object[]> Select(Type[] types) => types.Select(t => new object[] { t });
}