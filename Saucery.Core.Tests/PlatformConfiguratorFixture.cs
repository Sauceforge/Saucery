using Saucery.Core.Dojo;

namespace Saucery.Core.Tests;

public class PlatformConfiguratorFixture {
    public PlatformConfigurator PlatformConfigurator { get; }

    public PlatformConfiguratorFixture() {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }
}