using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows10Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Windows10Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "internet explorer" };
            PlatformNameForOption = "Windows 10";
        }
    }
}
