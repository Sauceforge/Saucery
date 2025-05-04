using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//Senoma
public sealed class Mac14Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac14Platform(SupportedPlatform sp) : base (sp)
    {
        Selenium4BrowserNames = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI];
        BrowsersWithLatestVersion = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI];
        PlatformNameForOption = "macOS 14";
        ScreenResolutions = [ SauceryConstants.SCREENRES_1440_900 ];
    }
}
