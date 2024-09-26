using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.Dojo.Browsers.ConcreteProducts.Apple;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.Apple;

internal class IOSBrowserCreator(SupportedPlatform sp) : BrowserCreator(sp)
{
    public override BrowserBase? Create(string platformNameForOption, List<string> screenResolutions) => 
        new IOSBrowser(Platform, screenResolutions, platformNameForOption);
}