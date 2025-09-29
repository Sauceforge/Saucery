namespace Saucery.Core.Util;

public static class SauceryConstants {
    //ACTIVATION DETAILS
    internal const string MY_USERNAME = "saucefauge";
    public static readonly string MY_USERNAME_LOWER = MY_USERNAME.ToLower();
    internal const string COMPANYNAME = "SauceForge";
    internal const string EDITION = "Enterprise";
    internal const string KEYNAME = "SauceryKey";
    internal const string REGISTRYROOT = @"SOFTWARE\" + COMPANYNAME + BACKSLASH;
    internal const string LICENCE_DATE_FORMAT = "ddMMyyyy";
    internal const int FREE_TRIAL_PERIOD = 30;
    internal const int PAID_LICENCE_PERIOD = 366;

    internal const string PRODUCTNAME = "Saucery";
    internal const string VERSION = "4.0.0.0";
    internal const string SAUCERY_LICENCE_FILE = @"\Saucery.lic"; //need the backslash here!
    internal const string LICENCE_PRODUCTCODE = "N3";
    internal const string TRIAL_PRODUCTCODE = "3T";
    internal static readonly int[] NUNIT3POSARRAY = [3, 6, 9, 12, 15, 18, 21, 24, 27, 30];
    internal const string SAUCERY_ASSEMBLY = "<Reference Include=\"Saucery";
    internal const string SAUCERYTESTER_ASSEMBLY = "<ProjectReference Include=\"..\\Saucery\\Saucery.csproj\">";

    internal const string SOLUTION_MASK = ASTERIX + ".sln";
    internal const string PROJECT_EXTENSION = ".csproj";
    internal const string PROJECT_MASK = ASTERIX + PROJECT_EXTENSION;
    internal const string ASSEMBLYINFO_RELATIVE_PATH = @"\Properties\AssemblyInfo.cs";
    internal const string ASSEMBLY_FILE_GUID_LINE = "[assembly: Guid";
    internal const string SOLUTION_FILE_PROJECT_LINE = "Project(";

    //TEST URL
    public const string SAUCELABS_HUB = "https://ondemand.us-west-1.saucelabs.com:443/wd/hub";
    
    public static readonly int MISSING_CHROMIUM_VERSION = 82;

    //CAPABILITIES
    internal const string SCREEN_RESOLUTION_CAPABILITY = "screenResolution";
    internal const string APPLE_AUTOMATION_NAME = "XCUITest";
    internal const string ANDROID_AUTOMATION_NAME = "UiAutomator2";
    internal const string SAUCE_APPIUM_VERSION_CAPABILITY = "appiumVersion";
    internal const string SELENIUM_VERSION_CAPABILITY = "seleniumVersion";
    internal const string SAUCE_SESSIONNAME_CAPABILITY = "name";
    internal const string SAUCE_BUILDNAME_CAPABILITY = "build";
    internal const string SAUCE_USERNAME_CAPABILITY = "username";
    internal const string SAUCE_ACCESSKEY_CAPABILITY = "accessKey";
    internal const string SAUCE_BROWSER_NAME_CAPABILITY = "browserName";
    internal const string SAUCE_PLATFORM_CAPABILITY = "platform";
    internal const string SAUCE_PLATFORM_NAME_CAPABILITY = "platformName";
    internal const string SAUCE_PLATFORM_VERSION_CAPABILITY = "PlatformVersion";
    internal const string SAUCE_APPIUM_PLATFORM_VERSION_CAPABILITY = "platformVersion";
    internal const string SAUCE_VERSION_CAPABILITY = "version";
    internal const string SAUCE_DEVICE_CAPABILITY = "device";
    internal const string SAUCE_DEVICE_NAME_CAPABILITY = "DeviceName";
    internal const string SAUCE_APPIUM_DEVICE_NAME_CAPABILITY = "deviceName";
    internal const string SAUCE_DEVICE_ORIENTATION_CAPABILITY = "deviceOrientation";
    internal const string SAUCE_NATIVE_APP_CAPABILITY = "app";
    internal const string SAUCE_VUOP_CAPABILITY = "videoUploadOnPass";
    internal const string SAUCE_OPTIONS_CAPABILITY = "sauce:options";

    //SAUCE ENVIRONMENT VARIABLES
    public static readonly string SAUCE_USER_NAME = "SAUCE_USER_NAME";
    public static readonly string SAUCE_API_KEY = "SAUCE_API_KEY";
    internal const string SAUCE_ONDEMAND_BROWSERS = "SAUCE_ONDEMAND_BROWSERS";
    internal const string SAUCE_NATIVE_APP = "SAUCE_NATIVE_APP";
    internal const string JENKINS_BUILD_NUMBER = "JENKINS_BUILD_NUMBER";
    internal const string BAMBOO_BUILD_NUMBER = "SAUCE_BAMBOO_BUILDNUMBER";
    internal const string BUILD_NUMBER = "BUILD_BUILDNUMBER";
    internal const string RECOMMENDED_APPIUM_VERSION = "RECOMMENDED_APPIUM_VERSION";

    //TIMEOUT
    public const int SELENIUM_COMMAND_TIMEOUT = 500;

    //TUNNELLING
    internal const int TUNNEL_CONNECT_RETRY_COUNT = 3;

    //REST
    internal const string JSON_CONTENT_TYPE = "application/json";
    internal const string RESTAPI_LIMIT_EXCEEDED_INDICATOR = "API rate limit exceeded for";
    internal const string RESTAPI_LIMIT_EXCEEDED_MSG = "API rate limit exceeded. We have to wait. See https://saucelabs.com/blog/announcing-new-rest-api-rate-limits for more details.";
    internal const string SAUCE_REST_BASE = "https://api.us-west-1.saucelabs.com/rest/";
    internal const string SAUCE_REAL_DEVICE_REST_BASE = "https://api.us-west-1.saucelabs.com/";
    internal const string ACCOUNT_CONCURRENCY_REQUEST = "v1.2/users/{0}/concurrency";
    internal const string JOB_REQUEST = "v1/{0}/jobs/{1}";
    internal const string RD_JOB_REQUEST = "v1/rdc/jobs/{0}";
    internal const string RECOMMENDED_APPIUM_REQUEST = "v1/info/platforms/appium";
    internal const string SUPPORTED_PLATFORMS_REQUEST = "v1/info/platforms/all";
    internal const string SUPPORTED_REALDEVICE_PLATFORMS_REQUEST = "v1/rdc/devices";
    internal const string RD_JOBS_REQUEST = "/v1/rdc/jobs";
    internal const string JSON_SEGMENT_CONTAINER = "{{{0}}}";
    internal const string NUGET_API = "https://packages.nuget.org/api/v2";
    internal const string NUGET_VERSION = "{0}.{1}.{2}";

    //FLOW CONTROL
    internal const int SAUCELABS_FLOW_WAIT = 3000;

    //DEVICES
    internal const string APPLE_IOS = "Apple IOS";
    internal const string ANDROID_PLATFORM = "ANDROID";
    internal const string GOOGLE = "Google";
    internal const string LG = "LG";
    internal const string SAMSUNG = "Samsung";
    internal const string GOOGLE_LOWER = "google";
    internal const string SAMSUNG_LOWER = "samsung";
    internal const string ANDROID_LOWER = "android";
    internal const string APPLE_IPHONE = "iphone";
    internal const string APPLE_IPAD = "ipad";
    internal const string ANDROID = "Android";
    internal const string SAFARI_BROWSER = "Safari";
    internal const string CHROME_BROWSER = "Chrome";
    internal const string IPAD_SIMULATOR = "iPad Simulator";
    internal const string IPHONE_SIMULATOR = "iPhone Simulator";
    internal const string IPAD_DEVICE = "iPad";
    internal const string IPHONE_DEVICE = "iPhone";

    //MISCELLANEOUS
    internal const string APPLE_TITLE = "Apple";
    internal const string PASSED = "passed";
    internal const string FAILED = "failed";
    internal const string NULL_STRING = "null";
    public static readonly string DOT = ".";
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
    internal const string HYPHEN = "-";
    internal const string UNDERSCORE = "_";
    internal const string RESOLUTION = "1280x1024";
    internal const string YEAR_FORMAT = "yyyy";

    //NOTIFICATION MESSAGES
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
    internal const string ANDROID_ON_APPIUM = "Android Emulator on Appium";
    internal const string REAL_ANDROID_ON_APPIUM = "Android Real Device on Appium";
    internal const string IOS_ON_APPIUM = "IOS Emulator on Appium";
    internal const string REAL_IOS_ON_APPIUM = "IOS Real Device on Appium";
    internal const string ANDROID_ON_WEBDRIVER = "Android on WebDriver";
    internal const string IOS_ON_WEBDRIVER = "IOS on WebDriver";
    internal const string DESKTOP_ON_WEBDRIVER = "Desktop on WebDriver";
    internal const string NUM_VALID_PLATFORMS = "{0} of {1} platforms requested are valid";

    //TEST VISIBILITY
    internal const string VISIBILITY_KEY = "public";
    internal const string VISIBILITY_PUBLIC = "public";
    internal const string VISIBILITY_PUBLIC_RESTRICTED = "public restricted";
    internal const string VISIBILITY_SHARE = "share";
    internal const string VISIBILITY_TEAM = "team";
    internal const string VISIBILITY_PRIVATE = "private";

    //PLATFORMS
    public const string PLATFORM_WINDOWS_11 = "Windows 11";
    public const string PLATFORM_WINDOWS_10 = "Windows 10";
    public const string PLATFORM_WINDOWS_81 = "Windows 2012 R2";
    public const string PLATFORM_WINDOWS_8 = "Windows 2012";
    public const string PLATFORM_MAC_15 = "Mac 15";
    public const string PLATFORM_MAC_14 = "Mac 14";
    public const string PLATFORM_MAC_13 = "Mac 13";
    public const string PLATFORM_MAC_12 = "Mac 12";
    public const string PLATFORM_MAC_11 = "Mac 11";
    public const string PLATFORM_IOS = "iOS";
    public const string PLATFORM_LINUX = "Linux";

    //BROWSERS
    public const string BROWSER_CHROME = "chrome";
    public const string BROWSER_FIREFOX = "firefox";
    public const string BROWSER_IE = "internet explorer";
    public const string BROWSER_EDGE = "MicrosoftEdge";
    public const string BROWSER_EDGE_LOWER = "microsoftedge";
    public const string BROWSER_SAFARI = "safari";

    //BROWSER VERSIONS
    public static readonly string BROWSER_VERSION_DEV = "dev";
    public static readonly string BROWSER_VERSION_BETA = "beta";
    public static readonly string BROWSER_VERSION_LATEST = "latest";
    public static readonly string BROWSER_VERSION_LATEST_MINUS1 = "latest-1";
    public static readonly string PLATFORM_SEPARATOR = "->";
    public static readonly List<string> BROWSER_VERSIONS_LATEST = [BROWSER_VERSION_LATEST_MINUS1, BROWSER_VERSION_LATEST];
    public static readonly List<string> BROWSER_VERSIONS_DEVS = [BROWSER_VERSION_BETA, BROWSER_VERSION_DEV];
    internal static readonly List<string> BROWSER_VERSIONS_NONNUMERIC = [.. BROWSER_VERSIONS_LATEST, .. BROWSER_VERSIONS_DEVS];

    //SCREEN RESOLUTIONS
    public static readonly string SCREENRES_800_600 = "800x600";
    public static readonly string SCREENRES_1024_768 = "1024x768";
    public static readonly string SCREENRES_1152_720 = "1152x720";
    public static readonly string SCREENRES_1152_864 = "1152x864";
    public static readonly string SCREENRES_1152_900 = "1152x900";
    public static readonly string SCREENRES_1280_720 = "1280x720";
    public static readonly string SCREENRES_1280_768 = "1280x768";
    public static readonly string SCREENRES_1280_800 = "1280x800";
    public static readonly string SCREENRES_1280_960 = "1280x960";
    public static readonly string SCREENRES_1280_1024 = "1280x1024";
    public static readonly string SCREENRES_1376_1032 = "1376x1032";
    public static readonly string SCREENRES_1400_1050 = "1400x1050";
    public static readonly string SCREENRES_1440_900 = "1440x900";
    public static readonly string SCREENRES_1600_900 = "1600x900";
    public static readonly string SCREENRES_1600_1200 = "1600x1200";
    public static readonly string SCREENRES_1680_1050 = "1680x1050";
    public static readonly string SCREENRES_1920_1080 = "1920x1080";
    public static readonly string SCREENRES_1920_1200 = "1920x1200";
    public static readonly string SCREENRES_1920_1440 = "1920x1440";
    public static readonly string SCREENRES_2048_1152 = "2048x1152";
    public static readonly string SCREENRES_2048_1536 = "2048x1536";
    public static readonly string SCREENRES_2360_1770 = "2360x1770";
    public static readonly string SCREENRES_2560_1600 = "2560x1600";

    //DEVICE ORIENTATIONS
    public static readonly string DEVICE_ORIENTATION_PORTRAIT = "portrait";
    public static readonly string DEVICE_ORIENTATION_LANDSCAPE = "landscape";
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/