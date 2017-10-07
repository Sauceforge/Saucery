package au.com.fullcirclesolutions.saucery.utils;

import au.com.fullcirclesolutions.saucery.capabilities.SaucePlatform;
import java.util.List;
import java.util.Date;
import java.text.SimpleDateFormat;

public class OnceOnlyMessages {
    public static void CopyrightBanner() {
        OnceOnlyWriter.WriteLine(SauceryConstants.CONSOLE_RUNNER, SauceryConstants.PRODUCTNAME);
        OnceOnlyWriter.WriteLine(SauceryConstants.COPYRIGHT_NOTICE, GetCurrentYear());
        OnceOnlyWriter.WriteLine(SauceryConstants.SPACE);
    }

    public static void TestingOn(List<SaucePlatform> platforms) {
        OnceOnlyWriter.WriteLine(platforms != null
            ? String.format(SauceryConstants.TESTING_ON, platforms.size(), getMoniker(platforms))
            : SauceryConstants.NO_PLATFORMS);
    }

    public static void OnDemand() {
        if (UserChecker.ItIsMe()) {
            OnceOnlyWriter.WriteLine(SauceryConstants.ON_DEMAND, Enviro.SauceOnDemandBrowsers());
        }
    }

//    public static void UsingLicenced() {
//        OnceOnlyWriter.WriteLine(SauceryConstants.LICENCED_VERSION, SauceryConstants.PRODUCTNAME);
//    }
//
//    public static void UsingTrial() {
//        OnceOnlyWriter.WriteLine(SauceryConstants.TRIAL_VERSION, SauceryConstants.PRODUCTNAME);
//    }
//
//    public static void DaysRemaining(double remaining) {
//        OnceOnlyWriter.WriteLine(SauceryConstants.DAYS_REMAINING, remaining);
//    }

    private static String GetCurrentYear() {
        Date date1 = new Date();
        SimpleDateFormat x = new SimpleDateFormat(SauceryConstants.YEAR_FORMAT);
        return x.format(date1);
    }
    
    private static String getMoniker(List<SaucePlatform> platforms){
        return platforms.size() == 1 ? "platform" : "platforms";
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 24th December 2015
 * 
 */