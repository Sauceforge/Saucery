using NuGet.Versioning;

namespace Saucery.NuGet.Core;

public static class VersionResolver {
    public static string? FindNextVersion(
        string currentVersion, 
        IReadOnlyList<string> availableVersions, 
        bool includePrerelease = false) 
    {
        if(!NuGetVersion.TryParse(currentVersion, out var current))
            return null;

        NuGetVersion? best = null;
        
        foreach(var raw in availableVersions) 
        {
            if(!NuGetVersion.TryParse(raw, out var candidate))
                continue;

            if(!includePrerelease && candidate.IsPrerelease)
                continue;

            if(candidate > current && (best is null || candidate < best)) 
                best = candidate;
        }

        return best?.ToNormalizedString();
    }
}
