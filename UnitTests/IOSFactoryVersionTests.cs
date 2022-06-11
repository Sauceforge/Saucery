using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.Options;
using Saucery.Util;
using Shouldly;
using System.Collections;

namespace UnitTests;

[TestFixture]
public class IOSFactoryVersionTests
{
    static PlatformConfigurator PlatformConfigurator { get; set; }

    static IOSFactoryVersionTests()
    {
        PlatformConfigurator = new PlatformConfigurator();
    }

    [Test, TestCaseSource(typeof(IOSDataClass), "NotSupportedTestCases")]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();
        
        var opts = factory.CreateOptions("AppiumIOSOptionTest");
        opts.ShouldNotBeNull();
    }
}
public class IOSDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            var versions = new [] { "10.3", 
                                    "11.0", "11.1", "11.2", "11.3", 
                                    "12.0", "12.2", "12.4", 
                                    "13.0", "13.2", "13.4", 
                                    "14.0", "14.3", "14.4", "14.5", 
                                    "15.0", "15.2", "15.4" };
            foreach (var v in versions)
            {
                yield return new IOSPlatform("iPhone 7 Plus Simulator", v, SauceryConstants.DEVICE_ORIENTATION_PORTRAIT);
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
