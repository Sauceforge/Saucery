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
        [Test, TestCaseSource(typeof(IOSDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var configurator = new PlatformConfigurator();
            var validplatform = configurator.Validate(saucePlatform);
            validplatform.ShouldNotBeNull();

            //var factory = new OptionFactory(validplatform); //TODO: New way
            var factory = new OptionFactory(saucePlatform);   //TODO: Old way
            var result = factory.IsSupportedPlatform();
            result.ShouldBeTrue();
        }

        [Test, TestCaseSource(typeof(IOSDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var configurator = new PlatformConfigurator();
            var validplatform = configurator.Validate(saucePlatform);
            validplatform.ShouldBeNull();

            //var factory = new OptionFactory(validplatform); //TODO: New way
            var factory = new OptionFactory(saucePlatform);   //TODO: Old way
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
