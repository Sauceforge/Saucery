using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

public abstract class BrowserCreator
{
    internal readonly SupportedPlatform Platform;

    protected BrowserCreator(SupportedPlatform sp)
    {
        Platform = sp;
    }

    public abstract BrowserBase? Create(string platformNameForOption, List<string> screenResolutions);
}
