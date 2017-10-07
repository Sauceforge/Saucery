using System;

namespace Saucery.Util {
    public class SauceryConstants {
        //ACTIVATION DETAILS
        internal const string MY_USERNAME = "jenkinsvacc";
        internal const string COMPANYNAME = "Full Circle Solutions";
        internal const string EDITION = "Enterprise";
        internal const string KEYNAME = "SauceryKey";
        internal const string REGISTRYROOT = @"SOFTWARE\" + COMPANYNAME + BACKSLASH;
        internal const string LICENCE_DATE_FORMAT = "ddMMyyyy";
        internal const string PUBLIC_KEY = "BwIAAACkAABSU0EyAAQAAAEAAQDtZrs0gFK/FqIRSFYO8wqbwWUqk0Vul1ueiuOTdQrdkRy4bPT0Z8FTOWpAwlJXyxeJ+WgQmJlITEcb7VH0cICpJeWYPCGACUkqibDUGvNS/shLg4OoxPxP7u8kDnomqCSxhH5W0HXVrY73U3mq9tTlVVLvGqTbBG2/JMwnRSk70jURWrm1Py/vsMnluJU5kGUcbgFwF/ALCYBEttrmhavdgixswTfPT3LUg8IK44vKpVkSqegav+LpZlxwVvHLZvjZrfo8O3ZccC+XdFp+CdJdDbJ8KC8amqWz5hgeSciHzq2JRKppNhOslQQ/wDoxUJP7HDST4zgFf1UzGWHMdKnYAdoadFfCQ8kmHlmxEXWkVud9sZkG6fJbouTH2lCG0Ad5Z2URkgZJHhUTanbHqzzi8uS7AEM7DuIEwO99IpACEHlr8Il+Rbz7xIq3b/u8V7bhORoua1jmX5Wg6Du8zdpziWUxR+Yg53/A0BkWcgBci24Tun1PoDKuzK8yHPZiSEVYmX/bw6Htbzi+XyFNGF1TMfJaohhHi421MSzZMz+qKwSPQ+Ps0QJxPfvi/SDHUtlnkhmKEQ0gq5uu2cFIgjKUIb+S1NYJF3yW4oIiVBfrrBXx2wXkbRgDHlTPyrOXuFhzpjgMGjonEYMfHUSqFshObg6Go3r50jJQQDn7b230fhdtanz9M24raQRTlrC5Q6eEN0TqrwiTQoxpxa75mUHfd7YpHnButRC8YjrjdniHaxYSdk54TV3FPyN9niZfHzo=";
        internal const string SOLUTION_MASK = ASTERIX + ".sln";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string PROJECT_MASK = ASTERIX + PROJECT_EXTENSION;
        internal const string ASSEMBLYINFO_RELATIVE_PATH = @"\Properties\AssemblyInfo.cs";
        internal const string ASSEMBLY_FILE_GUID_LINE = "[assembly: Guid";
        internal const string SOLUTION_FILE_PROJECT_LINE = "Project(";

        //TEST URL
        internal const String SAUCELABS_HUB = "http://ondemand.saucelabs.com:80/wd/hub";

        //CAPABILITIES
        //internal const String RESOLUTION_CAPABILITY = "resolution";
        //internal const String SCREENRESOLUTION_CAPABILITY = "screen-resolution";
        internal const String SAUCE_APPIUM_VERSION_CAPABILITY = "appium-version";
        internal const String SAUCE_APPIUM_VERSION = "1.4.3";
        internal const double APPIUM_IOS_MINIMUM_VERSION = 6.1;
        internal const double APPIUM_ANDROID_MINIMUM_VERSION = 4.4;
        internal const String SAUCE_SESSIONNAME_CAPABILITY = "name";
        internal const String SAUCE_BUILDNAME_CAPABILITY = "build";
        internal const String SAUCE_USERNAME_CAPABILITY = "username";
        internal const String SAUCE_ACCESSKEY_CAPABILITY = "accessKey";
        internal const String SAUCE_BROWSER_NAME_CAPABILITY = "browserName";
        internal const String SAUCE_PLATFORM_CAPABILITY = "platform";
        internal const String SAUCE_PLATFORM_NAME_CAPABILITY = "platformName";
        internal const String SAUCE_PLATFORM_VERSION_CAPABILITY = "platformVersion";
        internal const String SAUCE_VERSION_CAPABILITY = "version";
        internal const String SAUCE_DEVICE_CAPABILITY = "device";
        internal const String SAUCE_DEVICE_NAME_CAPABILITY = "deviceName";
        internal const String SAUCE_DEVICE_ORIENTATION_CAPABILITY = "device-orientation";
        internal const String SAUCE_NATIVE_APP_CAPABILITY = "app";
        internal const String SAUCE_VUOP_CAPABILITY = "videoUploadOnPass";
        
        //SAUCE ENVIRONMENT VARIABLES
        internal const String SAUCE_USER_NAME = "SAUCE_USER_NAME";
        internal const String SAUCE_API_KEY = "SAUCE_API_KEY";
        public const String SAUCE_ONDEMAND_BROWSERS = "SAUCE_ONDEMAND_BROWSERS";
        internal const String JENKINS_BUILD_NUMBER = "JENKINS_BUILD_NUMBER";

        //TUNNELLING
        internal const int TUNNEL_CONNECT_RETRY_COUNT = 3;

        //REST
        internal const string SAUCE_REST_BASE = "https://saucelabs.com/rest/v1/";
        internal const string ACCOUNT_CONCURRENCY_REQUEST = "users/{0}/concurrency";
        internal const string JOB_REQUEST = "{0}/jobs/{1}";

        //FLOW CONTROL
        internal const int SAUCELABS_FLOW_WAIT = 1000;
        internal const int SAUCELABS_CLEANUP_WAIT = 500;
        
        //DEVICES
        internal const String LINUX = "Linux";
        internal const String APPLE_IOS = "Apple IOS";
        internal const String IOS_PLATFORM = "iOS";
        internal const String ANDROID_PLATFORM = "Android";
        internal const String ANDROID_DEVICE = "android";
        internal const String GOOGLE = "Google";
        internal const String LG = "LG";
        internal const String SAMSUNG = "Samsung";
        internal const String APPLE_IPHONE = "iphone";
        internal const String APPLE_IPAD = "ipad";
        internal const String ANDROID = "android";
        internal const String SAFARI_BROWSER = "Safari";
        internal const String CHROME_BROWSER = "chrome";
        internal const String DEFAULT_ANDROID_BROWSER = "browser";
        internal const String IPAD_SIMULATOR = "iPad Simulator";
        internal const String IPHONE_SIMULATOR = "iPhone Simulator";
        internal const String IPAD_DEVICE_NAME = "iPad";
        internal const String IPHONE_DEVICE_NAME = "iPhone";

        //MISCELLANEOUS
        internal const String APPLE_TITLE = "Apple";
        internal const String APPLE_BROWSE_LANDING = "Let's browse!";
        internal const String PASSED = "passed";
        internal const String FAILED = "failed";
        internal const String NULL_STRING = "null";
        internal const String SPACE = " ";
        internal const String ASTERIX = "*";
        internal const char SPACE_CHAR = ' ';
        internal const String BACKSLASH = @"\";
        internal const String LEFT_BRACKET = "(";
        internal const char LEFT_BRACKET_CHAR = '(';
        internal const char RIGHT_BRACKET_CHAR = ')';
        internal const String COLON = ":";
        internal const String UNDERSCORE = "_";
        internal const String RESOLUTION = "1280x1024";

        //SETTING UP
        internal const String SETTING_UP = "Setting up {0} for {1}";
        internal const String ANDROID_ON_APPIUM = "Android on Appium";
        internal const String IOS_ON_APPIUM = "IOS on Appium";
        internal const String ANDROID_ON_WEBDRIVER = "Android on WebDriver";
        internal const String IOS_ON_WEBDRIVER = "IOS on WebDriver";
        internal const String DESKTOP_ON_WEBDRIVER = "Desktop on WebDriver";

        //TEST VISIBILITY
        internal const String VISIBILITY_KEY = "public";
        internal const String VISIBILITY_PUBLIC = "public";
        internal const String VISIBILITY_PUBLIC_RESTRICTED = "public restricted";
        internal const String VISIBILITY_SHARE = "share";
        internal const String VISIBILITY_TEAM = "team";
        internal const String VISIBILITY_PRIVATE = "private";
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */