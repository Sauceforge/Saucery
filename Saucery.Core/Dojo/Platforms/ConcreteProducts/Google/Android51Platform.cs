using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google
{
    public class Android51Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Android51Platform(SupportedPlatform sp) : base(sp)
        {
            Selenium4BrowserNames = new List<string>();
            PlatformNameForOption = "Android";
        }
    }
}