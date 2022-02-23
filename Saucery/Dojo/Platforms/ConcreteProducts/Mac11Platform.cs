using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //BigSur
    public class Mac11Platform : PlatformBase
    {
        //public List<Mac11Browser> Browsers;

        public Mac11Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Mac11Browser>();
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
