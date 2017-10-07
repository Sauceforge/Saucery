using System;
using Saucery3.DataSources;

namespace Saucery3.Util {
    public class DebugMessages {
        public static void PrintPlatformDetails(PlatformTestData platform) {
            if (UserChecker.ItIsMe())
            {
                Console.WriteLine("DEBUG MESSAGE START");
                Console.WriteLine(@"SELENIUM_OS=" + platform.Os);
                Console.WriteLine(@"SELENIUM_BROWSER=" + platform.BrowserName);
                Console.WriteLine(@"SELENIUM_VERSION=" + platform.BrowserVersion);
                Console.WriteLine(@"SELENIUM_LONG_VERSION=" + platform.LongVersion);
                Console.WriteLine(@"SELENIUM_DEVICE_TYPE=" + platform.DeviceType);
                Console.WriteLine(@"SELENIUM_DEVICE=" + platform.Device);
                Console.WriteLine("DEBUG MESSAGE END");
            }
        }

        public static void ExtractJsonSegment(string json, int startIndex, int endIndex) {
            if (UserChecker.ItIsMe())
            {
                Console.WriteLine("DEBUG MESSAGE: ExtractJsonSegment params {0} {1} {2}", json, startIndex, endIndex);
                Console.Out.Flush();
            }
        }
    }
}

/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 25th August 2014
 * 
 */