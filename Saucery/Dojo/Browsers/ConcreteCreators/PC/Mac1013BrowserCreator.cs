﻿using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Browsers.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteCreators.PC
{
    internal class Mac1013BrowserCreator : BrowserCreator
    {
        public Mac1013BrowserCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserBase Create(string platformNameForOption)
        {
            return Platform.api_name switch
            {
                "chrome" => new ChromeBrowser(Platform, platformNameForOption),
                "MicrosoftEdge" => new EdgeBrowser(Platform, platformNameForOption),
                "firefox" => new FirefoxBrowser(Platform, platformNameForOption),
                "safari" => new SafariBrowser(Platform, platformNameForOption),
                _ => null,
            };
        }
    }
}