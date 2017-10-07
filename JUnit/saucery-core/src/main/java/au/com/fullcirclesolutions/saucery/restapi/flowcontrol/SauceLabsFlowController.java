package au.com.fullcirclesolutions.saucery.restapi.flowcontrol;

import au.com.fullcirclesolutions.saucery.restapi.flowcontrol.json.FlowControl;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import com.fasterxml.jackson.databind.ObjectMapper;

import com.saucelabs.saucerest.SauceREST;
import java.io.IOException;

public class SauceLabsFlowController extends FlowController {
    public String UserName;
    public String AccessKey;
    
    public SauceLabsFlowController(String username, String accessKey) {
        UserName = username;
        AccessKey = accessKey;
    }

    @Override
    public void ControlFlow() {
        while (TooManyTests()) {
            try {
                Thread.sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
            } catch (InterruptedException e) {
            }
        }
    }

    @Override
    protected boolean TooManyTests() {
        SauceREST sauceRest = new SauceREST(UserName, AccessKey);
        ObjectMapper mapper = new ObjectMapper();
        String remaining = getRemainingSection(sauceRest.getConcurrency());
        
        try {
            FlowControl flowcontrol = mapper.readValue(remaining, FlowControl.class);
            return flowcontrol.remaining.overall <= 0;
        } catch (IOException io){
            return true;
        }
    }
    
    private static String getRemainingSection(String json){
        String remainingSection = json.substring(json.indexOf("\"remaining"), json.length() - 3);
        return String.format("{%s}", remainingSection);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014
 */