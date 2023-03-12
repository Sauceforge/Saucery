using Saucery.Core.Dojo;

namespace Saucery.Core.Options.Base;

internal abstract class Creator {
    public abstract BaseOptions Create(BrowserVersion browserVersion, string testName);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/