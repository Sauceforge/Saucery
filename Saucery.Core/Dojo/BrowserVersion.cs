using System.Text;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

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

    private StringBuilder TestNameBuilder { get; set; }

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
        TestNameBuilder = new StringBuilder();
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
        TestNameBuilder = new StringBuilder();
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
        TestNameBuilder = new StringBuilder();
    }

    public void SetTestName(string testName)
    {
        TestNameBuilder = new StringBuilder();

        AppendPlatformField(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
            ? testName[..testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)]
            : testName);

        if (this.IsAMobileDevice())
        {
            AppendPlatformField(DeviceName);
            AppendPlatformField(DeviceOrientation!);
        }
        else
        {
            AppendPlatformField(Os);
            AppendPlatformField(BrowserName);
            AppendPlatformField(Name!);
            AppendPlatformField(ScreenResolution!);
        }

        AppendPlatformField(DateTime.Now.ToString("yyyyMMddHHmmssffff"));

        if(TestNameBuilder.Length > 0) 
        {
            TestName = TestNameBuilder.ToString();
        }
    }

    private void AppendPlatformField(string fieldToAdd)
    {
        if (!string.IsNullOrEmpty(fieldToAdd) &&
            !TestNameBuilder.ToString().Contains(fieldToAdd))
        {
            lock (TestNameBuilder)
            {
                if (TestNameBuilder.Length == 0)
                {
                    TestNameBuilder.Append($"{fieldToAdd}");
                }
                else
                {
                    TestNameBuilder.Append($"{SauceryConstants.UNDERSCORE}{fieldToAdd}");
                }
            }
        }
    }
}