package au.com.fullcirclesolutions.saucery.tests;

import au.com.fullcirclesolutions.saucery.capabilities.SaucePlatform;
import au.com.fullcirclesolutions.saucery.utils.Enviro;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.List;
import java.util.Arrays;
import java.util.LinkedList;

class EnvironmentBuilder {
    static LinkedList<String[]> BuildSeleniumEnvironments() {
        LinkedList<String[]> env = new LinkedList<>();
        try {
            ObjectMapper mapper = new ObjectMapper();
            List<SaucePlatform> saucePlatforms = Arrays.asList(mapper.readValue(Enviro.SauceOnDemandBrowsers(), SaucePlatform[].class));
            for (SaucePlatform platform : saucePlatforms) {
                String[] platformEntry = platform.toArray();
                if (platformEntry != null) {
                    env.add(platformEntry);
                }
            }
        } catch (Exception ex) {
            System.err.println("Caught Exception Message: " + ex.getMessage());
            System.err.println("Caught Exception Cause: " + ex.getCause().toString());
        }
        //System.out.println("Env count: " + env.size());
        return env;
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 27th September 2014
 */