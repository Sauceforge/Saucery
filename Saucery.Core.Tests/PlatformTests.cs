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
    private int ValidCount, InvalidCount;

    [OneTimeSetUp]
    public void OneTimeSetUp() {
        PlatformConfigurator = new(PlatformFilter.ALL);
    }

    [SetUp]
    public void Setup() {
        ValidCount = 0;
        InvalidCount = 0;
    }

    [Test]
    public void ValidDesktopPlatformTest() {
        PlatformExpander expander = new(PlatformConfigurator!, PlatformDataClass.DesktopPlatforms);
        List<SaucePlatform> expandedPlatforms = expander.Expand();
        var bvs = PlatformConfigurator!.FilterAll(expandedPlatforms);

        ProcessBrowserVersions(bvs);

        InvalidCount.ShouldBe(0);
        ValidCount.ShouldBeEquivalentTo(bvs.Count);
    }

    [Test]
    public void ValidEmulatedAndroidDevicesTest() {
        var bvs = PlatformConfigurator!.FilterAll(PlatformDataClass.EmulatedAndroidPlatforms);

        ProcessBrowserVersions(bvs);

        InvalidCount.ShouldBe(0);
        ValidCount.ShouldBeEquivalentTo(bvs.Count);
    }

    [Test]
    public void ValidEmulatedIOSDevicesTest() {
        var bvs = PlatformConfigurator!.FilterAll(PlatformDataClass.EmulatedIOSPlatforms);

        ProcessBrowserVersions(bvs);

        InvalidCount.ShouldBe(0);
        ValidCount.ShouldBeEquivalentTo(bvs.Count);
    }

    [Test]
    public void ValidRealAndroidDevicesTest() {
        var bvs = PlatformConfigurator!.FilterAll(PlatformDataClass.RealAndroidDevices);

        ProcessBrowserVersions(bvs);

        InvalidCount.ShouldBe(0);
        ValidCount.ShouldBeEquivalentTo(bvs.Count);
    }

    [Test]
    public void ValidRealIOSDevicesTest() {
        var bvs = PlatformConfigurator!.FilterAll(PlatformDataClass.RealIOSDevices);

        ProcessBrowserVersions(bvs);

        InvalidCount.ShouldBe(0);
        ValidCount.ShouldBeEquivalentTo(bvs.Count);
    }

    private void ProcessBrowserVersions(List<BrowserVersion> browserVersions) {
        foreach(var bv in browserVersions) {
            if(bv != null)
                ValidCount++;
            else
                InvalidCount++;
        }
    }

    public class PlatformDataClass {
        public static List<SaucePlatform> DesktopPlatforms
        {
            get
            {
                return
                [
                    new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119", SauceryConstants.SCREENRES_2560_1600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "100", SauceryConstants.SCREENRES_800_600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "latest"),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "latest", SauceryConstants.SCREENRES_800_600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
                    new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
                    new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
                    new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "15", SauceryConstants.SCREENRES_800_600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11", SauceryConstants.SCREENRES_800_600),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
                    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99", SauceryConstants.SCREENRES_800_600)
                ];
            }
        }

        public static List<SaucePlatform> EmulatedAndroidPlatforms
        {
            get
            {
                return 
                [
                    new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
                ];
            }
        }

        public static List<SaucePlatform> EmulatedIOSPlatforms
        {
            get
            {
                return
                [
                    new IOSPlatform("iPhone XS Simulator", "17.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
                ];
            }
        }

        public static List<SaucePlatform> RealAndroidDevices
        {
            get
            {
                return 
                [
                    new AndroidRealDevice("Google Pixel 8 | Android 15 Beta", "15"),
                    new AndroidRealDevice("Google Pixel 8 Pro", "14"),
                    new AndroidRealDevice("Google Pixel 7 Pro", "13"),
                    new AndroidRealDevice("Google Pixel 6a", "12"),
                    new AndroidRealDevice("Google Pixel 4a", "11"),
                    new AndroidRealDevice("Google Pixel 4 XL", "10"),
                    new AndroidRealDevice("Samsung Galaxy Tab S4 10.5", "9"),
                    new AndroidRealDevice("OnePlus 5", "8"),
                    new AndroidRealDevice("Samsung Galaxy Tab S2", "7"),
                    new AndroidRealDevice("Samsung Galaxy S5", "6"),
                ];
            }
        }

        public static List<SaucePlatform> RealIOSDevices
        {
            get
            {
                return
                [
                    new IOSRealDevice("iPhone 15 Plus", "18"),
                    new IOSRealDevice("iPhone 15 Pro Max", "17"),
                    new IOSRealDevice("iPhone 14 Pro Max", "16"),
                    new IOSRealDevice("iPhone 13 Pro Max", "15"),
                    new IOSRealDevice("iPhone 12 Pro Max", "14"),
                    new IOSRealDevice("iPhone 8 Plus", "13"),
                    new IOSRealDevice("iPad Air", "12"),
                ];
            }
        }
    }
}