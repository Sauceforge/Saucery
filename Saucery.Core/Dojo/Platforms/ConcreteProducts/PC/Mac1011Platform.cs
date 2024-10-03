using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//OS X El Capitan
public sealed class Mac1011Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac1011Platform(SupportedPlatform sp) : base(sp)
    {
        Selenium4BrowserNames = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE];
        BrowsersWithLatestVersion = [SauceryConstants.BROWSER_CHROME];
        PlatformNameForOption = "OS X 10.11";
        ScreenResolutions = [ SauceryConstants.SCREENRES_1024_768,
                              SauceryConstants.SCREENRES_1152_864,
                              SauceryConstants.SCREENRES_1280_960,
                              SauceryConstants.SCREENRES_1376_1032,
                              SauceryConstants.SCREENRES_1600_1200,
                              SauceryConstants.SCREENRES_1920_1440,
                              SauceryConstants.SCREENRES_2048_1536 ];
    }
}
