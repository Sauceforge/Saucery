using Saucery.Core.Dojo;

namespace Saucery.Core.Tests.NUnit.Fixtures;

public class PlatformConfiguratorEmulatedFixture {
    public readonly PlatformConfigurator PlatformConfigurator;

    public PlatformConfiguratorEmulatedFixture() => PlatformConfigurator = new PlatformConfigurator(PlatformFilter.Emulated);
}