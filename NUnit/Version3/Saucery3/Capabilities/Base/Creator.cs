using Saucery3.OnDemand;

namespace Saucery3.Capabilities.Base {
    internal abstract class Creator {
        public abstract BaseCapabilities Create(SaucePlatform platform, string testName);
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */