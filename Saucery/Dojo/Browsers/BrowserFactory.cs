using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteCreators.Apple;
using Saucery.Dojo.Browsers.ConcreteCreators.Google;
using Saucery.Dojo.Browsers.ConcreteCreators.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    public class BrowserFactory
    {
        public BrowserFactory()
        {
        }

        public static BrowserBase CreateBrowser(SupportedPlatform sp)
        {
            return sp.IsAndroidPlatform()
                ? new AndroidBrowserCreator(sp).Create()
                : sp.IsIOSPlatform()
                ? new IOSBrowserCreator(sp).Create()
                : sp.os switch
                {
                    "Windows 2008" => new Windows7BrowserCreator(sp).Create(),
                    "Windows 2012" => new Windows8BrowserCreator(sp).Create(),
                    "Windows 2012 R2" => new Windows81BrowserCreator(sp).Create(),
                    "Windows 10" => new Windows10BrowserCreator(sp).Create(),
                    "Windows 11" => new Windows11BrowserCreator(sp).Create(),
                    "Mac 11" => new Mac11BrowserCreator(sp).Create(),
                    "Mac 12" => new Mac12BrowserCreator(sp).Create(),
                    "Mac 10.10" => new Mac1010BrowserCreator(sp).Create(),
                    "Mac 10.11" => new Mac1011BrowserCreator(sp).Create(),
                    "Mac 10.12" => new Mac1012BrowserCreator(sp).Create(),
                    "Mac 10.13" => new Mac1013BrowserCreator(sp).Create(),
                    "Mac 10.14" => new Mac1014BrowserCreator(sp).Create(),
                    "Mac 10.15" => new Mac1015BrowserCreator(sp).Create(),
                    _ => null,
                };
        }
    }
}
