using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows81Platform : PlatformBase
    {
        //public List<Windows81Browser> Browsers;

        public Windows81Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Windows81Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                   sp.api_name == "firefox" ||
                                   sp.api_name == "internet explorer");
        }
    }
}
