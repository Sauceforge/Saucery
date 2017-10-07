package au.com.fullcirclesolutions.saucery.utils;

public class UserChecker {
    public static boolean ItIsMe() {
            return Enviro.SauceUserName().toLowerCase().equals(SauceryConstants.MY_USERNAME);
        }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 */