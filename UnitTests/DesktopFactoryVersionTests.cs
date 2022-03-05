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
    [Order(3)]
    public class DesktopFactoryVersionTests
    {
        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
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

        [Test, TestCaseSource(typeof(DesktopDataClass), "NotSupportedTestCases")]
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

        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
        public void DesktopOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var factory = new OptionFactory(saucePlatform);
            var opts = factory.CreateOptions("DesktopOptionTest");
            opts.ShouldNotBeNull();
        }
    }
    public class DesktopDataClass
    {
        public static IEnumerable SupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "latest"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "75"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "98"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "78"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "97"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "MicrosoftEdge", "79"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "MicrosoftEdge", "98"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "11"));
                
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "9999"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "25"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "3"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "9999"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "safari", "7"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "8"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "9999"));
            }
        }
    }
}
