using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //BigSur
    public class Mac11Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac11Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "safari" };
            PlatformNameForOption = "macOS 11.00";
        }
    }
}
