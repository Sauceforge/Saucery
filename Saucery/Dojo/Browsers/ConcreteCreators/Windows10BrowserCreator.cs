﻿using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators
{
    internal class Windows10BrowserCreator
    {
        private readonly SupportedPlatform Platform;

        public Windows10BrowserCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal BrowserBase Create()
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform),
                "MicrosoftEdge" => new EdgeBrowser(Platform),
                "firefox" => new FirefoxBrowser(Platform),
                "internet explorer" => new IEBrowser(Platform),
                _ => null,
            };
        }
    }
}