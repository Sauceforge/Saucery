﻿using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.PC
{
    //Monterey
    public class Mac12Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Mac12Platform(SupportedPlatform sp) : base (sp)
        {
            BrowserNames = new List<string> { "chrome", "firefox", "MicrosoftEdge", "safari" };
            PlatformNameForOption = "macOS 12";
        }
    }
}
