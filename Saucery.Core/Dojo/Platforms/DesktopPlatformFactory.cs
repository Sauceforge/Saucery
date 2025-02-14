using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteCreators.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteCreators.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms;

public static class DesktopPlatformFactory {
    public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.Os switch {
        SauceryConstants.PLATFORM_LINUX => new LinuxPlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_WINDOWS_11 => new Windows11PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_WINDOWS_10 => new Windows10PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_WINDOWS_81 => new Windows81PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_WINDOWS_7 => new Windows7PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_MAC_13 => new Mac13PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_MAC_12 => new Mac12PlatformCreator(sp).Create(),
        SauceryConstants.PLATFORM_MAC_11 => new Mac11PlatformCreator(sp).Create(),
        _ => null
    };
}
