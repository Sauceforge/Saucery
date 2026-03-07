using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests;

public class RealIOSFactoryVersionTests() {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [MethodDataSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.GetNotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.GetSupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform!);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public static class RealIOSDataClass {
    private static readonly string[] SupportedVersions = ["13", "14", "15", "16", "17", "18", "26"];

    public static IEnumerable<object?[]> GetSupportedTestCases() {
        foreach (var version in SupportedVersions) {
            yield return [new IOSRealDevice("iPhone.*", version)];
        }
    }

    public static IEnumerable<object?[]> GetNotSupportedTestCases() {
        yield return [new IOSRealDevice("NonExistent", "11")];
    }
}
