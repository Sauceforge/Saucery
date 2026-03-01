using NUnit.Framework;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

public class ConversionTests
{
    [Test]
    public void SanitisedLongVersionTest()
    {
        const string longVersion = "10.0.";
        var result = longVersion.EndsWith(SauceryConstants.DOT)
                        ? longVersion.Trim()[..(longVersion.Length - 1)]
                        : longVersion.Trim();
        // Optionally log the result if needed
        // Console.WriteLine($"SanitisedLongVersion returning string '{result}'");
        result.ShouldBe("10.0");
    }
}
