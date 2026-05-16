namespace Saucery.Core.Util;

public static class Enviro {
    internal static string? SauceNativeApp => 
        GetStringVar(SauceryConstants.SAUCE_NATIVE_APP);

    public static string? SauceUserName => 
        GetStringVar(SauceryConstants.SAUCE_USER_NAME);

    public static string? SauceApiKey => 
        GetStringVar(SauceryConstants.SAUCE_API_KEY);

    internal static string BuildName
    {
        get
        {
            var explicitBuildName = GetStringVar(SauceryConstants.SAUCERY_BUILD_NAME);

            if(!string.IsNullOrWhiteSpace(explicitBuildName)) {
                return explicitBuildName;
            }

            var framework = GetStringVar(SauceryConstants.SAUCERY_TEST_FRAMEWORK);
            var buildNumber = GetStringVar(SauceryConstants.BUILD_NUMBER) ?? IdGenerator.Id;

            return !string.IsNullOrWhiteSpace(framework) 
                ? $"Desktop_{framework}_{buildNumber}" 
                : $"Desktop_{buildNumber}";
        }
    }

    private static string? GetStringVar(string envVar) => 
        Environment.GetEnvironmentVariable(envVar);
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/