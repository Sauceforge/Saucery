using Saucery3.Capabilities.Base;
using Saucery3.Capabilities.ConcreteProducts;
using Saucery3.OnDemand;

namespace Saucery3.Capabilities.ConcreteCreators {
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