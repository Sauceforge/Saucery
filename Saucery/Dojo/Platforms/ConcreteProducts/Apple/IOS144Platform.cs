using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    internal class IOS144Platform : PlatformBase
    {
        public IOS144Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
        }
    }
}