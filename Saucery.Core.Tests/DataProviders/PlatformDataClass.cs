using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Tests.DataProviders;

public static class PlatformDataClass {
    public static List<SaucePlatform> DesktopPlatforms =>
    [
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "123"),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "124", SauceryConstants.SCREENRES_2560_1600),
        new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119", SauceryConstants.SCREENRES_2560_1600),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "100", SauceryConstants.SCREENRES_800_600),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_800_600),
        new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
        new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
        new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11", SauceryConstants.SCREENRES_800_600),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
        new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99", SauceryConstants.SCREENRES_800_600)
    ];

    public static List<SaucePlatform> EmulatedAndroidPlatforms =>
    [
        new AndroidPlatform("Google Pixel Tablet Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
    ];

    public static List<SaucePlatform> EmulatedIOSPlatforms =>
    [
        new IOSPlatform("iPhone 15 Pro Max Simulator", "17.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
    ];

    public static List<SaucePlatform> RealAndroidDevices =>
    [
        new AndroidRealDevice("Google.*", "15"),
        new AndroidRealDevice("Google Pixel 8 Pro", "15"),
        new AndroidRealDevice("Google Pixel 8 Pro", "14"),
        new AndroidRealDevice("Google Pixel 7 Pro", "13"),
        new AndroidRealDevice("Google Pixel 6a", "12"),
        new AndroidRealDevice("Google Pixel 4a", "11"),
        new AndroidRealDevice("Google Pixel 4 XL", "10"),
        new AndroidRealDevice("Samsung.*", "9")
    ];

    public static List<SaucePlatform> RealIOSDevices =>
    [
        new IOSRealDevice("iPhone 15 Pro Max", "18"),
        new IOSRealDevice("iPhone 15 Pro Max", "17"),
        new IOSRealDevice("iPhone 14 Pro Max", "16"),
        new IOSRealDevice("iPhone 13 Pro Max", "15"),
        new IOSRealDevice("iPhone 12 Pro Max", "14"),
        new IOSRealDevice("iPhone.*", "13"),
    ];
}