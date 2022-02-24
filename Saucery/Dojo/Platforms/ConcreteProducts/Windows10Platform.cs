using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows10Platform : PlatformBase
    {
        public Windows10Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "internet explorer" };
        }

        //public override bool IsDesktopPlatform(SupportedPlatform sp)
        //{
        //    return sp.IsDesktop() && (sp.api_name == "chrome" ||
        //                           sp.api_name == "firefox" ||
        //                           sp.api_name == "MicrosoftEdge" ||
        //                           sp.api_name == "internet explorer");
        //}
    }
}
