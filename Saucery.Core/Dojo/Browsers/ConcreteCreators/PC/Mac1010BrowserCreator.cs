using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.PC;

internal class Mac1010BrowserCreator : BrowserCreator
{
    public Mac1010BrowserCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override BrowserBase? Create(string platformNameForOption, List<string> screenResolutions) => Platform.api_name switch
    {
        SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption),
        SauceryConstants.BROWSER_EDGE => new EdgeBrowser(Platform, screenResolutions, platformNameForOption),
        _ => null,
    };
}