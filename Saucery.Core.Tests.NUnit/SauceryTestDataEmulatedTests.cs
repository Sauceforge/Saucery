using NUnit.Framework;
using Saucery.Core.DataSources;
using Saucery.Core.Tests.NUnit.DataProviders;
using Saucery.Core.Tests.NUnit.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

[NonParallelizable]
public class SauceryTestDataEmulatedTests : SauceryTestData {
    private PlatformConfiguratorEmulatedFixture _fixture = null!;

    [OneTimeSetUp]
    public void SetupFixture() => _fixture = new PlatformConfiguratorEmulatedFixture();

    [Test]
    public void DesktopPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }

    [Test]
    public void EmulatedAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedAndroidPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedAndroidPlatforms.Count);
    }

    [Test]
    public void EmulatedAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedIOSPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.EmulatedIOSPlatforms.Count);
    }
}
