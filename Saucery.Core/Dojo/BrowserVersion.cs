using System.Text;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo;

public class BrowserVersion {
    public string Os { get; set; }

    public string PlatformNameForOption { get; set; }

    public string BrowserName { get; set; }

    public string? Name { get; set; }

    private string AutomationBackend { get; set; }

    public string DeviceName { get; set; }

    public string RecommendedAppiumVersion { get; set; }

    private List<string> SupportedBackendVersions { get; set; }

    private List<string> DeprecatedBackendVersions { get; set; }

    public string? DeviceOrientation { get; set; }

    public string? ScreenResolution { get; set; }

    public PlatformType PlatformType { get; set; }

    public List<string> ScreenResolutions { get; set; }

    public BrowserVersion(SupportedPlatform sp, BrowserBase b) {
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
    }

    public BrowserVersion(BrowserBase b,
                          string platformNameForOption,
                          string latestStableVersion,
                          List<string>? supportedBackendVersions,
                          List<string>? deprecatedBackendVersions) {
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
        DeviceOrientation = string.Empty;
        ScreenResolution = string.Empty;
        PlatformType = platform.IsAnAndroidDevice() ? PlatformType.Android : PlatformType.Apple;
        ScreenResolutions = [];
    }

    /// <summary>
    /// Generates a unique test name from this configuration and the test context.
    /// This does not mutate the BrowserVersion instance.
    /// </summary>
    public static string GenerateTestName(BrowserVersion config, string baseTestName) {
        var builder = new StringBuilder();

        // Remove any existing bracketed suffix from test name
        var cleanTestName = baseTestName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
            ? baseTestName[..baseTestName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)]
            : baseTestName;

        AppendIfNotEmpty(builder, cleanTestName);

        if(config.IsAMobileDevice()) {
            AppendIfNotEmpty(builder, config.DeviceName);
            AppendIfNotEmpty(builder, config.DeviceOrientation!);
        } else {
            AppendIfNotEmpty(builder, config.Os);
            AppendIfNotEmpty(builder, config.BrowserName);
            AppendIfNotEmpty(builder, config.Name!);
            AppendIfNotEmpty(builder, config.ScreenResolution!);
        }

        // Add timestamp for uniqueness
        AppendIfNotEmpty(builder, DateTime.Now.ToString("yyyyMMddHHmmssffff"));

        return builder.ToString();
    }

    private static void AppendIfNotEmpty(StringBuilder builder, string value) {
        if(!string.IsNullOrEmpty(value) && !builder.ToString().Contains(value)) {
            if(builder.Length == 0) {
                builder.Append(value);
            } else {
                builder.Append($"{SauceryConstants.UNDERSCORE}{value}");
            }
        }
    }
}