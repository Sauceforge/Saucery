using System.Globalization;

namespace Saucery.Badges;

public static class BadgeDownloadFormatter {
    public static string FormatDownloadTotal(long total) {
        return total switch {
            >= 1_000_000 => $"{TrimTrailingZero(total / 1_000_000d)}M",
            >= 1_000 => $"{TrimTrailingZero(total / 1_000d)}K",
            _ => total.ToString(CultureInfo.InvariantCulture)
        };
    }

    private static string TrimTrailingZero(double value) {
        return value
            .ToString("0.0", CultureInfo.InvariantCulture)
            .Replace(".0", "", StringComparison.Ordinal);
    }
}
