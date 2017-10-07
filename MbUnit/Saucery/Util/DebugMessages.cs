using System;
using Saucery.TestDataSources;

namespace Saucery.Util {
    public class DebugMessages {
        public static void PrintPlatformDetails(PlatformTestData platform) {
            Console.WriteLine("SELENIUM_OS=" + platform.Os);
            Console.WriteLine("SELENIUM_BROWSER=" + platform.BrowserName);
            Console.WriteLine("SELENIUM_VERSION=" + platform.BrowserVersion);
            Console.WriteLine("SELENIUM_LONG_NAME=" + platform.LongName);
            Console.WriteLine("SELENIUM_LONG_VERSION=" + platform.LongVersion);
            Console.WriteLine("SELENIUM_DEVICE_TYPE=" + platform.DeviceType);
            Console.WriteLine("SELENIUM_DEVICE=" + platform.Device);
            Console.WriteLine("SELENIUM_DEVICE_ORIENTATION=" + platform.DeviceOrientation);
        }        
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 25th August 2014
 * 
 */