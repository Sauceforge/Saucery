package au.com.fullcirclesolutions.internaltesting;

import au.com.fullcirclesolutions.saucery.restapi.flowcontrol.json.FlowControl;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.saucelabs.saucerest.SauceREST;
import java.io.IOException;

public class RestTester {
    public static void main(String[] args) {
        boolean result = TooManyTests();
    }
    
    private static boolean TooManyTests() {
        SauceREST sauceRest = new SauceREST(SauceryConstants.MY_USERNAME, "ef77f071-8298-4fbd-90a4-a45a303576a7");
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
