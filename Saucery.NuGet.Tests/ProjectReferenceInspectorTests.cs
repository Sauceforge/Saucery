using Saucery.NuGet.Core;

namespace Saucery.NuGet.Tests;

public class ProjectReferenceInspectorTests {
    // CPM-style project: PAckageReference elements carry no Version attribute,
    // exactly like Saucery.Core.csproj in the real solution.
    private const string CpmCsproj = """
        <?xml version="1.0" encoding="utf-8"?>
        <Project Sdk="Microsoft.NET.Sdk">
            <PropertyGroup>
                <PackageVersion>4.10.16</PackageVersion>
            </PropertyGroup>
            <ItemGroup>
                <PackageReference Include="Selenium.Support" />
                <PackageReference Include="Selenium.WebDriver" />
                <PackageReference Include="RestSharp" />
            </ItemGroup>
        </Project>
        """;

    private static IReadOnlySet<string> UpdatedIds(params string[] ids) => 
        new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);

    [Fact]
    public void ReferencesAnyUpdatedPackage_RealCsprojPath_ReturnsTrue_WhenReferenced() {
        // Regression: the method must read the file at the given PATH. The prior
        // implementation called XmlDocument.LoadXml(path) (which parses the string as
        // XML content), so a real path always threw and the method always returned false,
        // silently skipping the CPM --bump-own-version step.
        var path = WriteTempCsProj(CpmCsproj);

        try {
            var result = ProjectReferenceInspector.ReferencesAnyUpdatedPackage(
                path, 
                UpdatedIds("Selenium.Support", "Selenium.WebDriver"));

            Assert.True(result);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReferencesAnyUpdatedPAckage_RealCsprojPath_ReturnsFalse_WhenNotReferenced() {
        var path = WriteTempCsProj(CpmCsproj);
        
        try {
            var result = ProjectReferenceInspector.ReferencesAnyUpdatedPackage(
                path,
                UpdatedIds("Newtonsoft.Json"));

            Assert.False(result);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReferencesAnyUpdatedPackage_MatchesCaseInsensitivity_WhenSetIsOrdinalIgnoreCase() {
        var path = WriteTempCsProj(CpmCsproj);
        
        try {
            var result = ProjectReferenceInspector.ReferencesAnyUpdatedPackage(
                path,
                UpdatedIds("selenium.webdriver"));

            Assert.True(result);
        } finally {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReferencesAnyUpdatedPackage_NonExistentPath_ReturnsFalse() {
        var missing = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.csproj");

        var result = ProjectReferenceInspector.ReferencesAnyUpdatedPackage(
            missing,
            UpdatedIds("Selenium.Support"));
        
        Assert.False(result);
    }

    private static string WriteTempCsProj(string content) {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csproj");
        File.WriteAllText(path, content);

        return path;
    }
}
