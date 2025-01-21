using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;
using System.Collections.Concurrent;

namespace Saucery.Core.Dojo;

public class BrowserVersion
{
    public string Os { get; set; }

    public string PlatformNameForOption { get; set; }

    public string BrowserName { get; set; }

    public string? Name { get; set; }

    private string AutomationBackend { get; set; }

    public string DeviceName { get; set; }

    public string RecommendedAppiumVersion { get; set; }

    private List<string> SupportedBackendVersions { get; set; }

    private List<string> DeprecatedBackendVersions { get; set; }

    public string? TestName { get; private set; }

    public string? DeviceOrientation { get; set; }

    public string? ScreenResolution { get; set; }

    public PlatformType PlatformType { get; set; }

    public List<string> ScreenResolutions { get; set; }

    private ConcurrentQueue<string> TestNameQueue { get; set; }

    public BrowserVersion(SupportedPlatform sp, BrowserBase b)
    {
        Os = sp.Os!;
        PlatformNameForOption = b.PlatformNameForOption;
        ScreenResolutions = b.ScreenResolutions;
        BrowserName = sp.api_name!;
        Name = sp.latest_stable_version != string.Empty 
            ? sp.latest_stable_version 
            : sp.short_version;
        AutomationBackend = sp.automation_backend!;
        DeviceName = sp.long_name!;
        RecommendedAppiumVersion = sp.recommended_backend_version!;
        SupportedBackendVersions = sp.supported_backend_versions!;
        DeprecatedBackendVersions = sp.deprecated_backend_versions!;
        TestNameQueue = new ConcurrentQueue<string>();
    }

    public BrowserVersion(BrowserBase b, 
                          string platformNameForOption,  
                          string latestStableVersion, 
                          List<string>? supportedBackendVersions,
                          List<string>? deprecatedBackendVersions)
    {
        Os = b.Os;
        PlatformNameForOption = platformNameForOption;
        BrowserName = b.Name;
        Name = latestStableVersion != string.Empty 
            ? latestStableVersion 
            : b.PlatformVersion;
        AutomationBackend = b.AutomationBackend;
        DeviceName = b.DeviceName;
        RecommendedAppiumVersion = b.RecommendedAppiumVersion!;
        SupportedBackendVersions = supportedBackendVersions!;
        DeprecatedBackendVersions = deprecatedBackendVersions!;
        ScreenResolutions = b.ScreenResolutions;
        TestNameQueue = new ConcurrentQueue<string>();
    }

    public BrowserVersion(SaucePlatform platform) {
        Os = platform.Os;
        PlatformNameForOption = platform.LongVersion;
        BrowserName = string.Empty;
        Name = platform.LongName;
        AutomationBackend = "appium";
        DeviceName = platform.LongName;
        RecommendedAppiumVersion = "latest";
        SupportedBackendVersions = [];
        DeprecatedBackendVersions = [];
        TestName = string.Empty;
        DeviceOrientation = string.Empty;
        ScreenResolution = string.Empty;
        PlatformType = platform.IsAnAndroidDevice() ? PlatformType.Android : PlatformType.Apple;
        ScreenResolutions = [];
        TestNameQueue = new ConcurrentQueue<string>();
    }

    public void SetTestName(string testName)
    {
        TestNameQueue = new ConcurrentQueue<string>();
        if(!TestNameQueue.Contains(testName)) 
        {
            TestNameQueue.Enqueue(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
                                    ? testName[..testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)]
                                    : testName);
        }

        if (this.IsAMobileDevice())
        {
            EnqueuePlatformField(DeviceName);
            
            if (!string.IsNullOrEmpty(DeviceOrientation))
            {
                EnqueuePlatformField(DeviceOrientation);
            }
        }
        else
        {
            EnqueuePlatformField(Os);
            EnqueuePlatformField(BrowserName);
            EnqueuePlatformField(Name!);
            
            if (!string.IsNullOrEmpty(ScreenResolution))
            {
                EnqueuePlatformField(ScreenResolution);
            }
        }

        if(!TestNameQueue.IsEmpty) 
        {
            TestName = string.Join("", TestNameQueue);
        }
    }

    private void EnqueuePlatformField(string fieldToAdd)
    {
        if(!string.IsNullOrEmpty(fieldToAdd) &&
            !TestNameQueue.IsEmpty &&
           !TestNameQueue.Contains(fieldToAdd)) 
        {
            TestNameQueue.Enqueue($"{SauceryConstants.UNDERSCORE}{fieldToAdd}");
        }
    }
}