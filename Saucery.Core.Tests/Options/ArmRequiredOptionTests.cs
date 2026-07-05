using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.Google;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.Core.Options.Base;
using Saucery.Core.Options.ConcreteProducts;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;
using Shouldly;

using SauceryPlatformType = Saucery.Core.OnDemand.PlatformType;

namespace Saucery.Core.Tests.Options;

public sealed class ArmRequiredOptionTests {
    public enum OptionKind {
        Chrome,
        Edge,
        Firefox,
        IE,
        Safari,
        EmulatedIOS,
        EmulatedAndroid,
        RealDeviceAndroid,
        RealDeviceIOS
    }

    [Test]
    [Arguments(OptionKind.Chrome)]
    [Arguments(OptionKind.Edge)]
    [Arguments(OptionKind.Firefox)]
    [Arguments(OptionKind.IE)]
    [Arguments(OptionKind.Safari)]
    [Arguments(OptionKind.EmulatedIOS)]
    public async Task Target_option_builders_add_armRequired_when_browser_version_requires_arm(
        OptionKind optionKind) {
        var browserVersion = CreateBrowserVersionFor(optionKind, isArmRequired: true);

        var options = CreateOptions(optionKind, browserVersion);
        var driverOptions = options.GetOpts(browserVersion.PlatformType);

        driverOptions.ShouldNotBeNull();

        var sauceOptions = GetSauceOptions(driverOptions!);

        sauceOptions.ContainsKey(SauceryConstants.ARM_REQUIRED_CAPABILITY).ShouldBeTrue();
        sauceOptions[SauceryConstants.ARM_REQUIRED_CAPABILITY].ShouldBe(true);

        await Task.CompletedTask;
    }

    [Test]
    [Arguments(OptionKind.Chrome)]
    [Arguments(OptionKind.Edge)]
    [Arguments(OptionKind.Firefox)]
    [Arguments(OptionKind.IE)]
    [Arguments(OptionKind.Safari)]
    [Arguments(OptionKind.EmulatedIOS)]
    public async Task Target_option_builders_do_not_add_armRequired_when_browser_version_does_not_require_arm(
        OptionKind optionKind) {
        var browserVersion = CreateBrowserVersionFor(optionKind, isArmRequired: false);

        var options = CreateOptions(optionKind, browserVersion);
        var driverOptions = options.GetOpts(browserVersion.PlatformType);

        driverOptions.ShouldNotBeNull();

        var sauceOptions = GetSauceOptions(driverOptions!);

        sauceOptions.ContainsKey(SauceryConstants.ARM_REQUIRED_CAPABILITY).ShouldBeFalse();

        await Task.CompletedTask;
    }

    [Test]
    [Arguments(OptionKind.EmulatedAndroid)]
    [Arguments(OptionKind.RealDeviceAndroid)]
    [Arguments(OptionKind.RealDeviceIOS)]
    public async Task Non_target_option_builders_do_not_add_armRequired_even_when_browser_version_requires_arm(
        OptionKind optionKind) {
        var browserVersion = CreateBrowserVersionFor(optionKind, isArmRequired: true);

        var options = CreateOptions(optionKind, browserVersion);
        var driverOptions = options.GetOpts(browserVersion.PlatformType);

        driverOptions.ShouldNotBeNull();

        var sauceOptions = GetSauceOptions(driverOptions!);

        sauceOptions.ContainsKey(SauceryConstants.ARM_REQUIRED_CAPABILITY).ShouldBeFalse();

        await Task.CompletedTask;
    }

    private static BaseOptions CreateOptions(OptionKind optionKind, BrowserVersion browserVersion) =>
        optionKind switch {
            OptionKind.Chrome => new ChromeBrowserOptions(browserVersion, "arm-required-test"),
            OptionKind.Edge => new EdgeBrowserOptions(browserVersion, "arm-required-test"),
            OptionKind.Firefox => new FirefoxBrowserOptions(browserVersion, "arm-required-test"),
            OptionKind.IE => new IEBrowserOptions(browserVersion, "arm-required-test"),
            OptionKind.Safari => new SafariBrowserOptions(browserVersion, "arm-required-test"),
            OptionKind.EmulatedIOS => new EmulatedIOSOptions(browserVersion, "arm-required-test"),
            OptionKind.EmulatedAndroid => new EmulatedAndroidOptions(browserVersion, "arm-required-test"),
            OptionKind.RealDeviceAndroid => new RealDeviceAndroidOptions(browserVersion, "arm-required-test"),
            OptionKind.RealDeviceIOS => new RealDeviceIOSOptions(browserVersion, "arm-required-test"),
            _ => throw new ArgumentOutOfRangeException(nameof(optionKind), optionKind, null)
        };

    private static BrowserVersion CreateBrowserVersionFor(OptionKind optionKind, bool isArmRequired) =>
        optionKind switch {
            OptionKind.Chrome => CreateDesktopBrowserVersion(
                CreateSupportedDesktopPlatform(
                    SauceryConstants.PLATFORM_MAC_14,
                    SauceryConstants.BROWSER_CHROME),
                platform => new ChromeBrowser(
                    platform,
                    [SauceryConstants.SCREENRES_1024_768],
                    "macOS 14",
                    isArmRequired),
                SauceryPlatformType.Chrome),

            OptionKind.Edge => CreateDesktopBrowserVersion(
                CreateSupportedDesktopPlatform(
                    SauceryConstants.PLATFORM_MAC_14,
                    SauceryConstants.BROWSER_EDGE),
                platform => new EdgeBrowser(
                    platform,
                    [SauceryConstants.SCREENRES_1024_768],
                    "macOS 14",
                    isArmRequired),
                SauceryPlatformType.Edge),

            OptionKind.Firefox => CreateDesktopBrowserVersion(
                CreateSupportedDesktopPlatform(
                    SauceryConstants.PLATFORM_MAC_14,
                    SauceryConstants.BROWSER_FIREFOX),
                platform => new FirefoxBrowser(
                    platform,
                    [SauceryConstants.SCREENRES_1024_768],
                    "macOS 14",
                    isArmRequired),
                SauceryPlatformType.Firefox),

            OptionKind.IE => CreateDesktopBrowserVersion(
                CreateSupportedDesktopPlatform(
                    SauceryConstants.PLATFORM_WINDOWS_10,
                    SauceryConstants.BROWSER_IE),
                platform => new IEBrowser(
                    platform,
                    [SauceryConstants.SCREENRES_1024_768],
                    "Windows 10",
                    isArmRequired),
                SauceryPlatformType.IE),

            OptionKind.Safari => CreateDesktopBrowserVersion(
                CreateSupportedDesktopPlatform(
                    SauceryConstants.PLATFORM_MAC_14,
                    SauceryConstants.BROWSER_SAFARI),
                platform => new SafariBrowser(
                    platform,
                    [SauceryConstants.SCREENRES_1024_768],
                    "macOS 14",
                    isArmRequired),
                SauceryPlatformType.Safari),

            OptionKind.EmulatedIOS => CreateEmulatedIosBrowserVersion(isArmRequired),

            OptionKind.EmulatedAndroid => CreateEmulatedAndroidBrowserVersion(isArmRequired),

            OptionKind.RealDeviceAndroid => CreateRealDeviceAndroidBrowserVersion(isArmRequired),

            OptionKind.RealDeviceIOS => CreateRealDeviceIosBrowserVersion(isArmRequired),

            _ => throw new ArgumentOutOfRangeException(nameof(optionKind), optionKind, null)
        };

    private static SupportedPlatform CreateSupportedDesktopPlatform(string os, string browserName) =>
        new() {
            Os = os,
            api_name = browserName,
            automation_backend = "webdriver",
            long_name = string.Empty,
            latest_stable_version = "latest",
            short_version = "latest",
            recommended_backend_version = "latest",
            supported_backend_versions = [],
            deprecated_backend_versions = []
        };

    private static BrowserVersion CreateDesktopBrowserVersion(
        SupportedPlatform supportedPlatform,
        Func<SupportedPlatform, BrowserBase> createBrowser,
        SauceryPlatformType platformType) {
        var browser = createBrowser(supportedPlatform);

        return new BrowserVersion(
            browser,
            browser.PlatformNameForOption,
            "latest",
            [],
            []) {
            ScreenResolution = SauceryConstants.SCREENRES_1024_768,
            PlatformType = platformType
        };
    }

    private static BrowserVersion CreateEmulatedIosBrowserVersion(bool isArmRequired) {
        var supportedPlatform = new SupportedPlatform {
            Os = SauceryConstants.PLATFORM_IOS,
            api_name = "iphone",
            automation_backend = "appium",
            long_name = "iPhone 15 Simulator",
            latest_stable_version = "17.5",
            short_version = "17.5",
            recommended_backend_version = "latest",
            supported_backend_versions = [],
            deprecated_backend_versions = []
        };

        var browser = new IOSBrowser(
            supportedPlatform,
            [],
            SauceryConstants.PLATFORM_IOS,
            isArmRequired);

        return new BrowserVersion(
            browser,
            SauceryConstants.PLATFORM_IOS,
            "17.5",
            [],
            []) {
            DeviceName = "iPhone 15 Simulator",
            DeviceOrientation = "portrait",
            PlatformType = SauceryPlatformType.Apple
        };
    }

    private static BrowserVersion CreateEmulatedAndroidBrowserVersion(bool isArmRequired) {
        var supportedPlatform = new SupportedPlatform {
            Os = SauceryConstants.ANDROID,
            api_name = SauceryConstants.ANDROID,
            automation_backend = "appium",
            long_name = "Google Pixel 9 GoogleAPI Emulator",
            latest_stable_version = "15",
            short_version = "15",
            recommended_backend_version = "latest",
            supported_backend_versions = [],
            deprecated_backend_versions = []
        };

        var browser = new AndroidBrowser(
            supportedPlatform,
            SauceryConstants.ANDROID,
            isArmRequired);

        return new BrowserVersion(
            browser,
            SauceryConstants.ANDROID,
            "15",
            [],
            []) {
            DeviceName = "Google Pixel 9 GoogleAPI Emulator",
            DeviceOrientation = "portrait",
            PlatformType = SauceryPlatformType.Android
        };
    }

    private static BrowserVersion CreateRealDeviceAndroidBrowserVersion(bool isArmRequired) {
        var browserVersion = CreateEmulatedAndroidBrowserVersion(isArmRequired);

        browserVersion.DeviceName = "Google Pixel.*";
        browserVersion.PlatformNameForOption = "15";
        browserVersion.PlatformType = SauceryPlatformType.Android;

        return browserVersion;
    }

    private static BrowserVersion CreateRealDeviceIosBrowserVersion(bool isArmRequired) {
        var browserVersion = CreateEmulatedIosBrowserVersion(isArmRequired);

        browserVersion.DeviceName = "iPhone.*";
        browserVersion.PlatformNameForOption = "17.5";
        browserVersion.PlatformType = SauceryPlatformType.Apple;

        return browserVersion;
    }

    private static IDictionary<string, object> GetSauceOptions(DriverOptions driverOptions) {
        var capabilities = driverOptions.ToCapabilities();

        var rawSauceOptions = capabilities.GetCapability(SauceryConstants.SAUCE_OPTIONS_CAPABILITY);

        rawSauceOptions.ShouldNotBeNull();
        rawSauceOptions.ShouldBeAssignableTo<IDictionary<string, object>>();

        return (IDictionary<string, object>)rawSauceOptions!;
    }
}