using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Monterey
    public class Mac12Platform : PlatformBase
    {
        //public List<Mac12Browser> Browsers;

        public Mac12Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Mac12Browser>();
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
