using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Catalina
    public class Mac1015Platform : PlatformBase
    {
        //public List<Mac1015Browser> Browsers;

        public Mac1015Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "safari" };
        }

        //public override bool IsDesktopPlatform(SupportedPlatform sp)
        //{
        //    return sp.IsDesktop() && (sp.api_name == "chrome" ||
        //                              sp.api_name == "firefox" ||
        //                              sp.api_name == "MicrosoftEdge" ||
        //                              sp.api_name == "safari");
        //}
    }
}
