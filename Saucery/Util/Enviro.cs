﻿using System;

namespace Saucery.Util; 

public static class Enviro {
    internal static string SauceNativeApp => GetStringVar(SauceryConstants.SAUCE_NATIVE_APP);

    public static string SauceUserName => GetStringVar(SauceryConstants.SAUCE_USER_NAME);

    public static string SauceApiKey => GetStringVar(SauceryConstants.SAUCE_API_KEY);

    internal static string BuildName => string.Format("Desktop_{0}", GetStringVar(SauceryConstants.BUILD_NUMBER) ?? IDGenerator.Id);

    private static string GetStringVar(string envVar)
    {
        return envVar == null ? null : Environment.GetEnvironmentVariable(envVar);
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/