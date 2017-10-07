using System;

namespace Saucery.Util {
    internal class Sanitiser {
        public static string SanitisePlatformField(String field) {
            return field.Equals(SauceryConstants.NULL_STRING) ? null : field;
        }

        //public static String SanitiseBrowserVersion(String version){
        //    return version.TrimEnd('.');
        //}

        public static string RemoveSpaces(string expected, string actual) {
            return !actual.Contains(SauceryConstants.SPACE) && expected.Contains(SauceryConstants.SPACE)
                ? actual
                : expected;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th August 2014
 * 
 */