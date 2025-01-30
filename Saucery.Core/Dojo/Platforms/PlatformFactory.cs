using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms;

public static class PlatformFactory
{
    public static PlatformBase? CreatePlatform(SupportedPlatform sp) => 
        sp.IsAndroidPlatform()
            ? AndroidPlatformFactory.CreatePlatform(sp)
            : sp.IsIOSPlatform()
                ? ApplePlatformFactory.CreatePlatform(sp)
                : DesktopPlatformFactory.CreatePlatform(sp);

    public static PlatformBase? CreateRealPlatform(SupportedPlatform sp) =>
        sp.Manufacturer?[0] == "Apple" 
            ? ApplePlatformFactory.CreateRealPlatform(sp)
            : AndroidPlatformFactory.CreateRealPlatform(sp);
}
