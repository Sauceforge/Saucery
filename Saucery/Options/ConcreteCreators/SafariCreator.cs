using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Options.ConcreteProducts;

namespace Saucery.Options.ConcreteCreators
{
    internal class SafariCreator : Creator {
        public override BaseOptions Create(SaucePlatform platform, string testName) {
            return new SafariBrowserOptions(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */