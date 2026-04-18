using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class CsprojUpdaterProjectReferenceSyncTests
{
    [Fact]
    public async Task UpdateAsync_SyncWithProjectReference_UsesReferencedProjectPackageVersion()
    {
        var root = Path.Combine(Path.GetTempPath(), $"testproj_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try {
            var coreDir = Path.Combine(root, "Saucery.Core");
            var appDir = Path.Combine(root, "Saucery");
            Directory.CreateDirectory(coreDir);
            Directory.CreateDirectory(appDir);

            var coreCsproj = Path.Combine(coreDir, "Saucery.Core.csproj");
            var appCsproj = Path.Combine(appDir, "Saucery.csproj");

            var coreContent = """
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <PackageVersion>4.5.6</PackageVersion>
              </PropertyGroup>
            </Project>
            """;

            var appContent = """
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <PackageVersion>1.0.0</PackageVersion>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
                <ProjectReference Include="..\Saucery.Core\Saucery.Core.csproj" />
              </ItemGroup>
            </Project>
            """;

            File.WriteAllText(coreCsproj, coreContent);
            File.WriteAllText(appCsproj, appContent);

            var apiClient = new StubNuGetApiClient([]);
            var updater = new CsprojUpdater(apiClient);

            var result = await updater.UpdateAsync(appCsproj, syncWithPackageId: "Saucery.Core", ct: TestContext.Current.CancellationToken);

            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Equal("4.5.6", result.NewPackageVersion);

            var written = await File.ReadAllTextAsync(appCsproj, TestContext.Current.CancellationToken);
            Assert.Contains("<PackageVersion>4.5.6</PackageVersion>", written);
        } finally {
            try { Directory.Delete(root, true); } catch { }
        }
    }

    [Fact]
    public async Task UpdateAsync_SyncWithProjectReference_UsesUpdatedReferencedProject_WhenProcessedEarlier()
    {
        var root = Path.Combine(Path.GetTempPath(), $"testproj_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try {
            var coreDir = Path.Combine(root, "Saucery.Core");
            var appDir = Path.Combine(root, "Saucery");
            Directory.CreateDirectory(coreDir);
            Directory.CreateDirectory(appDir);

            var coreCsproj = Path.Combine(coreDir, "Saucery.Core.csproj");
            var appCsproj = Path.Combine(appDir, "Saucery.csproj");

            var coreContent = """
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <PackageVersion>1.0.0</PackageVersion>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="12.0.0" />
              </ItemGroup>
            </Project>
            """;

            var appContent = """
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <PackageVersion>1.0.0</PackageVersion>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
                <ProjectReference Include="..\Saucery.Core\Saucery.Core.csproj" />
              </ItemGroup>
            </Project>
            """;

            File.WriteAllText(coreCsproj, coreContent);
            File.WriteAllText(appCsproj, appContent);

            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                { "Newtonsoft.Json", new[] { "12.0.0", "13.0.0" } }
            });
            var updater = new CsprojUpdater(apiClient);

            var coreResult = await updater.UpdateAsync(coreCsproj, bumpOwnVersion: true, ct: TestContext.Current.CancellationToken);
            Assert.True(coreResult.Success);
            Assert.NotNull(coreResult.NewPackageVersion);

            var appResult = await updater.UpdateAsync(appCsproj, syncWithPackageId: "Saucery.Core", ct: TestContext.Current.CancellationToken);
            Assert.True(appResult.Success);
            Assert.Equal(coreResult.NewPackageVersion, appResult.NewPackageVersion);

            var written = await File.ReadAllTextAsync(appCsproj, TestContext.Current.CancellationToken);
            Assert.Contains($"<PackageVersion>{coreResult.NewPackageVersion}</PackageVersion>", written);
        } finally {
            try { Directory.Delete(root, true); } catch { }
        }
    }

    private sealed class StubNuGetApiClient(Dictionary<string, string[]> data) : INuGetApiClient {
        public Task<IReadOnlyList<string>> GetAvailableVersionsAsync(string packageId, CancellationToken cancellationToken = default) {
            var versions = data.TryGetValue(packageId, out var list) ? list : [];
            return Task.FromResult<IReadOnlyList<string>>(versions);
        }
    }
}