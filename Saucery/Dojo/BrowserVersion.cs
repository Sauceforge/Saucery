using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class BrowserVersion
    {
        public string Os { get; set; }
        public string PlatformNameForOption { get; set; }
        public string BrowserName { get; set; }
        public string Name { get; set; }
        public string AutomationBackend { get; set; }
        public string DeviceName { get; set; }
        public string RecommendedAppiumVersion { get; set; }
        public List<string> SupportedBackendVersions { get; set; }
        public List<object> DeprecatedBackendVersions { get; set; }

        public BrowserVersion(SupportedPlatform sp, string platformNameForOption)
        {
            Os = sp.os;
            PlatformNameForOption = platformNameForOption;
            BrowserName = sp.api_name;
            Name = sp.latest_stable_version != string.Empty ? sp.latest_stable_version : sp.short_version;
            AutomationBackend = sp.automation_backend;
            DeviceName = sp.long_name;
            RecommendedAppiumVersion = sp.recommended_backend_version;
            SupportedBackendVersions = sp.supported_backend_versions;
            DeprecatedBackendVersions = sp.deprecated_backend_versions;
        }
    }
}