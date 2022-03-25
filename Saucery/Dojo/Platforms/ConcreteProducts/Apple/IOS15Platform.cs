using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    public class IOS15Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public IOS15Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
            PlatformNameForOption = SauceryConstants.PLATFORM_IOS;
        }
    }
}