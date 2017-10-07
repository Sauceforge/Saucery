package au.com.fullcirclesolutions.saucery.registry;

import java.util.prefs.Preferences;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;

public class RegistryHelper {

    static ModifyRegistry RegModifier = new ModifyRegistry(IsAdministrator());

    public static boolean IsAlreadyActivated() {
        String key = RegModifier.Read(SauceryConstants.KEYNAME);
        return key != null && (!key.equals(SauceryConstants.EMPTY_STRING));
    }

    public static void StoreActivation(String publicKey) {
        RegModifier.Write(SauceryConstants.KEYNAME, publicKey);
    }

    public static boolean IsAdministrator() {
        Preferences prefs = Preferences.systemRoot();
        try {
            prefs.put("foo", "bar"); // SecurityException on Windows
            prefs.remove("foo");
            prefs.flush(); // BackingStoreException on Linux
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 22nd September 2010
 */