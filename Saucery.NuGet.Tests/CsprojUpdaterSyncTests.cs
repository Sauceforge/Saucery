using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class CsprojUpdaterSyncTests
{
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
    public async Task UpdateAsync_SyncWithDependency_UsesUpdatedDependencyVersion_WhenDependencyBumped()
    {
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersionAndDependency);
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["TUnit"] = new[] { "2.0.0", "3.0.0" }
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(path, syncWithPackageId: "TUnit");

            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("3.0.0", result.NewPackageVersion);
            var written = await File.ReadAllTextAsync(path);
            Assert.Contains("<PackageVersion>3.0.0</PackageVersion>", written);
            Assert.Contains("Version=\"3.0.0\"", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithDependency_SyncsEvenWhenDependencyUnchanged()
    {
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersionAndDependency);
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["TUnit"] = new[] { "2.0.0" }
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(path, syncWithPackageId: "TUnit");

            Assert.True(result.Success);
            Assert.Empty(result.Updates);
            Assert.Equal("2.0.0", result.NewPackageVersion);
            var written = await File.ReadAllTextAsync(path);
            Assert.Contains("<PackageVersion>2.0.0</PackageVersion>", written);
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
            var versions = data.TryGetValue(packageId, out var list) ? list : Array.Empty<string>();
            return Task.FromResult<IReadOnlyList<string>>(versions);
        }
    }
}
