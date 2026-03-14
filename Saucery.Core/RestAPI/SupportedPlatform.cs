using System.Text.Json.Serialization;

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
    public string? Os { get; set; }


    //REAL DEVICE
    [JsonPropertyName("name")]
    public string? RealDeviceName { get; set; }
    [JsonPropertyName("abiType")]
    public string? AbiType { get; set; }
    [JsonPropertyName("apiLevel")]
    public int ApiLevel { get; set; }
    [JsonPropertyName("cpuCores")]
    public int CpuCores { get; set; }
    [JsonPropertyName("cpuFrequency")]
    public int CpuFrequency { get; set; }
    [JsonPropertyName("defaultOrientation")]
    public string? DefaultOrientation { get; set; }
    [JsonPropertyName("dpi")]
    public int Dpi { get; set; }
    [JsonPropertyName("hasOnScreenButtons")]
    public bool HasOnScreenButtons { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("internalOrientation")]
    public string? InternalOrientation { get; set; }
    [JsonPropertyName("internalStorageSize")]
    public int InternalStorageSize { get; set; }
    [JsonPropertyName("isArm")]
    public bool IsArm { get; set; }
    [JsonPropertyName("isKeyGuardDisabled")]
    public bool IsKeyGuardDisabled { get; set; }
    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }
    [JsonPropertyName("isRooted")]
    public bool IsRooted { get; set; }
    [JsonPropertyName("isTablet")]
    public bool IsTablet { get; set; }
    [JsonPropertyName("manufacturer")]
    public List<string>? Manufacturer { get; set; }
    [JsonPropertyName("modelNumber")]
    public string? ModelNumber { get; set; }
    [JsonPropertyName("osVersion")]
    public string? OsVersion { get; set; }
    [JsonPropertyName("pixelsPerPoint")]
    public int PixelsPerPoint { get; set; }
    [JsonPropertyName("ramSize")]
    public int RamSize { get; set; }
    [JsonPropertyName("resolutionHeight")]
    public int ResolutionHeight { get; set; }
    [JsonPropertyName("resolutionWidth")]
    public int ResolutionWidth { get; set; }
    [JsonPropertyName("screenSize")]
    public double ScreenSize { get; set; }
    [JsonPropertyName("sdCardSize")]
    public int SdCardSize { get; set; }
    [JsonPropertyName("supportsAppiumWebAppTesting")]
    public bool SupportsAppiumWebAppTesting { get; set; }
    [JsonPropertyName("supportsGlobalProxy")]
    public bool SupportsGlobalProxy { get; set; }
    [JsonPropertyName("supportsMinicapSocketConnection")]
    public bool SupportsMinicapSocketConnection { get; set; }
    [JsonPropertyName("supportsMockLocations")]
    public bool SupportsMockLocations { get; set; }
    [JsonPropertyName("cpuType")]
    public string? CpuType { get; set; }
    [JsonPropertyName("deviceFamily")]
    public string? DeviceFamily { get; set; }
    [JsonPropertyName("dpiName")]
    public string? DpiName { get; set; }
    [JsonPropertyName("isAlternativeIoEnabled")]
    public bool IsAlternativeIoEnabled { get; set; }
    [JsonPropertyName("supportsManualWebTesting")]
    public bool SupportsManualWebTesting { get; set; }
    [JsonPropertyName("supportsMultiTouch")]
    public bool SupportsMultiTouch { get; set; }
    [JsonPropertyName("supportsXcuiTest")]
    public bool SupportsXcuiTest { get; set; }

    public int short_version_as_int => 
        int.TryParse(short_version, out _) 
            ? int.Parse(short_version) 
            : 0;

    public bool IsIOSPlatform() => 
        automation_backend!.Equals("appium") && 
        recommended_backend_version != null && 
        (api_name!.Equals("iphone") || api_name.Equals("ipad"));

    public bool IsAndroidPlatform() => 
        api_name == "android";

    public bool IsMobilePlatform() => 
        automation_backend!.Equals("appium") &&
        recommended_backend_version != null &&
        api_name is "iphone" or "ipad" or "android";
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 18th September 2014
* 
*/