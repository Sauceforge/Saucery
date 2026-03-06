using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests;

public class RealAndroidFactoryVersionTests()
{
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [MethodDataSource(typeof(RealAndroidDataClass), nameof(RealAndroidDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(typeof(RealAndroidDataClass), nameof(RealAndroidDataClass.SupportedTestCases))]
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
    public static IEnumerable<SaucePlatform> SupportedTestCases
    =>
        [
            new AndroidRealDevice("Google Pixel 9", "16"),
            new AndroidRealDevice("Google Pixel 9", "15"),
            new AndroidRealDevice("Google Pixel 8 Pro", "14"),
            new AndroidRealDevice("Google Pixel 7 Pro", "13"),
            new AndroidRealDevice("Google Pixel 6a", "12"),
            new AndroidRealDevice("Google Pixel 4a", "11"),
            new AndroidRealDevice("Google Pixel 4 XL", "10"),
            new AndroidRealDevice("Samsung Galaxy Tab S3", "9")
        ];

    public static IEnumerable<SaucePlatform> NotSupportedTestCases
    =>
        [
            new AndroidRealDevice("NonExistent", "1")
        ];
}
