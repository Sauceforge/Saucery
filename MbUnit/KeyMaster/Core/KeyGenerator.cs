using System;
using System.Security.Cryptography;
using Saucery.Activation;

namespace KeyMaster.Core {
    public static class KeyGenerator {
        private static Tuple<string, string> CreateKeyPair() {
            var cspParams = new CspParameters {ProviderType = 1};
            var rsaProvider = new RSACryptoServiceProvider(1024, cspParams);
            var publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            var privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return new Tuple<string, string>(privateKey, publicKey);
        }

        public static string GetSymmetricKey() {
            var pair = CreateKeyPair();
            var privateKey = pair.Item1;

            const string testString = "The quick brown fox jumped over the lazy dog.";

            var encrypted = ReversibleEncryption.Encrypt(privateKey, testString);
            var decrypted = ReversibleEncryption.Decrypt(privateKey, encrypted);

            return decrypted.Equals(testString) ? privateKey : null;
        }
    }
}