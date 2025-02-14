using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Util;
using Shouldly;
using System.Collections;

namespace Saucery.Core.Tests;

[TestFixture]
public class IOSFactoryVersionTests
{
    private PlatformConfigurator? PlatformConfigurator { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }

    [Test, TestCaseSource(typeof(IOSDataClass), nameof(IOSDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test, TestCaseSource(typeof(IOSDataClass), nameof(IOSDataClass.SupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();
        
        var tuple = factory.CreateOptions("AppiumIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}
internal static class IOSDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            var versions = new [] { "14.0", "14.3", "14.4", "14.5", 
                                    "15.0", "15.2", "15.4", 
                                    "16.0", "16.1", "16.2",
                                    "17.0" };
            foreach (var v in versions)
            {
                yield return new IOSPlatform("iPhone Simulator", v, SauceryConstants.DEVICE_ORIENTATION_PORTRAIT);
            }
        }
    }

    public static IEnumerable NotSupportedTestCases
    {
        get
        {
            yield return new IOSPlatform("NonExistent", "13.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT);
        }
    }
}
