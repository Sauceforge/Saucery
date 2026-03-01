using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.XUnitv3.Fixtures;

public class PlatformConfiguratorAllFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorAllFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
}