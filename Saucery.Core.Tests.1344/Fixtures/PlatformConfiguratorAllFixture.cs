using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.Fixtures;

public class PlatformConfiguratorAllFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorAllFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
}