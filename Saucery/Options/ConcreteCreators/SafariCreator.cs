using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Options.ConcreteProducts;

namespace Saucery.Options.ConcreteCreators
{
    internal class SafariCreator : Creator {
        public override BaseOptions Create(BrowserVersion browserVersion, string testName) {
            return new SafariBrowserOptions(browserVersion, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */