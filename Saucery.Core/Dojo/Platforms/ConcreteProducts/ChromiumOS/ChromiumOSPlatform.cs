using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.ChromiumOS;

public sealed class ChromiumOSPlatform(SupportedPlatform sp) 
    : PlatformBase(sp,
                   SauceryConstants.PLATFORM_CHROMIUM_OS,
                   [SauceryConstants.BROWSER_CHROME],
                   [SauceryConstants.BROWSER_CHROME],
                   [SauceryConstants.SCREENRES_1366_768,
                    SauceryConstants.SCREENRES_1600_900,
                    SauceryConstants.SCREENRES_1536_864,
                    SauceryConstants.SCREENRES_2880_1620,
                    SauceryConstants.SCREENRES_1280_720,
                    SauceryConstants.SCREENRES_1920_1080])
{
}
