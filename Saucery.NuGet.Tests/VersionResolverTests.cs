using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class VersionResolverTests {
    [Fact]
    public void FindNextVersion_ReturnsSmallestVersionAboveCurrent() {
        var versions = new[] { "1.0.0", "1.1.0", "1.2.0", "2.0.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions);
        Assert.Equal("1.1.0", result);
    }

    [Fact]
    public void FindNextVersion_ReturnsNull_WhenNoHigherVersionExists() {
        var versions = new[] { "1.0.0", "0.9.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_NoCeiling_StepsToLatestOverMultipleRuns() {
        // Simulates repeated runs: without --versions-behind each run advances one step,
        // and there is nothing preventing the sequence from reaching the latest version.
        var versions = new[] { "1.0.0", "1.0.1", "1.0.2", "1.0.3" };
        var current = "1.0.0";
        var steps  = new List<string>();
        string? next;
        while((next = VersionResolver.FindNextVersion(current, versions)) is not null) {
            steps.Add(next);
            current = next;
        }
        Assert.Equal(["1.0.1", "1.0.2", "1.0.3"], steps);
    }

    [Fact]
    public void FindNextVersion_NoCeiling_ReturnsNull_WhenAlreadyAtLatest() {
        var versions = new[] { "1.0.0", "1.0.1", "1.0.2" };
        var result = VersionResolver.FindNextVersion("1.0.2", versions);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_ExcludesPrerelease_ByDefault() {
        var versions = new[] { "1.0.0", "1.1.0-alpha", "1.1.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions, includePrerelease: false);
        Assert.Equal("1.1.0", result);
    }

    [Fact]
    public void FindNextVersion_IncludesPrerelease_WhenFlagIsSet() {
        var versions = new[] { "1.0.0", "1.1.0-alpha", "1.1.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions, includePrerelease: true);
        Assert.Equal("1.1.0-alpha", result);
    }

    [Fact]
    public void FindNextVersion_ReturnsNull_ForUnparsableCurrentVersion() {
        var versions = new[] { "1.0.0", "2.0.0" };
        var result = VersionResolver.FindNextVersion("not-a-version", versions);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_ReturnsNull_ForEmptyAvailableVersions() {
        var result = VersionResolver.FindNextVersion("1.0.0", []);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_SkipUnparsableVersionsInList() {
        var versions = new[] { "1.0.0", "bad-version", "1.1.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions);
        Assert.Equal("1.1.0", result);
    }

    // --- versionsBehindlatest ceiling tests ---

    [Fact]
    public void FindNextVersion_WithVersionsBehind_StopsAtCeiling() {
        // Latest = 1.0.3 (index 3), N=2 -> ceiling = index 1 = 1.0.1
        var versions = new [] { "1.0.0", "1.0.1", "1.0.2", "1.0.3" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions, versionsBehindLatest: 2);
        Assert.Equal("1.0.1", result);
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehind_DoesNothing_WhenAlreadyAtCeiling() {
        // Latest = 1.0.3 (index 3), N=2 -> ceiling = 1.0.1; current = 1.0.1 -> do nothing
        var versions = new[] { "1.0.0", "1.0.1", "1.0.2", "1.0.3" };
        var result = VersionResolver.FindNextVersion("1.0.1", versions, versionsBehindLatest: 2);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehind_DoesNothing_WhenAboveCeiling() {
        // Current = 1.0.2, ceiling = 1.0.1 -> current > ceiling -> do nothing
        var versions = new[] { "1.0.0", "1.0.1", "1.0.2", "1.0.3" };
        var result = VersionResolver.FindNextVersion("1.0.2", versions, versionsBehindLatest: 2);
        Assert.Null(result);
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehind_NonContiguousVersions() {
        // Available: 1.5 -> 1.6.28 -> 1.6.49 -> 1.6.99 -> 1.7; N=2 -> ceiling = index 2 = 1.6.49
        var versions = new[] { "1.5", "1.6.28", "1.6.49", "1.6.99", "1.7" };
        var result = VersionResolver.FindNextVersion("1.5", versions, versionsBehindLatest: 2);
        Assert.Equal("1.6.28", result);
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehind_NonContiguousVersions_CeilingIsNext() {
        // Current = 1.6.28, ceiling = 1.6.49 -> upgrade to ceiling
        var versions = new[] { "1.5", "1.6.28", "1.6.49", "1.6.99", "1.7" };
        var result = VersionResolver.FindNextVersion("1.6.28", versions, versionsBehindLatest: 2);
        Assert.Equal("1.6.49", result);
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehindZero_BehavesLikeNoCeiling() {
        // N=0 -> ceiling = latest -> standard behaviour
        var versions = new[] { "1.0.0", "1.1.0", "1.2.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions, versionsBehindLatest: 0);
        Assert.Equal("1.1.0", result); 
    }

    [Fact]
    public void FindNextVersion_WithVersionsBehind_ReturnsNull_WhenNExceedsAvailableCount() {
        // Only 2 versions: N=5 -> no valid calling index
        var versions = new[] { "1.0.0", "1.1.0" };
        var result = VersionResolver.FindNextVersion("1.0.0", versions, versionsBehindLatest: 5);
        Assert.Null(result);
    }
}
