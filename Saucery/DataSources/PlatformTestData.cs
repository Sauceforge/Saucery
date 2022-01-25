using Newtonsoft.Json;
using Saucery.DataSources.Base;
using Saucery.OnDemand;
using Saucery.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.DataSources
{

    public class PlatformTestData : IEnumerable {
        #region Attributes
        internal static List<SaucePlatform> Platforms { get; set; }
        internal static Compositor Compositor { get; set; }
        internal static CompositorBuilder Builder { get; set; }

        internal string Os { get; set; }
        internal string Platform { get; set; }
        internal string BrowserName { get; set; }
        internal string BrowserVersion { get; set; }
        internal string LongName { get; set; }
        internal string LongVersion { get; set; }
        internal string Url { get; set; }
        internal string Device { get; set; }
        internal string DeviceType { get; set; }
        internal string DeviceOrientation { get; set; }
        #endregion

        #region Methods
        internal double ParseBrowserVersion() {
            return double.Parse(BrowserVersion);
        }

        #endregion

        static PlatformTestData()
        {
            //Console.WriteLine("Start static PlatformTestData()");
            //Console.WriteLine(@"After CheckActivation in PlatformTestData");

            Builder = new CompositorBuilder();
            Compositor = Builder.Build();
            Compositor.Compose();
            Platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);

            //OnceOnlyMessages.TestingOn(Platforms);
            //OnceOnlyMessages.OnDemand();
        }

        public IEnumerator GetEnumerator() {
            return Platforms?.Select(platform => new SaucePlatform(Sanitiser.SanitisePlatformField(platform.Os),
                        Sanitiser.SanitisePlatformField(platform.Browser),
                        Sanitiser.SanitisePlatformField(platform.BrowserVersion),
                        Sanitiser.SanitisePlatformField(platform.Platform),
                        Sanitiser.SanitisePlatformField(platform.LongName),
                        Sanitiser.SanitisePlatformField(platform.LongVersion),
                        Sanitiser.SanitisePlatformField(platform.Url),
                        platform.Device ?? SauceryConstants.NULL_STRING,
                        platform.DeviceOrientation ?? SauceryConstants.NULL_STRING)).GetEnumerator();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */