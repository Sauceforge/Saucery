using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;

public sealed class Android13Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Android13Platform(SupportedPlatform sp) : base(sp)
    {
        Selenium4BrowserNames = [];
        PlatformNameForOption = "Android";
    }
}