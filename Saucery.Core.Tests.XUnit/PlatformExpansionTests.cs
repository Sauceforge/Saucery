using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Tests.XUnit.Fixtures;
using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class PlatformExpansionTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Fact]
    public void NumericNonNumericRangeTest()
    {
        List<SaucePlatform> platforms =
        [
            new DesktopPlatform(
                SauceryConstants.PLATFORM_WINDOWS_11,
                SauceryConstants.BROWSER_CHROME,
                $"75->{SauceryConstants.BROWSER_VERSION_DEV}",
                SauceryConstants.SCREENRES_2560_1600)
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet
            .Find(e => e.BrowserVersion.Equals("82"))
            .ShouldBeNull(); //Chrome didn't release version 82 due to Covid-19.
        expandedSet.Count.ShouldBeGreaterThanOrEqualTo(31);
    }

    [Fact]
    public void NoExpansionTest()
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99")
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(platforms.Count);
    }

    [Fact]
    public void NumericOnlyRangeTest()
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75->102", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75->102", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "78->101"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "79->101")
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(103);
    }

    [Fact]
    public void NonNumericOnlyRangeTest()
    {
        List<SaucePlatform> platforms =
        [
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST_MINUS1}->{SauceryConstants.BROWSER_VERSION_LATEST_MINUS1}", SauceryConstants.SCREENRES_2560_1600), //1
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST}->{SauceryConstants.BROWSER_VERSION_LATEST}", SauceryConstants.SCREENRES_2560_1600),               //1
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_BETA}->{SauceryConstants.BROWSER_VERSION_BETA}", SauceryConstants.SCREENRES_2560_1600),                   //1
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_DEV}->{SauceryConstants.BROWSER_VERSION_DEV}", SauceryConstants.SCREENRES_2560_1600),                     //1
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST_MINUS1}->{SauceryConstants.BROWSER_VERSION_DEV}", SauceryConstants.SCREENRES_2560_1600),           //4
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST_MINUS1}->{SauceryConstants.BROWSER_VERSION_BETA}", SauceryConstants.SCREENRES_2560_1600),          //3
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST_MINUS1}->{SauceryConstants.BROWSER_VERSION_LATEST}", SauceryConstants.SCREENRES_2560_1600),        //2
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST}->{SauceryConstants.BROWSER_VERSION_DEV}", SauceryConstants.SCREENRES_2560_1600),                  //3
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_LATEST}->{SauceryConstants.BROWSER_VERSION_BETA}", SauceryConstants.SCREENRES_2560_1600),                 //2
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"{SauceryConstants.BROWSER_VERSION_BETA}->{SauceryConstants.BROWSER_VERSION_DEV}", SauceryConstants.SCREENRES_2560_1600)                     //2
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(20);
    }

    [Fact]
    public void BadRangeTest()
    {
        List<SaucePlatform> platforms =
        [
            //Desktop Platforms
            new DesktopPlatform(
                SauceryConstants.PLATFORM_WINDOWS_11,
                SauceryConstants.BROWSER_CHROME,
                "75->100->latest",
                SauceryConstants.SCREENRES_2560_1600),
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(0);
    }

    [Fact]
    public void MobileExpansionTest()
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms - Not supported.  This should never be done
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.2->15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
        ];

        PlatformExpander expander = new(_fixture.PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();

        //It is not the job of the PlatformExpander to remove this from the requested set so just return it.
        //PlatformConfigurator Filtering will ensure it is not validated.
        expandedSet.Count.ShouldBe(1);
    }
}
