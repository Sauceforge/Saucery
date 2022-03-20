using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    [Order(5)]
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
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldNotBeNull();

            var factory = new OptionFactory(validplatform); //TODO: New way
            factory.ShouldNotBeNull();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldBeNull();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void AppiumIOSOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldBeNull();

            var factory = new OptionFactory(validplatform); //TODO: New way
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
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "13.0", "", "iphone", "1.21.0", "portrait");

                yield return new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone XS Max Simulator", "15.0", "", "iphone", "1.22.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone XS Max Simulator", "14.5", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone XS Max Simulator", "14.4", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone XS Max Simulator", "14.3", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone XS Max Simulator", "14.0", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "13.4", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "13.2", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "13.0", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.15", "iPhone XS Max Simulator", "12.4", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.14", "iPhone XS Max Simulator", "12.2", "", "iphone", "1.21.0", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.13", "iPhone XS Max Simulator", "12.0", "", "iphone", "1.9.1", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.13", "iPhone 5s Simulator", "11.3", "", "iphone", "1.9.1", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.12", "iPhone 5s Simulator", "11.2", "", "iphone", "1.9.1", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.12", "iPhone 5s Simulator", "11.1", "", "iphone", "1.9.1", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.12", "iPhone 5s Simulator", "11.0", "", "iphone", "1.9.1", "portrait");
                yield return new SaucePlatform("iOS", "iphone", "", "Mac 10.12", "iPhone 5s Simulator", "10.3", "", "iphone", "1.9.1", "portrait");
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
