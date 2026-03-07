using Fixtures;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Util;
using Shouldly;

namespace Set6;

public class IOSFactoryVersionTests6() 
{
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [MethodDataSource(typeof(IOSDataClass6), nameof(IOSDataClass6.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(typeof(IOSDataClass6), nameof(IOSDataClass6.SupportedTestCases))]
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

internal static class IOSDataClass6
{
    public static IEnumerable<SaucePlatform> SupportedTestCases => IOSPlatformVersions.Select(v => new IOSPlatform("iPhone Simulator", v, SauceryConstants.DEVICE_ORIENTATION_PORTRAIT));

    public static IEnumerable<SaucePlatform> NotSupportedTestCases => 
        [
            new IOSPlatform("NonExistent", "13.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT)
        ];
    
    internal static readonly string[] IOSPlatformVersions =
    [
        "14.0","14.3","14.4","14.5",
        "15.0","15.2","15.4","15.5",
        "16.0","16.1","16.2","16.4",
        "17.0","17.5",
        "18.0","18.6",
    ];
}
