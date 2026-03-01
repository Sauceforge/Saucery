using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.XUnit.Fixtures;

public class PlatformConfiguratorRealDeviceFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorRealDeviceFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
}