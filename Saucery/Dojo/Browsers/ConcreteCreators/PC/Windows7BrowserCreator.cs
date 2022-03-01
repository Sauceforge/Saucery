using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC
{
    internal class Windows7BrowserCreator : BrowserCreator
    {
        public Windows7BrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform, platformNameForOption),
                "firefox" => new FirefoxBrowser(Platform, platformNameForOption),
                "internet explorer" => new IEBrowser(Platform, platformNameForOption),
                _ => null,
            };
        }
    }
}