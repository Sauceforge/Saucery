﻿using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//Sonoma
public sealed class Mac14Platform : PlatformBase
{
    public override string PlatformNameForOption { get; set; }

    public Mac14Platform(SupportedPlatform sp) : base (sp)
    {
        Selenium4BrowserNames = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_SAFARI];
        BrowsersWithLatestVersion = [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_SAFARI];
        PlatformNameForOption = "macOS 14";
        ScreenResolutions = [ SauceryConstants.SCREENRES_1024_768,
                              SauceryConstants.SCREENRES_1152_864,
                              SauceryConstants.SCREENRES_1280_960,
                              SauceryConstants.SCREENRES_1376_1032,
                              SauceryConstants.SCREENRES_1440_900,
                              SauceryConstants.SCREENRES_1600_1200,
                              SauceryConstants.SCREENRES_1920_1440,
                              SauceryConstants.SCREENRES_2048_1536 ];
    }
}
