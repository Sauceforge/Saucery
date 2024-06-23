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
    public List<SupportedRealDevicePlatform> AvailableRealDevices { get; set; } = [];

    public PlatformConfigurator(PlatformFilter filter)
    {
        switch (filter)
        {
            case PlatformFilter.ALL:
                ConstructAllPlatforms();
                break;
            case PlatformFilter.EMULATED:
                ConstructEmulatedPlatforms();
                break;
            case PlatformFilter.REALDEVICE:
                ConstructRealDevices();
                break;
            default:
                break;
        }
    }

    //public PlatformConfigurator()
    //{
    //    ConstructAllPlatforms();
    //}

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

        AvailableRealDevices.AddRange(supportedRealDevices!);

        //foreach (var rd in supportedRealDevices)
        //{
        //    AvailableRealDevices.AddPlatform(rd);
        //}
    }

    private static List<SupportedPlatform> FilterSupportedPlatforms(List<SupportedPlatform> supportedPlatforms)
    {
        List<SupportedPlatform> filteredPlatforms =
        [
            //Not filtered for Min and Max Versions yet
            .. FindLinuxPlatforms(supportedPlatforms),
            .. FindWindowsPlatforms(supportedPlatforms),
            .. FindMacPlatforms(supportedPlatforms, [ SauceryConstants.PLATFORM_MAC_1010,
                                                      SauceryConstants.PLATFORM_MAC_1011,
                                                      SauceryConstants.PLATFORM_MAC_1012,
                                                      SauceryConstants.PLATFORM_MAC_1013,
                                                      SauceryConstants.PLATFORM_MAC_1014,
                                                      SauceryConstants.PLATFORM_MAC_1015,
                                                      SauceryConstants.PLATFORM_MAC_11,
                                                      SauceryConstants.PLATFORM_MAC_12,
                                                      SauceryConstants.PLATFORM_MAC_13 ]),
            .. FindMobilePlatforms(supportedPlatforms, ["iphone", "ipad"]),
            .. FindMobilePlatforms(supportedPlatforms, ["android"]),
        ];

        return filteredPlatforms;
    }

    internal int FindMaxBrowserVersion(SaucePlatform platform)
    {
        //Desktop Platform Only
        var availablePlatform = AvailablePlatforms.Find(p => p.Name.Equals(platform.Os));
        var browser = availablePlatform?.Browsers.Find(b => b.Name.Equals(platform.Browser));
        var numericBrowserVersions = browser?.BrowserVersions.Where(x => x.Name!.Any(char.IsNumber) && x.Name != SauceryConstants.BROWSER_VERSION_LATEST_MINUS1);
        var browserVersion = numericBrowserVersions?.Aggregate((maxItem, nextItem) => (int.Parse(maxItem.Name!) > int.Parse(nextItem.Name!)) ? maxItem : nextItem);

        return int.Parse(browserVersion?.Name!);
    }

    private static List<SupportedPlatform> FindLinuxPlatforms(List<SupportedPlatform> platforms) => platforms.FindAll(p => p.os! == "Linux" && p.automation_backend!.Equals("webdriver") && p.device! == null);

    private static List<SupportedPlatform> FindWindowsPlatforms(List<SupportedPlatform> platforms) => platforms.FindAll(p => p.os!.Contains("Windows") && p.automation_backend!.Equals("webdriver"));

    private static List<SupportedPlatform> FindMacPlatforms(List<SupportedPlatform> platforms, IReadOnlyCollection<string> oses) => platforms.FindAll(p => oses.Any(o => o.Equals(p.os)) && p.automation_backend!.Equals("webdriver") && !p.api_name!.Equals("ipad") && !p.api_name.Equals("iphone"));

    private static List<SupportedPlatform> FindMobilePlatforms(List<SupportedPlatform> platforms, IReadOnlyCollection<string> apis) => platforms.FindAll(p => apis.Any(a => a.Equals(p.api_name)) && p.automation_backend!.Equals("appium"));

    public void AddLatestBrowserVersion(string version)
    {
        foreach (var b in AvailablePlatforms.SelectMany(p => p.Browsers.Where(b => p.BrowsersWithLatestVersion != null && p.BrowsersWithLatestVersion.Contains(b.Name))))
        {
            b.BrowserVersions.Add(new BrowserVersion(b, version, version, null, null));
        }
    }

    public List<BrowserVersion> FilterAll(List<SaucePlatform> platforms)
    {
        var bvs = (from p in platforms
                   let bv = Filter(p)
                   where bv != null
                   select bv).ToList();
        Console.WriteLine(SauceryConstants.NUM_VALID_PLATFORMS, bvs.Count, platforms.Count);
        return bvs;
    }

    public BrowserVersion? Filter(SaucePlatform platform)
    {
        var bv = Validate(platform);
        if (bv != null)
        {
            bv.ScreenResolution = platform.ScreenResolution;
            bv.DeviceOrientation = platform.DeviceOrientation;
        }
        return bv;
    }

    public BrowserVersion? Validate(SaucePlatform requested)
    {
        requested.Classify();
        BrowserVersion? browserVersion;
        switch (requested.PlatformType)
        {
            case PlatformType.Chrome:
            case PlatformType.Edge:
            case PlatformType.Firefox:
            case PlatformType.IE:
            case PlatformType.Safari:
                browserVersion = AvailablePlatforms.FindDesktopBrowser(requested);
                break;
            case PlatformType.Android:
                browserVersion = AvailablePlatforms.FindAndroidBrowser(requested);
                break;
            case PlatformType.Apple:
                browserVersion = AvailablePlatforms.FindIOSBrowser(requested);
                break;
            default:
                Console.WriteLine($"Requested Platform Not Found: {0}", requested.LongName);
                return null;
        }

        return browserVersion?.Classify();
    }
}