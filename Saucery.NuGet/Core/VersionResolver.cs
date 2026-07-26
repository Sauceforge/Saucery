using NuGet.Versioning;

namespace Saucery.NuGet.Core;

public static class VersionResolver {
    public static string? FindNextVersion(
        string currentVersion, 
        IReadOnlyList<string> availableVersions, 
        bool includePrerelease = false,
        int? versionsBehindLatest = null) 
    {
        if(!NuGetVersion.TryParse(currentVersion, out var current)) {
            return null;
        }

        var parsed = availableVersions
            .Select(raw => NuGetVersion.TryParse(raw, out var v) ? v : null)
            .Where(v => v is not null && (includePrerelease || !v.IsPrerelease))
            .Select(v => v!)
            .OrderBy(v => v)
            .ToList();

        NuGetVersion? ceiling = null;
        if(versionsBehindLatest.HasValue) {
            var ceilingIndex = parsed.Count - 1 - versionsBehindLatest.Value;
            if(ceilingIndex < 0) {
                return null;
            }

            ceiling = parsed[ceilingIndex];

            if(current >= ceiling) {
                return null;
            }
        }

        NuGetVersion? best = null;
        foreach(var candidate in parsed) 
        {
            if(candidate <= current) {
                continue;
            }

            if(ceiling is not null && candidate > ceiling) {
                continue;
            }

            if(best is null || candidate < best) {
                best = candidate;
            }
        }

        return best?.ToNormalizedString();
    }
}
