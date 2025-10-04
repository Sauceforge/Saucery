using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class IOSFactoryVersionTests(PlatformConfiguratorFixture fixture) : IClassFixture<PlatformConfiguratorFixture> 
{
    private readonly PlatformConfiguratorFixture _fixture = fixture;

    public static IEnumerable<object[]> SupportedTestCases()
    {
        foreach (var testCase in IOSDataClass.SupportedTestCases)
        {
            yield return new object[] { testCase };
        }
    }

    public static IEnumerable<object[]> NotSupportedTestCases()
    {
        foreach (var testCase in IOSDataClass.NotSupportedTestCases)
        {
            yield return new object[] { testCase };
        }
    }

    [Theory]
    [MemberData(nameof(NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(SupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

internal static class IOSDataClass
{
    public static IEnumerable<SaucePlatform> SupportedTestCases
    {
        get
        {
            var versions = new[] { "14.0", "14.3", "14.4", "14.5",
                                   "15.0", "15.2", "15.4",
                                   "16.0", "16.1", "16.2",
                                   "17.0", "17.5", "18.0" };
            foreach (var v in versions)
            {
                yield return new IOSPlatform("iPhone Simulator", v, SauceryConstants.DEVICE_ORIENTATION_PORTRAIT);
            }
        }
    }

    public static IEnumerable<SaucePlatform> NotSupportedTestCases
    {
        get
        {
            yield return new IOSPlatform("NonExistent", "13.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT);
        }
    }
}
