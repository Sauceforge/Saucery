﻿using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple
{
    public class IOS132Platform : PlatformBase
    {
        public override string PlatformNameForOption { get; set; }

        public IOS132Platform(SupportedPlatform sp) : base(sp)
        {
            Selenium4BrowserNames = new List<string>();
            PlatformNameForOption = SauceryConstants.PLATFORM_IOS;
        }
    }
}