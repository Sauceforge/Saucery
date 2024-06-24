namespace Saucery.Core.RestAPI;

public class SupportedPlatform {
    public List<string>? deprecated_backend_versions { get; set; }
    public string? short_version { get; set; }
    public string? long_name { get; set; }
    public string? recommended_backend_version { get; set; }
    public string? long_version { get; set; }
    public string? api_name { get; set; }
    public List<string>? supported_backend_versions { get; set; }
    public string? device { get; set; }
    public string? latest_stable_version { get; set; }
    public string? automation_backend { get; set; }
    public string? os { get; set; }


    //REAL DEVICE
    public string? AbiType { get; set; }
    public int ApiLevel { get; set; }
    public int CpuCores { get; set; }
    public int CpuFrequency { get; set; }
    public string? DefaultOrientation { get; set; }
    public int Dpi { get; set; }
    public bool HasOnScreenButtons { get; set; }
    public string? Id { get; set; }
    public string? InternalOrientation { get; set; }
    public int InternalStorageSize { get; set; }
    public bool IsArm { get; set; }
    public bool IsKeyGuardDisabled { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsRooted { get; set; }
    public bool IsTablet { get; set; }
    public List<string>? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }
    public string? Os { get; set; }
    public string? OsVersion { get; set; }
    public int PixelsPerPoint { get; set; }
    public int RamSize { get; set; }
    public int ResolutionHeight { get; set; }
    public int ResolutionWidth { get; set; }
    public double ScreenSize { get; set; }
    public int SdCardSize { get; set; }
    public bool SupportsAppiumWebAppTesting { get; set; }
    public bool SupportsGlobalProxy { get; set; }
    public bool SupportsMinicapSocketConnection { get; set; }
    public bool SupportsMockLocations { get; set; }
    public string? CpuType { get; set; }
    public string? DeviceFamily { get; set; }
    public string? DpiName { get; set; }
    public bool IsAlternativeIoEnabled { get; set; }
    public bool SupportsManualWebTesting { get; set; }
    public bool SupportsMultiTouch { get; set; }
    public bool SupportsXcuiTest { get; set; }

    public int short_version_as_int => int.TryParse(short_version, out int discard) ? int.Parse(short_version) : 0;

    public bool IsIOSPlatform() => automation_backend!.Equals("appium") && 
                                   recommended_backend_version != null && 
                                   (api_name!.Equals("iphone") || api_name.Equals("ipad"));

    public bool IsAndroidPlatform() => api_name == "android";//return automation_backend.Equals("appium") &&//       recommended_backend_version != null &&//       api_name.Equals("android");

    public bool IsMobilePlatform() => automation_backend!.Equals("appium") &&
                                      recommended_backend_version != null &&
                                      api_name is "iphone" or "ipad" or "android";
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 18th September 2014
* 
*/