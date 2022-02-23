using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    internal class Mac11BrowserCreator
    {
        private readonly SupportedPlatform Platform;

        public Mac11BrowserCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal BrowserBase Create()
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform),
                "MicrosoftEdge" => new EdgeBrowser(Platform),
                "firefox" => new FirefoxBrowser(Platform),
                "safari" => new SafariBrowser(Platform),
                _ => null,
            };
        }
    }
}