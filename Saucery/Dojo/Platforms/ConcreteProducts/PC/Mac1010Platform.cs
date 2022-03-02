using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //OS X Yosemite
    public class Mac1010Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac1010Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string> { "chrome", "MicrosoftEdge" };
            BrowsersWithLatestVersion = new List<string> { };
            PlatformNameForOption = "OS X 10.10";
        }
    }
}
