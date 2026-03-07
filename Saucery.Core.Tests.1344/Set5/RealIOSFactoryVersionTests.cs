using Fixtures;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Shouldly;

namespace Set5;

public class RealIOSFactoryVersionTests5() {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [MethodDataSource(typeof(RealIOSDataClass5), nameof(RealIOSDataClass5.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(typeof(RealIOSDataClass5), nameof(RealIOSDataClass5.SupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform!);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public class RealIOSDataClass5 {
    public static IEnumerable<object?[]> SupportedTestCases =>
        new[] { "13", "14", "15", "16", "17", "18", "26" }
            .Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

    public static IEnumerable<object?[]> NotSupportedTestCases
        =>
        [
            [new IOSRealDevice("NonExistent", "11")]
        ];
}
