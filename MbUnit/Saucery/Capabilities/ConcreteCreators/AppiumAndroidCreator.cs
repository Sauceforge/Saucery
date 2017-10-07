using System;
using Saucery.Capabilities.Base;
using Saucery.Capabilities.ConcreteProducts;
using Saucery.TestDataSources;

namespace Saucery.Capabilities.ConcreteCreators {
    internal class AppiumAndroidCreator : Creator {
        public override BaseCapabilities Create(PlatformTestData platform, String testName, string nativeApp = null, bool useChromeOnAndroid = false) {
            return new AppiumAndroidCapabilities(platform, testName, useChromeOnAndroid, nativeApp);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */