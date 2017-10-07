package au.com.fullcirclesolutions.saucery.restapi;

import org.apache.http.HttpHeaders;
import org.apache.http.auth.AuthScope;
import org.apache.http.auth.UsernamePasswordCredentials;
import org.apache.http.client.CredentialsProvider;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.BasicCredentialsProvider;
import org.apache.http.impl.client.HttpClients;


public class SauceLabsWebRequester extends WebRequester {

    public SauceLabsWebRequester(String username, String accessKey) {
        super(username, accessKey);
    }

    @Override
    public CloseableHttpResponse GetResponse() {
        try {
            HttpGet request = new HttpGet(CurlURL);
            request.addHeader(HttpHeaders.ACCEPT, "application/json");
            CredentialsProvider credsProvider = new BasicCredentialsProvider();
            credsProvider.setCredentials(AuthScope.ANY, new UsernamePasswordCredentials(UserName, AccessKey));
            HTTPClient = HttpClients.custom().setDefaultCredentialsProvider(credsProvider).build();
            return HTTPClient.execute(request);
        } catch (Exception ex) {
            return null;
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 26th March 2014
 */