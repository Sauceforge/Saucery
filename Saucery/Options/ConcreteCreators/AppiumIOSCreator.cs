﻿using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Options.ConcreteProducts;

namespace Saucery.Options.ConcreteCreators
{
    internal class AppiumIOSCreator : Creator {
        public override BaseOptions Create(SaucePlatform platform, string testName)
        {
            return new AppiumIOSOptions(platform, testName);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */