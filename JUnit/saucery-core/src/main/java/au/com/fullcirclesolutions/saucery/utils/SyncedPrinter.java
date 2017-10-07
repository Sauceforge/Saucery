package au.com.fullcirclesolutions.saucery.utils;

public class SyncedPrinter {
    public static void println(String s) {
        synchronized (System.out) {
            System.out.println(s);
            System.out.flush();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014 
 */