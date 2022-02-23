using Saucery.Dojo.Browsers;
using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using System;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.Base
{
    public abstract class PlatformBase : IPlatform
    {
        //protected SupportedPlatform Sp;
        public string Name { get; set; }
        public string AutomationBackend { get; set; }
        public string RecommendedAppiumVersion { get; set; }
        public List<string> SupportedBackendVersions { get; set; }
        public List<object> DeprecatedBackendVersions { get; set; }
        public List<BrowserBase> Browsers;
        private BrowserFactory BrowserFactory;
        

        public PlatformBase(SupportedPlatform sp)
        {
            Name = sp.os;
            AutomationBackend = sp.automation_backend;
            RecommendedAppiumVersion = sp.recommended_backend_version;
            SupportedBackendVersions = sp.supported_backend_versions;
            DeprecatedBackendVersions = sp.deprecated_backend_versions;
            Browsers = new List<BrowserBase>();
            BrowserFactory = new BrowserFactory(sp);
        }

        public abstract bool IsDesktopPlatform(SupportedPlatform sp);

        public BrowserBase FindBrowser(SupportedPlatform sp)
        {
            Predicate<BrowserBase> desktop = b => b.Name.Equals(sp.api_name);
            Predicate<BrowserBase> mobile = b => b.Name.Equals(sp.api_name) && b.DeviceName != null && b.DeviceName.Equals(sp.long_name);

            BrowserBase result = null;
            foreach (var b in Browsers)
            {
                result = sp.IsMobilePlatform() ? Browsers.Find(mobile) : Browsers.Find(desktop);
            }
            return result;
        }
    }
}
