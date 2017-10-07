package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.Enviro;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import org.openqa.selenium.remote.DesiredCapabilities;

abstract class BaseCapabilities {
    DesiredCapabilities Caps = null;
    private final String _testName;

    BaseCapabilities(String testName) {
        _testName = testName;
    }

    void AddSauceLabsCapabilities() {
        //This sets the Session column
        Caps.setCapability(SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, _testName);
        //This sets the Build column
        Caps.setCapability(SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Enviro.BuildNumber());
        //Improve performance on SauceLabs
        Caps.setCapability(SauceryConstants.SAUCE_VUOP_CAPABILITY, false);
        //Caps.setCapability(Constants.VISIBILITY_KEY, Constants.VISIBILITY_TEAM);
    }

    void AddSauceLabsCapabilities(String nativeApp) {
        AddSauceLabsCapabilities();
        if (nativeApp != null) {
            Caps.setCapability(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
        }
    }

    DesiredCapabilities GetCaps() {
        return Caps;
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014 
 */