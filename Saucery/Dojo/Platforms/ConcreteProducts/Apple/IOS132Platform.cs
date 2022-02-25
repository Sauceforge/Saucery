using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    internal class IOS132Platform : PlatformBase
    {
        public IOS132Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
        }
    }
}