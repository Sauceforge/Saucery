package au.com.fullcirclesolutions.saucery.utils;

public class Enviro {
    public static String SauceNativeApp() {
        return getStringVar(SauceryConstants.SAUCE_NATIVE_APP);
    }

    public static String SauceUserName() {
        return getStringVar(SauceryConstants.SAUCE_USER_NAME);
    }

    public static String SauceApiKey() {
        return getStringVar(SauceryConstants.SAUCE_API_KEY);
    }

    public static String SauceOnDemandBrowsers() {
        return getStringVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS);
    }

    public static boolean SauceUseChromeOnAndroid() {
        return getBoolVar(SauceryConstants.SAUCE_USE_CHROME_ON_ANDROID);
    }
    
    public static String BuildNumber(){
        String jenkins = JenkinsBuildNumber();
        return !IsNullOrEmpty(jenkins) ? jenkins : BambooBuildNumber();
    }

    private static String JenkinsBuildNumber() {
        return getStringVar(SauceryConstants.JENKINS_BUILD_NUMBER);
    }
    
    private static String BambooBuildNumber() {
        return getStringVar(SauceryConstants.BAMBOO_BUILD_NUMBER);
    }

    private static String getStringVar(String envVar) {
        return IsNullOrEmpty(envVar) ? null : System.getenv(envVar);
    }

    private static boolean getBoolVar(String envVar) {
        String v = getStringVar(envVar);
        return v == null ? false : Boolean.getBoolean(v);
    }

    private static int getIntVar(String envVar) {
        String v = getStringVar(envVar);
        return  v == null ? 0 : Integer.parseInt(v);
    }

    private static double getDoubleVar(String envVar) {
        String v = getStringVar(envVar);
        return v == null ? 0 : Double.parseDouble(v);
    }
    
    private static boolean IsNullOrEmpty(String str){
        return str == null || str.trim().isEmpty();
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 */