using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC
{
    internal class Mac1011BrowserCreator : BrowserCreator
    {
        public Mac1011BrowserCreator(SupportedPlatform sp): base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform, platformNameForOption),
                "MicrosoftEdge" => new EdgeBrowser(Platform, platformNameForOption),
                "firefox" => new FirefoxBrowser(Platform, platformNameForOption),
                _ => null,
            };
        }
    }
}