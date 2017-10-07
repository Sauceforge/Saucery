package au.com.fullcirclesolutions.saucery.restapi.flowcontrol;

import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.ParseException;
import org.apache.http.util.EntityUtils;

//import au.com.fullcirclesolutions.saucery.restapi.WebRequester;

public abstract class FlowController {

    //protected WebRequester Requester = null;

    protected abstract boolean TooManyTests();

    public abstract void ControlFlow();

    protected String GetResponseString(HttpResponse response) {
        try {
            return EntityUtils.toString(response.getEntity());
        } catch (ParseException | IOException e) {
            return null;
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014 
 */