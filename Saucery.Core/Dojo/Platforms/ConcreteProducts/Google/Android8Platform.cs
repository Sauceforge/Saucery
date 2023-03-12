using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google
{
    public class Android8Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Android8Platform(SupportedPlatform sp) : base(sp)
        {
            Selenium4BrowserNames = new List<string>();
            PlatformNameForOption = "Android";
        }
    }
}