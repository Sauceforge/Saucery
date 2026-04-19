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
