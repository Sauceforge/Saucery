namespace Saucery.Core.OnDemand;

public class RangeClassifer
{
    internal static PlatformRange Classify(string[] requestedVersions)
    {
        string lowerBound = requestedVersions[0];
        string upperBound = requestedVersions[1];

        return requestedVersions.Length != 2
            ? PlatformRange.Invalid
            : int.TryParse(lowerBound, out int _) && int.TryParse(upperBound, out int _)
                ? PlatformRange.NumericOnly
                : !int.TryParse(lowerBound, out int _) && !int.TryParse(upperBound, out int _)
                    ? PlatformRange.NonNumericOnly
                    : int.TryParse(lowerBound, out int _) && !int.TryParse(upperBound, out int _)
                        ? PlatformRange.NumericNonNumeric
                        : PlatformRange.Invalid;
    }
}
