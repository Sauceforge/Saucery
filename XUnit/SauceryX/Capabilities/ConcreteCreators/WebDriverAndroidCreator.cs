using SauceryX.Capabilities.Base;
using SauceryX.Capabilities.ConcreteProducts;
using SauceryX.OnDemand;

namespace SauceryX.Capabilities.ConcreteCreators {
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