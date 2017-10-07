using System.Security.Principal;
using Saucery2.Util;

namespace Saucery2.Registry {
    internal class RegistryHelper {
        readonly static ModifyRegistry RegModifier = new ModifyRegistry(IsAdministrator());

        public static bool IsAlreadyActivated() {
            #if DEBUG
                var key = RegModifier.Read(SauceryConstants.KEYNAME);
                return key != null && (!key.Equals(string.Empty) && !string.IsNullOrEmpty(key));
                //return true;
            #else
                var key = RegModifier.Read(SauceryConstants.KEYNAME);
                return key != null && (!key.Equals(string.Empty) && !string.IsNullOrEmpty(key));
#endif
        }

        public static void StoreActivation(string publicKey) {
            RegModifier.Write(SauceryConstants.KEYNAME, publicKey);
        }

        public static bool IsAdministrator() {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
/*
 * Author: Andrew Gray, Full Circle Solutions
 * Date: 22nd September 2010
 * 
 */