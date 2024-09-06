using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.Base;

public abstract class PlatformBase
{
    public string Name { get; set; }
    public abstract string PlatformNameForOption {get;set;}
    public string AutomationBackend { get; set; }
    public string RecommendedAppiumVersion { get; set; }
    public List<string> SupportedBackendVersions { get; set; }
    public List<string> DeprecatedBackendVersions { get; set; }
    public string? PlatformVersion { get; set; }
    public List<string>? Selenium4BrowserNames { get; set; }
    public List<string>? ScreenResolutions { get; set; }
    public List<string>? BrowsersWithLatestVersion { get; set; }

    public List<BrowserBase> Browsers { get; set; }

    protected PlatformBase(SupportedPlatform sp)
    {
        Name = sp.Os!;
        AutomationBackend = sp.automation_backend!;
        RecommendedAppiumVersion = sp.recommended_backend_version!;
        SupportedBackendVersions = sp.supported_backend_versions!;
        DeprecatedBackendVersions = sp.deprecated_backend_versions!;

        if(sp.automation_backend == null)
            PlatformVersion = sp.OsVersion?.Split(".")[0];
        else {
            if(sp.IsMobilePlatform()) {
                PlatformVersion = sp.short_version;
            }
        }
        
        Browsers = [];
    }
}
