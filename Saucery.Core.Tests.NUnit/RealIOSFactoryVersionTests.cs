using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Options;
using Saucery.Core.Tests.NUnit.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

public class RealIOSFactoryVersionTests {
    private PlatformConfiguratorAllFixture _fixture = null!;

    [OneTimeSetUp]
    public void SetupFixture() => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [TestCaseSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.NotSupportedTestCases))]
    public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldBeNull();
    }

    [Test]
    [TestCaseSource(typeof(RealIOSDataClass), nameof(RealIOSDataClass.SupportedTestCases))]
    public void AppiumIOSOptionTest(SaucePlatform saucePlatform) {
        var validPlatform = _fixture.PlatformConfigurator.Filter(saucePlatform);
        validPlatform.ShouldNotBeNull();

        var factory = new OptionFactory(validPlatform!);
        factory.ShouldNotBeNull();

        var tuple = factory.CreateOptions("AppiumRealIOSOptionTest");
        tuple.opts.ShouldNotBeNull();
    }
}

public class RealIOSDataClass {
    public static IEnumerable<object?[]> SupportedTestCases => sourceArray.Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

    public static IEnumerable<object?[]> NotSupportedTestCases =>
        [
            [new IOSRealDevice("NonExistent", "11")]
        ];

    private static readonly string[] sourceArray = ["13", "14", "15", "16", "17", "18", "26"];
}
