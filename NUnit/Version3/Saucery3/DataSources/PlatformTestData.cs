using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Saucery3.OnDemand;
using Saucery3.Util;

namespace Saucery3.DataSources {

    public class PlatformTestData : IEnumerable {
        #region Attributes
        internal static List<SaucePlatform> Platforms { get; set; }

        internal string Os { get; set; }
        internal string Platform { get; set; }
        internal string BrowserName { get; set; }
        internal string BrowserVersion { get; set; }
        internal string LongName { get; set; }
        internal string LongVersion { get; set; }
        internal string Url { get; set; }
        internal string Device { get; set; }
        internal string DeviceType { get; set; }
        internal string DeviceOrientation { get; set; }
        #endregion

        #region Methods
        internal double ParseBrowserVersion() {
            return double.Parse(BrowserVersion);
        }

        #endregion

        static PlatformTestData() {
            //Console.WriteLine("Start static PlatformTestData()");
            //Console.WriteLine(@"After CheckActivation in PlatformTestData");
            Platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);
            OnceOnlyMessages.TestingOn(Platforms);
        }

        public IEnumerator GetEnumerator() {
            return Platforms != null
                    ? Platforms.Select(platform => new SaucePlatform(Sanitiser.SanitisePlatformField(platform.Os),
                        Sanitiser.SanitisePlatformField(platform.Platform),
                        Sanitiser.SanitisePlatformField(platform.Browser),
                        Sanitiser.SanitisePlatformField(platform.BrowserVersion),
                        Sanitiser.SanitisePlatformField(platform.LongName),
                        Sanitiser.SanitisePlatformField(platform.LongVersion),
                        Sanitiser.SanitisePlatformField(platform.Url),
                        platform.Device ?? SauceryConstants.NULL_STRING,
                        platform.DeviceOrientation ?? SauceryConstants.NULL_STRING)).GetEnumerator()
                    : null;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */