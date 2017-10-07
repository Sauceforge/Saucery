package au.com.fullcirclesolutions.internaltesting;

import org.junit.runner.JUnitCore;
import org.junit.runner.Result;

public class JUnitTester {

    public static void main(String[] args) {
        Result result = JUnitCore.runClasses(TestJUnit.class);
        if (result.wasSuccessful()) {
            System.out.println("JUnit responded successfully");
        } else {
            System.out.println("JUnit has issues!");
            result.getFailures().stream().forEach((failure) -> {
                System.out.println(failure.toString());
            });
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 15th November 2013
 */
