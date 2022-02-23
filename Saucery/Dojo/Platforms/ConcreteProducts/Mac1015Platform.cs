using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    //Catalina
    public class Mac1015Platform : PlatformBase
    {
        //public List<Mac1015Browser> Browsers;

        public Mac1015Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Mac1015Browser>();
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
