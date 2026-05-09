using Xunit;

namespace Saucery.Badges.Tests;

public sealed class BadgeDownloadFormatterTests {
    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(999, "999")]
    public void FormatDownloadTotal_WhenBelowOneThousand_ReturnsRawNumber(
        long total,
        string expected) {
        var actual = BadgeDownloadFormatter.FormatDownloadTotal(total);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1_000, "1K")]
    [InlineData(1_049, "1K")]
    [InlineData(1_050, "1.1K")]
    [InlineData(12_300, "12.3K")]
    [InlineData(12_349, "12.3K")]
    [InlineData(12_350, "12.4K")]
    [InlineData(999_949, "999.9K")]
    [InlineData(999_950, "1000K")]
    public void FormatDownloadTotal_WhenThousands_ReturnsKWithOneDecimalWhenNeeded(
        long total,
        string expected) {
        var actual = BadgeDownloadFormatter.FormatDownloadTotal(total);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1_000_000, "1M")]
    [InlineData(1_049_999, "1M")]
    [InlineData(1_050_000, "1.1M")]
    [InlineData(1_500_000, "1.5M")]
    [InlineData(12_345_678, "12.3M")]
    [InlineData(12_350_000, "12.4M")]
    [InlineData(27_800_000, "27.8M")]
    public void FormatDownloadTotal_WhenMillions_ReturnsMWithOneDecimalWhenNeeded(
        long total,
        string expected) {
        var actual = BadgeDownloadFormatter.FormatDownloadTotal(total);

        Assert.Equal(expected, actual);
    }
}