using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests;

[TestFixture]
public class MerlinPlatformTests {
    private PlatformConfigurator? PlatformConfigurator { get; set; }
    private int _validCount;

    [OneTimeSetUp]
    public void OneTimeSetUp() {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }

    [SetUp]
    public void Setup() {
        _validCount = 0;
    }

    [Test]
    public void ValidDesktopPlatformTest() {
        PlatformExpander expander = new(PlatformConfigurator!, PlatformDataClass.DesktopPlatforms);
        var expandedPlatforms = expander.Expand();
        var bvs = PlatformConfigurator!
            .FilterAll(expandedPlatforms);

        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidEmulatedAndroidDevicesTest() {
        var bvs = PlatformConfigurator!
            .FilterAll(PlatformDataClass.EmulatedAndroidPlatforms);

        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidEmulatedIOSDevicesTest() {
        var bvs = PlatformConfigurator!
            .FilterAll(PlatformDataClass.EmulatedIOSPlatforms);

        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidRealAndroidDevicesTest() {
        var bvs = PlatformConfigurator!
            .FilterAll(PlatformDataClass.RealAndroidDevices);

        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidRealIOSDevicesTest() {
        var bvs = PlatformConfigurator!
            .FilterAll(PlatformDataClass.RealIOSDevices);

        ProcessBrowserVersions(bvs);
    }

    [Theory]
    [TestCaseSource(typeof(PlatformDataClass), nameof(PlatformDataClass.DesktopPlatforms))]
    [TestCaseSource(typeof(PlatformDataClass), nameof(PlatformDataClass.EmulatedAndroidPlatforms))]
    [TestCaseSource(typeof(PlatformDataClass), nameof(PlatformDataClass.EmulatedIOSPlatforms))]
    [TestCaseSource(typeof(PlatformDataClass), nameof(PlatformDataClass.RealAndroidDevices))]
    [TestCaseSource(typeof(PlatformDataClass), nameof(PlatformDataClass.RealIOSDevices))]
    public void TestNameTest(SaucePlatform platforms) {
        var bvs = PlatformConfigurator!.FilterAll([platforms]);

        foreach(var bv in bvs) {
            bv.SetTestName(TestContext.CurrentContext.Test.Name);
            bv.TestName.ShouldNotBeNullOrEmpty();

            if (bv.IsAMobileDevice())
            {
                bv.TestName.ShouldContain(bv.DeviceName);
                bv.TestName.ShouldContain(bv.DeviceOrientation!);
            }
            else
            {
                bv.TestName.ShouldContain(bv.Os);
                bv.TestName.ShouldContain(bv.BrowserName);
                bv.TestName.ShouldContain(bv.Name!);
                if(!string.IsNullOrEmpty(bv.ScreenResolution)) {
                    bv.TestName.ShouldContain(bv.ScreenResolution);
                }
            }
        }
    }

    private void ProcessBrowserVersions(List<BrowserVersion> browserVersions) {
        foreach(var bv in browserVersions) {
            if(bv != null)
                _validCount++;
        }

        _validCount.ShouldBeEquivalentTo(browserVersions.Count);
    }

    private static class PlatformDataClass {
        public static List<SaucePlatform> DesktopPlatforms =>
        [
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "123"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "124", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "100", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "15", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99", SauceryConstants.SCREENRES_800_600)
        ];

        public static List<SaucePlatform> EmulatedAndroidPlatforms =>
        [
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
        ];

        public static List<SaucePlatform> EmulatedIOSPlatforms =>
        [
            new IOSPlatform("iPhone 15 Pro Max Simulator", "17.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
        ];

        public static List<SaucePlatform> RealAndroidDevices =>
        [
            new AndroidRealDevice("Google.*", "15"),
            new AndroidRealDevice("Google Pixel 8 | Android 15 Beta", "15"),
            new AndroidRealDevice("Google Pixel 8 Pro", "14"),
            new AndroidRealDevice("Google Pixel 7 Pro", "13"),
            new AndroidRealDevice("Google Pixel 6a", "12"),
            new AndroidRealDevice("Google Pixel 4a", "11"),
            new AndroidRealDevice("Google Pixel 4 XL", "10"),
            new AndroidRealDevice("Samsung Galaxy Tab S4 10.5", "9")
        ];

        public static List<SaucePlatform> RealIOSDevices =>
        [
            new IOSRealDevice("iPhone 15 Plus", "18"),
            new IOSRealDevice("iPhone 15 Pro Max", "17"),
            new IOSRealDevice("iPhone 14 Pro Max", "16"),
            new IOSRealDevice("iPhone 13 Pro Max", "15"),
            new IOSRealDevice("iPhone 12 Pro Max", "14"),
            new IOSRealDevice("iPhone 8 Plus", "13"),
        ];
    }
}