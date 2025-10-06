using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Tests.DataProviders;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests;

public class MerlinPlatformTests()
{
    private static PlatformConfiguratorAllFixture _fixture = null!;
    private int _validCount = 0;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    public void ValidDesktopPlatformTest()
    {
        PlatformExpander expander = new(_fixture.PlatformConfigurator, PlatformDataClass.DesktopPlatforms);
        var expandedPlatforms = expander.Expand();
        var bvs = _fixture.PlatformConfigurator.FilterAll(expandedPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidEmulatedAndroidDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.EmulatedAndroidPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidEmulatedIOSDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.EmulatedIOSPlatforms);
        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidRealAndroidDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator .FilterAll(PlatformDataClass.RealAndroidDevices);
        ProcessBrowserVersions(bvs);
    }

    [Test]
    public void ValidRealIOSDevicesTest()
    {
        var bvs = _fixture.PlatformConfigurator.FilterAll(PlatformDataClass.RealIOSDevices);
        ProcessBrowserVersions(bvs);
    }

    public static IEnumerable<Func<SaucePlatform>> AllPlatforms()
        => new[]
        {
            PlatformDataClass.DesktopPlatforms,
            PlatformDataClass.EmulatedAndroidPlatforms,
            PlatformDataClass.EmulatedIOSPlatforms,
            PlatformDataClass.RealAndroidDevices,
            PlatformDataClass.RealIOSDevices
        }
        .SelectMany(x => x)
        .Select(p => (Func<SaucePlatform>)(() => p));

    [Test]
    [MethodDataSource(nameof(AllPlatforms))]
    public void TestNameTest(SaucePlatform platform)
    {
        PlatformExpander expander = new(_fixture.PlatformConfigurator, [platform]);
        var expandedPlatforms = expander.Expand();

        var bvs = _fixture.PlatformConfigurator.FilterAll(expandedPlatforms);

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