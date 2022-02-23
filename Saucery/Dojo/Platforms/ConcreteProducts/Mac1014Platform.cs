using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Mojave
    public class Mac1014Platform : PlatformBase
    {
        //public List<Mac1014Browser> Browsers;

        public Mac1014Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Mac1014Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                      sp.api_name == "firefox" ||
                                      sp.api_name == "MicrosoftEdge" ||
                                      sp.api_name == "safari");
        }
    }
}
