using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class CsprojUpdaterTests {
    private const string OptedInCsproj = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <ItemGroup>
            <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
            <PackageReference Include="Newtonsoft.Json" Version="12.0.0" />
            <PackageReference Include="Serilog" Version="2.10.0" />
          </ItemGroup>
        </Project>
        """;

    private const string NotOptedInCsproj = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <ItemGroup>
            <PackageReference Include="Newtonsoft.Json" Version="12.0.0" />
          </ItemGroup>
        </Project>
        """;

    private const string OptedInCsprojWithPackageVersion = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <PackageVersion>2.0.0</PackageVersion>
          </PropertyGroup>  
          <ItemGroup>
            <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
            <PackageReference Include="Newtonsoft.Json" Version="12.0.0" />
          </ItemGroup>
        </Project>
        """;

    [Fact]
    public async Task UpdateAsync_BumpsOwnVersion_WhenDependenciesChanged() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersion);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = new[] { "12.0.0", "13.0.0" }
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                bumpOwnVersion: true, 
                versionSegment: VersionSegment.Patch, 
                ct: ct);

            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("2.0.1", result.NewPackageVersion);
            Assert.Contains("<PackageVersion>2.0.1</PackageVersion>", await File.ReadAllTextAsync(path, ct));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_DoesNotBumpOwnVersion_WhenDependenciesUnchanged() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsprojWithPackageVersion);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = new[] { "12.0.0" }
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                bumpOwnVersion: true, 
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Empty(result.Updates);
            Assert.Null(result.NewPackageVersion);
            Assert.Contains("<PackageVersion>2.0.0</PackageVersion>", await File.ReadAllTextAsync(path, ct));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsOptedIn_ReturnsFalse_WhenNextNuGetAbsent() {
        var path = WriteTempCsproj(NotOptedInCsproj);
        
        try {
            Assert.False(CsprojUpdater.IsOptedIn(path));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_DryRun_DoesNotModifyFile() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var originalContent = await File.ReadAllTextAsync(path, ct);
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = new[] { "12.0.0", "13.0.0", "13.0.1" },
                ["Serilog"] = new[] { "2.10.0", "2.11.0", "3.0.0" }
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                dryRun: true, 
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Equal(2, result.Updates.Count);
            Assert.Equal(originalContent, await File.ReadAllTextAsync(path, ct));
        } finally {
            File.Delete(path);
        }
    }


    [Fact]
    public async Task UpdateAsync_WritesNextVersionsToFile() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = new[] { "12.0.0", "13.0.0" },
                ["Serilog"] = new[] { "2.10.0", "2.11.0" }
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(path, dryRun: false, ct: ct);
            
            Assert.True(result.Success);
            Assert.Equal(2, result.Updates.Count);
            
            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Version=\"13.0.0\"", written);
            Assert.Contains("Version=\"2.11.0\"", written);
            //OptIn Marker should remain unchanged
            Assert.Contains("Include=\"Saucery.NuGet\" Version=\"1.0.0\"", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoUpdates_AllPackagesAreLatest() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = new[] { "12.0.0" },
                ["Serilog"] = new[] { "2.10.0" }
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(path, dryRun: false, ct: ct);

            Assert.True(result.Success);
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsOptedIn_ReturnsTrue_WhenPropertyIsTrue() {
        var path = WriteTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
              </PropertyGroup>
            </Project>
            """);

        try {
            var result = CsprojUpdater.IsOptedIn(path);

            Assert.True(result);
        } finally {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("True")]
    [InlineData(" yes ")]
    [InlineData("1")]
    public void IsOptedIn_ReturnsTrue_WhenPropertyHasTruthyValue(string value) {
        var path = WriteTempCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <SauceryNuGetOptIn>{value}</SauceryNuGetOptIn>
              </PropertyGroup>
            </Project>
            """);

        try {
            var result = CsprojUpdater.IsOptedIn(path);

            Assert.True(result);
        } finally {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("0")]
    [InlineData("no")]
    [InlineData("")]
    public void IsOptedIn_ReturnsFalse_WhenPropertyHasNonTruthyValue(string value) {
        var path = WriteTempCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <SauceryNuGetOptIn>{value}</SauceryNuGetOptIn>
              </PropertyGroup>
            </Project>
            """);

        try {
            var result = CsprojUpdater.IsOptedIn(path);

            Assert.False(result);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsOptedIn_ReturnsFalse_WhenPropertyIsMissing() {
        var path = WriteTempCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        try {
            var result = CsprojUpdater.IsOptedIn(path);

            Assert.False(result);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ExcludePackage_IsNotUpdated() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                excludePackageIds: ["Serilog"], 
                dryRun: false, 
                ct: ct);

            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Newtonsoft.Json", result.Updates[0].PackageId);
            
            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Version=\"13.0.0\"", written);
            Assert.Contains("Version=\"2.10.0\"", written); // Serilog should remain unchanged
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_MultipleExcludedPackages_NoneAreUpdated() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                excludePackageIds: ["Newtonsoft.Json", "Serilog"], 
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Empty(result.Updates);
            
            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"12.0.0\"", written); // Newtonsoft.Json should remain unchanged
            Assert.Contains("Include=\"Serilog\" Version=\"2.10.0\"", written); // Serilog should remain unchanged
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_NonExcludedPackages_StillUpdate() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                excludePackageIds: ["Newtonsoft.Json"], 
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Serilog", result.Updates[0].PackageId);
            
            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"12.0.0\"", written); // Newtonsoft.Json should remain unchanged
            Assert.Contains("Include=\"Serilog\" Version=\"2.11.0\"", written); // Serilog should be updated
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UdateAsync_ExcludeIsCaseInsensitive() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                excludePackageIds: ["NEWTONSOFT.JSON"],
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Serilog", result.Updates[0].PackageId);
            
            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"12.0.0\"", written); // Newtonsoft.Json should remain unchanged
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_EmptyExcludeList_HasNoEffect() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj(OptedInCsproj);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new CsprojUpdater(apiClient);
            var result = await updater.UpdateAsync(
                path, 
                excludePackageIds: null, 
                ct: ct);
            
            Assert.True(result.Success);
            Assert.Equal(2, result.Updates.Count);
            
            //var written = await File.ReadAllTextAsync(path, ct);
            //Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"13.0.0\"", written); // Newtonsoft.Json should be updated
            //Assert.Contains("Include=\"Serilog\" Version=\"2.11.0\"", written); // Serilog should be updated
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_PerPackageVersionsBehind_CapsThatPackage_WhileSiblingAdvances() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
                <PackageReference Include="Newtonsoft.Json" Version="13.0.0" VersionsBehind="1" />
                <PackageReference Include="Serilog" Version="2.10.0" />
              </ItemGroup>
            </Project>
            """);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["13.0.0", "14.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new CsprojUpdater(apiClient);
            //No CLI ceiling (versionBehindLatest defaults to null).
            var result = await updater.UpdateAsync(path, ct: ct);

            Assert.True(result.Success);
            // Newtonsoft.Json is one behind latest (14.0.0); VersionsBehind="1" puts the
            // ceiling at 13.0.0, so it is already at the ceiling and is skipped. Only Serilog updates.
            Assert.Single(result.Updates);
            Assert.Equal("Serilog", result.Updates[0].PackageId);

            var written = await File.ReadAllTextAsync(path, ct);
            // Newtonsoft.Json unchanged, and the attribute is preserved.
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"13.0.0\" VersionsBehind=\"1\"", written);
            // Serilog has no attribute and no CLI ceiling -> advances one step.
            Assert.Contains("Include=\"Serilog\" Version=\"2.11.0\"", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_PerPackageVersionsBehindZero_OverridesGlobalCli_AndUpdates() {
        var ct = new CancellationToken();
        var path = WriteTempCsproj("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
                <PackageReference Include="Newtonsoft.Json" Version="12.0.0" VersionsBehind="0" />
              </ItemGroup>
            </Project>
            """);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"]
            });

            var updater = new CsprojUpdater(apiClient);
            //A global ceiling of 2 alone would skip this package (ceiling index < 0),
            // but the per-package VersionsBehind="0" overrides it and allows the update.
            var result = await updater.UpdateAsync(path, versionsBehindLatest: 2, ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Newtonsoft.Json", result.Updates[0].PackageId);
            Assert.Equal("13.0.0", result.Updates[0].ToVersion);

            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"13.0.0\" VersionsBehind=\"0\"", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void CollectVersionsBehindOverrides_ReadsAttributes_AndMostConservativeWins() {
        // CPM-style references (no Version attribute), plus one with a Version attribute.
        var projectA = WriteTempCsproj("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" VersionsBehind="1" />
                <PackageReference Include="Serilog" VersionsBehind="3" />
                <PackageReference Include="NoOverridePackage" />
              </ItemGroup>
            </Project>
            """);
        var projectB = WriteTempCsproj("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" VersionsBehind="2" />
                <PackageReference Include="Bogus" Version="34.0.0" VersionsBehind="0" />
                <PackageReference Include="Invalid" VersionsBehind="abc" />
              </ItemGroup>
            </Project>
            """);

        try {
            var map = CsprojUpdater.CollectVersionsBehindOverrides([projectA, projectB]);

            Assert.Equal(2, map["Newtonsoft.Json"]); // most conservative (largest N) across projects
            Assert.Equal(3, map["Serilog"]);
            Assert.Equal(0, map["Bogus"]);
            Assert.False(map.ContainsKey("NoOverridePackage")); // no attribute
            Assert.False(map.ContainsKey("Invalid"));           // invalid value ignored
        } finally {
            File.Delete(projectA);
            File.Delete(projectB);
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
