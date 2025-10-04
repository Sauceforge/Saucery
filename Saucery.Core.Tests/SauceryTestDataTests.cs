using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.Tests.DataProviders;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class SauceryTestDataTests(PlatformConfiguratorFixture fixture) : SauceryTestData, IClassFixture<PlatformConfiguratorFixture> {
    private readonly PlatformConfiguratorFixture _fixture = fixture;

    [Fact]
    public void AllPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
        GetAllPlatforms().Count().ShouldBe(34);
    }

    [Fact]
    public void DesktopPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
        GetAllPlatforms().Count().ShouldBe(34);
    }

    [Fact]
    public void RealAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.RealAndroidDevices, PlatformFilter.RealDevice);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealAndroidDevices.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.RealAndroidDevices.Count);
    }

    [Fact]
    public void RealAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.RealIOSDevices, PlatformFilter.RealDevice);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealIOSDevices.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.RealIOSDevices.Count);
    }

    [Fact]
    public void EmulatedAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedAndroidPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedAndroidPlatforms.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.EmulatedAndroidPlatforms.Count);
    }

    [Fact]
    public void EmulatedAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedIOSPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedIOSPlatforms.Count);
        GetAllPlatforms().Count().ShouldBe(PlatformDataClass.EmulatedIOSPlatforms.Count);
    }
}
