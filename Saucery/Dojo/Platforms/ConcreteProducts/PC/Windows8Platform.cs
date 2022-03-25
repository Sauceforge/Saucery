using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows8Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Windows8Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_IE };
            BrowsersWithLatestVersion = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX };
            PlatformNameForOption = "Windows 8";
        }
    }
}
