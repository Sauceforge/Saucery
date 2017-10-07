package au.com.fullcirclesolutions.internaltesting;


import au.com.fullcirclesolutions.saucery.registry.ModifyRegistry;

public class RegistryTester {

    public static void main(String[] args) {
        ModifyRegistry registry = new ModifyRegistry(false);
        registry.Write("registrytester", "just a test");
        String readVal = registry.Read("registrytester");
        System.out.println("readVal=" + readVal);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014 
 */
