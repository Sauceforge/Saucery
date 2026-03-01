using Saucery.Core.DataSources;
using Saucery.Core.Tests.XUnitv3.DataProviders;
using Saucery.Core.Tests.XUnitv3.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnitv3;

public class SauceryTestDataEmulatedTests(PlatformConfiguratorEmulatedFixture fixture) : SauceryTestData, IClassFixture<PlatformConfiguratorEmulatedFixture>
{
    private readonly PlatformConfiguratorEmulatedFixture _fixture = fixture;

    [Fact]
    public void DesktopPlatformsTest()
    {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }

    [Fact]
    public void EmulatedAndroidDeviceTest()
    {
        SetPlatforms(PlatformDataClass.EmulatedAndroidPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedAndroidPlatforms.Count);
    }

    [Fact]
    public void EmulatedAppleDeviceTest()
    {
        SetPlatforms(PlatformDataClass.EmulatedIOSPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedIOSPlatforms.Count);
    }
}
