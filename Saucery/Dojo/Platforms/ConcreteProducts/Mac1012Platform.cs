using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Sierra
    public class Mac1012Platform : PlatformBase
    {
        //public List<Mac1012Browser> Browsers;

        public Mac1012Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge" };
        }
    }
}
