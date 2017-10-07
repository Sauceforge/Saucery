package au.com.fullcirclesolutions.saucery.restapi.flowcontrol.json;

import com.fasterxml.jackson.annotation.JsonProperty;

public class Remaining {
    @JsonProperty("overall")
    public int overall;
    @JsonProperty("mac")
    public int mac;
    @JsonProperty("manual")
    public int manual;
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014 
 */