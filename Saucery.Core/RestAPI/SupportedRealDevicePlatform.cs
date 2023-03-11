namespace Saucery.Core.RestAPI
{
    public class SupportedRealDevicePlatform
    {
        public SupportedRealDevicePlatform(string osVersion)
        {
            OsVersion = osVersion;
        }

        public string AbiType { get; set; }
        public int ApiLevel { get; set; }
        public int CpuCores { get; set; }
        public int CpuFrequency { get; set; }
        public string DefaultOrientation { get; set; }
        public int Dpi { get; set; }
        public bool HasOnScreenButtons { get; set; }
        public string Id { get; set; }
        public string InternalOrientation { get; set; }
        public int InternalStorageSize { get; set; }
        public bool IsArm { get; set; }
        public bool IsKeyGuardDisabled { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsRooted { get; set; }
        public bool IsTablet { get; set; }
        public List<string> Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string Name { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
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
        public string CpuType { get; set; }
        public string DeviceFamily { get; set; }
        public string DpiName { get; set; }
        public bool IsAlternativeIoEnabled { get; set; }
        public bool SupportsManualWebTesting { get; set; }
        public bool SupportsMultiTouch { get; set; }
        public bool SupportsXcuiTest { get; set; }
    }
}