using Saucery.Core.Dojo;

namespace Fixtures;

public class PlatformConfiguratorEmulatedFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorEmulatedFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.Emulated);
}