using Saucery.Dojo.Browsers;
using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class AvailablePlatform
    {
        //private SupportedPlatform Sp;
        //private BrowserFactory BrowserFactory;
        //public string Name => Sp.os;
        //public string AutomationBackend => Sp.automation_backend;
        //public string RecommendedAppiumVersion => Sp.recommended_backend_version;
        //public List<string> SupportedBackendVersions => Sp.supported_backend_versions;
        //public List<object> DeprecatedBackendVersions => Sp.deprecated_backend_versions;
        //public List<BrowserBase> Browsers;

        //public AvailablePlatform(SupportedPlatform sp)
        //{
        //    //Sp = sp;
        //    //BrowserFactory = new BrowserFactory(sp);
        //    //Browsers = new List<BrowserBase>();
            
        //}

        //public bool IsDesktopPlatform()
        //{
        //    return IsDesktop() && (Sp.api_name == "chrome" ||
        //                           Sp.api_name == "firefox" ||
        //                           Sp.api_name == "MicrosoftEdge" ||
        //                           Sp.api_name == "internet explorer" ||
        //                           Sp.api_name == "safari");
        //}
        //private bool IsDesktop()
        //{
        //    return AutomationBackend.Equals("webdriver");
        //}

        //public bool IsMobilePlatform()
        //{
        //    return IsMobile() && (Sp.api_name == "iphone" || Sp.api_name == "ipad" || Sp.api_name == "android");
        //}

        //private bool IsMobile()
        //{
        //    return AutomationBackend.Equals("appium");
        //}

        //public BrowserBase FindBrowser(PlatformBase sp)
        //{
        //    Predicate<BrowserBase> desktop = b => b.Name.Equals(sp.api_name);
        //    Predicate<BrowserBase> mobile = b => b.Name.Equals(sp.api_name) && b.DeviceName != null && b.DeviceName.Equals(sp.long_name);

        //    foreach (var b in Browsers)
        //    {
        //        return b.Name == null 
        //                ? Browsers.Find(desktop)
        //                : Browsers.Find(mobile);
        //    }
        //    return null;
        //}
    }
}