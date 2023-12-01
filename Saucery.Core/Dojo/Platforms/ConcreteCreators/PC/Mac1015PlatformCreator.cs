﻿using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.PC;

internal class Mac1015PlatformCreator(SupportedPlatform sp) : PlatformCreator(sp)
{
    public override PlatformBase Create() => new Mac1015Platform(Platform);
}