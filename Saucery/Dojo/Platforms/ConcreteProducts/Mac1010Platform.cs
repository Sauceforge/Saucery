using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //OS X Yosemite
    public class Mac1010Platform : PlatformBase
    {
        public Mac1010Platform(SupportedPlatform sp) : base(sp)
        {
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                      sp.api_name == "MicrosoftEdge");
        }
    }
}
