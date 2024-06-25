﻿namespace Saucery.Core.Util;

internal static class UserChecker {
    internal static bool ItIsMe() => 
        Enviro.SauceUserName != null && 
        Enviro.SauceUserName.ToLower().Equals(SauceryConstants.MY_USERNAME_LOWER);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 29th July 2014
* 
*/