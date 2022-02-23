using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Sierra
    public class Mac1012Platform : PlatformBase
    {
        //public List<Mac1012Browser> Browsers;

        public Mac1012Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Mac1012Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                      sp.api_name == "firefox" ||
                                      sp.api_name == "MicrosoftEdge");
        }
    }
}
