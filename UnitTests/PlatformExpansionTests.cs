using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.Util;
using Shouldly;
using System.Collections.Generic;

namespace UnitTests;

[TestFixture]
public class PlatformExpansionTests
{
    static PlatformConfigurator PlatformConfigurator { get; set; }

    static PlatformExpansionTests()
    {
        PlatformConfigurator = new(PlatformFilter.ALL);
    }

    [Test]
    public void NumericNonNumericRangeTest()
    {
        var platforms = new List<SaucePlatform>
        {
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, $"75->{SauceryConstants.BROWSER_VERSION_DEV}", SauceryConstants.SCREENRES_2560_1600)
        };

        //PlatformConfigurator configurator = new();
        PlatformExpander expander = new(PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Find(e => e.BrowserVersion.Equals(82)).ShouldBeNull(); //Chrome didn't release version 82 due to Covid-19.
        expandedSet.Count.ShouldBeGreaterThanOrEqualTo(31);
    }

    [Test]
    public void NoExpansionTest()
    {
        var platforms = new List<SaucePlatform>
        {
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99")
        };

        PlatformExpander expander = new(PlatformConfigurator, platforms);
        var expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(platforms.Count);
    }

    [Test]
    public void NumericOnlyRangeTest()
    {
        var platforms = new List<SaucePlatform>
        {
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75->102", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75->102", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "78->101"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78->101"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "79->101")
        };

        PlatformExpander expander = new(PlatformConfigurator, platforms);
        List<SaucePlatform> expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(128);
    }

    [Test]
    public void NonNumericOnlyRangeTest()
    {
        var platforms = new List<SaucePlatform>
        {
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
        };

        PlatformExpander expander = new(PlatformConfigurator, platforms);
        List<SaucePlatform> expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(20);
    }

    [Test]
    public void BadRangeTest()
    {
        var platforms = new List<SaucePlatform>
        {
            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75->100->latest", SauceryConstants.SCREENRES_2560_1600),
        };

        PlatformExpander expander = new(PlatformConfigurator, platforms);
        List<SaucePlatform> expandedSet = expander.Expand();
        expandedSet.Count.ShouldBe(0);
    }

    [Test]
    public void MobileExpansionTest()
    {
        var platforms = new List<SaucePlatform>
        {
            //Mobile Platforms - Not supported.  This should never be done
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.2->15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),
        };

        PlatformExpander expander = new(PlatformConfigurator, platforms);
        List<SaucePlatform> expandedSet = expander.Expand();
        
        //It is not the job of the PlatformExpander to remove this from the requested set so just return it.
        //PlatformConfigurator Filtering will ensure it is not validated.
        expandedSet.Count.ShouldBe(1);
    }
}
