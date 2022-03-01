using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.Apple
{
    internal class IOSBrowserCreator : BrowserCreator
    {
        public IOSBrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return new IOSBrowser(Platform, platformNameForOption);
        }
    }
}