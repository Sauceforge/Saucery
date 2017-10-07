package au.com.fullcirclesolutions.saucery.capabilities;

class DesktopCreator extends Creator {
    @Override
    public BaseCapabilities Create(SaucePlatform platform, String testName) {
        return new DesktopCapabilities(platform, testName);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014 
 */