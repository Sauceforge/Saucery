using Saucery.Dojo.Platforms;
using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class PlatformConfigurator
    {
        private PlatformFactory PlatformFactory;
        public List<PlatformBase> AvailablePlatforms { get; set; }

        public PlatformConfigurator(List<SupportedPlatform> platforms)
        {
            AvailablePlatforms = new List<PlatformBase>();

            foreach (var sp in platforms)
            {
                AvailablePlatforms.AddPlatform(sp);

                //if (!sp.os.Equals("Linux"))
                //{
                    //AvailablePlatforms.AddPlatform(sp);
                //}
            }
        }
    }
}
