using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //High Sierra
    public class Mac1013Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac1013Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI };
            BrowsersWithLatestVersion = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE };
            PlatformNameForOption = "macOS 10.13";
        }
    }
}
