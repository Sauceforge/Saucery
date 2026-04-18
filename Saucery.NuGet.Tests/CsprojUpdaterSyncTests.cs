using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class CsprojUpdaterSyncTests {
    private const string OptedInCsprojWithPackageVersionAndDependency = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <PackageVersion>1.0.0</PackageVersion>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
            <PackageReference Include="TUnit" Version="2.0.0" />
          </ItemGroup>
        </Project>
        """;

    [Fact]
    public async Task UpdateAsync_SyncWithDependency_UsesUpdatedDependencyVersion_WhenDependencyBumped() {
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersionAndDependency);
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["TUnit"] = ["2.0.0", "3.0.0"]
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Equal("3.0.0", result.NewPackageVersion);

            Assert.Equal(2, result.Updates.Count);

            var dependencyUpdate = Assert.Single(result.Updates, x => x.PackageId == "TUnit");
            Assert.Equal("2.0.0", dependencyUpdate.FromVersion);
            Assert.Equal("3.0.0", dependencyUpdate.ToVersion);

            var packageVersionUpdate = Assert.Single(result.Updates, x => x.PackageId == "PackageVersion");
            Assert.Equal("1.0.0", packageVersionUpdate.FromVersion);
            Assert.Equal("3.0.0", packageVersionUpdate.ToVersion);

            var written = await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
            Assert.Contains("<PackageVersion>3.0.0</PackageVersion>", written);
            Assert.Contains("""<PackageReference Include="TUnit" Version="3.0.0" />""", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithDependency_SyncsEvenWhenDependencyUnchanged() {
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersionAndDependency);
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["TUnit"] = ["2.0.0"]
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path,
                syncWithPackageId: "TUnit",
                ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Equal("2.0.0", result.NewPackageVersion);

            var update = Assert.Single(result.Updates);
            Assert.Equal("PackageVersion", update.PackageId);
            Assert.Equal("1.0.0", update.FromVersion);
            Assert.Equal("2.0.0", update.ToVersion);

            var written = await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
            Assert.Contains("<PackageVersion>2.0.0</PackageVersion>", written);
            Assert.Contains("""<PackageReference Include="TUnit" Version="2.0.0" />""", written);
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
        public Task<IReadOnlyList<string>> GetAvailableVersionsAsync(string packageId, CancellationToken cancellationToken = default) {
            var versions = data.TryGetValue(packageId, out var list) ? list : [];
            return Task.FromResult<IReadOnlyList<string>>(versions);
        }
    }
}