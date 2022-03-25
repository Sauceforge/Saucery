using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;
using Saucery.Util;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC
{
    internal class Mac1012BrowserCreator : BrowserCreator
    {
        public Mac1012BrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return Platform.api_name switch
            {
                SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, platformNameForOption),
                SauceryConstants.BROWSER_EDGE => new EdgeBrowser(Platform, platformNameForOption),
                SauceryConstants.BROWSER_FIREFOX => new FirefoxBrowser(Platform, platformNameForOption),
                _ => null,
            };
        }
    }
}