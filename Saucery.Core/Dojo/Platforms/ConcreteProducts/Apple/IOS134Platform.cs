﻿using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;

public class IOS134Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public IOS134Platform(SupportedPlatform sp) : base(sp)
    {
        Selenium4BrowserNames = [];
        PlatformNameForOption = SauceryConstants.PLATFORM_IOS;
    }
}