using System;
using Saucery.TestDataSources;

namespace Saucery.Capabilities.Base {
    internal abstract class Creator {
        public abstract BaseCapabilities Create(PlatformTestData platform, String testName, string nativeApp = null, bool useChromeOnAndroid = false);
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */