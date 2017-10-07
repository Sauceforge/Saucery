using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Saucery2.Util;

namespace Saucery2.OnDemand {
    public class SaucePlatform {
        #region Attributes

        public string Os { get; set; }
        public string Platform { get; set; }
        public string Browser { get; set; }

        [JsonProperty(PropertyName = "browser-version")]
        public string BrowserVersion { get; set; }

        [JsonProperty(PropertyName = "long-name")]
        public string LongName { get; set; }

        [JsonProperty(PropertyName = "long-version")]
        public string LongVersion { get; set; }

        public string Url { get; set; }
        public string Device { get; set; }

        //[JsonProperty(PropertyName = "device-type")]
        //public string DeviceType { get; set; }

        [JsonProperty(PropertyName = "device-orientation")]
        public string DeviceOrientation { get; set; }

        #endregion

        #region Methods

        internal double ParseBrowserVersion() {
            return double.Parse(BrowserVersion);
        }

        public static IEnumerable<SaucePlatform> GetPlatforms {
            get {
                OnceOnlyMessages.OnDemand();
                //var platforms = new JsonDeserializer().Deserialize<List<SaucePlatform>>(new RestResponse { Content = ondemand });
                var platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);
                OnceOnlyMessages.TestingOn(platforms);

                return platforms != null
                    ? platforms.Select(platform => new SaucePlatform {
                        Os = Sanitiser.SanitisePlatformField(platform.Os),
                        Platform = Sanitiser.SanitisePlatformField(platform.Platform),
                        Browser = Sanitiser.SanitisePlatformField(platform.Browser),
                        BrowserVersion = Sanitiser.SanitisePlatformField(platform.BrowserVersion),
                        LongName = Sanitiser.SanitisePlatformField(platform.LongName),
                        LongVersion = Sanitiser.SanitisePlatformField(platform.LongVersion),
                        Url = Sanitiser.SanitisePlatformField(platform.Url),
                        Device = platform.Device ?? SauceryConstants.NULL_STRING,
                        //DeviceType = platform.DeviceType ?? SauceryConstants.NULL_STRING,
                        DeviceOrientation = platform.DeviceOrientation ?? SauceryConstants.NULL_STRING
                    })
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