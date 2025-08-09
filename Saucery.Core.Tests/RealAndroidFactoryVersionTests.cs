using System.Collections;
using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Shouldly;

namespace Saucery.Core.Tests;

[TestFixture]
public class RealAndroidFactoryVersionTests
{
    private PlatformConfigurator? PlatformConfigurator { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }

    [Test, TestCaseSource(typeof(RealAndroidDataClass), nameof(RealAndroidDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test, TestCaseSource(typeof(RealAndroidDataClass), nameof(RealAndroidDataClass.SupportedTestCases))]
    public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealAndroidOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}
public static class RealAndroidDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            yield return new AndroidRealDevice("Google Pixel 9", "16");
            yield return new AndroidRealDevice("Google Pixel 9", "15");
            yield return new AndroidRealDevice("Google Pixel 8 Pro", "14");
            yield return new AndroidRealDevice("Google Pixel 7 Pro", "13");
            yield return new AndroidRealDevice("Google Pixel 6a", "12");
            yield return new AndroidRealDevice("Google Pixel 4a", "11");
            yield return new AndroidRealDevice("Google Pixel 4 XL", "10");
            yield return new AndroidRealDevice("Samsung Galaxy Tab S3", "9");
        }
    }

    public static IEnumerable NotSupportedTestCases
    {
        get
        {
            yield return new AndroidRealDevice("NonExistent", "1");
        }
    }
}
