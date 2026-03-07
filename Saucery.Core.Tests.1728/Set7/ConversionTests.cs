using Saucery.Core.Util;
using Shouldly;

namespace Set7;

public class ConversionTests67
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
