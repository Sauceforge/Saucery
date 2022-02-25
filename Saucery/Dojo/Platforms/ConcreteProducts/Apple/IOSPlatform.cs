using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Apple
{
    internal class IOSPlatform : PlatformBase
    {
        public IOSPlatform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
        }
    }
}