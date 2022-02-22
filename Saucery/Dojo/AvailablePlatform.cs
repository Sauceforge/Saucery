using Saucery.RestAPI;
using System;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class AvailablePlatform
    {
        private SupportedPlatform Sp;
        public string Name;
        public string AutomationBackend;
        public string RecommendedAppiumVersion;
        public List<Browser> Browsers;

        public AvailablePlatform(SupportedPlatform sp)
        {
            Sp = sp;
            Name = Sp.os;
            AutomationBackend = Sp.automation_backend;
            RecommendedAppiumVersion = Sp.recommended_backend_version;
            Browsers = new List<Browser>();
        }

        public bool IsDesktopPlatform()
        {
            return IsDesktop() && (Sp.api_name == "chrome" ||
                                   Sp.api_name == "firefox" ||
                                   Sp.api_name == "MicrosoftEdge" ||
                                   Sp.api_name == "internet explorer" ||
                                   Sp.api_name == "safari");
        }
        private bool IsDesktop()
        {
            return AutomationBackend.Equals("webdriver");
        }

        public bool IsMobilePlatform()
        {
            return IsMobile() && (Sp.api_name == "iphone" || Sp.api_name == "ipad" || Sp.api_name == "android");
        }

        private bool IsMobile()
        {
            return AutomationBackend.Equals("appium");
        }

        public Browser FindBrowser(SupportedPlatform platform)
        {
            Predicate<Browser> desktop = b => b.Api.Equals(platform.api_name);
            Predicate<Browser> mobile = b => b.Api.Equals(platform.api_name) && b.DeviceName != null && b.DeviceName.Equals(platform.long_name);

            foreach (var b in Browsers)
            {
                return b.DeviceName == null 
                        ? Browsers.Find(desktop)
                        : Browsers.Find(mobile);
            }
            return null;
        }
    }
}