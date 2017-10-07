using System;
using Saucery2.OnDemand;

namespace Saucery2.Util {
    public class DebugMessages {
        public static void PrintPlatformDetails(SaucePlatform platform) {
            Console.WriteLine(@"SELENIUM_OS=" + platform.Os);
            Console.WriteLine(@"SELENIUM_BROWSER=" + platform.Browser);
            Console.WriteLine(@"SELENIUM_VERSION=" + platform.BrowserVersion);
            Console.WriteLine(@"SELENIUM_LONG_VERSION=" + platform.LongVersion);
            //Console.WriteLine(@"SELENIUM_DEVICE_TYPE=" + platform.DeviceType);
            Console.WriteLine(@"SELENIUM_DEVICE=" + platform.Device);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 25th August 2014
 * 
 */