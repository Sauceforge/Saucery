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
public class AndroidFactoryVersionTests
{
    private PlatformConfigurator? PlatformConfigurator { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.ALL);
    }

    [Test, TestCaseSource(typeof(AndroidDataClass), nameof(AndroidDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test, TestCaseSource(typeof(AndroidDataClass), nameof(AndroidDataClass.SupportedTestCases))]
    public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = PlatformConfigurator!.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var opts = factory.CreateOptions("AppiumAndroidOptionTest");
        opts.ShouldNotBeNull();
    }
}
public class AndroidDataClass
{
    public static IEnumerable SupportedTestCases
    {
        get
        {
            yield return new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel 7a GoogleAPI Emulator", "13.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel 5 GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel 4a GoogleAPI Emulator", "11.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel 3a GoogleAPI Emulator", "10.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel 3 GoogleAPI Emulator", "9.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "8.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "8.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "7.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "7.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Android GoogleAPI Emulator", "6.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
            yield return new AndroidPlatform("Android GoogleAPI Emulator", "5.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
        }
    }

    public static IEnumerable NotSupportedTestCases
    {
        get
        {
            yield return new AndroidPlatform("Google Pixel 3 GoogleAPI Emulator", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE);
        }
    }
}
