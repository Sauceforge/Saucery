using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

public class ConversionTests
{
    [Fact]
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
