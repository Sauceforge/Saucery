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

        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.ShouldNotBeNull();

            validPlatform.Classify();
            var factory = new OptionFactory(validPlatform);
            factory.ShouldNotBeNull();
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
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1015, "iPhone XS Max Simulator", "13.0", "iphone", "1.21.0", "portrait");

                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone XS Max Simulator", "15.0", "iphone", "1.22.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone XS Max Simulator", "14.5", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone XS Max Simulator", "14.4", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone XS Max Simulator", "14.3", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone XS Max Simulator", "14.0", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1015, "iPhone XS Max Simulator", "13.4", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1015, "iPhone XS Max Simulator", "13.2", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1015, "iPhone XS Max Simulator", "13.0", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1015, "iPhone XS Max Simulator", "12.4", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1014, "iPhone XS Max Simulator", "12.2", "iphone", "1.21.0", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1013, "iPhone XS Max Simulator", "12.0", "iphone", "1.9.1", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1013, "iPhone 5s Simulator", "11.3", "iphone", "1.9.1", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1012, "iPhone 5s Simulator", "11.2", "iphone", "1.9.1", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1012, "iPhone 5s Simulator", "11.1", "iphone", "1.9.1", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1012, "iPhone 5s Simulator", "11.0", "iphone", "1.9.1", "portrait");
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_1012, "iPhone 5s Simulator", "10.3", "iphone", "1.9.1", "portrait");
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new MobilePlatform(SauceryConstants.PLATFORM_IOS, "", "999", SauceryConstants.PLATFORM_MAC_11, "NonExistent", "13.0", "NonExistent", "1.21.0", "portrait");
            }
        }
    }
}
