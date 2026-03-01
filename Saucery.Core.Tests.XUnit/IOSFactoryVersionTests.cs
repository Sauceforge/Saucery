using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.XUnit.Fixtures;
using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class IOSFactoryVersionTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Theory]
    [MemberData(nameof(IOSDataClass.NotSupportedTestCases), MemberType = typeof(IOSDataClass))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(IOSDataClass.SupportedTestCases), MemberType = typeof(IOSDataClass))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
        tuple.browserVersion.ShouldNotBeNull();
        tuple.browserVersion.ShouldBeEquivalentTo(validPlatform);
    }
}

internal static class IOSDataClass
{
    public static IEnumerable<object?[]> SupportedTestCases =>
        IOSPlatformVersions.Select(v => new object?[] { new IOSPlatform("iPhone Simulator", v, SauceryConstants.DEVICE_ORIENTATION_PORTRAIT) });

    public static IEnumerable<object?[]> NotSupportedTestCases =>
        [[new IOSPlatform("NonExistent", "13.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT)]];

    internal static readonly string[] IOSPlatformVersions =
    [
        "14.0","14.3","14.4","14.5",
        "15.0","15.2","15.4","15.5",
        "16.0","16.1","16.2","16.4",
        "17.0","17.5",
        "18.0","18.6",
    ];
}
