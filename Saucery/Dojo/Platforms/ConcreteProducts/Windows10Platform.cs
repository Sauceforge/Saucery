using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows10Platform : PlatformBase
    {
        //public List<Windows10Browser> Browsers;

        public Windows10Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Windows10Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                   sp.api_name == "firefox" ||
                                   sp.api_name == "MicrosoftEdge" ||
                                   sp.api_name == "internet explorer");
        }
    }
}
