using Saucery.Core.Dojo;

namespace Fixtures;

public class PlatformConfiguratorAllFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorAllFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
}