using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    internal class IOSBrowserCreator
    {
        private SupportedPlatform Platform;

        public IOSBrowserCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal BrowserBase Create()
        {
            return new IOSBrowser(Platform);
        }
    }
}