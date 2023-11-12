using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteCreators.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms;

public class PlatformFactory
{
    public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.IsAndroidPlatform()
            ? AndroidPlatformFactory.CreatePlatform(sp)
            : sp.IsIOSPlatform()
            ? ApplePlatformFactory.CreatePlatform(sp)
            : sp.os switch
            {
                SauceryConstants.PLATFORM_WINDOWS_11 => new Windows11PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_WINDOWS_10 => new Windows10PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_WINDOWS_81 => new Windows81PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_WINDOWS_7 => new Windows7PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_13 => new Mac13PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_12 => new Mac12PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_11 => new Mac11PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1015 => new Mac1015PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1014 => new Mac1014PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1013 => new Mac1013PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1012 => new Mac1012PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1011 => new Mac1011PlatformCreator(sp).Create(),
                SauceryConstants.PLATFORM_MAC_1010 => new Mac1010PlatformCreator(sp).Create(),
                _ => null
            };
}
