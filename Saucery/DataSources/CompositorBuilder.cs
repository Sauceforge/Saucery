using Saucery.DataSources.Base;
using Saucery.Util;

namespace Saucery.DataSources
{
    internal class CompositorBuilder
    {
        public static Compositor Build()
        {
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