namespace Saucery.Core.Util; 

internal class Sanitiser {
    public static string? SanitisePlatformField(string field) =>
        field.Equals(SauceryConstants.NULL_STRING)
            ? null 
            : field;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th August 2014
* 
*/