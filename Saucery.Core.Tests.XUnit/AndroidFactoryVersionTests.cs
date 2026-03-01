using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.XUnit.Fixtures;
using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class AndroidFactoryVersionTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Theory]
    [MemberData(nameof(AndroidDataClass.NotSupportedTestCases), MemberType = typeof(AndroidDataClass))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(AndroidDataClass.SupportedTestCases), MemberType = typeof(AndroidDataClass))]
    public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var (opts, _) = factory.CreateOptions("AppiumAndroidOptionTest");
        opts.ShouldNotBeNull();
    }
}

public static class AndroidDataClass
{
    public static IEnumerable<object?[]> SupportedTestCases =>
        [
            [new AndroidPlatform("Google Pixel 9 Pro Emulator", "16.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 9 Pro Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 8 Pro Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 7 Pro Emulator", "13.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 5 Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 4a Emulator", "11.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 3a Emulator", "10.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel 3 Emulator", "9.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel Emulator", "8.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Pixel Emulator", "8.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Nexus 9 Tablet Emulator", "7.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Nexus 9 Tablet Emulator", "7.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Nexus 9 Tablet Emulator", "6.0", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)],
            [new AndroidPlatform("Google Nexus 10 Tablet Emulator", "5.1", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)]
        ];

    public static IEnumerable<object?[]> NotSupportedTestCases =>
        [
            [new AndroidPlatform("Google Pixel 3 GoogleAPI Emulator", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)]
        ];
}
