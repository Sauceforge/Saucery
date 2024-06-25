using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

public abstract class BrowserCreator(SupportedPlatform sp) {
    internal readonly SupportedPlatform Platform = sp;

    public abstract BrowserBase? Create(string platformNameForOption, List<string> screenResolutions);
}
