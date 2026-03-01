using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.XUnitv3.Fixtures;

public class PlatformConfiguratorRealDeviceFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorRealDeviceFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
}