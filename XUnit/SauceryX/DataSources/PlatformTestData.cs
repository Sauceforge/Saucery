using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SauceryX.Activation;
using SauceryX.OnDemand;
using SauceryX.Util;

namespace SauceryX.DataSources {
    public class PlatformTestData : IEnumerable {
        #region Attributes
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

        #region Constructors
        static PlatformTestData() {
            if (UserChecker.ItIsMe()) {
                Console.WriteLine(@"onDemand={0}", Enviro.SauceOnDemandBrowsers);
            }
        }

        #endregion

        #region Methods
        internal double ParseBrowserVersion() {
            return double.Parse(BrowserVersion);
        }

        //public static IEnumerable<PlatformTestData> GetPlatforms {
        //    get {
        //        var validator = new ActivationValidator();
        //        validator.CheckActivation();
        //        //var platforms = new JsonDeserializer().Deserialize<List<SaucePlatform>>(new RestResponse { Content = ondemand });
        //        var platforms = JsonConvert.DeserializeObject(Enviro.SauceOnDemandBrowsers, typeof(List<SaucePlatform>)) as List<SaucePlatform>;

        //        return platforms != null
        //            ? platforms.Select(platform => new PlatformTestData {
        //                Os = Sanitiser.SanitisePlatformField(platform.Os),
        //                Platform = Sanitiser.SanitisePlatformField(platform.Platform),
        //                BrowserName = Sanitiser.SanitisePlatformField(platform.Browser),
        //                BrowserVersion = Sanitiser.SanitisePlatformField(platform.BrowserVersion),
        //                LongName = Sanitiser.SanitisePlatformField(platform.LongName),
        //                LongVersion = Sanitiser.SanitisePlatformField(platform.LongVersion),
        //                Url = Sanitiser.SanitisePlatformField(platform.Url),
        //                Device = platform.Device ?? SauceryConstants.NULL_STRING,
        //                DeviceType = platform.DeviceType ?? SauceryConstants.NULL_STRING,
        //                DeviceOrientation = platform.DeviceOrientation ?? SauceryConstants.NULL_STRING})
        //            : null;
        //    }
        //}

        #endregion

        public IEnumerator GetEnumerator() {
            var validator = new ActivationValidator();
            validator.CheckActivation();
            //var platforms = new JsonDeserializer().Deserialize<List<SaucePlatform>>(new RestResponse { Content = Enviro.SauceOnDemandBrowsers });
            var platforms = JsonConvert.DeserializeObject(Enviro.SauceOnDemandBrowsers, typeof(List<SaucePlatform>)) as List<SaucePlatform>;
            
            return platforms != null
                    ? platforms.Select(platform => new SaucePlatform(Sanitiser.SanitisePlatformField(platform.Os),
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