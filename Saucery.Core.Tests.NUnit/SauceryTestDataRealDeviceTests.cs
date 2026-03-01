using NUnit.Framework;
using Saucery.Core.DataSources;
using Saucery.Core.Tests.NUnit.DataProviders;
using Saucery.Core.Tests.NUnit.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

[NonParallelizable]
public class SauceryTestDataRealDeviceTests() : SauceryTestData {
    private static PlatformConfiguratorRealDeviceFixture _fixture = null!;

    [OneTimeSetUp]
    public void SetupFixture() => _fixture = new PlatformConfiguratorRealDeviceFixture();

    [Test]
    public void RealAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.RealAndroidDevices, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealAndroidDevices.Count);
    }

    [Test]
    public void RealAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.RealIOSDevices, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(PlatformDataClass.RealIOSDevices.Count);
    }
}
