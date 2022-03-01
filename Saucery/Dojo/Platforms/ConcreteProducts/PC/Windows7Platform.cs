using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    public class Windows7Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }
        
        public Windows7Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "internet explorer" };
            PlatformNameForOption = "Windows 7";
        }
    }
}
