using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows7Platform : PlatformBase
    {
        //public List<Windows7Browser> Browsers;

        public Windows7Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Windows7Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                   sp.api_name == "firefox" ||
                                   sp.api_name == "internet explorer");
        }
    }
}
