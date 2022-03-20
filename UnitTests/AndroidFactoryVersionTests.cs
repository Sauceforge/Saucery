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
        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
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

        [Test, TestCaseSource(typeof(AndroidDataClass), "NotSupportedTestCases")]
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
