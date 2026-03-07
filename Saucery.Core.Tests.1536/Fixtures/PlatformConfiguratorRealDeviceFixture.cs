using Saucery.Core.Dojo;

namespace Fixtures;

public class PlatformConfiguratorRealDeviceFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorRealDeviceFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
}