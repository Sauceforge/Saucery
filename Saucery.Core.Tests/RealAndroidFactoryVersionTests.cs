using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

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

        var tuple = factory.CreateOptions("AppiumRealAndroidOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public static class RealAndroidDataClass
{
    public static IEnumerable<object[]> SupportedTestCases
    {
        get
        {
            yield return new object[] { new AndroidRealDevice("Google Pixel 9", "16") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 9", "15") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 8 Pro", "14") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 7 Pro", "13") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 6a", "12") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 4a", "11") };
            yield return new object[] { new AndroidRealDevice("Google Pixel 4 XL", "10") };
            yield return new object[] { new AndroidRealDevice("Samsung Galaxy Tab S3", "9") };
        }
    }

    public static IEnumerable<object[]> NotSupportedTestCases
    {
        get
        {
            yield return new object[] { new AndroidRealDevice("NonExistent", "1") };
        }
    }
}
