
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Tests.XUnitv3.DataProviders;
using Saucery.Core.Tests.XUnitv3.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnitv3;

public class MerlinPlatformTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;
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
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.RealAndroidDevices);
        ProcessBrowserVersions(bvs);
    }

    [Fact]
    public void ValidRealIOSDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.RealIOSDevices);
        ProcessBrowserVersions(bvs);
    }

    public static IEnumerable<object?[]> AllPlatforms() =>
        new[]
        {
            PlatformDataClass.DesktopPlatforms,
            PlatformDataClass.EmulatedAndroidPlatforms,
            PlatformDataClass.EmulatedIOSPlatforms,
            PlatformDataClass.RealAndroidDevices,
            PlatformDataClass.RealIOSDevices
        }
        .SelectMany(x => x)
        .Select(p => new object?[] { p });

    [Theory]
    [MemberData(nameof(AllPlatforms))]
    public void TestNameTest(SaucePlatform platform)
    {
        // PlatformExpander expects a List<SaucePlatform>; provide a List instead of an array
        PlatformExpander expander = new(_fixture.PlatformConfigurator, [platform]);
        var expandedPlatforms = expander.Expand();

        var bvs = _fixture.PlatformConfigurator.FilterAll(expandedPlatforms);

        foreach (var bv in bvs)
        {
            var testName = BrowserVersion.GenerateTestName(bv, nameof(TestNameTest));
            testName.ShouldNotBeNullOrEmpty();

            if (bv.IsAMobileDevice())
            {
                testName.ShouldContain(bv.DeviceName);
                testName.ShouldContain(bv.DeviceOrientation!);
            }
            else
            {
                testName.ShouldContain(bv.Os);
                testName.ShouldContain(bv.BrowserName);
                testName.ShouldContain(bv.Name!);
                if (!string.IsNullOrEmpty(bv.ScreenResolution))
                {
                    testName.ShouldContain(bv.ScreenResolution);
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