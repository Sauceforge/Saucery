using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Google
{
    internal class AndroidPlatform : PlatformBase
    {
        public AndroidPlatform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string>();
        }
    }
}