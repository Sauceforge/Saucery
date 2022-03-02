using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows81Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Windows81Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "internet explorer" };
            BrowsersWithLatestVersion = new List<string> { "chrome", "firefox" };
            PlatformNameForOption = "Windows 8.1";
        }
    }
}
