using OpenQA.Selenium.Appium;
using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Options.ConcreteProducts;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Tests.Options;

public class EmulatedAndroidOptionsTests {
    [Test]
    public async Task CurrentMajor_DoesNotContainAppiumVersion() {
        BrowserVersion browserVersion = CreateBrowserVersion(
            SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR,
            "2.19.0");

        AppiumOptions options = CreateOptions(browserVersion);

        bool containsAppiumVersion = ContainsAppiumVersion(options);

        await Assert.That(containsAppiumVersion).IsFalse();
    }

    [Test]
    public async Task PreviousMajor_DoesNotContainAppiumVersion() {
        BrowserVersion browserVersion = CreateBrowserVersion(
            SauceryConstants.BROWSER_VERSION_PREVIOUS_MAJOR,
            "2.19.0");

        AppiumOptions options = CreateOptions(browserVersion);

        bool containsAppiumVersion = ContainsAppiumVersion(options);

        await Assert.That(containsAppiumVersion).IsFalse();
    }

    [Test]
    public async Task SpecificVersion_ContainsRecommendedAppiumVersion() {
        const string platformVersion = "15.0";
        const string recommendedAppiumVersion = "2.19.0";

        BrowserVersion browserVersion = CreateBrowserVersion(
            platformVersion,
            recommendedAppiumVersion);

        AppiumOptions options = CreateOptions(browserVersion);

        var capabilities = options.ToCapabilities();
        string capabilityName = GetAppiumVersionCapabilityName();

        await Assert.That(
            capabilities.HasCapability(capabilityName)).IsTrue();

        await Assert.That(
                capabilities.GetCapability(capabilityName)?.ToString())
            .IsEqualTo(recommendedAppiumVersion);
    }

    [Test]
    public async Task SpecificVersion_WithNoRecommendedAppiumVersion_ContainsLatest() {
        const string platformVersion = "15.0";

        BrowserVersion browserVersion = CreateBrowserVersion(
            platformVersion,
            string.Empty);

        AppiumOptions options = CreateOptions(browserVersion);

        var capabilities = options.ToCapabilities();
        string capabilityName = GetAppiumVersionCapabilityName();

        await Assert.That(
            capabilities.HasCapability(capabilityName)).IsTrue();

        await Assert.That(
                capabilities.GetCapability(capabilityName)?.ToString())
            .IsEqualTo("latest");
    }

    private static AppiumOptions CreateOptions(BrowserVersion browserVersion) {
        TestableEmulatedAndroidOptions sut = new(
            browserVersion,
            nameof(EmulatedAndroidOptionsTests));

        return sut.Options;
    }

    private static BrowserVersion CreateBrowserVersion(
        string platformVersion,
        string recommendedAppiumVersion) {
        SupportedPlatform supportedPlatform = new() {
            Os = SauceryConstants.ANDROID,
            api_name = "android",
            long_name = "Android Emulator",
            short_version = platformVersion,
            latest_stable_version = platformVersion,
            automation_backend = "appium",
            recommended_backend_version = recommendedAppiumVersion,
            supported_backend_versions = [],
            deprecated_backend_versions = []
        };

        TestAndroidBrowser browser = new(supportedPlatform);

        return new BrowserVersion(supportedPlatform, browser);
    }

    private static bool ContainsAppiumVersion(AppiumOptions options) {
        var capabilities = options.ToCapabilities();

        return capabilities.HasCapability(
            GetAppiumVersionCapabilityName());
    }

    private static string GetAppiumVersionCapabilityName() =>
        $"appium:{SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY}";

    private sealed class TestableEmulatedAndroidOptions(
        BrowserVersion browserVersion,
        string testName)
        : EmulatedAndroidOptions(browserVersion, testName) {
        public AppiumOptions Options => (AppiumOptions)Opts!;
    }

    private sealed class TestAndroidBrowser(SupportedPlatform supportedPlatform) : BrowserBase(
            supportedPlatform,
            [],
            SauceryConstants.ANDROID) {
        public override BrowserVersion? FindVersion(
            SupportedPlatform supportedPlatform) =>
            null;

        public override bool IsSupportedVersion(
            SupportedPlatform supportedPlatform) =>
            true;
    }
}