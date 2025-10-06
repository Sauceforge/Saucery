using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

//Ventura
public sealed class Mac13Platform(SupportedPlatform sp) 
    : PlatformBase(sp, 
                   "macOS 13", 
                   [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI],
                   [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE],
                   [SauceryConstants.SCREENRES_1024_768,
                    SauceryConstants.SCREENRES_1152_864,
                    SauceryConstants.SCREENRES_1280_960,
                    SauceryConstants.SCREENRES_1376_1032,
                    SauceryConstants.SCREENRES_1440_900,
                    SauceryConstants.SCREENRES_1600_1200,
                    SauceryConstants.SCREENRES_1920_1440,
                    SauceryConstants.SCREENRES_2048_1536 ])
{
}
