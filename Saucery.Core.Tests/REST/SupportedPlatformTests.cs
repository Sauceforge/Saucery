using Saucery.Core.Dojo;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.REST;

public class SupportedPlatformTests {
    [Fact]
    public void SupportedRealDevicePlatformTest() {
        PlatformConfigurator configurator = new(PlatformFilter.RealDevice);
        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldBeEmpty();
        realDevices.ShouldNotBeNull();
    }

    [Fact]
    public void SupportedEmulatedPlatformTest() {
        PlatformConfigurator configurator = new(PlatformFilter.Emulated);

        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldNotBeNull();
        realDevices.ShouldBeEmpty();
    }
}
