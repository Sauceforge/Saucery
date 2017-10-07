package au.com.fullcirclesolutions.saucery.restapi.flowcontrol.json;

import com.fasterxml.jackson.annotation.JsonProperty;

public class FlowControl {
    @JsonProperty("remaining")
    public Remaining remaining;
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014 
 */