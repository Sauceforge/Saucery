package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;

class Sanitiser {
    static String SanitisePlatformField(String field) {
        return field.equals(SauceryConstants.NULL_STRING) ? null : field;
    }
    
    static String SanitiseTestName(String testName, SaucePlatform platform) {
        StringBuilder shortTestName = new StringBuilder();
        shortTestName.append(testName.substring(0, testName.indexOf(SauceryConstants.LEFT_SQUARE_BRACKET)));
        return platform.IsADesktopPlatform()
            ? DesktopTestName(shortTestName, platform)
            :  MobileTestName(shortTestName, platform);
    }

    private static String DesktopTestName(StringBuilder shortTestName, SaucePlatform platform){
        return AppendPlatformField(
                    AppendPlatformField(
                            AppendPlatformField(shortTestName, 
                                                platform.Os), 
                            platform.Browser), 
                    platform.BrowserVersion).toString();
    }
        
    private static String MobileTestName(StringBuilder shortTestName, SaucePlatform platform){
        return AppendPlatformField(
                    AppendPlatformField(
                            AppendPlatformField(shortTestName, platform.LongName),platform.BrowserVersion), 
                    platform.DeviceOrientation).toString();
    }

    private static StringBuilder AppendPlatformField(StringBuilder testName, String fieldToAdd) {
        return testName.append(SauceryConstants.UNDERSCORE).append(fieldToAdd);
    }
    
    static String SanitiseBrowserVersion(String version) {
        return version.endsWith(".")
                ? version.substring(0, version.length() - 1)
                : version;
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 15th November 2013 
 */