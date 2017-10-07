using NUnit.Framework.Constraints;

namespace Saucery2.Util {
    public class SauceryConstants {
        //ACTIVATION DETAILS
        internal const string MY_USERNAME = "agray2";
        internal static readonly string MY_USERNAME_LOWER = MY_USERNAME.ToLower();
        internal const string COMPANYNAME = "Full Circle Solutions";
        internal const string EDITION = "Enterprise";
        internal const string KEYNAME = "SauceryKey";
        internal const string REGISTRYROOT = @"SOFTWARE\" + COMPANYNAME + BACKSLASH;
        internal const string LICENCE_DATE_FORMAT = "ddMMyyyy";
        internal const string PUBLIC_KEY = "44C9B9B2D6C2D11CBF7CD9DF9C37178462A1CE514FCED53CE9B04561C35614C5E525AEE5E3064CFB68D676522F92992721A60374032A200D07CE956B25B87B81";
        internal const string FIRST_KEY = "0CDCF65343FEDEE34702856B251025AD80D4FE58C2481B584D7FB246BAC23A6DB4E44E5654A856F182A673D6953FD42725FAA316DE6A78059DECE599BF233D00";
        
        internal const int FREE_TRIAL_PERIOD = 30;
        internal const int PAID_LICENCE_PERIOD = 366;
        
        internal const string SOLUTION_MASK = ASTERIX + ".sln";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string PROJECT_MASK = ASTERIX + PROJECT_EXTENSION;
        internal const string ASSEMBLYINFO_RELATIVE_PATH = @"\Properties\AssemblyInfo.cs";
        internal const string ASSEMBLY_FILE_GUID_LINE = "[assembly: Guid";
        internal const string SOLUTION_FILE_PROJECT_LINE = "Project(";

        //TEST URL
        internal const string SAUCELABS_HUB = "http://ondemand.saucelabs.com:80/wd/hub";

        //CAPABILITIES
        //internal const string RESOLUTION_CAPABILITY = "resolution";
        //internal const string SCREENRESOLUTION_CAPABILITY = "screen-resolution";
        internal const string SAUCE_APPIUM_VERSION_CAPABILITY = "appium-version";
        //internal const string APPIUM_VERSION = "1.4.13";
        internal const double APPIUM_IOS_MINIMUM_VERSION = 6.1;
        internal const double APPIUM_ANDROID_MINIMUM_VERSION = 4.4;
        internal const string SAUCE_SESSIONNAME_CAPABILITY = "name";
        internal const string SAUCE_BUILDNAME_CAPABILITY = "build";
        internal const string SAUCE_USERNAME_CAPABILITY = "username";
        internal const string SAUCE_ACCESSKEY_CAPABILITY = "accessKey";
        internal const string SAUCE_BROWSER_NAME_CAPABILITY = "browserName";
        internal const string SAUCE_PLATFORM_CAPABILITY = "platform";
        internal const string SAUCE_PLATFORM_NAME_CAPABILITY = "platformName";
        internal const string SAUCE_PLATFORM_VERSION_CAPABILITY = "platformVersion";
        internal const string SAUCE_VERSION_CAPABILITY = "version";
        internal const string SAUCE_DEVICE_CAPABILITY = "device";
        internal const string SAUCE_DEVICE_NAME_CAPABILITY = "deviceName";
        internal const string SAUCE_DEVICE_ORIENTATION_CAPABILITY = "device-orientation";
        internal const string SAUCE_NATIVE_APP_CAPABILITY = "app";
        internal const string SAUCE_VUOP_CAPABILITY = "videoUploadOnPass";
        
        //SAUCE ENVIRONMENT VARIABLES
        internal const string SAUCE_USER_NAME = "SAUCE_USER_NAME";
        internal const string SAUCE_API_KEY = "SAUCE_API_KEY";
        internal const string SAUCE_ONDEMAND_BROWSERS = "SAUCE_ONDEMAND_BROWSERS";
        internal const string SAUCE_NATIVE_APP = "SAUCE_NATIVE_APP";
        internal const string SAUCE_USE_CHROME_ON_ANDROID = "SAUCE_USE_CHROME_ON_ANDROID";
        internal const string JENKINS_BUILD_NUMBER = "JENKINS_BUILD_NUMBER";
        internal const string BAMBOO_BUILD_NUMBER = "SAUCE_BAMBOO_BUILDNUMBER";
        internal const string RECOMMENDED_APPIUM_VERSION = "RECOMMENDED_APPIUM_VERSION";

        //TUNNELLING
        internal const int TUNNEL_CONNECT_RETRY_COUNT = 3;

        //REST
        internal const string JSON_CONTENT_TYPE = "application/json";
        internal const string RESTAPI_LIMIT_EXCEEDED_INDICATOR = "API rate limit exceeded for";
        internal const string RESTAPI_LIMIT_EXCEEDED_MSG = "API rate limit exceeded. We have to wait. See https://saucelabs.com/blog/announcing-new-rest-api-rate-limits for more details.";
        internal const string SAUCE_REST_BASE = "https://saucelabs.com/rest/v1/";
        internal const string ACCOUNT_CONCURRENCY_REQUEST = "users/{0}/concurrency";
        internal const string JOB_REQUEST = "{0}/jobs/{1}";
        internal const string RECOMMENDED_APPIUM_REQUEST = "info/platforms/appium";
        internal const string JSON_SEGMENT_CONTAINER = "{{{0}}}";
        internal const string NUGET_API = "https://packages.nuget.org/api/v2";
        internal const string NUGET_VERSION = "{0}.{1}.{2}";

        //FLOW CONTROL
        internal const int SAUCELABS_FLOW_WAIT = 3000;

        //DEVICES
        internal const string LINUX = "Linux";
        internal const string APPLE_IOS = "Apple IOS";
        internal const string IOS_PLATFORM = "iOS";
        internal const string ANDROID_PLATFORM = "ANDROID";
        internal const string ANDROID_DEVICE = "android";
        internal const string GOOGLE = "Google";
        internal const string LG = "LG";
        internal const string SAMSUNG = "Samsung";
        internal const string APPLE_IPHONE = "iphone";
        internal const string APPLE_IPAD = "ipad";
        internal const string ANDROID = "android";
        internal const string SAFARI_BROWSER = "Safari";
        internal const string CHROME_BROWSER = "chrome";
        internal const string DEFAULT_ANDROID_BROWSER = "browser";
        internal const string IPAD_SIMULATOR = "iPad Simulator";
        internal const string IPHONE_SIMULATOR = "iPhone Simulator";
        internal const string IPAD_DEVICE = "iPad";
        internal const string IPHONE_DEVICE = "iPhone";

        //MISCELLANEOUS
        internal const string APPLE_TITLE = "Apple";
        internal const string PASSED = "passed";
        internal const string FAILED = "failed";
        internal const string NULL_STRING = "null";
        internal const string SPACE = " ";
        internal const string TWO_SPACES = "  ";
        internal const string ASTERIX = "*";
        internal const char SPACE_CHAR = ' ';
        internal const string BACKSLASH = @"\";
        internal const string LEFT_BRACKET = "(";
        internal const char LEFT_BRACKET_CHAR = '(';
        internal const char RIGHT_BRACKET_CHAR = ')';
        internal const string LEFT_SQUARE_BRACKET = "[";
        internal const string COLON = ":";
        internal const string UNDERSCORE = "_";
        internal const string RESOLUTION = "1280x1024";
        internal const string YEAR_FORMAT = "yyyy";

        //NOTIFICATION MESSAGES
        //internal const string CONSOLE_RUNNER = "{0} Console Runner {1}";
        internal const string CONSOLE_RUNNER = "{0} Console Runner";
        internal const string COPYRIGHT_NOTICE = "Copyright (C) 2008-{0} Andrew Gray";
        internal const string TESTING_ON = "Testing on {0} {1}";
        internal const string NO_PLATFORMS = "Platforms is null";
        internal const string ON_DEMAND = "OnDemand={0}";
        internal const string LICENCED_VERSION = "You are using the licenced version of {0}.";
        internal const string TRIAL_VERSION = "You are using the trial version of {0}.";
        internal const string DAYS_REMAINING = "You have {0} days remaining.";
        internal const string NEW_VERSION_AVAILABLE = "You are using version {0}. Version {1} is available!";

        //SETTING UP
        internal const string SETTING_UP = "Setting up {0} for {1}";
        internal const string SETTING_UP_APPIUM = "Setting up {0} for {1} {2}";
        internal const string ANDROID_ON_APPIUM = "Android on Appium";
        internal const string IOS_ON_APPIUM = "IOS on Appium";
        internal const string ANDROID_ON_WEBDRIVER = "Android on WebDriver";
        internal const string IOS_ON_WEBDRIVER = "IOS on WebDriver";
        internal const string DESKTOP_ON_WEBDRIVER = "Desktop on WebDriver";

        //TEST VISIBILITY
        internal const string VISIBILITY_KEY = "public";
        internal const string VISIBILITY_PUBLIC = "public";
        internal const string VISIBILITY_PUBLIC_RESTRICTED = "public restricted";
        internal const string VISIBILITY_SHARE = "share";
        internal const string VISIBILITY_TEAM = "team";
        internal const string VISIBILITY_PRIVATE = "private";
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */