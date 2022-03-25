using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Saucery.Util;
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
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "latest"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "78"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "98"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "79"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1014, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1013, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1012, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1011, SauceryConstants.BROWSER_CHROME, "99"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_MAC_1010, SauceryConstants.BROWSER_CHROME, "87"));
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "9999"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "25"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "3"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_FIREFOX, "9999"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_SAFARI, "7"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "8"));
                yield return new TestCaseData(new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "9999"));
            }
        }
    }
}
