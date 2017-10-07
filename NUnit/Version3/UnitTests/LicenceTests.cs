using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Saucery3.Activation;

namespace UnitTests {
    [TestFixture]
    public class LicenceTests : TestBase {
        private const string Name = "Andrew Gray";
        private const string Company = "Full Circle";
        private const string Email = "andrew.paul.gray@gmail.com";
        //private const string NUnit2Product = "N2";
        private const string NUnit3Product =  "N3";
        //private const string JUnit4Product = "J4";
        private static readonly int[] Nunit3PosArray = {3, 6, 9, 12, 15, 18, 21, 24, 27, 30};
        //private static readonly int[] Junit4PosArray = {4, 8, 12, 16, 20, 24, 28, 32, 36, 40};
        private static readonly DateTime DateNow = DateTime.Now;
        private const string RawInput = Name + Company + Email;
        private const string PublicKey = "BwIAAACkAABSU0EyAAQAAAEAAQDtZrs0gFK/FqIRSFYO8wqbwWUqk0Vul1ueiuOTdQrdkRy4bPT0Z8FTOWpAwlJXyxeJ+WgQmJlITEcb7VH0cICpJeWYPCGACUkqibDUGvNS/shLg4OoxPxP7u8kDnomqCSxhH5W0HXVrY73U3mq9tTlVVLvGqTbBG2/JMwnRSk70jURWrm1Py/vsMnluJU5kGUcbgFwF/ALCYBEttrmhavdgixswTfPT3LUg8IK44vKpVkSqegav+LpZlxwVvHLZvjZrfo8O3ZccC+XdFp+CdJdDbJ8KC8amqWz5hgeSciHzq2JRKppNhOslQQ/wDoxUJP7HDST4zgFf1UzGWHMdKnYAdoadFfCQ8kmHlmxEXWkVud9sZkG6fJbouTH2lCG0Ad5Z2URkgZJHhUTanbHqzzi8uS7AEM7DuIEwO99IpACEHlr8Il+Rbz7xIq3b/u8V7bhORoua1jmX5Wg6Du8zdpziWUxR+Yg53/A0BkWcgBci24Tun1PoDKuzK8yHPZiSEVYmX/bw6Htbzi+XyFNGF1TMfJaohhHi421MSzZMz+qKwSPQ+Ps0QJxPfvi/SDHUtlnkhmKEQ0gq5uu2cFIgjKUIb+S1NYJF3yW4oIiVBfrrBXx2wXkbRgDHlTPyrOXuFhzpjgMGjonEYMfHUSqFshObg6Go3r50jJQQDn7b230fhdtanz9M24raQRTlrC5Q6eEN0TqrwiTQoxpxa75mUHfd7YpHnButRC8YjrjdniHaxYSdk54TV3FPyN9niZfHzo=";

        [Test]
        public void AcceptanceTest() {
            const string rawInput = Name + Company + Email;
            var dateNowString = DateTime.Now.ToString("ddMMyyyy");
            var productAndDate = NUnit3Product + dateNowString;
            var activationKey = InsertProductAndDate(productAndDate, Nunit3PosArray, rawInput.ComputeHash());
            const string myGuid = "d010fe00-bd6a-4c94-85ff-da9ba45b769d";
            
            Assert.That(Hasher.VerifyIdentity(rawInput, activationKey));

            var encryptedLicenceBytes = ReversibleEncryption.Encrypt(PublicKey, activationKey + myGuid);
            var writableLicence = encryptedLicenceBytes.AsHex();

            Assert.That(FileHelper.LicenceStringIsValidContent(writableLicence));

        }

        [Test]
        public void LicenceStringIsValidContent() {
            const string licence = "0C01614ADE00BC13BEFF530C5D93E1B10CD917B35FE98A94BA7738159BEC679ACF0C5C68D5C7F1AB91AA85D5E8CA37345B291547E76A5147D615EF23C25422D269F8A202F7EBE394D83292F764CFA83C7D994D02DA6A278BE7F2A7A869947D99F2B32A6E4FC3A8713E755339E8DE357FB86E6883645BA37DB8E16170B4B24308";
            var writtenActivationKeyGS = ReversibleEncryption.Decrypt(PublicKey, licence.AsSauceryBytes());
            var writtenGS = writtenActivationKeyGS.ExtractGuidSalt();
            const string expectedGuidSalt = "d010fe00-bd6a-4c94-85ff-da9ba45b769d";

            var sauceryActivationKey = writtenActivationKeyGS.Remove(writtenActivationKeyGS.Length - 36);
            Assert.That(Hasher.ProductAndDateIsValid(sauceryActivationKey) &&
                   StringComparer.OrdinalIgnoreCase.Compare(expectedGuidSalt, writtenGS) == 0);
        }

        [Test]
        public void DateIsTooOldTest() {
            var dateFromYearsAgo = new DateTime(2010, 01, 01);
            Assert.That((DateTime.Now - dateFromYearsAgo).TotalDays > 366);
        }

        [Test]
        public void DateIsNotTooOldTest() {
            var dateThisYear = new DateTime(2015, 01, 01);
            Assert.That((DateTime.Now - dateThisYear).TotalDays < 366);
        }

        [Test]
        public void WrittenLicenceProductAndDateCheck() {
            const string writtenHashGS = "4ccNd137b1149dc02f61329201513256d47ba5895f8051f85b" + "d010fe00-bd6a-4c94-85ff-da9ba45b769d";
            var sauceryActivationKey = writtenHashGS.Remove(writtenHashGS.Length - 36);

            Assert.That(Hasher.ProductAndDateIsValid(sauceryActivationKey));
        }

        [Test]
        public void GenerationTest() {
            var hash = RawInput.ComputeHash();
            var dateString = DateNow.ToString("ddMMyyyy");
            var hashAndProductAndDate = InsertProductAndDate(NUnit3Product + dateString, Nunit3PosArray, hash);
            var guidSalt = FileHelper.GetGuid();

            var elb = ReversibleEncryption.Encrypt(PublicKey, hashAndProductAndDate + guidSalt);
            var writableLicence = elb.AsHex();

            //Start undoing what we did
            var elbAfterAsHexAsBytes = writableLicence.AsBytes();
            Assert.That(StructuralComparisons.StructuralEqualityComparer.Equals(elb, elbAfterAsHexAsBytes));

            var decryptedLicenceBytes = ReversibleEncryption.Decrypt(PublicKey, elbAfterAsHexAsBytes);
            Assert.That(decryptedLicenceBytes.Equals(hashAndProductAndDate + guidSalt));

            var restoredHashAndProductAndDate = decryptedLicenceBytes.Remove(decryptedLicenceBytes.Length - 36);
            Assert.That(restoredHashAndProductAndDate.Equals(hashAndProductAndDate));

            var restoredHash = ExtractProductAndDate(Nunit3PosArray, restoredHashAndProductAndDate);
            Assert.That(restoredHash[1].Equals(hash));
            Assert.That(restoredHash[0].Equals(NUnit3Product + dateString));
        }

        //From FCSLicenceService
        private static string InsertProductAndDate(string dataToInsert, IReadOnlyList<int> posArray, string input) {
            for (var i = 0; i < posArray.Count; i++) {
                var inputPosition = posArray[i];
                input = input.Insert(inputPosition, dataToInsert[i].ToString());
            }
            return input;
        }

        private static string[] ExtractProductAndDate(IReadOnlyList<int> posArray, string input) {
            var sb = new StringBuilder();

            //Reverses InsertProductAndDate
            for (var i = 0; i < posArray.Count; i++) {
                var extractPosition = (i == 0 ? posArray[i] : posArray[i] - i);
                sb.Append(input[extractPosition]);
                input = input.Remove(extractPosition, 1);
            }
            var retVals = new List<string> {sb.ToString(), input};
            return retVals.ToArray();
        }
    }
}