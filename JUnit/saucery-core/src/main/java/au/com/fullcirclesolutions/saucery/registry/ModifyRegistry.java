package au.com.fullcirclesolutions.saucery.registry;

import java.util.prefs.Preferences;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;

/// <summary>
/// An useful class to read/write/delete/count registry keys
/// </summary>
public class ModifyRegistry {
    public boolean ShowError;

    public String RootSubKey;
    public String ProductSubKey;
    public String VersionSubKey;
    public String EditionSubKey;
    public String NestedKey;

    public Preferences BaseRegistryKey;

    public ModifyRegistry(boolean isAdministrator) {
        BaseRegistryKey = isAdministrator ? Preferences.systemRoot() : Preferences.userRoot();
        RootSubKey = SauceryConstants.COMPANYNAME;
        ProductSubKey = SauceryConstants.PRODUCTNAME;
        VersionSubKey = SauceryConstants.VERSION;
        EditionSubKey = SauceryConstants.EDITION;
        NestedKey = RootSubKey + SauceryConstants.FORWARDSLASH
                + ProductSubKey + SauceryConstants.FORWARDSLASH
                + VersionSubKey + SauceryConstants.FORWARDSLASH
                + EditionSubKey;
    }

    public String Read(String keyName) {
        Preferences targetKey = BaseRegistryKey.node(NestedKey);
        return targetKey.get(keyName, null);
    }

    public boolean Write(String keyName, String value) {
        try {
            Preferences targetKey = BaseRegistryKey.node(NestedKey);
            targetKey.put(keyName, value);
            targetKey.flush();
            String readValue = targetKey.get(keyName, null);
            return (readValue == null ? value == null : readValue.equals(value));
        } catch (Exception e) {
            return false;
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2010
 */