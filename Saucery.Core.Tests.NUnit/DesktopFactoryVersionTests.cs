using NUnit.Framework;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.NUnit.Fixtures;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

public class DesktopFactoryVersionTests
{
    private PlatformConfiguratorAllFixture _fixture = null!;

    [OneTimeSetUp]
    public void SetupFixture() => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [TestCaseSource(typeof(DesktopDataClass), nameof(DesktopDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Validate(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [TestCaseSource(typeof(DesktopDataClass), nameof(DesktopDataClass.SupportedTestCases))]
    public void DesktopOptionTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Validate(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform);
        factory.ShouldNotBeNull();

        var (opts, browserVersion) = factory.CreateOptions("DesktopOptionTest");
        opts.ShouldNotBeNull();
    }
}

public static class DesktopDataClass
{
    public static IEnumerable<SaucePlatform> SupportedTestCases
    =>
        [
            new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_1280_1024),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_1280_1024),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "78"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "98"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "79"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_CHROME, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_SAFARI, "16"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, "99")
        ];

    public static IEnumerable<SaucePlatform> NotSupportedTestCases
        =>
        [
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "9999"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "25"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "3"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "9999"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_SAFARI, "7"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "8"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "9999")
        ];
}
