using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;
using System.Text;

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
        Os = sp.os!;
        PlatformNameForOption = b.PlatformNameForOption;
        ScreenResolutions = b.ScreenResolutions;
        BrowserName = sp.api_name!;
        Name = sp.latest_stable_version != string.Empty ? sp.latest_stable_version : sp.short_version;
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
        Name = latestStableVersion != string.Empty ? latestStableVersion : b.PlatformVersion;
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

    //public BrowserVersion(string os,
    //                      string platformNameForOption,
    //                      string browserName,
    //                      string name,
    //                      string automationBackend,
    //                      string deviceName,
    //                      string recommendedAppiumVersion,
    //                      List<string> supportedBackendVersions,
    //                      List<string> deprecatedBackendVersions,
    //                      string testName,
    //                      string deviceOrientation,
    //                      string screenResolution,
    //                      PlatformType platformType,
    //                      List<string> screenResolutions)
    //{
    //    Os = os;
    //    PlatformNameForOption = platformNameForOption;
    //    BrowserName = browserName; 
    //    Name = name;
    //    AutomationBackend = automationBackend;
    //    DeviceName = deviceName;
    //    RecommendedAppiumVersion = recommendedAppiumVersion;
    //    SupportedBackendVersions = supportedBackendVersions;
    //    DeprecatedBackendVersions = deprecatedBackendVersions;
    //    TestName = testName;
    //    DeviceOrientation = deviceOrientation;
    //    ScreenResolution = screenResolution;
    //    PlatformType = platformType;
    //    ScreenResolutions = screenResolutions;
    //    TestNameBuilder = new StringBuilder();
    //}

    public object[] ToObjectArray() =>
        [
            Os,
            PlatformNameForOption,
            BrowserName,
            Name!,
            AutomationBackend,
            DeviceName,
            RecommendedAppiumVersion,
            SupportedBackendVersions,
            DeprecatedBackendVersions,
            TestName!,
            DeviceOrientation!,
            ScreenResolution!,
            PlatformType,
            ScreenResolutions
        ];

    public void SetTestName(string testName)
    {
        TestNameBuilder.Clear();
        TestNameBuilder.Append(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
                                ? testName[..testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)]
                                : testName);

        if (this.IsAMobileDevice())
        {
            AppendPlatformField(DeviceName);
            AppendPlatformField(Name!);
            
            if (!string.IsNullOrEmpty(DeviceOrientation))
            {
                AppendPlatformField(DeviceOrientation);
            }
        }
        else
        {
            AppendPlatformField(Os);
            AppendPlatformField(BrowserName);
            AppendPlatformField(Name!);
            
            if (!string.IsNullOrEmpty(ScreenResolution))
            {
                AppendPlatformField(ScreenResolution);
            }
        }

        TestName = TestNameBuilder.ToString();

        //TestName = this.IsAMobileDevice()
        //    ? string.IsNullOrEmpty(DeviceOrientation)
        //        ? AppendPlatformField(AppendPlatformField(DeviceName), Name).ToString()
        //        : AppendPlatformField(AppendPlatformField(AppendPlatformField(DeviceName), Name), DeviceOrientation).ToString()
        //    : string.IsNullOrEmpty(ScreenResolution)
        //        ? AppendPlatformField(AppendPlatformField(AppendPlatformField(Os), BrowserName), Name).ToString()
        //        : AppendPlatformField(AppendPlatformField(AppendPlatformField(AppendPlatformField(Os), BrowserName), Name), ScreenResolution).ToString();
    }

    private void AppendPlatformField(string fieldToAdd)
    {
        TestNameBuilder.Append($"{SauceryConstants.UNDERSCORE}{fieldToAdd}");
    }
}