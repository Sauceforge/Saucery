using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.Base
{
    public abstract class PlatformBase : IPlatform
    {
        public string Name { get; set; }
        public string AutomationBackend { get; set; }
        public string RecommendedAppiumVersion { get; set; }
        public List<string> SupportedBackendVersions { get; set; }
        public List<object> DeprecatedBackendVersions { get; set; }
        protected List<string> BrowserNames { get; set; }
        public List<BrowserBase> Browsers;
        

        public PlatformBase(SupportedPlatform sp)
        {
            Name = sp.os;
            AutomationBackend = sp.automation_backend;
            RecommendedAppiumVersion = sp.recommended_backend_version;
            SupportedBackendVersions = sp.supported_backend_versions;
            DeprecatedBackendVersions = sp.deprecated_backend_versions;
            Browsers = new List<BrowserBase>();
        }

        public bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && BrowserNames.Contains(sp.api_name);
        }
    }
}
