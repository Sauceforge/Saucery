using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//High Sierra
public sealed class Mac1013Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac1013Platform(SupportedPlatform sp) : base(sp)
    {
        Selenium4BrowserNames = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI];
        BrowsersWithLatestVersion = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE];
        PlatformNameForOption = "macOS 10.13";
        ScreenResolutions = [ SauceryConstants.SCREENRES_1024_768,
                              SauceryConstants.SCREENRES_1152_864,
                              SauceryConstants.SCREENRES_1280_960,
                              SauceryConstants.SCREENRES_1376_1032,
                              SauceryConstants.SCREENRES_1400_1050,
                              SauceryConstants.SCREENRES_1600_1200,
                              SauceryConstants.SCREENRES_1920_1440,
                              SauceryConstants.SCREENRES_2048_1536, 
                              SauceryConstants.SCREENRES_2360_1770 ];
    }
}
