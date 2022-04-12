using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.Options;
using Saucery.Util;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    public class IOSFactoryVersionTests
    {
        static PlatformConfigurator PlatformConfigurator { get; set; }

        static IOSFactoryVersionTests()
        {
            PlatformConfigurator = new PlatformConfigurator();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.ShouldBeNull();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
        {
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.ShouldNotBeNull();

            validPlatform.Classify();
            var factory = new OptionFactory(validPlatform);
            factory.ShouldNotBeNull();
            
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
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "15.0", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "14.5", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "14.4", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "14.3", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "14.0", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "13.4", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "13.2", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "13.0", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "12.4", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "12.2", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone XS Max Simulator", "12.0", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone 5s Simulator", "11.3", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone 5s Simulator", "11.2", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone 5s Simulator", "11.1", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone 5s Simulator", "11.0", "portrait");
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "iPhone 5s Simulator", "10.3", "portrait");
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new IOSPlatform(SauceryConstants.PLATFORM_IOS, "NonExistent", "13.0", "portrait");
            }
        }
    }
}
