using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //OS X El Capitan
    public class Mac1011Platform : PlatformBase
    {
        public Mac1011Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge" };
        }

        //public override bool IsDesktopPlatform(SupportedPlatform sp)
        //{
            
        //    return sp.IsDesktop() && (sp.api_name == "chrome" ||
        //                              sp.api_name == "firefox" ||
        //                              sp.api_name == "MicrosoftEdge");
        //}
    }
}
