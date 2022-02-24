using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators
{
    internal class AndroidBrowserCreator : BrowserCreator
    {
        public AndroidBrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create()
        {
            return new AndroidBrowser(Platform);
        }
    }
}