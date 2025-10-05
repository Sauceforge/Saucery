using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.Fixtures;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests;

public class DesktopFactoryVersionTests() 
{
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) 
    {
        _fixture = new PlatformConfiguratorAllFixture();
    }

    public static IEnumerable<object[]> SupportedTestCases()
    {
        foreach (var testCase in DesktopDataClass.SupportedTestCases)
        {
            yield return new object[] { testCase };
        }
    }

    public static IEnumerable<object[]> NotSupportedTestCases()
    {
        foreach (var testCase in DesktopDataClass.NotSupportedTestCases)
        {
            yield return new object[] { testCase };
        }
    }

    [Test]
    [MethodDataSource(nameof(NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
    {
        var validPlatform = _fixture.PlatformConfigurator.Validate(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(nameof(SupportedTestCases))]
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
    {
        get
        {
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST);
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_LATEST);
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_1280_1024);
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_1280_1024);
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "78");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "98");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "79");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "99");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_CHROME, "99");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_SAFARI, "16");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, "99");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, "99");
        }
    }

    public static IEnumerable<SaucePlatform> NotSupportedTestCases
    {
        get
        {
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "9999");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "25");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "3");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "9999");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_SAFARI, "7");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "8");
            yield return new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "9999");
        }
    }
}
