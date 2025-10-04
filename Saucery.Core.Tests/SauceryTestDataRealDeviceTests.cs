using Saucery.Core.DataSources;
using Saucery.Core.Tests.DataProviders;
using Saucery.Core.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class SauceryTestDataRealDeviceTests(PlatformConfiguratorRealDeviceFixture fixture) : SauceryTestData, IClassFixture<PlatformConfiguratorRealDeviceFixture> {
    private readonly PlatformConfiguratorRealDeviceFixture _fixture = fixture;

    [Fact]
    public void RealAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.RealAndroidDevices, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealAndroidDevices.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.RealAndroidDevices.Count);
    }

    [Fact]
    public void RealAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.RealIOSDevices, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealIOSDevices.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.RealIOSDevices.Count);
    }
}
