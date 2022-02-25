﻿using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //High Sierra
    public class Mac1013Platform : PlatformBase
    {
        public Mac1013Platform(SupportedPlatform sp) : base(sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "safari" };
        }
    }
}
