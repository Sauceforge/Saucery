using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.Base;

public abstract class BrowserCreator
{
    internal readonly SupportedPlatform Platform;

    public BrowserCreator(SupportedPlatform sp)
    {
        Platform = sp;
    }

    public abstract BrowserBase Create(string platformNameForOption, List<string> screenResolutions);
}
