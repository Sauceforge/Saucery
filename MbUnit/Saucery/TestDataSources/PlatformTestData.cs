using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gallio.Framework;
using Newtonsoft.Json;
using Saucery.OnDemand;
using Saucery.Util;

namespace Saucery.TestDataSources {
    public class PlatformTestData {
        #region Attributes
        internal string Os { get; set; }
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
            return Double.Parse(BrowserVersion);
        }

        public static IEnumerable GetPlatforms {
            get {
                DiagnosticLog.WriteLine("GetPlatforms called!");
                var ondemand = Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_ONDEMAND_BROWSERS);
                if (UserChecker.ItIsMe()) {
                    DiagnosticLog.WriteLine("onDemand={0}", ondemand);
                }
                //var platforms = new JsonDeserializer().Deserialize<List<SaucePlatform>>(new RestResponse { Content = ondemand });
                var platforms = JsonConvert.DeserializeObject(ondemand, typeof(List<SaucePlatform>)) as List<SaucePlatform>;

                if (platforms != null) {
                    DiagnosticLog.WriteLine("Testing on {0} platforms", platforms.Count);
                }

                return platforms != null
                    ? platforms.Select(platform => new PlatformTestData {
                        Os = Sanitiser.SanitisePlatformField(platform.Os),
                        BrowserName = Sanitiser.SanitisePlatformField(platform.Browser),
                        BrowserVersion = Sanitiser.SanitisePlatformField(platform.BrowserVersion),
                        LongName = Sanitiser.SanitisePlatformField(platform.LongName),
                        LongVersion = Sanitiser.SanitisePlatformField(platform.LongVersion),
                        Url = Sanitiser.SanitisePlatformField(platform.Url),
                        Device = platform.Device ?? SauceryConstants.NULL_STRING,
                        DeviceType = platform.DeviceType ?? SauceryConstants.NULL_STRING,
                        DeviceOrientation = platform.DeviceOrientation ?? SauceryConstants.NULL_STRING })
                    : null;
            }
        }

        #endregion
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */