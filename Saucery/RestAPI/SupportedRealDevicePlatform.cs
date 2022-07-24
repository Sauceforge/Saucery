using System.Collections.Generic;

namespace Saucery.RestAPI
{
    public class SupportedRealDevicePlatform
    {
        public string abiType { get; set; }
        public int apiLevel { get; set; }
        public int cpuCores { get; set; }
        public int cpuFrequency { get; set; }
        public string defaultOrientation { get; set; }
        public int dpi { get; set; }
        public bool hasOnScreenButtons { get; set; }
        public string id { get; set; }
        public string internalOrientation { get; set; }
        public int internalStorageSize { get; set; }
        public bool isArm { get; set; }
        public bool isKeyGuardDisabled { get; set; }
        public bool isPrivate { get; set; }
        public bool isRooted { get; set; }
        public bool isTablet { get; set; }
        public List<string> manufacturer { get; set; }
        public string modelNumber { get; set; }
        public string name { get; set; }
        public string os { get; set; }
        public string osVersion { get; set; }
        public int pixelsPerPoint { get; set; }
        public int ramSize { get; set; }
        public int resolutionHeight { get; set; }
        public int resolutionWidth { get; set; }
        public double screenSize { get; set; }
        public int sdCardSize { get; set; }
        public bool supportsAppiumWebAppTesting { get; set; }
        public bool supportsGlobalProxy { get; set; }
        public bool supportsMinicapSocketConnection { get; set; }
        public bool supportsMockLocations { get; set; }
        public string cpuType { get; set; }
        public string deviceFamily { get; set; }
        public string dpiName { get; set; }
        public bool isAlternativeIoEnabled { get; set; }
        public bool supportsManualWebTesting { get; set; }
        public bool supportsMultiTouch { get; set; }
        public bool supportsXcuiTest { get; set; }
    }
}