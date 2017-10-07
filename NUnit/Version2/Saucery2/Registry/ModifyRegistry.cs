using System;
using Microsoft.Win32;
using Saucery2.Log;
using Saucery2.Util;

namespace Saucery2.Registry {
    /// <summary>
    /// An useful class to read/write/delete/count registry keys
    /// </summary>
    internal class ModifyRegistry {
        /// <summary>
        /// A property to show or hide error messages 
        /// (default = false)
        /// </summary>
        public bool ShowError { get; set; }

        /// <summary>
        /// A property to set the SubKey value
        /// (default = "SOFTWARE\\" + Application.ProductName.ToUpper())
        /// </summary>
        public string RootSubKey { get; set; }
        public string ProductSubKey { get; set; }
        public string VersionSubKey { get; set; }
        public string EditionSubKey { get; set; }
        public string NestedKey { get; set; }

        /// <summary>
        /// A property to set the BaseRegistryKey value.
        /// (default = Registry.LocalMachine)
        /// </summary>
        public RegistryKey BaseRegistryKey { get; set; }

        public ModifyRegistry(bool isAdministrator) {
            BaseRegistryKey = isAdministrator ? Microsoft.Win32.Registry.LocalMachine : Microsoft.Win32.Registry.CurrentUser;
            RootSubKey = SauceryConstants.REGISTRYROOT;
            ProductSubKey = Saucery2Constants.PRODUCTNAME;
            VersionSubKey = Saucery2Constants.VERSION;
            EditionSubKey = SauceryConstants.EDITION;
            NestedKey = RootSubKey + SauceryConstants.BACKSLASH +
                        ProductSubKey + SauceryConstants.BACKSLASH +
                        VersionSubKey + SauceryConstants.BACKSLASH +
                        EditionSubKey;
        }

        /// <summary>
        /// To read a registry key.
        /// input: KeyName (string)
        /// output: value (string) 
        /// </summary>
        public string Read(string keyName) {
            var regCuBase64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            var targetKey = regCuBase64.OpenSubKey(NestedKey);
            if(targetKey == null) {
                return null;
            }
            try {
                // If the RegistryKey exists I get its value
                // or null is returned.
                var objValue = targetKey.GetValue(keyName);
                return objValue == null ? null : objValue.ToString();
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return null;
            }
        }

        /// <summary>
        /// To write into a registry key.
        /// input: KeyName (string) , Value (object)
        /// output: true or false 
        /// </summary>
        public bool Write(string keyName, object value) {
            try {
                var regCuBase64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                var targetKey = regCuBase64.CreateSubKey(NestedKey);
                if(targetKey != null) {
                    targetKey.SetValue(keyName, value);
                }
                EventLogger.WriteInfo(string.Format("Wrote {0} to {1} in subkey {2} of BRK {3}", value, keyName, NestedKey, regCuBase64));
                return true;
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return false;
            }
        }

        /// <summary>
        /// To delete a registry key.
        /// input: KeyName (string)
        /// output: true or false 
        /// </summary>
        public bool DeleteKey(string keyName) {
            try {
                // Setting
                var rk = BaseRegistryKey;
                var sk1 = rk.CreateSubKey(NestedKey);
                // If the RegistrySubKey doesn't exists -> (true)
                if (sk1 == null) {
                    return true;
                }
                sk1.DeleteValue(keyName);

                return true;
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return false;
            }
        }

        /// <summary>
        /// To delete a sub key and any child.
        /// input: void
        /// output: true or false 
        /// </summary>
        public bool DeleteSubKeyTree() {
            try {
                // Setting
                var rk = BaseRegistryKey;
                var sk1 = rk.OpenSubKey(NestedKey);
                // If the RegistryKey exists, I delete it
                if (sk1 != null) {
                    rk.DeleteSubKeyTree(NestedKey);
                }
                return true;
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return false;
            }
        }

        /// <summary>
        /// Retrieve the count of subkeys at the current key.
        /// input: void
        /// output: number of subkeys
        /// </summary>
        public int SubKeyCount() {
            try {
                // Setting
                var rk = BaseRegistryKey;
                var sk1 = rk.OpenSubKey(NestedKey);
                // If the RegistryKey exists...
                return sk1 != null ? sk1.SubKeyCount : 0;
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return 0;
            }
        }

        /// <summary>
        /// Retrieve the count of values in the key.
        /// input: void
        /// output: number of keys
        /// </summary>
        public int ValueCount() {
            try {
                // Setting
                var rk = BaseRegistryKey;
                var sk1 = rk.OpenSubKey(NestedKey);
                // If the RegistryKey exists...
                return sk1 != null ? sk1.ValueCount : 0;
            } catch (Exception) {
                // AAAAAAAAAAARGH, an error!
                return 0;
            }
        }
    }
}
/*
 * Author: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2010
 * 
 */