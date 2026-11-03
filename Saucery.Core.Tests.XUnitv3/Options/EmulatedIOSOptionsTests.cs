using OpenQA.Selenium.Appium;
using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Options.ConcreteProducts;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;
using Xunit;

namespace Saucery.Core.Tests.XUnitv3.Options;

public class EmulatedIOSOptionsTests {
    [Fact]
    public void CurrentMajor_DoesNotContainAppiumVersion() {
        BrowserVersion browserVersion = CreateBrowserVersion(
            SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR,
            "2.19.0");

        AppiumOptions options = CreateOptions(browserVersion);

        bool containsAppiumVersion = ContainsAppiumVersion(options);

        Assert.False(containsAppiumVersion);
    }

    [Fact]
    public void PreviousMajor_DoesNotContainAppiumVersion() {
        BrowserVersion browserVersion = CreateBrowserVersion(
            SauceryConstants.BROWSER_VERSION_PREVIOUS_MAJOR,
            "2.19.0");

        AppiumOptions options = CreateOptions(browserVersion);

        bool containsAppiumVersion = ContainsAppiumVersion(options);

        Assert.False(containsAppiumVersion);
    }

    [Fact]
    public void SpecificVersion_ContainsRecommendedAppiumVersion() {
        const string platformVersion = "18.6";
        const string recommendedAppiumVersion = "2.19.0";

        BrowserVersion browserVersion = CreateBrowserVersion(
            platformVersion,
            recommendedAppiumVersion);

        AppiumOptions options = CreateOptions(browserVersion);

        var capabilities = options.ToCapabilities();
        string capabilityName = GetAppiumVersionCapabilityName();

        Assert.True(capabilities.HasCapability(capabilityName));

        Assert.Equal(
            recommendedAppiumVersion,
            capabilities.GetCapability(capabilityName)?.ToString());
    }

    [Fact]
    public void SpecificVersion_WithNoRecommendedAppiumVersion_ContainsLatest() {
        const string platformVersion = "18.6";

        BrowserVersion browserVersion = CreateBrowserVersion(
            platformVersion,
            string.Empty);

        AppiumOptions options = CreateOptions(browserVersion);

        var capabilities = options.ToCapabilities();
        string capabilityName = GetAppiumVersionCapabilityName();

        Assert.True(capabilities.HasCapability(capabilityName));

        Assert.Equal(
            "latest",
            capabilities.GetCapability(capabilityName)?.ToString());
    }

    private static AppiumOptions CreateOptions(BrowserVersion browserVersion) {
        TestableEmulatedIOSOptions sut = new(
            browserVersion,
            nameof(EmulatedIOSOptionsTests));

        return sut.Options;
    }

    private static BrowserVersion CreateBrowserVersion(
        string platformVersion,
        string recommendedAppiumVersion) {
        SupportedPlatform supportedPlatform = new() {
            Os = SauceryConstants.PLATFORM_IOS,
            api_name = "iphone",
            long_name = "iPhone Simulator",
            short_version = platformVersion,
            latest_stable_version = platformVersion,
            automation_backend = "appium",
            recommended_backend_version = recommendedAppiumVersion,
            supported_backend_versions = [],
            deprecated_backend_versions = []
        };

        TestIOSBrowser browser = new(supportedPlatform);

        return new BrowserVersion(supportedPlatform, browser);
    }

    private static bool ContainsAppiumVersion(AppiumOptions options) {
        var capabilities = options.ToCapabilities();

        return capabilities.HasCapability(
            GetAppiumVersionCapabilityName());
    }

    private static string GetAppiumVersionCapabilityName() =>
        $"appium:{SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY}";

    private sealed class TestableEmulatedIOSOptions(
        BrowserVersion browserVersion,
        string testName)
        : EmulatedIOSOptions(browserVersion, testName) {
        public AppiumOptions Options => (AppiumOptions)Opts!;
    }

    private sealed class TestIOSBrowser : BrowserBase {
        public TestIOSBrowser(SupportedPlatform supportedPlatform)
            : base(
                supportedPlatform,
                [],
                SauceryConstants.PLATFORM_IOS) {
        }

        public override BrowserVersion? FindVersion(
            SupportedPlatform supportedPlatform) =>
            null;

        public override bool IsSupportedVersion(
            SupportedPlatform supportedPlatform) =>
            true;
    }
}