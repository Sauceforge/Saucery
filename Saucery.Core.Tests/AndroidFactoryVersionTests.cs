using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.Fixtures;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests;

public class AndroidFactoryVersionTests {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [MethodDataSource(typeof(AndroidDataClass), nameof(AndroidDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(typeof(AndroidDataClass), nameof(AndroidDataClass.SupportedTestCases))]
    public void AppiumAndroidOptionTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var (opts, _) = factory.CreateOptions("AppiumAndroidOptionTest");
        opts.ShouldNotBeNull();
    }
}

public static class AndroidDataClass {
    public static IEnumerable<SaucePlatform> SupportedTestCases 
        => 
        [
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "16.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 7a GoogleAPI Emulator", "13.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 5 GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 4a GoogleAPI Emulator", "11.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 3a GoogleAPI Emulator", "10.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel 3 GoogleAPI Emulator", "9.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "8.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "8.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "7.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Google Pixel C GoogleAPI Emulator", "7.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Android GoogleAPI Emulator", "6.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
            new AndroidPlatform("Android GoogleAPI Emulator", "5.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)
        ];

    public static IEnumerable<SaucePlatform> NotSupportedTestCases
        =>
        [
            new AndroidPlatform("Google Pixel 3 GoogleAPI Emulator", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)
        ];
}
