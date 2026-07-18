using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

/// <summary>
/// Tests covering --sync-with behavior in a CPM (Central Package Management) solution,
/// where PackageReference elements carry no Version attribute, and the version is resolved
/// via externalResolvedVersions (built from Directory.Packages.props)
/// </summary>
public class CsprojUpdaterCpnSyncTests {
    // CPM-style project: PackageReference has no Version attribute
    private const string CpmOptedInCsproj = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
            <PackageVersion>1.0.0</PackageVersion>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="TUnit" />
            <PackageReference Include="Newtonsoft.Json" />
          </ItemGroup>
        </Project>
        """;

    [Fact]
    public async Task UpdateAsync_SyncWithExternalVersion_UsesResolvedVersionFromProps() {
        var path = WriteTempCsproj(CpmOptedInCsproj);
        try {
            var apiClient = new StubNuGetApiClient([]);

            var externalVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["TUnit"] = "3.0.0"
            };

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                externalResolvedVersions: externalVersions,
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Equal("3.0.0", result.NewPackageVersion);

            var written = await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
            Assert.Contains("<PackageVersion>3.0.0</PackageVersion>", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithExternalVersion_DryRun_DoesNotWriteFile() {
        var path = WriteTempCsproj(CpmOptedInCsproj);
        try {
            var originalContent = await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
            var apiClient = new StubNuGetApiClient([]);

            var externalVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["TUnit"] = "3.0.0"
            };

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                externalResolvedVersions: externalVersions,
                dryRun: true,
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Equal("3.0.0", result.NewPackageVersion);
            Assert.Equal(originalContent, await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken));

        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithExternalVersion_AlreadyCurrent_NoUpdate() {
        var path = WriteTempCsproj(CpmOptedInCsproj);
        try {
            var apiClient = new StubNuGetApiClient([]);

            // External versuion matches the current <PackageVersion> in the csproj
            var externalVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["TUnit"] = "1.0.0"
            };

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                externalResolvedVersions: externalVersions,
                ct: TestContext.Current.CancellationToken);
            
            Assert.True(result.Success);
            Assert.Null(result.NewPackageVersion);
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithExternalVersion_NotFound_NoSync() {
        var path = WriteTempCsproj(CpmOptedInCsproj);
        try {
            var apiClient = new StubNuGetApiClient([]);

            // External version does not contain the sync-with package
            var externalVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["SomeOtherPackage"] = "5.0.0"
            };
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                externalResolvedVersions: externalVersions,
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Null(result.NewPackageVersion);
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithExternalVersion_IsCaseInsensitive() {
        var path = WriteTempCsproj(CpmOptedInCsproj);
        try {
            var apiClient = new StubNuGetApiClient([]);

            // External version has different casing than the sync-with package
            var externalVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["TUNIT"] = "3.0.0"
            };

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "tunit",
                externalResolvedVersions: externalVersions,
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Equal("3.0.0", result.NewPackageVersion);
        } finally {
            File.Delete(path);
        }
    }

    private static string WriteTempCsproj(string content) {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csproj");
        File.WriteAllText(path, content);
        return path;
    }

    private sealed class StubNuGetApiClient(Dictionary<string, string[]> data) : INuGetApiClient {
        public Task<IReadOnlyList<string>> GetAvailableVersionsAsync(string packageId, CancellationToken ct) {
            var versions = data.TryGetValue(packageId, out var v) ? v : Array.Empty<string>();
            return Task.FromResult((IReadOnlyList<string>)versions);
        }
    }
}
