using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.RestAPI.SupportedPlatforms;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    [Order(5)]
    public class IOSFactoryVersionTests
    {
        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);

            var platformAcquirer = new SauceLabsPlatformAcquirer();
            var platforms = platformAcquirer.AcquirePlatforms();
            var configurator = new PlatformConfigurator(platforms);

            var validplatform = configurator.Validate(saucePlatform);
            validplatform.ShouldNotBeNull();
            //var factory = new OptionFactory(validplatform);
            var factory = new OptionFactory(saucePlatform);
            var result = factory.IsSupportedPlatform();
            result.ShouldBeTrue();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);

            var platformAcquirer = new SauceLabsPlatformAcquirer();
            var platforms = platformAcquirer.AcquirePlatforms();
            var configurator = new PlatformConfigurator(platforms);

            var validplatform = configurator.Validate(saucePlatform);
            validplatform.ShouldBeNull();
            //var factory = new OptionFactory(validplatform);
            var factory = new OptionFactory(saucePlatform);
            var result = factory.IsSupportedPlatform();
            result.ShouldBeFalse();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var factory = new OptionFactory(saucePlatform);
            var opts = factory.CreateOptions("AppiumIOSOptionTest");
            opts.ShouldNotBeNull();
        }
    }
    public class IOSDataClass
    {
        public static IEnumerable SupportedTestCases
        {
            get
            {
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "13.0", "", "iphone", "1.21.0", "portrait");
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new SaucePlatform("iOS", "", "999", "Mac 11", "NonExistent", "13.0", "", "NonExistent", "1.21.0", "portrait");
            }
        }
    }
}
