using Newtonsoft.Json;
using Saucery.Util;

namespace Saucery.OnDemand.Base
{
    public class SaucePlatform {
        #region Attributes

        public string Os { get; set; }
        public string Platform { get; set; }
        public string Browser { get; set; }

        [JsonProperty(PropertyName = "browser-version")]
        public string BrowserVersion { get; set; }

        public string ScreenResolution { get; set; }

        [JsonProperty(PropertyName = "long-name")]
        public string LongName { get; set; }

        [JsonProperty(PropertyName = "long-version")]
        public string LongVersion { get; set; }

        public string Url { get; set; }
        public string Device { get; set; }

        [JsonProperty(PropertyName = "device-orientation")]
        public string DeviceOrientation { get; set; }

        public PlatformType PlatformType { get; set; }

        public string AppiumVersion { get; set; }

        public string TestName { get; set; }

        #endregion

        #region Constructors

        //static SaucePlatform() {
        //    //OnceOnlyMessages.OnDemand();
        //}

        public SaucePlatform(string desktopPlatformName = "", string browser = "", string browserVersion = "", string screenResolution = "", string platform = "", string longName = "",
            string longVersion = "", string url = "", string device = "", string appiumVersion = "", string deviceOrientation = "") {
            Os = Sanitiser.SanitisePlatformField(desktopPlatformName);
            Browser = Sanitiser.SanitisePlatformField(browser);
            BrowserVersion = Sanitiser.SanitisePlatformField(browserVersion);
            ScreenResolution = Sanitiser.SanitisePlatformField(screenResolution);
            Platform = Sanitiser.SanitisePlatformField(platform);
            LongName = Sanitiser.SanitisePlatformField(longName);
            LongVersion = Sanitiser.SanitisePlatformField(longVersion);
            Url = Sanitiser.SanitisePlatformField(url);
            Device = device ?? SauceryConstants.NULL_STRING;
            AppiumVersion = Sanitiser.SanitisePlatformField(appiumVersion);
            DeviceOrientation = deviceOrientation ?? SauceryConstants.NULL_STRING;
        }

        #endregion
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */