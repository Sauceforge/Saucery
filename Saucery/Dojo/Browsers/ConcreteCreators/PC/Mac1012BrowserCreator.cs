using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC;

internal class Mac1012BrowserCreator : BrowserCreator
{
    public Mac1012BrowserCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override BrowserBase Create(string platformNameForOption, List<string> screenResolutions) => Platform.api_name switch
    {
        SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption),
        SauceryConstants.BROWSER_EDGE => new EdgeBrowser(Platform, screenResolutions, platformNameForOption),
        SauceryConstants.BROWSER_FIREFOX => new FirefoxBrowser(Platform, screenResolutions, platformNameForOption),
        _ => null,
    };
}