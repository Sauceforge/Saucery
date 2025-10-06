using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;

public sealed class LinuxPlatform(SupportedPlatform sp) 
    : PlatformBase(sp, 
                   "Linux",
                   [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX],
                   [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX],
                   [SauceryConstants.SCREENRES_800_600,
                    SauceryConstants.SCREENRES_1024_768,
                    SauceryConstants.SCREENRES_1152_864,
                    SauceryConstants.SCREENRES_1280_768,
                    SauceryConstants.SCREENRES_1280_800,
                    SauceryConstants.SCREENRES_1280_960,
                    SauceryConstants.SCREENRES_1280_1024,
                    SauceryConstants.SCREENRES_1400_1050,
                    SauceryConstants.SCREENRES_1440_900,
                    SauceryConstants.SCREENRES_1600_1200,
                    SauceryConstants.SCREENRES_1680_1050,
                    SauceryConstants.SCREENRES_1920_1080,
                    SauceryConstants.SCREENRES_1920_1200 ])
{
}
