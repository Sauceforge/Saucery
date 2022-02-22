using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class PlatformConfigurator
    {
        public List<AvailablePlatform> AvailablePlatforms { get; set; }

        public PlatformConfigurator(List<SupportedPlatform> platforms)
        {
            AvailablePlatforms = new List<AvailablePlatform>();

            foreach (var sp in platforms)
            {
                //if (!sp.os.Equals("Linux"))
                //{
                    AvailablePlatforms.AddPlatform(sp);
                //}
            }
        }
    }
}
