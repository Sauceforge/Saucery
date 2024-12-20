﻿using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteCreators.Apple;
using Saucery.Core.Dojo.Browsers.ConcreteCreators.Google;
using Saucery.Core.Dojo.Browsers.ConcreteCreators.Linux;
using Saucery.Core.Dojo.Browsers.ConcreteCreators.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers;

public class BrowserFactory
{
    public static BrowserBase? CreateBrowser(SupportedPlatform sp, List<string> screenResolutions) => sp.IsAndroidPlatform()
            ? new AndroidBrowserCreator(sp).Create("Android", null!)
            : sp.IsIOSPlatform()
            ? new IOSBrowserCreator(sp).Create("iOS", null!)
            : sp.Os switch
            {
                SauceryConstants.PLATFORM_LINUX => new LinuxBrowserCreator(sp).Create("Linux", screenResolutions),
                SauceryConstants.PLATFORM_WINDOWS_11 => new Windows11BrowserCreator(sp).Create("Windows 11", screenResolutions),
                SauceryConstants.PLATFORM_WINDOWS_10 => new Windows10BrowserCreator(sp).Create("Windows 10", screenResolutions),
                SauceryConstants.PLATFORM_WINDOWS_81 => new Windows81BrowserCreator(sp).Create("Windows 8.1", screenResolutions),
                SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8BrowserCreator(sp).Create("Windows 8", screenResolutions),
                SauceryConstants.PLATFORM_WINDOWS_7 => new Windows7BrowserCreator(sp).Create("Windows 7", screenResolutions),
                SauceryConstants.PLATFORM_MAC_13 => new Mac13BrowserCreator(sp).Create("macOS 13", screenResolutions),
                SauceryConstants.PLATFORM_MAC_12 => new Mac12BrowserCreator(sp).Create("macOS 12", screenResolutions),
                SauceryConstants.PLATFORM_MAC_11 => new Mac11BrowserCreator(sp).Create("macOS 11.00", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1015 => new Mac1015BrowserCreator(sp).Create("macOS 10.15", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1014 => new Mac1014BrowserCreator(sp).Create("macOS 10.14", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1013 => new Mac1013BrowserCreator(sp).Create("macOS 10.13", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1012 => new Mac1012BrowserCreator(sp).Create("macOS 10.12", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1011 => new Mac1011BrowserCreator(sp).Create("OS X 10.11", screenResolutions),
                SauceryConstants.PLATFORM_MAC_1010 => new Mac1010BrowserCreator(sp).Create("OS X 10.10", screenResolutions),
                _ => null,
            };

    //public static BrowserBase CreateRealBrowser(SupportedPlatform sp) => sp.Manufacturer?[0] == "Apple"
    //        ? new IOSBrowserCreator(sp).Create("iOS", null!)
    //        : new AndroidBrowserCreator(sp).Create("Android", null!);
}
