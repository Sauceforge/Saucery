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
    [Order(1)]
    public class AndroidFactoryVersionTests
    {
        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
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

        [Test, TestCaseSource(typeof(AndroidDataClass), "NotSupportedTestCases")]
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

        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
        public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var factory = new OptionFactory(saucePlatform);
            var opts = factory.CreateOptions("AppiumAndroidOptionTest");
            opts.ShouldNotBeNull();
        }
    }
    public class AndroidDataClass
    {
        public static IEnumerable SupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform("Linux", "", "12.0", "android", "Google Pixel 5 GoogleAPI Emulator", "12.0", "", "android", "1.22.1", "landscape"));
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform("Linux", "android", "android", "10", "Google Pixel 3 GoogleAPI Emulator", "10.0.", "", "android", "1.22.1", "landscape"));
            }
        }
    }
}
