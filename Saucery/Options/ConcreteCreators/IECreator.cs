﻿using Saucery.Dojo;
using Saucery.Options.Base;
using Saucery.Options.ConcreteProducts;

namespace Saucery.Options.ConcreteCreators;

internal class IECreator : Creator {
    public override BaseOptions Create(BrowserVersion browserVersion, string testName) {
        return new IEBrowserOptions(browserVersion, testName);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/