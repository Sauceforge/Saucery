namespace Saucery.Util {
    internal class Sanitiser {
        public static string SanitisePlatformField(string field) {
            return field.Equals(SauceryConstants.NULL_STRING) ? null : field;
        }

        public static string RemoveSpaces(string expected, string actual) {
            return !actual.Contains(SauceryConstants.SPACE) && expected.Contains(SauceryConstants.SPACE)
                ? actual
                : expected;
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th August 2014
 * 
 */