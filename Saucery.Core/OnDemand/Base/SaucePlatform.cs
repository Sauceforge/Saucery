using Newtonsoft.Json;
using Saucery.Core.Util;

namespace Saucery.Core.OnDemand.Base;

public class SaucePlatform(string desktopPlatformName = "",
                     string browser = "",
                     string browserVersion = "",
                     string screenResolution = "",
                     string platform = "",
                     string longName = "",
                     string longVersion = "",
                     string deviceOrientation = "")
{
    #region Attributes

    public string Os { get; set; } = Sanitiser.SanitisePlatformField(desktopPlatformName)!;
    public string Platform { get; set; } = Sanitiser.SanitisePlatformField(platform)!;
    public string Browser { get; set; } = Sanitiser.SanitisePlatformField(browser)!;

    [JsonProperty(PropertyName = "browser-version")]
    public string BrowserVersion { get; set; } = Sanitiser.SanitisePlatformField(browserVersion)!;

    public string ScreenResolution { get; set; } = Sanitiser.SanitisePlatformField(screenResolution)!;

    [JsonProperty(PropertyName = "long-name")]
    public string LongName { get; set; } = Sanitiser.SanitisePlatformField(longName)!;

    [JsonProperty(PropertyName = "long-version")]
    public string LongVersion { get; set; } = Sanitiser.SanitisePlatformField(longVersion)!;

    [JsonProperty(PropertyName = "device-orientation")]
    public string DeviceOrientation { get; set; } = deviceOrientation ?? SauceryConstants.NULL_STRING;

    public PlatformType PlatformType { get; set; }

    public string? TestName { get; set; }

    #endregion

    public bool NeedsExpansion() => BrowserVersion.Replace(SauceryConstants.SPACE, string.Empty).Contains(SauceryConstants.PLATFORM_SEPARATOR);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/