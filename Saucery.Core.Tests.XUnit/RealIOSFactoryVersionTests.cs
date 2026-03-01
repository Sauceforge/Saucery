using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.XUnit.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class RealIOSFactoryVersionTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Theory]
    [MemberData(nameof(RealIOSDataClass.NotSupportedTestCases), MemberType = typeof(RealIOSDataClass))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(RealIOSDataClass.SupportedTestCases), MemberType = typeof(RealIOSDataClass))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform!);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public class RealIOSDataClass
{
    public static IEnumerable<object?[]> SupportedTestCases => sourceArray.Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

    public static IEnumerable<object?[]> NotSupportedTestCases =>
        [[new IOSRealDevice("NonExistent", "11")]];

    private static readonly string[] sourceArray = ["13", "14", "15", "16", "17", "18", "26"];
}
