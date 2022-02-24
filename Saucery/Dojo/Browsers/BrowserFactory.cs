using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteCreators;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    public class BrowserFactory
    {
        private SupportedPlatform Platform { get; set; }

        public BrowserFactory(SupportedPlatform platform)
        {
            Platform = platform;
        }

        public BrowserBase CreateBrowser()
        {
            return Platform.IsAndroidPlatform()
                ? new AndroidBrowserCreator(Platform).Create()
                : Platform.IsIOSPlatform()
                ? new IOSBrowserCreator(Platform).Create()
                : Platform.os switch
                {
                    "Windows 2008" => new Windows7BrowserCreator(Platform).Create(),
                    "Windows 2012" => new Windows8BrowserCreator(Platform).Create(),
                    "Windows 2012 R2" => new Windows81BrowserCreator(Platform).Create(),
                    "Windows 10" => new Windows10BrowserCreator(Platform).Create(),
                    "Windows 11" => new Windows11BrowserCreator(Platform).Create(),
                    "Mac 11" => new Mac11BrowserCreator(Platform).Create(),
                    "Mac 12" => new Mac12BrowserCreator(Platform).Create(),
                    "Mac 10.10" => new Mac1010BrowserCreator(Platform).Create(),
                    "Mac 10.11" => new Mac1011BrowserCreator(Platform).Create(),
                    "Mac 10.12" => new Mac1012BrowserCreator(Platform).Create(),
                    "Mac 10.13" => new Mac1013BrowserCreator(Platform).Create(),
                    "Mac 10.14" => new Mac1014BrowserCreator(Platform).Create(),
                    "Mac 10.15" => new Mac1015BrowserCreator(Platform).Create(),
                    _ => null,
                };
        }
    }
}
