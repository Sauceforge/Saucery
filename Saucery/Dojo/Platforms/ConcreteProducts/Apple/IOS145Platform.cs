using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    internal class IOS145Platform : PlatformBase
    {
        public IOS145Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
        }
    }
}