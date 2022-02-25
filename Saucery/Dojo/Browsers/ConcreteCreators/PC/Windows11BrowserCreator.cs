using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC
{
    internal class Windows11BrowserCreator : BrowserCreator
    {
        public Windows11BrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create()
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform),
                "MicrosoftEdge" => new EdgeBrowser(Platform),
                "firefox" => new FirefoxBrowser(Platform),
                _ => null,
            };
        }
    }
}