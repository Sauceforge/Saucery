using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.Google;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteCreators.Google;

internal class AndroidBrowserCreator : BrowserCreator
{
    public AndroidBrowserCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override BrowserBase Create(string platformNameForOption, List<string> screenResolutions) => new AndroidBrowser(Platform, platformNameForOption);
}