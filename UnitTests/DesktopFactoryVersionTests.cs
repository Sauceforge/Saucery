using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Shouldly;
using System.Collections;

namespace UnitTests {
    [TestFixture]
    public class DesktopFactoryVersionTests
    {
        static PlatformConfigurator PlatformConfigurator { get; set; }

        static DesktopFactoryVersionTests()
        {
            PlatformConfigurator = new PlatformConfigurator();
        }

        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldNotBeNull();

            var factory = new OptionFactory(validplatform); //TODO: New way
            factory.ShouldNotBeNull();
        }

        [Test, TestCaseSource(typeof(DesktopDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldBeNull();
        }

        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
        public void DesktopOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validplatform = PlatformConfigurator.Validate(saucePlatform);
            validplatform.ShouldNotBeNull();

            var factory = new OptionFactory(validplatform); //TODO: New way
            factory.ShouldNotBeNull();

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
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "78"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "98"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "MicrosoftEdge", "79"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "MicrosoftEdge", "99"));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "11"));
                yield return new TestCaseData(new SaucePlatform("Windows 11", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Windows 2012 R2", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Windows 2012", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Windows 2008", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 12", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 11", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.15", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.14", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.13", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.12", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.11", "chrome", "99"));
                yield return new TestCaseData(new SaucePlatform("Mac 10.10", "chrome", "87"));
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
