using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.RestAPI.SupportedPlatforms;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo;

public class PlatformConfigurator
{
    private SauceLabsPlatformAcquirer PlatformAcquirer { get; set; } = new();
    private SauceLabsRealDeviceAcquirer RealDeviceAcquirer { get; set; } = new();
    public List<PlatformBase> AvailablePlatforms { get; set; } = [];
    public List<PlatformBase> AvailableRealDevices { get; set; } = [];

    public PlatformConfigurator(PlatformFilter filter)
    {
        if(AvailablePlatforms.Any())
        {
            DebugMessages.AvailablePlatformsAlreadyPopulated();
        }
        else
        {
            DebugMessages.AvailablePlatformsEmpty();
            switch (filter)
            {
                case PlatformFilter.All:
                    ConstructAllPlatforms();
                    break;
                case PlatformFilter.Emulated:
                    ConstructEmulatedPlatforms();
                    break;
                case PlatformFilter.RealDevice:
                    ConstructRealDevices();
                    break;
            }
        }
    }

    private void ConstructAllPlatforms()
    {
        ConstructRealDevices();
        ConstructEmulatedPlatforms();
    }

    private void ConstructEmulatedPlatforms()
    {
        var supportedPlatforms = PlatformAcquirer.AcquirePlatforms();
        var filteredSupportedPlatforms = FilterSupportedPlatforms(supportedPlatforms!);

        foreach (var sp in filteredSupportedPlatforms)
        {
            AvailablePlatforms.AddPlatform(sp);
        }

        AddLatestBrowserVersion(SauceryConstants.BROWSER_VERSION_LATEST);
        AddLatestBrowserVersion(SauceryConstants.BROWSER_VERSION_LATEST_MINUS1);
    }

    private void ConstructRealDevices()
    {
        var supportedRealDevices = RealDeviceAcquirer.AcquireRealDevicePlatforms();

        foreach (var sp in supportedRealDevices!)
        {
            AvailableRealDevices.AddRealPlatform(sp);
        }
    }

    private static List<SupportedPlatform> FilterSupportedPlatforms(List<SupportedPlatform> supportedPlatforms) => 
        [
            ..FindLinuxPlatforms(supportedPlatforms),
            ..FindWindowsPlatforms(supportedPlatforms),
            ..FindMacPlatforms(supportedPlatforms,
            [
                SauceryConstants.PLATFORM_MAC_1010,
                SauceryConstants.PLATFORM_MAC_1011,
                SauceryConstants.PLATFORM_MAC_1012,
                SauceryConstants.PLATFORM_MAC_1013,
                SauceryConstants.PLATFORM_MAC_1014,
                SauceryConstants.PLATFORM_MAC_1015,
                SauceryConstants.PLATFORM_MAC_11,
                SauceryConstants.PLATFORM_MAC_12,
                SauceryConstants.PLATFORM_MAC_13
            ]),
            ..FindMobilePlatforms(supportedPlatforms, ["iphone", "ipad"]),
            ..FindMobilePlatforms(supportedPlatforms, ["android"])
        ];

    internal int FindMaxBrowserVersion(SaucePlatform platform)
    {
        var availablePlatform = AvailablePlatforms.Find(p => p.Name.Equals(platform.Os));
        var browser = availablePlatform?.Browsers.Find(b => b.Name.Equals(platform.Browser));
        var numericBrowserVersions = browser?.BrowserVersions
            .Where(x =>
                x.Name!.Any(char.IsNumber) &&
                x.Name != SauceryConstants.BROWSER_VERSION_LATEST_MINUS1);
        var browserVersion = numericBrowserVersions?
            .Aggregate((maxItem, nextItem) =>
                int.Parse(maxItem.Name!) > int.Parse(nextItem.Name!)
                    ? maxItem
                    : nextItem);

        return int.Parse(browserVersion?.Name!);
    }

    private static List<SupportedPlatform> FindLinuxPlatforms(List<SupportedPlatform> platforms) =>
        platforms.FindAll(p =>
            p.Os == "Linux" &&
            p.automation_backend!.Equals("webdriver") &&
            p.device == null);

    private static List<SupportedPlatform> FindWindowsPlatforms(List<SupportedPlatform> platforms) =>
        platforms.FindAll(p =>
            p.Os!.Contains("Windows") &&
            p.automation_backend!.Equals("webdriver"));

    private static List<SupportedPlatform> FindMacPlatforms(List<SupportedPlatform> platforms, IReadOnlyCollection<string> oses) =>
        platforms.FindAll(p =>
            oses.Any(o => o.Equals(p.Os)) &&
            p.automation_backend!.Equals("webdriver") &&
            !p.api_name!.Equals("ipad") &&
            !p.api_name.Equals("iphone"));

    private static List<SupportedPlatform> FindMobilePlatforms(List<SupportedPlatform> platforms, IReadOnlyCollection<string> apis) =>
        platforms.FindAll(p =>
            apis.Any(a => a.Equals(p.api_name)) &&
            p.automation_backend!.Equals("appium"));

    private void AddLatestBrowserVersion(string version)
    {
        foreach (var browser in AvailablePlatforms
                     .SelectMany(p =>
                         p.Browsers.Where(b =>
                             p.BrowsersWithLatestVersion != null &&
                             p.BrowsersWithLatestVersion.Contains(b.Name))))
        {
            browser.BrowserVersions.Add(new BrowserVersion(browser, version, version, null, null));
        }
    }

    public List<BrowserVersion> FilterAll(List<SaucePlatform> platforms)
    {
        var browserVersions = platforms
            .Select(Filter)
            .Where(bv => bv != null)
            .ToList();

        Console.WriteLine(SauceryConstants.NUM_VALID_PLATFORMS, browserVersions.Count, platforms.Count);

        return browserVersions!;
    }

    public BrowserVersion? Filter(SaucePlatform platform)
    {
        if (platform.IsARealDevice())
        {
            return ValidateReal(platform) != null
                ? new BrowserVersion(platform)
                : null;
        }

        var browserVersion = Validate(platform);
        if (browserVersion != null)
        {
            browserVersion.ScreenResolution = platform.ScreenResolution;
            browserVersion.DeviceOrientation = platform.DeviceOrientation;
        }

        return browserVersion;
    }

    public BrowserVersion? Validate(SaucePlatform requested)
    {
        requested.Classify();
        BrowserVersion? browserVersion = requested.PlatformType switch
        {
            PlatformType.Chrome or
            PlatformType.Edge or
            PlatformType.Firefox or
            PlatformType.IE or
            PlatformType.Safari => AvailablePlatforms.FindDesktopBrowser(requested),
            PlatformType.Android => AvailablePlatforms.FindAndroidBrowser(requested),
            PlatformType.Apple => AvailablePlatforms.FindIOSBrowser(requested),
            _ => null
        };

        return browserVersion?.Classify();
    }

    private PlatformBase? ValidateReal(SaucePlatform requested)
    {
        requested.Classify();
        return requested.PlatformType switch
        {
            PlatformType.Android => AvailableRealDevices.FindAndroidPlatform(requested),
            PlatformType.Apple => AvailableRealDevices.FindIOSPlatform(requested),
            _ => null
        };
    }
}
