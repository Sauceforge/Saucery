﻿using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.Google;

internal class Android16PlatformCreator(SupportedPlatform sp) : PlatformCreator(sp)
{
    public override PlatformBase Create() => new Android16Platform(Platform);
}