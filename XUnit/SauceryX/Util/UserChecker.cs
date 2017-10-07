namespace SauceryX.Util {
    internal static class UserChecker {
        internal static bool ItIsMe() {
            return Enviro.SauceUserName.ToLower().Equals(SauceryConstants.MY_USERNAME);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 29th July 2014
 * 
 */