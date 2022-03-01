using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.Base
{
    public abstract class PlatformBase : IPlatform
    {
        public string Name { get; set; }
        public abstract string PlatformNameForOption {get;set;}
        public string AutomationBackend { get; set; }
        public string RecommendedAppiumVersion { get; set; }
        public List<string> SupportedBackendVersions { get; set; }
        public List<object> DeprecatedBackendVersions { get; set; }
        public string PlatformVersion { get; set; }
        public List<string> BrowserNames { get; set; }
        
        public List<BrowserBase> Browsers { get; set; }
        

        public PlatformBase(SupportedPlatform sp)
        {
            Name = sp.os;
            AutomationBackend = sp.automation_backend;
            RecommendedAppiumVersion = sp.recommended_backend_version;
            SupportedBackendVersions = sp.supported_backend_versions;
            DeprecatedBackendVersions = sp.deprecated_backend_versions;
            if (sp.IsMobilePlatform())
            {
                PlatformVersion = sp.short_version;
            }
            Browsers = new List<BrowserBase>();
        }

        public bool IsMobilePlatform()
        {
            return RecommendedAppiumVersion != null && AutomationBackend.Equals("appium");
        }
    }
}
