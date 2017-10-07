using System;
using Saucery.Capabilities.Base;
using Saucery.Capabilities.ConcreteProducts;
using Saucery.TestDataSources;

namespace Saucery.Capabilities.ConcreteCreators {
    internal class AppiumIOSCreator : Creator {
        public override BaseCapabilities Create(PlatformTestData platform, String testName, string nativeApp = null, bool useChromeOnAndroid = false) {
            return new AppiumIOSCapabilities(platform, testName, nativeApp);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */