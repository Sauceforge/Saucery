using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //Sierra
    public class Mac1012Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac1012Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge" };
            PlatformNameForOption = "macOS 10.12";
        }
    }
}
