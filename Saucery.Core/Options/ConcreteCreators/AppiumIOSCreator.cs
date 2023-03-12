using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Options.ConcreteProducts;

namespace Saucery.Core.Options.ConcreteCreators;

internal class AppiumIOSCreator : Creator {
    public override BaseOptions Create(BrowserVersion browserVersion, string testName) => new AppiumIOSOptions(browserVersion, testName);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/