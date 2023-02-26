using Newtonsoft.Json;
using Saucery.Util;

namespace Saucery.OnDemand.Base;

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

    [JsonProperty(PropertyName = "device-orientation")]
    public string DeviceOrientation { get; set; }

    public PlatformType PlatformType { get; set; }

    public string TestName { get; set; }

    #endregion

    #region Constructors

    public SaucePlatform(string desktopPlatformName = "", 
                         string browser = "", 
                         string browserVersion = "", 
                         string screenResolution = "", 
                         string platform = "", 
                         string longName = "",
                         string longVersion = "", 
                         string deviceOrientation = "") {
        Os = Sanitiser.SanitisePlatformField(desktopPlatformName);
        Browser = Sanitiser.SanitisePlatformField(browser);
        BrowserVersion = Sanitiser.SanitisePlatformField(browserVersion);
        ScreenResolution = Sanitiser.SanitisePlatformField(screenResolution);
        Platform = Sanitiser.SanitisePlatformField(platform);
        LongName = Sanitiser.SanitisePlatformField(longName);
        LongVersion = Sanitiser.SanitisePlatformField(longVersion);
        DeviceOrientation = deviceOrientation ?? SauceryConstants.NULL_STRING;
    }

    #endregion

    public bool NeedsExpansion() => BrowserVersion.Replace(SauceryConstants.SPACE, string.Empty).Contains(SauceryConstants.PLATFORM_SEPARATOR);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/