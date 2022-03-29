using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC 
{
    internal class Windows81BrowserCreator : BrowserCreator
    {
        public Windows81BrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption, List<string> screenResolutions)
        {
            return Platform.api_name switch
            {
                SauceryConstants.BROWSER_CHROME => new ChromeBrowser(Platform, screenResolutions, platformNameForOption),
                SauceryConstants.BROWSER_FIREFOX => new FirefoxBrowser(Platform, screenResolutions, platformNameForOption),
                SauceryConstants.BROWSER_IE => new IEBrowser(Platform, screenResolutions, platformNameForOption),
                _ => null,
            };
        }
    }
}