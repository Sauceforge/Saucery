using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators
{
    internal class Windows8BrowserCreator
    {
        private readonly SupportedPlatform Platform;

        public Windows8BrowserCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal BrowserBase Create()
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform),
                "firefox" => new FirefoxBrowser(Platform),
                "internet explorer" => new IEBrowser(Platform),
                _ => null,
            };
        }
    }
}