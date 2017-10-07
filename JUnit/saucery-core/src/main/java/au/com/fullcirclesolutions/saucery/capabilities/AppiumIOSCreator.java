package au.com.fullcirclesolutions.saucery.capabilities;

class AppiumIOSCreator extends Creator {
    @Override
    public BaseCapabilities Create(SaucePlatform platform, String testName) {
        return new AppiumIOSCapabilities(platform, testName);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 */