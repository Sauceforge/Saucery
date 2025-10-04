using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Tests.DataProviders;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class MerlinPlatformTests(PlatformConfiguratorFixture fixture) : IClassFixture<PlatformConfiguratorFixture> 
{
    private readonly PlatformConfiguratorFixture _fixture = fixture;
    private int _validCount = 0;

    [Fact]
    public void ValidDesktopPlatformTest()
    {
        PlatformExpander expander = new(_fixture.PlatformConfigurator, PlatformDataClass.DesktopPlatforms);
        var expandedPlatforms = expander.Expand();
        var bvs = _fixture.PlatformConfigurator.FilterAll(expandedPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Fact]
    public void ValidEmulatedAndroidDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.EmulatedAndroidPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Fact]
    public void ValidEmulatedIOSDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.EmulatedIOSPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Fact]
    public void ValidRealAndroidDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator .FilterAll(PlatformDataClass.RealAndroidDevices);
        ProcessBrowserVersions(bvs);
    }

    [Fact]
    public void ValidRealIOSDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.RealIOSDevices);
        ProcessBrowserVersions(bvs);
    }

    public static IEnumerable<object[]> AllPlatforms()
    {
        foreach (var platform in PlatformDataClass.DesktopPlatforms)
            yield return new object[] { platform };
        foreach (var platform in PlatformDataClass.EmulatedAndroidPlatforms)
            yield return new object[] { platform };
        foreach (var platform in PlatformDataClass.EmulatedIOSPlatforms)
            yield return new object[] { platform };
        foreach (var platform in PlatformDataClass.RealAndroidDevices)
            yield return new object[] { platform };
        foreach (var platform in PlatformDataClass.RealIOSDevices)
            yield return new object[] { platform };
    }

    [Theory]
    [MemberData(nameof(AllPlatforms))]
    public void TestNameTest(SaucePlatform platform)
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll([platform]);

        foreach (var bv in bvs)
        {
            bv.SetTestName(nameof(TestNameTest));
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
                if (!string.IsNullOrEmpty(bv.ScreenResolution))
                {
                    bv.TestName.ShouldContain(bv.ScreenResolution);
                }
            }
        }
    }

    private void ProcessBrowserVersions(List<BrowserVersion> browserVersions)
    {
        _validCount = 0;
        foreach (var bv in browserVersions)
        {
            if (bv != null)
            {
                _validCount++;
            }
        }

        _validCount.ShouldBeEquivalentTo(browserVersions.Count);
    }
}