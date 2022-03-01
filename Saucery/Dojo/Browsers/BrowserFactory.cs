using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteCreators.Apple;
using Saucery.Dojo.Browsers.ConcreteCreators.Google;
using Saucery.Dojo.Browsers.ConcreteCreators.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    public class BrowserFactory
    {
        public static BrowserBase CreateBrowser(SupportedPlatform sp)
        {
            return sp.IsAndroidPlatform()
                ? new AndroidBrowserCreator(sp).Create("Android")
                : sp.IsIOSPlatform()
                ? new IOSBrowserCreator(sp).Create("iOS")
                : sp.os switch
                {
                    "Windows 2008" => new Windows7BrowserCreator(sp).Create("Windows 7"),
                    "Windows 2012" => new Windows8BrowserCreator(sp).Create("Windows 8"),
                    "Windows 2012 R2" => new Windows81BrowserCreator(sp).Create("Windows 8.1"),
                    "Windows 10" => new Windows10BrowserCreator(sp).Create("Windows 10"),
                    "Windows 11" => new Windows11BrowserCreator(sp).Create("Windows 11"),
                    "Mac 11" => new Mac11BrowserCreator(sp).Create("macOS 11.00"),
                    "Mac 12" => new Mac12BrowserCreator(sp).Create("macOS 12"),
                    "Mac 10.10" => new Mac1010BrowserCreator(sp).Create("OS X 10.10"),
                    "Mac 10.11" => new Mac1011BrowserCreator(sp).Create("OS X 10.11"),
                    "Mac 10.12" => new Mac1012BrowserCreator(sp).Create("macOS 10.12"),
                    "Mac 10.13" => new Mac1013BrowserCreator(sp).Create("macOS 10.13"),
                    "Mac 10.14" => new Mac1014BrowserCreator(sp).Create("macOS 10.14"),
                    "Mac 10.15" => new Mac1015BrowserCreator(sp).Create("macOS 10.15"),
                    _ => null,
                };
        }
    }
}
