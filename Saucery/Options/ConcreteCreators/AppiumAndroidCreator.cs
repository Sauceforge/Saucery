using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Options.ConcreteProducts;

namespace Saucery.Options.ConcreteCreators
{
    internal class AppiumAndroidCreator : Creator {
        public override BaseOptions Create(SaucePlatform platform, string testName)
        {
            return new AppiumAndroidOptions(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */