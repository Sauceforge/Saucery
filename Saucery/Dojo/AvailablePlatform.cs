using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class AvailablePlatform
    {
        public string Name;
        public string Version;
        public string AutomationBackend;
        public string RecommendedAppiumVersion;
        public List<Browser> Browsers;

        public AvailablePlatform(SupportedPlatform sp)
        {
            Name = sp.os;
            Version = sp.short_version;
            AutomationBackend = sp.automation_backend;
            RecommendedAppiumVersion = sp.recommended_backend_version;
            Browsers = new List<Browser>();
        }

        public Browser FindBrowser(SupportedPlatform platform)
        {
            return Browsers.Find(b => b.Api.Equals(platform.api_name));
        }
    }
}