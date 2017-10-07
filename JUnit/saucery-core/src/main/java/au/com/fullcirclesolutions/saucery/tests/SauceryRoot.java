package au.com.fullcirclesolutions.saucery.tests;

import au.com.fullcirclesolutions.saucery.capabilities.CapabilityFactory;
import au.com.fullcirclesolutions.saucery.capabilities.SaucePlatform;
import au.com.fullcirclesolutions.saucery.restapi.flowcontrol.SauceLabsFlowController;
import au.com.fullcirclesolutions.saucery.utils.Enviro;
import au.com.fullcirclesolutions.saucery.utils.OnceOnlyMessages;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.saucelabs.common.SauceOnDemandAuthentication;
import com.saucelabs.common.SauceOnDemandSessionIdProvider;
import com.saucelabs.junit.ConcurrentParameterized;
import com.saucelabs.junit.SauceOnDemandTestWatcher;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Rule;
import org.junit.rules.TestName;
import org.openqa.selenium.remote.DesiredCapabilities;

public abstract class SauceryRoot implements SauceOnDemandSessionIdProvider {
    protected static final SauceLabsFlowController SauceLabsFlowController;
    protected final SaucePlatform platform;
    protected String sessionId;
    public SauceOnDemandAuthentication Authentication = new SauceOnDemandAuthentication(Enviro.SauceUserName(), Enviro.SauceApiKey());

    public @Rule
    SauceOnDemandTestWatcher ResultReportingTestWatcher = new SauceOnDemandTestWatcher(this, Authentication, true);
    public @Rule
    TestName TestName = new TestName();

    static {
        SauceLabsFlowController = new SauceLabsFlowController(Enviro.SauceUserName(), Enviro.SauceApiKey());
    }

    public SauceryRoot(String os, String platformType, String browser, String browserVersion, String longName, String longVersion, String url, String device, String deviceOrientation) {
        platform = new SaucePlatform(os, platformType, browser, browserVersion, longName, longVersion, url, device, deviceOrientation);
    }

    @ConcurrentParameterized.Parameters
    public static LinkedList<String[]> getEnvironments() throws Exception {
        return EnvironmentBuilder.BuildSeleniumEnvironments();
    }

    @BeforeClass
    public static void RunBeforeAnyTests() throws Exception {
        OnceOnlyMessages.CopyrightBanner();
        OnceOnlyMessages.OnDemand();
        ObjectMapper mapper = new ObjectMapper();
        List<SaucePlatform> saucePlatforms = Arrays.asList(mapper.readValue(Enviro.SauceOnDemandBrowsers(), SaucePlatform[].class));
        OnceOnlyMessages.TestingOn(saucePlatforms);
    }

    @Before
    public void setUp() throws Exception {
        //ActivationValidator.CheckActivation();
        DesiredCapabilities caps = CapabilityFactory.CreateCapabilities(platform, TestName.getMethodName());
        InitialiseDriver(caps, 30);
    }

    public abstract void InitialiseDriver(DesiredCapabilities caps, int waitSecs);

    @Override
    public String getSessionId() {
        return sessionId;
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 */