using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class PlatformConfigurator
    {
        private List<AvailablePlatform> AvailablePlatforms;

        public PlatformConfigurator(List<SupportedPlatform> platforms)
        {
            AvailablePlatforms = new List<AvailablePlatform>();

            foreach(var sp in platforms)
            {
                AvailablePlatforms.AddPlatform(sp);
            }
        }
    }
}
