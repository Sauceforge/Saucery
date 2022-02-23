﻿using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteCreators;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    public class PlatformFactory
    {
        //private SupportedPlatform Sp { get; set; }

        //public PlatformFactory(SupportedPlatform platform)
        //{
        //    //Sp = platform;
        //}

        public PlatformBase CreatePlatform(SupportedPlatform sp)
        {
            return sp.IsAndroidPlatform()
                ? new AndroidPlatformCreator(sp).Create()
                : sp.IsIOSPlatform()
                ? new IOSPlatformCreator(sp).Create()
                : sp.os switch
            {
                "Windows 2008" => new Windows7PlatformCreator(sp).Create(),
                "Windows 2012" => new Windows8PlatformCreator(sp).Create(),
                "Windows 2012 R2" => new Windows81PlatformCreator(sp).Create(),
                "Windows 10" => new Windows10PlatformCreator(sp).Create(),
                "Windows 11" => new Windows11PlatformCreator(sp).Create(),
                "Mac 11" => new Mac11PlatformCreator(sp).Create(),
                "Mac 12" => new Mac12PlatformCreator(sp).Create(),
                "Mac 10.10" => new Mac1010PlatformCreator(sp).Create(),
                "Mac 10.11" => new Mac1011PlatformCreator(sp).Create(),
                "Mac 10.12" => new Mac1012PlatformCreator(sp).Create(),
                "Mac 10.13" => new Mac1013PlatformCreator(sp).Create(),
                "Mac 10.14" => new Mac1014PlatformCreator(sp).Create(),
                "Mac 10.15" => new Mac1015PlatformCreator(sp).Create(),
                //"Linux" => new AndroidPlatformCreator(sp).Create(),
                _ => null
            };
        }
    }
}
