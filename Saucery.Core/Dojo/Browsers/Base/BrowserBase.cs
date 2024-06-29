using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

public abstract class BrowserBase : IBrowser
{
    public string Os { get; set; }

    public string PlatformNameForOption { get; set; }

    public string AutomationBackend { get; set; }

    public string Name { get; set; }

    public string DeviceName { get; set; }

    public string? PlatformVersion { get; set; }

    internal string? RecommendedAppiumVersion { get; set; }

    internal List<string> ScreenResolutions { get; set; }

    internal List<BrowserVersion> BrowserVersions { get; set; }


    protected BrowserBase(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption)
    {
        Os = sp.os!;
        PlatformNameForOption = platformNameForOption;
        AutomationBackend = sp.automation_backend!;
        Name = sp.api_name!;
        DeviceName = sp.long_name!;
        ScreenResolutions = screenResolutions;

        if(sp.automation_backend == null) {
            PlatformVersion = sp.OsVersion?.Split(".")[0];
        }
        else {
            if(sp.IsMobilePlatform()) {
                PlatformVersion = sp.short_version;
                RecommendedAppiumVersion = sp.recommended_backend_version;
            }
            //ScreenResolutions = screenResolutions; --might need to move back here
        }

        BrowserVersions = [];
    }

    public abstract BrowserVersion? FindVersion(SupportedPlatform sp);
    
    public abstract bool IsSupportedVersion(SupportedPlatform sp);
}