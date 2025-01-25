using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Tests.DataProviders;
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
    public void TestNameTest(SaucePlatform platform) {
        var bvs = PlatformConfigurator!.FilterAll([platform]);

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
}