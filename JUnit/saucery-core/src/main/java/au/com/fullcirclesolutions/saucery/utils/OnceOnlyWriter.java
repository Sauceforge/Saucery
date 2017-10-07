package au.com.fullcirclesolutions.saucery.utils;

import java.util.Arrays;
import java.util.HashSet;

public class OnceOnlyWriter {
    private static final HashSet<String> WrittenMessages = new HashSet<>();

    public static void WriteLine(String message) {
        if (!WrittenMessages.contains(message)) {
            System.out.println(message);
            System.out.flush();
            WrittenMessages.add(message);
        }
    }

    public static void Write(String message) {
        if (!WrittenMessages.contains(message)) {
            System.out.print(message);
            System.out.flush();
            WrittenMessages.add(message);
        }
    }

    public static void WriteLine(String messageWithParams, Object o1) {
        WriteLine(String.format(messageWithParams, o1));
    }

    public static void WriteLine(String messageWithParams, Object o1, Object o2) {
        WriteLine(String.format(messageWithParams, o1, o2));
    }
        
    public static void WriteLine(boolean value) {
        WriteLine(value ? "true" : "false");
    }

    public static void WriteLine(char value) {
        WriteLine(String.valueOf(value));
    }

    public static void WriteLine(char[] buffer) {
        WriteLine(Arrays.toString(buffer));
    }

    public static void WriteLine(double value) {
        WriteLine(String.valueOf(value));
    }

    public static void WriteLine(float value) {
        WriteLine(String.valueOf(value));
    }

    public static void WriteLine(int value) {
        WriteLine(String.valueOf(value));
    }
 
    public static void WriteLine(long value) {
        WriteLine(String.valueOf(value));
    }

    public static void WriteLine(Object value) {
        WriteLine(value.toString());
    }
   
    public static void WriteLine(String format, Object arg0, Object arg1, Object arg2) {
        WriteLine(String.format(format, arg0, arg1, arg2));
    }
        
    public static void WriteLine(String format, Object[] arg) {
        WriteLine(arg == null ? String.format(format, null, null) : String.format(format, arg));
    }

    public static void Write(String format, Object arg0) {
        Write(String.format(format, arg0));
    }

    public static void Write(String format, Object arg0, Object arg1) {
        Write(String.format(format, arg0, arg1));
    }

    public static void Write(String format, Object arg0, Object arg1, Object arg2) {
        Write(String.format(format, arg0, arg1, arg2));
    }

    public static void Write(String format, Object[] arg) {
        Write(arg == null ? String.format(format, null, null) : String.format(format, arg));
    }

    public static void Write(boolean value) {
        Write(value ? "true" : "false");
    }

    public static void Write(char value) {
        Write(String.valueOf(value));
    }

    public static void Write(char[] buffer) {
        Write(Arrays.toString(buffer));
    }

    public static void Write(double value) {
        Write(String.valueOf(value));
    }

    public static void Write(float value) {
        Write(String.valueOf(value));
    }

    public static void Write(int value) {
        Write(String.valueOf(value));
    }

    public static void Write(long value) {
        Write(String.valueOf(value));
    }
    
    public static void Write(Object value) {
        Write(value.toString());
    }
}
/*
 * Author: Andrew Gray, Full Circle Solutions
 * Date: 17th December 2015
 * 
 */