using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC;

internal class Windows11BrowserCreator : BrowserCreator
{
    public Windows11BrowserCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override BrowserBase Create(string platformNameForOption, List<string> screenResolutions)
    {
        return Platform.api_name switch
        {
            SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption),
            SauceryConstants.BROWSER_EDGE => new EdgeBrowser(Platform, screenResolutions, platformNameForOption),
            SauceryConstants.BROWSER_FIREFOX => new FirefoxBrowser(Platform, screenResolutions, platformNameForOption),
            _ => null,
        };
    }
}