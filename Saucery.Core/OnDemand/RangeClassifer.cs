namespace Saucery.Core.OnDemand;

public class RangeClassifer
{
    internal static PlatformRange Classify(string[] requestedVersions)
    {
        var lowerBound = requestedVersions[0];
        var upperBound = requestedVersions[1];

        return requestedVersions.Length != 2
            ? PlatformRange.Invalid
            : int.TryParse(lowerBound, out var _) && int.TryParse(upperBound, out var _)
                ? PlatformRange.NumericOnly
                : !int.TryParse(lowerBound, out var _) && !int.TryParse(upperBound, out var _)
                    ? PlatformRange.NonNumericOnly
                    : int.TryParse(lowerBound, out var _) && !int.TryParse(upperBound, out var _)
                        ? PlatformRange.NumericNonNumeric
                        : PlatformRange.Invalid;
    }
}
