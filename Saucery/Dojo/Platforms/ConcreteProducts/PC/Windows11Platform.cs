using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows11Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Windows11Platform(SupportedPlatform sp) : base (sp)
        {
            Selenium4BrowserNames = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE };
            BrowsersWithLatestVersion = new List<string> { };
            PlatformNameForOption = "Windows 11";
            ScreenResolutions = new List<string> { SauceryConstants.SCREENRES_800_600,
                                                   SauceryConstants.SCREENRES_1024_768,
                                                   SauceryConstants.SCREENRES_1152_864,
                                                   SauceryConstants.SCREENRES_1280_768,
                                                   SauceryConstants.SCREENRES_1280_800,
                                                   SauceryConstants.SCREENRES_1280_960,
                                                   SauceryConstants.SCREENRES_1280_1024,
                                                   SauceryConstants.SCREENRES_1400_1050,
                                                   SauceryConstants.SCREENRES_1440_900,
                                                   SauceryConstants.SCREENRES_1600_1200,
                                                   SauceryConstants.SCREENRES_1680_1050,
                                                   SauceryConstants.SCREENRES_1920_1080,
                                                   SauceryConstants.SCREENRES_1920_1200,
                                                   SauceryConstants.SCREENRES_2560_1600 };
        }
    }
}
