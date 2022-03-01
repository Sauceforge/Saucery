using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.Google
{
    internal class AndroidBrowserCreator : BrowserCreator
    {
        public AndroidBrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return new AndroidBrowser(Platform, platformNameForOption);
        }
    }
}