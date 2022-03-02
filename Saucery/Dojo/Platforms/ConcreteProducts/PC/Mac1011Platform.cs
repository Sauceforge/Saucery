using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //OS X El Capitan
    public class Mac1011Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac1011Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge" };
            BrowsersWithLatestVersion = new List<string> { "chrome" };
            PlatformNameForOption = "OS X 10.11";
        }
    }
}
