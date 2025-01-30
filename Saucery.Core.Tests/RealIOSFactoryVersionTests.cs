using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Shouldly;
using System.Collections;

namespace Saucery.Core.Tests;

[TestFixture]
public class RealIOSFactoryVersionTests
{
    private PlatformConfigurator? PlatformConfigurator { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }

    [Test, TestCaseSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test, TestCaseSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.SupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();
        
        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}
public static class RealIOSDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            var versions = new [] { "13", "14", "15", "16", "17", "18" };
            return from v in versions
                   select new IOSRealDevice("iPhone.*", v);
        }
    }

    public static IEnumerable NotSupportedTestCases
    {
        get
        {
            yield return new IOSRealDevice("NonExistent", "11");
        }
    }
}
