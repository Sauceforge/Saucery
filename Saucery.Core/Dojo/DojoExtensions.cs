﻿using Saucery.Core.Dojo.Browsers;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Platforms;
using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo;

public static class DojoExtensions
{
    public static void AddRealPlatform(this List<PlatformBase> platforms, SupportedPlatform sp) {
        var p = platforms.FindRealPlatform(sp);
        if(p == null) {
            //first one
            p = PlatformFactory.CreateRealPlatform(sp);
            if(p == null) {
                //SauceLabs may have just added it to the platform configurator. Don't fall over.
                return;
            }
            platforms.Add(p);
        }
    }

    public static void AddPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
    {
        var p = platforms.FindPlatform(sp);
        if (p == null)
        {
            //first one
            p = PlatformFactory.CreatePlatform(sp);
            if(p == null)
            {
                //SauceLabs may have just added it to the platform configurator. Don't fall over.
                return;
            }
            p.Browsers.AddBrowser(sp, p.ScreenResolutions!);
            platforms.Add(p);
        }
        else
        {
            p.Browsers.AddBrowser(sp, p.ScreenResolutions!);
        }
    }

    public static PlatformBase? FindRealPlatform(this List<PlatformBase> platforms, SupportedPlatform sp) =>
        platforms.Count == 0
            ? null
            : platforms
                .Find(mp =>
                    mp.Name.Equals(sp.Os, StringComparison.Ordinal) &&
                    mp.PlatformVersion!.Equals(sp.OsVersion?.Split(".")[0], StringComparison.Ordinal));

    public static PlatformBase? FindPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
    {
        if(platforms.Count == 0)
            return null;

        var mobilePlatforms = platforms.FindAll(p => p.AutomationBackend.Equals("appium"));
        var desktopPlatforms = platforms.FindAll(p=> p.AutomationBackend.Equals("webdriver"));

        return sp.IsMobilePlatform()
            ? mobilePlatforms
                .Find(mp =>
                    mp.Name.Equals(sp.Os, StringComparison.Ordinal) &&
                    mp.PlatformVersion!.Equals(sp.short_version, StringComparison.Ordinal))
            : desktopPlatforms.Find(dp => dp.Name.Equals(sp.Os, StringComparison.Ordinal));
    }

    public static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp, List<string> screenResolutions)
    {
        var b = browsers.FindBrowser(sp);

        if (b == null) {
            //first one
            b = BrowserFactory.CreateBrowser(sp, screenResolutions);
            b?.AddVersion(browsers, sp, false);
        } else
        {
            b.AddVersion(browsers, sp, true);
        }
    }

    //public static void AddRealBrowser(this List<BrowserBase> browsers, SupportedPlatform sp) {
    //    var b = browsers.FindRealBrowser(sp);

    //    if(b == null) {
    //        //first one
    //        b = BrowserFactory.CreateRealBrowser(sp);
    //        b?.AddVersion(browsers, sp, false);
    //    } else {
    //        b.AddVersion(browsers, sp, true);
    //    }
    //}

    private static void AddVersion(this BrowserBase b, List<BrowserBase> browsers, SupportedPlatform sp, bool findVersion)
    {
        if (findVersion)
        {
            //Browser found or created, now add version
            if (b.IsSupportedVersion(sp) && b.FindVersion(sp) == null)
            {
                var bv = new BrowserVersion(sp, b);
                b.BrowserVersions.Add(bv);
            }
        }
        else
        {
            if (b.IsSupportedVersion(sp))
            {
                var bv = new BrowserVersion(sp, b);
                b.BrowserVersions.Add(bv);
                browsers.Add(b);
            }
        }
    }

    public static IList<T> GetPlatform<T>(this List<PlatformBase> availablePlatforms) where T : PlatformBase => availablePlatforms.OfType<T>().ToList();

    public static BrowserVersion? FindDesktopBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        PlatformBase? platform = sp.Os switch
        {
            SauceryConstants.PLATFORM_LINUX => platforms.GetPlatform<LinuxPlatform>()[0],
            SauceryConstants.PLATFORM_WINDOWS_11 => platforms.GetPlatform<Windows11Platform>()[0],
            SauceryConstants.PLATFORM_WINDOWS_10 => platforms.GetPlatform<Windows10Platform>()[0],
            SauceryConstants.PLATFORM_WINDOWS_81 => platforms.GetPlatform<Windows81Platform>()[0],
            SauceryConstants.PLATFORM_WINDOWS_8 => platforms.GetPlatform<Windows8Platform>()[0],
            SauceryConstants.PLATFORM_WINDOWS_7 => platforms.GetPlatform<Windows7Platform>()[0],
            SauceryConstants.PLATFORM_MAC_13 => platforms.GetPlatform<Mac13Platform>()[0],
            SauceryConstants.PLATFORM_MAC_12 => platforms.GetPlatform<Mac12Platform>()[0],
            SauceryConstants.PLATFORM_MAC_11 => platforms.GetPlatform<Mac11Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1015 => platforms.GetPlatform<Mac1015Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1014 => platforms.GetPlatform<Mac1014Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1013 => platforms.GetPlatform<Mac1013Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1012 => platforms.GetPlatform<Mac1012Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1011 => platforms.GetPlatform<Mac1011Platform>()[0],
            SauceryConstants.PLATFORM_MAC_1010 => platforms.GetPlatform<Mac1010Platform>()[0],
            _ => null
        };

        var browsers = platform?.Browsers.Find(b => 
            b.Os.Equals(sp.Os, StringComparison.Ordinal) && 
            b.Name.ToLower().Equals(sp.Browser.ToLower()));

        if (browsers == null)
        {
            return null;
        }

        return sp.ScreenResolution == string.Empty
            ? browsers.BrowserVersions
                .Find(v => 
                    v.Os.Equals(sp.Os, StringComparison.Ordinal) && 
                    v.BrowserName.ToLower().Equals(sp.Browser.ToLower()) && 
                    v.Name!.ToLower().Equals(sp.BrowserVersion.ToLower()))
            : browsers.BrowserVersions
                .Find(v => 
                    v.Os.Equals(sp.Os, StringComparison.Ordinal) && 
                    v.BrowserName.ToLower().Equals(sp.Browser.ToLower()) && 
                    v.Name!.ToLower().Equals(sp.BrowserVersion.ToLower()) && 
                    v.ScreenResolutions.Contains(sp.ScreenResolution));
    }

    public static BrowserVersion? FindAndroidBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        PlatformBase? platform = null;
        var platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        platform = platformToSearchFor switch {
                "Linux 15.0" => platforms.GetPlatform<Android15Platform>()[0],
                "Linux 14.0" => platforms.GetPlatform<Android14Platform>()[0],
                "Linux 13.0" => platforms.GetPlatform<Android13Platform>()[0],
                "Linux 12.0" => platforms.GetPlatform<Android12Platform>()[0],
                "Linux 11.0" => platforms.GetPlatform<Android11Platform>()[0],
                "Linux 10.0" => platforms.GetPlatform<Android10Platform>()[0],
                "Linux 9.0" => platforms.GetPlatform<Android9Platform>()[0],
                "Linux 8.1" => platforms.GetPlatform<Android81Platform>()[0],
                "Linux 8.0" => platforms.GetPlatform<Android8Platform>()[0],
                "Linux 7.1" => platforms.GetPlatform<Android71Platform>()[0],
                "Linux 7.0" => platforms.GetPlatform<Android7Platform>()[0],
                "Linux 6.0" => platforms.GetPlatform<Android6Platform>()[0],
                "Linux 5.1" => platforms.GetPlatform<Android51Platform>()[0],
                _ => platform
            };

        var browsers = platform?.Browsers
            .Find(b => 
                b.Os.Equals(sp.Os, StringComparison.Ordinal) && 
                b.DeviceName.Equals(sp.LongName, StringComparison.Ordinal));

        return browsers == null
            ? null
            : browsers.BrowserVersions.Count == 1 
                ? browsers.BrowserVersions[0] 
                : browsers.BrowserVersions
                    .Find(v => 
                        v.Os.Equals(sp.Os, StringComparison.Ordinal) && 
                        v.DeviceName.Equals(sp.LongName, StringComparison.Ordinal) && 
                        v.Name!.Equals(sp.BrowserVersion, StringComparison.Ordinal));
    }

    public static PlatformBase FindAndroidPlatform(this List<PlatformBase> platforms, SaucePlatform sp) {
        PlatformBase? platform = null;
        var platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        platform = platformToSearchFor switch {
            "Linux 15" => platforms.GetPlatform<Android15Platform>()[0],
            "Linux 14" => platforms.GetPlatform<Android14Platform>()[0],
            "Linux 13" => platforms.GetPlatform<Android13Platform>()[0],
            "Linux 12" => platforms.GetPlatform<Android12Platform>()[0],
            "Linux 11" => platforms.GetPlatform<Android11Platform>()[0],
            "Linux 10" => platforms.GetPlatform<Android10Platform>()[0],
            "Linux 9" => platforms.GetPlatform<Android9Platform>()[0],
            "Linux 8" => platforms.GetPlatform<Android8Platform>()[0],
            "Linux 7" => platforms.GetPlatform<Android7Platform>()[0],
            _ => platform
        };

        return platform!;
    }

    public static BrowserVersion? FindIOSBrowser(this List<PlatformBase> platforms, SaucePlatform sp) {
        PlatformBase? platform = null;
        var platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        platform = platformToSearchFor switch {
                "iOS 17.0" => platforms.GetPlatform<IOS17Platform>()[0],
                "iOS 16.2" => platforms.GetPlatform<IOS162Platform>()[0],
                "iOS 16.1" => platforms.GetPlatform<IOS161Platform>()[0],
                "iOS 16.0" => platforms.GetPlatform<IOS16Platform>()[0],
                "iOS 15.4" => platforms.GetPlatform<IOS154Platform>()[0],
                "iOS 15.2" => platforms.GetPlatform<IOS152Platform>()[0],
                "iOS 15.0" => platforms.GetPlatform<IOS15Platform>()[0],
                "iOS 14.5" => platforms.GetPlatform<IOS145Platform>()[0],
                "iOS 14.4" => platforms.GetPlatform<IOS144Platform>()[0],
                "iOS 14.3" => platforms.GetPlatform<IOS143Platform>()[0],
                "iOS 14.0" => platforms.GetPlatform<IOS14Platform>()[0],
                "iOS 13.4" => platforms.GetPlatform<IOS134Platform>()[0],
                "iOS 13.2" => platforms.GetPlatform<IOS132Platform>()[0],
                "iOS 13.0" => platforms.GetPlatform<IOS13Platform>()[0],
                "iOS 12.4" => platforms.GetPlatform<IOS124Platform>()[0],
                "iOS 12.2" => platforms.GetPlatform<IOS122Platform>()[0],
                "iOS 12.0" => platforms.GetPlatform<IOS12Platform>()[0],
                "iOS 11.3" => platforms.GetPlatform<IOS113Platform>()[0],
                "iOS 11.2" => platforms.GetPlatform<IOS112Platform>()[0],
                "iOS 11.1" => platforms.GetPlatform<IOS111Platform>()[0],
                "iOS 11.0" => platforms.GetPlatform<IOS11Platform>()[0],
                "iOS 10.3" => platforms.GetPlatform<IOS103Platform>()[0],
                _ => platform
            };

        var browsers = platform?.Browsers
            .Find(b => 
                b.PlatformNameForOption.Equals(sp.Os, StringComparison.Ordinal) && 
                b.DeviceName.Equals(sp.LongName, StringComparison.Ordinal));

        return browsers == null
            ? null
            : browsers.BrowserVersions.Count == 1 
                ? browsers.BrowserVersions[0] 
                : browsers.BrowserVersions
                    .Find(v => 
                        v.PlatformNameForOption.Equals(sp.Os, StringComparison.Ordinal) && 
                        v.DeviceName.Equals(sp.LongName, StringComparison.Ordinal) && 
                        v.Name!.Equals(sp.BrowserVersion, StringComparison.Ordinal));
    }

    public static PlatformBase FindIOSPlatform(this List<PlatformBase> platforms, SaucePlatform sp) {
        PlatformBase? platform = null;
        var platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        platform = platformToSearchFor switch {
                "iOS 18" => platforms.GetPlatform<IOS18Platform>()[0],
                "iOS 17" => platforms.GetPlatform<IOS17Platform>()[0],
                "iOS 16" => platforms.GetPlatform<IOS16Platform>()[0],
                "iOS 15" => platforms.GetPlatform<IOS15Platform>()[0],
                "iOS 14" => platforms.GetPlatform<IOS14Platform>()[0],
                "iOS 13" => platforms.GetPlatform<IOS13Platform>()[0],
                "iOS 12" => platforms.GetPlatform<IOS12Platform>()[0],
                _ => platform
            };

        return platform!;
    }

    private static BrowserBase? FindBrowser(this IEnumerable<BrowserBase> browsers, SupportedPlatform sp) => 
        browsers.FirstOrDefault(b => 
            b.Name.Equals(sp.api_name, StringComparison.Ordinal) &&
            b.DeviceName.Equals(sp.long_name, StringComparison.Ordinal) &&
            b.Os.Equals(sp.Os, StringComparison.Ordinal));

    //private static BrowserBase? FindRealBrowser(this IEnumerable<BrowserBase> browsers, SupportedPlatform sp) => 
    //    browsers.FirstOrDefault(b => 
    //        b.Name.Equals(sp.api_name, StringComparison.Ordinal) && 
    //        b.DeviceName.Equals(sp.long_name, StringComparison.Ordinal) &&
    //        b.Os.Equals(sp.Os, StringComparison.Ordinal));

    public static BrowserVersion Classify(this BrowserVersion browserVersion)
    {
        if (browserVersion.IsAnAndroidDevice())
        {
            browserVersion.PlatformType = PlatformType.Android;
            return browserVersion;
        }

        if (browserVersion.IsAnAppleDevice())
        {
            browserVersion.PlatformType = PlatformType.Apple;
            return browserVersion;
        }

        //Desktop
        browserVersion.PlatformType = browserVersion.BrowserName.ToLower() switch
        {
            SauceryConstants.BROWSER_CHROME => PlatformType.Chrome,
            SauceryConstants.BROWSER_FIREFOX => PlatformType.Firefox,
            SauceryConstants.BROWSER_IE => PlatformType.IE,
            SauceryConstants.BROWSER_EDGE_LOWER => PlatformType.Edge,
            SauceryConstants.BROWSER_SAFARI => PlatformType.Safari,
            _ => browserVersion.PlatformType
        };

        return browserVersion;
    }
}