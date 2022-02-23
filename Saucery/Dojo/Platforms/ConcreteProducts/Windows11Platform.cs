using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows11Platform : PlatformBase
    {
        //public List<Windows11Browser> Browsers;

        public Windows11Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Windows11Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                   sp.api_name == "firefox" ||
                                   sp.api_name == "MicrosoftEdge");
        }
    }
}
