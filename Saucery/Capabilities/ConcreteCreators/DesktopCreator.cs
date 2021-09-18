using Saucery3.Capabilities.Base;
using Saucery3.Capabilities.ConcreteProducts;
using Saucery3.OnDemand;

namespace Saucery3.Capabilities.ConcreteCreators {
    internal class DesktopCreator : Creator {
        public override BaseCapabilities Create(SaucePlatform platform, string testName) {
            return new DesktopCapabilities(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 18th September 2014
 * 
 */