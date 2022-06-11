using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC;

//BigSur
public class Mac11Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac11Platform(SupportedPlatform sp) : base (sp)
    {
        Selenium4BrowserNames = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI};
        BrowsersWithLatestVersion = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE };
        PlatformNameForOption = "macOS 11.00";
        ScreenResolutions = new List<string> { SauceryConstants.SCREENRES_1024_768,
                                               SauceryConstants.SCREENRES_1152_864,
                                               SauceryConstants.SCREENRES_1280_960,
                                               SauceryConstants.SCREENRES_1376_1032,
                                               SauceryConstants.SCREENRES_1440_900,
                                               SauceryConstants.SCREENRES_1600_1200,
                                               SauceryConstants.SCREENRES_1920_1440,
                                               SauceryConstants.SCREENRES_2048_1536 };
    }
}
