using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class RealIOSFactoryVersionTests(PlatformConfiguratorFixture fixture) : IClassFixture<PlatformConfiguratorFixture> {
    private readonly PlatformConfiguratorFixture _fixture = fixture;

    [Theory]
    [MemberData(nameof(RealIOSDataClass.NotSupportedTestCases), MemberType = typeof(RealIOSDataClass))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(RealIOSDataClass.SupportedTestCases), MemberType = typeof(RealIOSDataClass))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform!);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public static class RealIOSDataClass {
    public static IEnumerable<object?[]> SupportedTestCases =>
        new[] { "13", "14", "15", "16", "17", "18", "26" }
            .Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

    public static IEnumerable<object?[]> NotSupportedTestCases
    {
        get
        {
            yield return new object?[] { new IOSRealDevice("NonExistent", "11") };
        }
    }
}
