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
        
        if (sp.IsMobilePlatform())
        {
            PlatformVersion = sp.short_version;
            RecommendedAppiumVersion = sp.recommended_backend_version;
        }
        ScreenResolutions = screenResolutions;
        BrowserVersions = new List<BrowserVersion>();
    }

    public abstract BrowserVersion? FindVersion(SupportedPlatform sp);
    
    public abstract bool IsSupportedVersion(SupportedPlatform sp);
}