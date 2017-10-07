using SauceryX.Capabilities.Base;
using SauceryX.Capabilities.ConcreteProducts;
using SauceryX.OnDemand;

namespace SauceryX.Capabilities.ConcreteCreators {
    internal class AppiumIOSCreator : Creator {
        public override BaseCapabilities Create(SaucePlatform platform, string testName)
        {
            return new AppiumIOSCapabilities(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */