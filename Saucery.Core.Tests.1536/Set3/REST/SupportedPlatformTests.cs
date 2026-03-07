using Saucery.Core.Dojo;
using Shouldly;

namespace Set3.REST;

public class SupportedPlatformTests3 {
    [Test]
    public void SupportedRealDevicePlatformTest() {
        PlatformConfigurator configurator = new(PlatformFilter.RealDevice);
        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldBeEmpty();
        realDevices.ShouldNotBeNull();
    }

    [Test]
    public void SupportedEmulatedPlatformTest() {
        PlatformConfigurator configurator = new(PlatformFilter.Emulated);

        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldNotBeNull();
        realDevices.ShouldBeEmpty();
    }
}
