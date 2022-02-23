using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators
{
    internal class AndroidBrowserCreator
    {
        private SupportedPlatform Platform;

        public AndroidBrowserCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal BrowserBase Create()
        {
            return new AndroidBrowser(Platform);
        }
    }
}