﻿using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Platforms.ConcreteProducts.Google
{
    public class Android71Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public Android71Platform(SupportedPlatform sp) : base(sp)
        {
            Selenium4BrowserNames = new List<string>();
            PlatformNameForOption = "Android";
        }
    }
}