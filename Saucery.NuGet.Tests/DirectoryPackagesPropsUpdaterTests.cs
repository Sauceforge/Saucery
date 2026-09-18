using Saucery.NuGet.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saucery.NuGet.Tests;

public class DirectoryPackagesPropsUpdaterTests {
    private const string SimplePropsContent = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project>
          <ItemGroup>
            <PackageVersion Include="Newtonsoft.Json" Version="12.0.0" />
            <PackageVersion Include="Serilog" Version="2.10.0" />
          </ItemGroup>
        </Project>
        """;

    [Fact]
    public async Task UpdateAsync_UpdatesPackageVersionElements() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, ct: ct);

            Assert.True(result.Success);
            Assert.Equal(2, result.Updates.Count);

            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Version=\"13.0.0\"", written);
            Assert.Contains("Version=\"2.11.0\"", written);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_DryRun_DoesNotModifyFile() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);

        try {
            var originalContent = await File.ReadAllTextAsync(path, ct);
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, dryRun: true, ct: ct);
            
            Assert.True(result.Success);
            Assert.Equal(2, result.Updates.Count);
            Assert.Equal(originalContent, await File.ReadAllTextAsync(path, ct));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoUpdates_WhenAllPackagesAreLatest() { 
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0"],
                ["Serilog"] = ["2.10.0"]
            });
            
            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, ct: ct);
            
            Assert.True(result.Success);
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_RespectsCliExcludePackages() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, excludePackageIds: ["Serilog"], ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Newtonsoft.Json", result.Updates[0].PackageId);

            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Version=\"13.0.0\"", written); // Newtonsoft.Json should not be updated
            Assert.Contains("Version=\"2.10.0\"", written); // Serilog should be updated
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_RespectsPerFileExclusions() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project>
              <ItemGroup>
                <SauceryNuGetExclude>Serilog</SauceryNuGetExclude>  
                <PackageVersion Include="Newtonsoft.Json" Version="12.0.0" />
                <PackageVersion Include="Serilog" Version="2.10.0" />
              </ItemGroup>
            </Project>
            """);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, ct: ct);

            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Newtonsoft.Json", result.Updates[0].PackageId);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ExclusionIsCaseInsensitive() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });
            
            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, excludePackageIds: ["NEWTONSOFT.JSON"], ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Serilog", result.Updates[0].PackageId);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_ForMalformedXml() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile("<Project><Broken>");
        
        try {
            var apiClient = new StubNuGetApiClient([]);
            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, ct: ct);

            Assert.False(result.Success);
            Assert.Contains("Failed to parse XML", result.Error);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_PreservesUtf8BomEncoding() {
        var ct = new CancellationToken();
        var content = """
            <?xml version="1.0" encoding="utf-8"?>
            <Project>
              <ItemGroup>
                <PackageVersion Include="Newtonsoft.Json" Version="12.0.0" />
              </ItemGroup>
            </Project>
            """;

        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.props");
        await File.WriteAllTextAsync(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true), ct);

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            await updater.UpdateAsync(path, ct: ct);

            var bytes = await File.ReadAllBytesAsync(path, ct);
            Assert.Equal(0xEF, bytes[0]);
            Assert.Equal(0xBB, bytes[1]);
            Assert.Equal(0xBF, bytes[2]);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoUpdates_WhenFileHasNoPackageVersionElements() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile("""
            <?xml version="1.0" encoding="utf-8"?>
            <Project>
              <PropertyGroup>
                <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
              </PropertyGroup>
            </Project>
            """);

        try {
            var apiClient = new StubNuGetApiClient([]);
            var updater = new DirectoryPackagePropsUpdater(apiClient);
            var result = await updater.UpdateAsync(path, ct: ct);
            
            Assert.True(result.Success);
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadAllPackageVersions_ReturnsAllEntries() {
        var path = WriteTempPropsFile(SimplePropsContent);

        try {
            var result = DirectoryPackagePropsUpdater.ReadAllPackageVersions(path);

            Assert.Equal(2, result.Count);
            Assert.Equal("12.0.0", result["Newtonsoft.Json"]);
            Assert.Equal("2.10.0", result["Serilog"]);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadAllPackageVersions_ReturnsEmpty_WhenFilesDoesNotExist() {
        var result = DirectoryPackagePropsUpdater.ReadAllPackageVersions(
            Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid():N}.props"));

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_PerPackageVersionsBehindMap_OverridesGlobalCli() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent); // Newtonsoft.Json 12.0.0, Serilog 2.10.0

        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0"],
                ["Serilog"] = ["2.10.0", "2.11.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            // Global ceiling of 2 alone would skip both packages (ceiling index < 0),
            // but the per-package override map (from a csproj;s VersionBehind attribute)
            // sets Newtonsoft.Json to 0, allowing it to update while Serilog stays capped .
            var overrides = new Dictionary<string, int> { ["Newtonsoft.Json"] = 0 };
            var result = await updater.UpdateAsync(
                path, versionsBehindLatest: 2, perPackageVersionsBehind: overrides, ct: ct);
            
            Assert.True(result.Success);
            Assert.Single(result.Updates);
            Assert.Equal("Newtonsoft.Json", result.Updates[0].PackageId);
            Assert.Equal("13.0.0", result.Updates[0].ToVersion);

            var written = await File.ReadAllTextAsync(path, ct);
            Assert.Contains("Include=\"Newtonsoft.Json\" Version=\"13.0.0\"", written);
            Assert.Contains("Include=\"Serilog\" Version=\"2.10.0\"", written); // capped by global ceiling -> unchanged
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task UpdateAsync_PerPackageVersionsBehindMap_IsCaseInsensitiveOnPackageId() {
        var ct = new CancellationToken();
        var path = WriteTempPropsFile(SimplePropsContent);
        
        try {
            var apiClient = new StubNuGetApiClient(new Dictionary<string, string[]> {
                ["Newtonsoft.Json"] = ["12.0.0", "13.0.0", "14.0.0"],
                ["Serilog"] = ["2.10.0"]
            });

            var updater = new DirectoryPackagePropsUpdater(apiClient);
            //Map key differs in case from the PackageVersion Include; lookup must still match
            var overrides = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { 
                ["newtonsoft.json"] = 2 
            };
            var result = await updater.UpdateAsync(
                path, perPackageVersionsBehind: overrides, ct: ct);

            Assert.True(result.Success);
            // Newtonsoft.Json (latest 14.0.0) capped 2 behind -> ceiling 12.0.0 == current -> skipped.
            Assert.Empty(result.Updates);
        } finally {
            File.Delete(path);
        }
    }

    private static string WriteTempPropsFile(string content) {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.props");
        File.WriteAllText(path, content);
        return path;
    }

    private sealed class StubNuGetApiClient(Dictionary<string, string[]> data) : INuGetApiClient {
        public Task<IReadOnlyList<string>> GetAvailableVersionsAsync(string packageId, CancellationToken ct) { 
            var versions = data.TryGetValue(packageId, out var list) ? list : [];
            return Task.FromResult((IReadOnlyList<string>)versions);
        }
    }
}
