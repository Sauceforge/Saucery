package au.com.fullcirclesolutions.saucery.capabilities;

class WebDriverIOSCreator extends Creator {
    @Override
    public BaseCapabilities Create(SaucePlatform platform, String testName) {
        return new WebDriverIOSCapabilities(platform, testName);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 */