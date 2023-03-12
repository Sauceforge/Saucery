using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//OS X Yosemite
public class Mac1010Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac1010Platform(SupportedPlatform sp) : base(sp)
    {
        Selenium4BrowserNames = new List<string> { SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_EDGE };
        BrowsersWithLatestVersion = new List<string> { };
        PlatformNameForOption = "OS X 10.10";
        ScreenResolutions = new List<string> { SauceryConstants.SCREENRES_800_600,
                                               SauceryConstants.SCREENRES_1024_768,
                                               SauceryConstants.SCREENRES_1152_720,
                                               SauceryConstants.SCREENRES_1152_864,
                                               SauceryConstants.SCREENRES_1152_900,
                                               SauceryConstants.SCREENRES_1280_720,
                                               SauceryConstants.SCREENRES_1280_768,
                                               SauceryConstants.SCREENRES_1280_800,
                                               SauceryConstants.SCREENRES_1280_960,
                                               SauceryConstants.SCREENRES_1280_1024,
                                               SauceryConstants.SCREENRES_1376_1032,
                                               SauceryConstants.SCREENRES_1440_900,
                                               SauceryConstants.SCREENRES_1600_900,
                                               SauceryConstants.SCREENRES_1600_1200,
                                               SauceryConstants.SCREENRES_1680_1050,
                                               SauceryConstants.SCREENRES_1920_1080,
                                               SauceryConstants.SCREENRES_1920_1200,
                                               SauceryConstants.SCREENRES_1920_1440,
                                               SauceryConstants.SCREENRES_2048_1152,
                                               SauceryConstants.SCREENRES_2048_1536 };
    }
}
