package au.com.fullcirclesolutions.saucery.utils;

public class SauceryConstants {
    public static final String FORWARDSLASH = "/";
    public static final String EMPTY_STRING = "";

    //ACTIVATION DETAILS
    public static final String MY_USERNAME = "fullcicle";
    public static final String COMPANYNAME = "fullcirclesolutions";
    public static final String PRODUCTNAME = "Saucery";
    public static final String VERSION = "4.0.0.0";
    public static final String EDITION = "enterprise";
    public static final String KEYNAME = "saucerykey";
    public static final String SAUCERY_LICENCE_FILE = "\\Saucery4.lic"; //need the backslash here!
    public static final String PUBLIC_KEY = "BwIAAACkAABSU0EyAAQAAAEAAQDtZrs0gFK/FqIRSFYO8wqbwWUqk0Vul1ueiuOTdQrdkRy4bPT0Z8FTOWpAwlJXyxeJ+WgQmJlITEcb7VH0cICpJeWYPCGACUkqibDUGvNS/shLg4OoxPxP7u8kDnomqCSxhH5W0HXVrY73U3mq9tTlVVLvGqTbBG2/JMwnRSk70jURWrm1Py/vsMnluJU5kGUcbgFwF/ALCYBEttrmhavdgixswTfPT3LUg8IK44vKpVkSqegav+LpZlxwVvHLZvjZrfo8O3ZccC+XdFp+CdJdDbJ8KC8amqWz5hgeSciHzq2JRKppNhOslQQ/wDoxUJP7HDST4zgFf1UzGWHMdKnYAdoadFfCQ8kmHlmxEXWkVud9sZkG6fJbouTH2lCG0Ad5Z2URkgZJHhUTanbHqzzi8uS7AEM7DuIEwO99IpACEHlr8Il+Rbz7xIq3b/u8V7bhORoua1jmX5Wg6Du8zdpziWUxR+Yg53/A0BkWcgBci24Tun1PoDKuzK8yHPZiSEVYmX/bw6Htbzi+XyFNGF1TMfJaohhHi421MSzZMz+qKwSPQ+Ps0QJxPfvi/SDHUtlnkhmKEQ0gq5uu2cFIgjKUIb+S1NYJF3yW4oIiVBfrrBXx2wXkbRgDHlTPyrOXuFhzpjgMGjonEYMfHUSqFshObg6Go3r50jJQQDn7b230fhdtanz9M24raQRTlrC5Q6eEN0TqrwiTQoxpxa75mUHfd7YpHnButRC8YjrjdniHaxYSdk54TV3FPyN9niZfHzo=";
    
    //TEST URL
    public static final String SAUCELABS_HUB = "http://%s:%s@ondemand.saucelabs.com:80/wd/hub";

    //CAPABILITIES
    //public static final String RESOLUTION_CAPABILITY = "resolution";
    //public static final String SCREENRESOLUTION_CAPABILITY = "screen-resolution";
    public static final String SAUCE_APPIUM_VERSION_CAPABILITY = "appium-version";
    public static final String SAUCE_APPIUM_VERSION = "1.4.7";
    public static final double APPIUM_IOS_MINIMUM_VERSION = 6.1;
    public static final double APPIUM_ANDROID_MINIMUM_VERSION = 4.4;
    public static final String SAUCE_SESSIONNAME_CAPABILITY = "name";
    public static final String SAUCE_BUILDNAME_CAPABILITY = "build";
    public static final String SAUCE_USERNAME_CAPABILITY = "username";
    public static final String SAUCE_ACCESSKEY_CAPABILITY = "accessKey";
    public static final String SAUCE_BROWSER_NAME_CAPABILITY = "browserName";
    public static final String SAUCE_PLATFORM_CAPABILITY = "platform";
    public static final String SAUCE_PLATFORM_NAME_CAPABILITY = "platformName";
    public static final String SAUCE_PLATFORM_VERSION_CAPABILITY = "platformVersion";
    public static final String SAUCE_VERSION_CAPABILITY = "version";
    public static final String SAUCE_DEVICE_CAPABILITY = "device";
    public static final String SAUCE_DEVICE_NAME_CAPABILITY = "deviceName";
    public static final String SAUCE_DEVICE_ORIENTATION_CAPABILITY = "device-orientation";
    public static final String SAUCE_NATIVE_APP_CAPABILITY = "app";
    public static final String SAUCE_VUOP_CAPABILITY = "video-upload-on-pass";

    //SAUCE ENVIRONMENT VARIABLES
    public static final String SAUCE_USER_NAME = "SAUCE_USER_NAME";
    public static final String SAUCE_API_KEY = "SAUCE_API_KEY";
    public static final String SAUCE_ONDEMAND_BROWSERS = "SAUCE_ONDEMAND_BROWSERS";
    public static final String SAUCE_NATIVE_APP = "SAUCE_NATIVE_APP";
    public static final String SAUCE_USE_CHROME_ON_ANDROID = "SAUCE_USE_CHROME_ON_ANDROID";
    public static final String JENKINS_BUILD_NUMBER = "JENKINS_BUILD_NUMBER";
    public static final String BAMBOO_BUILD_NUMBER = "SAUCE_BAMBOO_BUILDNUMBER";

    //TUNNELLING
    public static final int TUNNEL_CONNECT_RETRY_COUNT = 3;

    //FLOW CONTROL
    public static final int SAUCELABS_FLOW_WAIT = 1000;

    //DEVICES
    public static final String LINUX = "Linux";
    public static final String APPLE_IOS = "Apple IOS";
    public static final String IOS_PLATFORM = "iOS";
    public static final String ANDROID_PLATFORM = "ANDROID";
    public static final String ANDROID_DEVICE = "android";
    public static final String GOOGLE = "Google";
    public static final String LG = "LG";
    public static final String SAMSUNG = "Samsung";
    public static final String APPLE_IPHONE = "iphone";
    public static final String APPLE_IPAD = "ipad";
    public static final String ANDROID = "android";
    public static final String SAFARI_BROWSER = "Safari";
    public static final String CHROME_BROWSER = "chrome";
    public static final String DEFAULT_ANDROID_BROWSER = "browser";
    public static final String IPAD_SIMULATOR = "iPad Simulator";
    public static final String IPHONE_SIMULATOR = "iPhone Simulator";
    public static final String IPAD_DEVICE = "iPad";
    public static final String IPHONE_DEVICE = "iPhone";

    //MISCELLANEOUS
    public static final String APPLE_TITLE = "Apple";
    public static final String PASSED = "passed";
    public static final String FAILED = "failed";
    public static final String NULL_STRING = "null";
    public static final String SPACE = " ";
    public static final String TWO_SPACES = "  ";
    public static final String LEFT_BRACKET = "(";
    public static final String LEFT_SQUARE_BRACKET = "[";
    public static final String COLON = ":";
    public static final String UNDERSCORE = "_";
    public static final String RESOLUTION = "1280x1024";
    public static final String YEAR_FORMAT = "yyyy";
    
    //NOTIFICATION MESSAGES
    public static final String CONSOLE_RUNNER = "%s Console Runner";
    public static final String COPYRIGHT_NOTICE = "Copyright (C) 2008-%s Andrew Gray";
    public static final String TESTING_ON = "Testing on %s %s.";
    public static final String NO_PLATFORMS = "Platforms is null";
    public static final String ON_DEMAND = "OnDemand=%s";
    public static final String LICENCED_VERSION = "You are using the licenced version of %s.";
    public static final String TRIAL_VERSION = "You are using the trial version of %s.";
    public static final String DAYS_REMAINING = "You have %s days remaining.";

    //SETTING UP
    public static final String SETTING_UP = "Setting up %s for %s";
    public static final String ANDROID_ON_APPIUM = "Android on Appium";
    public static final String IOS_ON_APPIUM = "IOS on Appium";
    public static final String ANDROID_ON_WEBDRIVER = "Android on WebDriver";
    public static final String IOS_ON_WEBDRIVER = "IOS on WebDriver";
    public static final String DESKTOP_ON_WEBDRIVER = "Desktop on WebDriver";

    //TEST VISIBILITY
    public static final String VISIBILITY_KEY = "public";
    public static final String VISIBILITY_PUBLIC = "public";
    public static final String VISIBILITY_PUBLIC_RESTRICTED = "public restricted";
    public static final String VISIBILITY_SHARE = "share";
    public static final String VISIBILITY_TEAM = "team";
    public static final String VISIBILITY_PRIVATE = "private";
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 15th November 2013
 */