using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class PackageVersionBumperTests {
    private const string CsprojWithPackageVersion = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <PackageVersion>1.2.3</PackageVersion>
          </PropertyGroup>
        </Project>
        """;

    private const string CsprojWithoutPackageVersion = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """;

    [Theory]
    [InlineData("1.2.3", VersionSegment.Patch, "1.2.4")]
    [InlineData("1.2.3", VersionSegment.Minor, "1.3.0")]
    [InlineData("1.2.3", VersionSegment.Major, "2.0.0")]
    [InlineData("4.10.6", VersionSegment.Patch, "4.10.7")]
    [InlineData("1.0.0", VersionSegment.Major, "2.0.0")]
    [InlineData("0.9.9", VersionSegment.Patch, "0.9.10")]
    public void IncrementVersion_ReturnsCorrectResult(string input, VersionSegment segment, string expected) {
        var result = PackageVersionBumper.IncrementVersion(input, segment);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IncrementVersion_StripsPrereleaseBeforeBumping() {
        var result = PackageVersionBumper.IncrementVersion("1.2.3-beta", VersionSegment.Patch);
        Assert.Equal("1.2.4", result);
    } 
    
    [Fact]
    public void IncrementVersion_ReturnsNull_ForEmptyInput() {
        Assert.Null(PackageVersionBumper.IncrementVersion("", VersionSegment.Patch));
        Assert.Null(PackageVersionBumper.IncrementVersion("   ", VersionSegment.Patch));
    }

    [Fact]
    public void IncrementVersion_ReturnsNull_ForNonNumericSegment() {
        Assert.Null(PackageVersionBumper.IncrementVersion("1.x.3", VersionSegment.Patch));
    }

    [Fact]
    public void IncrementVersion_HandlesTwoSegmentVersion() {
        var result = PackageVersionBumper.IncrementVersion("1.0", VersionSegment.Minor);
        Assert.Equal("1.1", result);
    }

    [Fact]
    public void ReadPackageVersion_ReturnsVersion_WhenPresent() {
        var path = WriteTempCsproj(CsprojWithPackageVersion);
                
        try {
            Assert.Equal("1.2.3", PackageVersionBumper.ReadPackageVersion(path));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadPackageVersion_ReturnsNull_WhenAbsent() {
        var path = WriteTempCsproj(CsprojWithoutPackageVersion);
                
        try {
            Assert.Null(PackageVersionBumper.ReadPackageVersion(path));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void Bump_WritesIncrementedVersion_ToFile() {
        var path = WriteTempCsproj(CsprojWithPackageVersion);
                
        try {
            
            var result = PackageVersionBumper.Bump(path, VersionSegment.Patch, false);
            
            Assert.Equal("1.2.4", result);
            Assert.Contains("<PackageVersion>1.2.4</PackageVersion>", File.ReadAllText(path));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void Bump_DryRun_DoesNotModifyFile() {
        var path = WriteTempCsproj(CsprojWithPackageVersion);

        try {
            var original = File.ReadAllText(path);
            var result = PackageVersionBumper.Bump(path, VersionSegment.Patch, true);
            Assert.Equal("1.2.4", result);
            Assert.Equal(original, File.ReadAllText(path));
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void Bump_ReturnsNull_WhenNoPackageVersionElement() {
        var path = WriteTempCsproj(CsprojWithoutPackageVersion);
        try {
            Assert.Null(PackageVersionBumper.Bump(path, VersionSegment.Patch, false));
        } finally {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData(VersionSegment.Minor, "1.3.0")]
    [InlineData(VersionSegment.Major, "2.0.0")]
    public void Bump_RespectsVersionSegment(VersionSegment segment, string expected) {
        var path = WriteTempCsproj(CsprojWithPackageVersion);
        try {
            var result = PackageVersionBumper.Bump(path, segment, false);
            Assert.Equal(expected, result);
            Assert.Contains($"<PackageVersion>{expected}</PackageVersion>", File.ReadAllText(path));
        } finally {
            File.Delete(path);
        }
    }

    private static string WriteTempCsproj(string content) {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csproj");
        File.WriteAllText(path, content);
        
        return path;
    }
}