using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    public class IOS111Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public IOS111Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
            PlatformNameForOption = "iOS";
        }
    }
}