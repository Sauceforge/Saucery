using NUnit.Framework;
using Saucery.OnDemand;
using Saucery.Options;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    public class DesktopFactoryVersionTests
    {
        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            var factory = new OptionFactory(saucePlatform);
            var result = factory.IsSupportedPlatform();
            result.ShouldBeTrue();
        }

        [Test, TestCaseSource(typeof(DesktopDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            var factory = new OptionFactory(saucePlatform);
            var result = factory.IsSupportedPlatform();
            result.ShouldBeFalse();
        }

        [Test, TestCaseSource(typeof(DesktopDataClass), "SupportedTestCases")]
        public void DesktopOptionTest(SaucePlatform saucePlatform)
        {
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
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "latest", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "62", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "61", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "54", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "53", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "safari", "12", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "safari", "11", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "11", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "microsoftedge", "80", "", "", "", "", "", ""));
            }
        }

        public static IEnumerable NotSupportedTestCases
        {
            get
            {
                yield return new TestCaseData(new SaucePlatform("Windows 10", "chrome", "60", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "firefox", "52", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "safari", "10", "", "", "", "", "", ""));
                yield return new TestCaseData(new SaucePlatform("Windows 10", "internet explorer", "10", "", "", "", "", "", ""));
            }
        }
    }
}
