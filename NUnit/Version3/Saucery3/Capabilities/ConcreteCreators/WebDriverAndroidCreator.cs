using Saucery3.Capabilities.Base;
using Saucery3.Capabilities.ConcreteProducts;
using Saucery3.OnDemand;

namespace Saucery3.Capabilities.ConcreteCreators {
    internal class WebDriverAndroidCreator : Creator {
        public override BaseCapabilities Create(SaucePlatform platform, string testName)
        {
            return new WebDriverAndroidCapabilities(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */