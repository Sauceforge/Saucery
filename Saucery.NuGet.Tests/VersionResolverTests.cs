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
}
