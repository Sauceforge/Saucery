using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.Google;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.Google;

internal class AndroidBrowserCreator(SupportedPlatform sp) : BrowserCreator(sp)
{
    public override BrowserBase? Create(string platformNameForOption, List<string> screenResolutions) => 
        new AndroidBrowser(Platform, platformNameForOption);
}