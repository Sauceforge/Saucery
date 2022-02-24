using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    internal class IOSBrowserCreator : BrowserCreator
    {
        public IOSBrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create()
        {
            return new IOSBrowser(Platform);
        }
    }
}