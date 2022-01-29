using NUnit.Framework;
using Saucery.OnDemand;
using Saucery.Options;
using Shouldly;
using System.Collections;

namespace UnitTests
{
    [TestFixture]
    public class AndroidFactoryVersionTests
    {
        [Test, TestCaseSource(typeof(AndroidDataClass), "SupportedTestCases")]
        public void IsSupportedPlatformTest(SaucePlatform saucePlatform)
        {
            saucePlatform = PlatformClassifer.Classify(saucePlatform);
            var factory = new OptionFactory(saucePlatform);
            var result = factory.IsSupportedPlatform();
            result.ShouldBeTrue();
        }

        //[Test, TestCaseSource(typeof(AndroidDataClass), "NotSupportedTestCases")]
        //public void IsNotSupportedPlatformTest(SaucePlatform saucePlatform)
        //{
        //    saucePlatform = PlatformClassifer.Classify(saucePlatform);
        //    var factory = new OptionFactory(saucePlatform);
        //    var result = factory.IsSupportedPlatform();
        //    result.ShouldBeFalse();
        //}

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
                yield return new TestCaseData(new SaucePlatform("", "", "latest", "android", "Google Pixel 3 GoogleAPI Emulator", "10.0.", "", "android", "landscape"));
            }
        }

        //public static IEnumerable NotSupportedTestCases
        //{
        //    get
        //    {
        //        yield return new TestCaseData(new SaucePlatform("android", "android", "android", "10", "Google Pixel 3 GoogleAPI Emulator", "10.0.", "", "android", "landscape"));
        //    }
        //}
    }
}
