using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Google
{
    public class Android11Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Android11Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
            PlatformNameForOption = "Android";
        }
    }
}