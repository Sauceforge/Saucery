using NUnit.Framework;
using Shouldly;
using System;

namespace UnitTests
{
    [TestFixture]
    class ConversionTests
    {
        [Test]
        public void SanitisedLongVersionTest()
        {
            var longVersion = "10.0.";
            var result = longVersion.EndsWith(SauceryConstants.DOT)
                            ? longVersion.Trim().Remove(longVersion.Length - 1)
                            : longVersion.Trim();
            Console.WriteLine("SanitisedLongVersion returning string '{0}'", result);
            result.ShouldBe("10.0");
        }
    }
}
