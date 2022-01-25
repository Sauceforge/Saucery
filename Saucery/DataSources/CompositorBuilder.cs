using Saucery.DataSources.Base;
using Saucery.Util;

namespace Saucery.DataSources
{
    internal class CompositorBuilder
    {
        public Compositor Build()
        {
            //return Enviro.SauceOnDemandBrowsers != null ? new EnvCompositor() : new BuiltInCompositor();
            if(Enviro.SauceOnDemandBrowsers != null) {
                DebugMessages.UsingEnvCompositor();
                return new EnvCompositor();
            } else {
                DebugMessages.UsingBuiltInCompositor();
                return new BuiltInCompositor();
            }
        }
    }
}