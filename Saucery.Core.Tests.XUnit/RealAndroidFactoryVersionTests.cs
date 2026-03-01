using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.XUnit.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class RealAndroidFactoryVersionTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Theory]
    [MemberData(nameof(RealAndroidDataClass.NotSupportedTestCases), MemberType = typeof(RealAndroidDataClass))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(RealAndroidDataClass.SupportedTestCases), MemberType = typeof(RealAndroidDataClass))]
    public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var (opts, _) = factory.CreateOptions("AppiumRealAndroidOptionTest");
        opts.ShouldNotBeNull();
    }
}

public static class RealAndroidDataClass
{
    public static IEnumerable<object[]> SupportedTestCases
    =>
        [
            [new AndroidRealDevice("Google Pixel 9", "16")],
            [new AndroidRealDevice("Google Pixel 9", "15")],
            [new AndroidRealDevice("Google Pixel 8 Pro", "14")],
            [new AndroidRealDevice("Google Pixel 7 Pro", "13")],
            [new AndroidRealDevice("Google Pixel 6a", "12")],
            [new AndroidRealDevice("Google Pixel 4a", "11")],
            [new AndroidRealDevice("Google Pixel 4 XL", "10")],
            [new AndroidRealDevice("Samsung Galaxy Tab S3", "9")]
        ];

    public static IEnumerable<object[]> NotSupportedTestCases
    =>
        [
            [new AndroidRealDevice("NonExistent", "1")]
        ];
}
