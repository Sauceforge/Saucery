package au.com.fullcirclesolutions.saucery.restapi;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClientBuilder;

public abstract class WebRequester {

    protected CloseableHttpClient HTTPClient = HttpClientBuilder.create().build();
    protected HttpEntity entity;
    public String UserName;
    public String AccessKey;
    protected String CurlURL;

    public WebRequester(String username, String accessKey) {
        UserName = username;
        AccessKey = accessKey;
    }

    public WebRequester(String username, String accessKey, String url) {
        this(username, accessKey);
        CurlURL = url;
    }

    public void SetURL(String url) {
        CurlURL = url;
    }

    public abstract HttpResponse GetResponse();
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014
 */
