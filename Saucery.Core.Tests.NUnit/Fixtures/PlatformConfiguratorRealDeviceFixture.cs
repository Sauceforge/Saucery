using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.NUnit.Fixtures;

public class PlatformConfiguratorRealDeviceFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorRealDeviceFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
}