using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.PC;

internal class Windows7BrowserCreator(SupportedPlatform sp) : BrowserCreator(sp)
{
    public override BrowserBase? Create(string platformNameForOption, List<string> screenResolutions) => Platform.api_name switch
    {
        SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption),
        SauceryConstants.BROWSER_FIREFOX => new FirefoxBrowser(Platform, screenResolutions, platformNameForOption),
        SauceryConstants.BROWSER_IE => new IEBrowser(Platform, screenResolutions, platformNameForOption),
        _ => null,
    };
}