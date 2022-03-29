using Newtonsoft.Json;
using Saucery.DataSources;
using Saucery.DataSources.Base;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Util;
using System.Collections;
using System.Collections.Generic;

//namespace Saucery.DataSources
namespace Merlin
{
    public class RequestedPlatformData : SauceryTestData
    {
        #region Attributes
        internal static List<SaucePlatform> Platforms { get; set; }
        internal static List<BrowserVersion> BrowserVersions { get; set; }
        internal static Compositor Compositor { get; set; }
        #endregion
        
        static RequestedPlatformData()
        //public PlatformTestData()
        {
            //Compositor = CompositorBuilder.Build();
            //Compositor.Compose();
            Platforms = new List<SaucePlatform>
            {
                //Desktop Platforms
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_2560_1600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "latest", SauceryConstants.SCREENRES_2560_1600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
                new SaucePlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),

                //Mobile Platforms
                new SaucePlatform("Linux", "Chrome", "89", "", "Android", "Google Pixel 6 Pro GoogleAPI Emulator", "12.0", "", "Android", "1.22.1", "portrait"),
                new SaucePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", "", SauceryConstants.PLATFORM_MAC_11, "iPhone 13 Pro Max Simulator", "15.0", "", "iphone", "1.22.0", "portrait")
            }.ClassifyAll();

            //Platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);
            //Platforms.ClassifyAll();
            BrowserVersions = new PlatformConfigurator().Filter(Platforms);
            //BrowserVersions.ClassifyAll();
        }

        //public IEnumerator GetEnumerator() {
        //    return BrowserVersions?.GetEnumerator();
        //}
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */