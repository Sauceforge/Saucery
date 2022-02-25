using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Mojave
    public class Mac1014Platform : PlatformBase
    {
        //public List<Mac1014Browser> Browsers;

        public Mac1014Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "safari" };
        }
    }
}
