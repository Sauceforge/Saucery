using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.ChromiumOS;

internal class ChromiumOSBrowserCreator(SupportedPlatform sp) : BrowserCreator(sp)
{
    public override BrowserBase? Create(
        string platformNameForOption, 
        List<string> screenResolutions, 
        bool isArmRequired = false) => 
        Platform.api_name switch
        {
            SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption, isArmRequired),
            _ => null,
        };
}