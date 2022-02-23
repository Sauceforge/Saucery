﻿using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    public class Windows8Platform : PlatformBase
    {
        //public List<Windows8Browser> Browsers;

        public Windows8Platform(SupportedPlatform sp) : base (sp)
        {
            //Browsers = new List<Windows8Browser>();
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return sp.IsDesktop() && (sp.api_name == "chrome" ||
                                   sp.api_name == "firefox" ||
                                   sp.api_name == "internet explorer");
        }
    }
}
