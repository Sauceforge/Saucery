using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows11Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Windows11Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge" };
            BrowsersWithLatestVersion = new List<string> { };
            PlatformNameForOption = "Windows 11";
        }
    }
}
