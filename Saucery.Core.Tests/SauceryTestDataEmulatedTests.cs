using Saucery.Core.DataSources;
using Saucery.Core.Tests.DataProviders;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests;

public class SauceryTestDataEmulatedTests() : SauceryTestData {
    private static PlatformConfiguratorEmulatedFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorEmulatedFixture();

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
