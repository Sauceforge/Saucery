using NUnit.Framework;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests;

[TestFixture]
public class ConversionTests
{
    [Test]
    public void SanitisedLongVersionTest()
    {
        const string longVersion = "10.0.";
        var result = longVersion.EndsWith(SauceryConstants.DOT)
                        ? longVersion.Trim().Remove(longVersion.Length - 1)
                        : longVersion.Trim();
        Console.WriteLine("SanitisedLongVersion returning string '{0}'", result);
        result.ShouldBe("10.0");
    }
}
