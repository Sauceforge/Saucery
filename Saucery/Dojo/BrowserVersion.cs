using Saucery.Dojo.Browsers.Base;
using Saucery.OnDemand;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo;

public class BrowserVersion
{
    public string Os { get; set; }
    public string PlatformNameForOption { get; set; }
    public string BrowserName { get; set; }
    public string Name { get; set; }
    public string AutomationBackend { get; set; }
    public string DeviceName { get; set; }
    public string RecommendedAppiumVersion { get; set; }
    public List<string> SupportedBackendVersions { get; set; }
    public List<string> DeprecatedBackendVersions { get; set; }
    public string TestName { get; set; }
    public string DeviceOrientation { get; set; }
    public string ScreenResolution { get; set; }

    public PlatformType PlatformType { get; set; }
    public List<string> ScreenResolutions { get; set; }

    public BrowserVersion(SupportedPlatform sp, BrowserBase b)
    {
        Os = sp.os;
        PlatformNameForOption = b.PlatformNameForOption;
        ScreenResolutions = b.ScreenResolutions;
        BrowserName = sp.api_name;
        Name = sp.latest_stable_version != string.Empty ? sp.latest_stable_version : sp.short_version;
        AutomationBackend = sp.automation_backend;
        DeviceName = sp.long_name;
        RecommendedAppiumVersion = sp.recommended_backend_version;
        SupportedBackendVersions = sp.supported_backend_versions;
        DeprecatedBackendVersions = sp.deprecated_backend_versions;
    }

    public BrowserVersion(BrowserBase b, 
                          string platformNameForOption,  
                          string latest_stable_version, 
                          List<string> supported_backend_versions,
                          List<string> deprecated_backend_versions)
    {
        Os = b.Os;
        PlatformNameForOption = platformNameForOption;
        BrowserName = b.Name;
        Name = latest_stable_version != string.Empty ? latest_stable_version : b.PlatformVersion;
        AutomationBackend = b.AutomationBackend;
        DeviceName = b.DeviceName;
        RecommendedAppiumVersion = b.RecommendedAppiumVersion;
        SupportedBackendVersions = supported_backend_versions;
        DeprecatedBackendVersions = deprecated_backend_versions;
        ScreenResolutions = b.ScreenResolutions;
    }
}