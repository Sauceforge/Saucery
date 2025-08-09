using Saucery.Core.Dojo.Browsers;
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
    public static void AddRealPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
    {
        var platform = platforms.FindRealPlatform(sp);
        if (platform == null)
        {
            platform = PlatformFactory.CreateRealPlatform(sp);
            if (platform != null)
            {
                platforms.Add(platform);
            }
        }
    }

    public static void AddPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
    {
        var platform = platforms.FindPlatform(sp);
        if (platform == null)
        {
            platform = PlatformFactory.CreatePlatform(sp);
            if (platform != null)
            {
                platform.Browsers.AddBrowser(sp, platform.ScreenResolutions!);
                platforms.Add(platform);
            }
        }
        else
        {
            platform.Browsers.AddBrowser(sp, platform.ScreenResolutions!);
        }
    }

    private static PlatformBase? FindRealPlatform(this List<PlatformBase> platforms, SupportedPlatform sp) =>
        platforms.FirstOrDefault(mp =>
            mp.Name.Equals(sp.Os, StringComparison.Ordinal) &&
            mp.PlatformVersion!.Equals(sp.OsVersion?.Split(".")[0], StringComparison.Ordinal));

    private static PlatformBase? FindPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
    {
        var mobilePlatforms = platforms.Where(p => p.AutomationBackend.Equals("appium")).ToList();
        var desktopPlatforms = platforms.Where(p => p.AutomationBackend.Equals("webdriver")).ToList();

        return sp.IsMobilePlatform()
            ? mobilePlatforms.FirstOrDefault(mp =>
                mp.Name.Equals(sp.Os, StringComparison.Ordinal) &&
                mp.PlatformVersion!.Equals(sp.short_version, StringComparison.Ordinal))
            : desktopPlatforms.FirstOrDefault(dp => dp.Name.Equals(sp.Os, StringComparison.Ordinal));
    }

    private static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp, List<string> screenResolutions)
    {
        var browser = browsers.FindBrowser(sp);
        if (browser == null)
        {
            browser = BrowserFactory.CreateBrowser(sp, screenResolutions);
            browser?.AddVersion(browsers, sp, false);
        }
        else
        {
            browser.AddVersion(browsers, sp, true);
        }
    }

    private static void AddVersion(this BrowserBase browser, List<BrowserBase> browsers, SupportedPlatform sp, bool findVersion)
    {
        if (findVersion)
        {
            if (browser.IsSupportedVersion(sp) && browser.FindVersion(sp) == null)
            {
                var browserVersion = new BrowserVersion(sp, browser);
                browser.BrowserVersions.Add(browserVersion);
            }
        }
        else
        {
            if (browser.IsSupportedVersion(sp))
            {
                var browserVersion = new BrowserVersion(sp, browser);
                browser.BrowserVersions.Add(browserVersion);
                browsers.Add(browser);
            }
        }
    }

    public static IList<T> GetPlatform<T>(this List<PlatformBase> availablePlatforms) where T : PlatformBase 
        => [.. availablePlatforms.OfType<T>()];

    public static BrowserVersion? FindDesktopBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        PlatformBase? platform = sp.Os switch
        {
            SauceryConstants.PLATFORM_LINUX => platforms.GetPlatform<LinuxPlatform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_WINDOWS_11 => platforms.GetPlatform<Windows11Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_WINDOWS_10 => platforms.GetPlatform<Windows10Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_WINDOWS_81 => platforms.GetPlatform<Windows81Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_WINDOWS_8 => platforms.GetPlatform<Windows8Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_WINDOWS_7 => platforms.GetPlatform<Windows7Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_MAC_15 => platforms.GetPlatform<Mac15Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_MAC_14 => platforms.GetPlatform<Mac14Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_MAC_13 => platforms.GetPlatform<Mac13Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_MAC_12 => platforms.GetPlatform<Mac12Platform>().FirstOrDefault(),
            SauceryConstants.PLATFORM_MAC_11 => platforms.GetPlatform<Mac11Platform>().FirstOrDefault(),
            _ => null
        };

        var browser = platform?.Browsers.FirstOrDefault(b =>
            b.Os.Equals(sp.Os, StringComparison.Ordinal) &&
            b.Name.Equals(sp.Browser, StringComparison.OrdinalIgnoreCase));

        return sp.ScreenResolution == string.Empty
            ? browser?.BrowserVersions.FirstOrDefault(v =>
                v.Os.Equals(sp.Os, StringComparison.Ordinal) &&
                v.BrowserName.Equals(sp.Browser, StringComparison.OrdinalIgnoreCase) &&
                v.Name!.Equals(sp.BrowserVersion, StringComparison.OrdinalIgnoreCase))
            : browser?.BrowserVersions.FirstOrDefault(v =>
                v.Os.Equals(sp.Os, StringComparison.Ordinal) &&
                v.BrowserName.Equals(sp.Browser, StringComparison.OrdinalIgnoreCase) &&
                v.Name!.Equals(sp.BrowserVersion, StringComparison.OrdinalIgnoreCase) &&
                v.ScreenResolutions.Contains(sp.ScreenResolution));
    }

    public static BrowserVersion? FindAndroidBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        string platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        PlatformBase? platform = platformToSearchFor switch
        {
            "Linux 16.0" => platforms.GetPlatform<Android16Platform>().FirstOrDefault(),
            "Linux 15.0" => platforms.GetPlatform<Android15Platform>().FirstOrDefault(),
            "Linux 14.0" => platforms.GetPlatform<Android14Platform>().FirstOrDefault(),
            "Linux 13.0" => platforms.GetPlatform<Android13Platform>().FirstOrDefault(),
            "Linux 12.0" => platforms.GetPlatform<Android12Platform>().FirstOrDefault(),
            "Linux 11.0" => platforms.GetPlatform<Android11Platform>().FirstOrDefault(),
            "Linux 10.0" => platforms.GetPlatform<Android10Platform>().FirstOrDefault(),
            "Linux 9.0" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
            "Linux 8.1" => platforms.GetPlatform<Android81Platform>().FirstOrDefault(),
            "Linux 8.0" => platforms.GetPlatform<Android8Platform>().FirstOrDefault(),
            "Linux 7.1" => platforms.GetPlatform<Android71Platform>().FirstOrDefault(),
            "Linux 7.0" => platforms.GetPlatform<Android7Platform>().FirstOrDefault(),
            "Linux 6.0" => platforms.GetPlatform<Android6Platform>().FirstOrDefault(),
            "Linux 5.1" => platforms.GetPlatform<Android51Platform>().FirstOrDefault(),
            _ => null
        };

        var browser = platform?.Browsers.FirstOrDefault(b =>
            b.Os.Equals(sp.Os, StringComparison.Ordinal) &&
            b.DeviceName.Equals(sp.LongName, StringComparison.Ordinal));

        return browser?.BrowserVersions.Count == 1
            ? browser.BrowserVersions[0]
            : browser?.BrowserVersions.FirstOrDefault(v =>
                v.Os.Equals(sp.Os, StringComparison.Ordinal) &&
                v.DeviceName.Equals(sp.LongName, StringComparison.Ordinal) &&
                v.Name!.Equals(sp.BrowserVersion, StringComparison.Ordinal));
    }

    public static PlatformBase? FindAndroidPlatform(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        string platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        return platformToSearchFor switch
        {
            "Linux 16" => platforms.GetPlatform<Android16Platform>().FirstOrDefault(),
            "Linux 15" => platforms.GetPlatform<Android15Platform>().FirstOrDefault(),
            "Linux 14" => platforms.GetPlatform<Android14Platform>().FirstOrDefault(),
            "Linux 13" => platforms.GetPlatform<Android13Platform>().FirstOrDefault(),
            "Linux 12" => platforms.GetPlatform<Android12Platform>().FirstOrDefault(),
            "Linux 11" => platforms.GetPlatform<Android11Platform>().FirstOrDefault(),
            "Linux 10" => platforms.GetPlatform<Android10Platform>().FirstOrDefault(),
            "Linux 9" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
            _ => null
        };
    }

    public static BrowserVersion? FindIOSBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        string platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        PlatformBase? platform = platformToSearchFor switch
        {
            "iOS 26.0" => platforms.GetPlatform<IOS26Platform>().FirstOrDefault(),
            "iOS 18.0" => platforms.GetPlatform<IOS18Platform>().FirstOrDefault(),
            "iOS 17.5" => platforms.GetPlatform<IOS175Platform>().FirstOrDefault(),
            "iOS 17.0" => platforms.GetPlatform<IOS17Platform>().FirstOrDefault(),
            "iOS 16.2" => platforms.GetPlatform<IOS162Platform>().FirstOrDefault(),
            "iOS 16.1" => platforms.GetPlatform<IOS161Platform>().FirstOrDefault(),
            "iOS 16.0" => platforms.GetPlatform<IOS16Platform>().FirstOrDefault(),
            "iOS 15.4" => platforms.GetPlatform<IOS154Platform>().FirstOrDefault(),
            "iOS 15.2" => platforms.GetPlatform<IOS152Platform>().FirstOrDefault(),
            "iOS 15.0" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),
            "iOS 14.5" => platforms.GetPlatform<IOS145Platform>().FirstOrDefault(),
            "iOS 14.4" => platforms.GetPlatform<IOS144Platform>().FirstOrDefault(),
            "iOS 14.3" => platforms.GetPlatform<IOS143Platform>().FirstOrDefault(),
            "iOS 14.0" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
            _ => null
        };

        var browser = platform?.Browsers.FirstOrDefault(b =>
            b.PlatformNameForOption.Equals(sp.Os, StringComparison.Ordinal) &&
            b.DeviceName.Equals(sp.LongName, StringComparison.Ordinal));

        return browser?.BrowserVersions.Count == 1
            ? browser.BrowserVersions[0]
            : browser?.BrowserVersions.FirstOrDefault(v =>
                v.PlatformNameForOption.Equals(sp.Os, StringComparison.Ordinal) &&
                v.DeviceName.Equals(sp.LongName, StringComparison.Ordinal) &&
                v.Name!.Equals(sp.BrowserVersion, StringComparison.Ordinal));
    }

    public static PlatformBase? FindIOSPlatform(this List<PlatformBase> platforms, SaucePlatform sp)
    {
        string platformToSearchFor = $"{sp.Os} {sp.LongVersion}";
        return platformToSearchFor switch
        {
            "iOS 26" => platforms.GetPlatform<IOS26Platform>().FirstOrDefault(),
            "iOS 18" => platforms.GetPlatform<IOS18Platform>().FirstOrDefault(),
            "iOS 17" => platforms.GetPlatform<IOS17Platform>().FirstOrDefault(),
            "iOS 16" => platforms.GetPlatform<IOS16Platform>().FirstOrDefault(),
            "iOS 15" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),
            "iOS 14" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
            "iOS 13" => platforms.GetPlatform<IOS13Platform>().FirstOrDefault(),
            _ => null
        };
    }

    private static BrowserBase? FindBrowser(this IEnumerable<BrowserBase> browsers, SupportedPlatform sp) =>
        browsers.FirstOrDefault(b =>
            b.Name.Equals(sp.api_name, StringComparison.Ordinal) &&
            b.DeviceName.Equals(sp.long_name, StringComparison.Ordinal) &&
            b.Os.Equals(sp.Os, StringComparison.Ordinal));

    public static BrowserVersion Classify(this BrowserVersion browserVersion)
    {
        browserVersion.PlatformType = browserVersion switch
        {
            _ when browserVersion.IsAnAndroidDevice() => PlatformType.Android,
            _ when browserVersion.IsAnAppleDevice() => PlatformType.Apple,
            _ => browserVersion.BrowserName.ToLower() switch
            {
                SauceryConstants.BROWSER_CHROME => PlatformType.Chrome,
                SauceryConstants.BROWSER_FIREFOX => PlatformType.Firefox,
                SauceryConstants.BROWSER_IE => PlatformType.IE,
                SauceryConstants.BROWSER_EDGE_LOWER => PlatformType.Edge,
                SauceryConstants.BROWSER_SAFARI => PlatformType.Safari,
                _ => browserVersion.PlatformType
            }
        };

        return browserVersion;
    }
}
