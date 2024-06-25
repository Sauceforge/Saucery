using NUnit.Framework;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Shouldly;
using System.Collections;

namespace Saucery.Core.Tests;

[TestFixture]
public class RealAndroidFactoryVersionTests
{
    private PlatformConfigurator? PlatformConfigurator { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.ALL);
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

        var opts = factory.CreateOptions("AppiumRealAndroidOptionTest");
        opts.ShouldNotBeNull();
    }
}
public class RealAndroidDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            yield return new AndroidRealDevice("Google Pixel 8 Pro", "14");
            yield return new AndroidRealDevice("Google Pixel 7 Pro", "13");
            yield return new AndroidRealDevice("Google Pixel 6a", "12");
            yield return new AndroidRealDevice("Google Pixel 4a", "11");
            yield return new AndroidRealDevice("Google Pixel 4 XL", "10");
            yield return new AndroidRealDevice("Samsung Galaxy Tab S3", "9");
            //yield return new AndroidRealDevice("Motorola Moto Z Play", "8");
            //yield return new AndroidRealDevice("Mason G450A1", "7");
            yield return new AndroidRealDevice("Samsung.*", "6");
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
