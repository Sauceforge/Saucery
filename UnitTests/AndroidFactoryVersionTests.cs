using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    [Order(1)]
    public class AndroidFactoryVersionTests
    {
        static PlatformConfigurator PlatformConfigurator { get; set; }

        static AndroidFactoryVersionTests() {
            PlatformConfigurator = new PlatformConfigurator();
        }

        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.Classify();
            validPlatform.ShouldNotBeNull();

            var factory = new OptionFactory(validPlatform); //TODO: New way
            factory.ShouldNotBeNull();
        }

        [Test, TestCaseSource(typeof(AndroidDataClass), "NotSupportedTestCases")]
        public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.ShouldBeNull();
        }

        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
        public void AppiumAndroidOptionTest(SaucePlatform saucePlatform)
        {
            saucePlatform.Classify();
            var validPlatform = PlatformConfigurator.Validate(saucePlatform);
            validPlatform.Classify();
            validPlatform.ShouldNotBeNull();

            var factory = new OptionFactory(validPlatform); //TODO: New way
            factory.ShouldNotBeNull();

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
                yield return new TestCaseData(new SaucePlatform("Linux", "", "11.0", "android", "Google Pixel 4a GoogleAPI Emulator", "11.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "10.0", "android", "Google Pixel 3a GoogleAPI Emulator", "10.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "9.0", "android", "Google Pixel 3 GoogleAPI Emulator", "9.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "8.1", "android", "Google Pixel C GoogleAPI Emulator", "8.1", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "8.0", "android", "Google Pixel C GoogleAPI Emulator", "8.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "7.1", "android", "Google Pixel C GoogleAPI Emulator", "7.1", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "7.0", "android", "Google Pixel C GoogleAPI Emulator", "7.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "6.0", "android", "Android GoogleAPI Emulator", "6.0", "", "android", "1.20.2", "landscape"));
                yield return new TestCaseData(new SaucePlatform("Linux", "", "5.1", "android", "Android GoogleAPI Emulator", "5.1", "", "android", "1.20.2", "landscape"));
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
