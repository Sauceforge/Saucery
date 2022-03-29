namespace Saucery.Util
{
    internal class OnceOnlyMessages {
        internal static void RestApiLimitExceeded() {
            OnceOnlyWriter.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 24th December 2015
 * 
 */