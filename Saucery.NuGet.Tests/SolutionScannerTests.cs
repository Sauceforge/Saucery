using Saucery.NuGet.Pipeline;

namespace Saucery.NuGet.Tests;

public class SolutionScannerTests {
    private static string CreateTempProject(string directory, string name, string content) {
        var dir = Path.Combine(directory, name);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{name}.csproj");
        File.WriteAllText(path, content);
        
        return path;
    }

    [Fact]
    public void GetProjectPaths_ReturnsExistingCsprojPaths() {
        var tempDir = Path.Combine(Path.GetTempPath(), $"slntest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        

        try {
            var projPath = CreateTempProject(tempDir, "MyProject", "<Project />");
            var relPath = Path.Combine("MyProject", "MyProject.csproj");

            var slnContent =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"MyProject\", \"" + relPath + "\", \"{GUID}\"\r\n" +
                "EndProject\r\n";

            var slnPath = Path.Combine(tempDir, "Test.sln");
            File.WriteAllText(slnPath, slnContent);

            var result = SolutionScanner.GetProjectPaths(slnPath);

            Assert.Single(result);
            Assert.Equal(projPath, result[0]);

        } finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void GetProjectPaths_IgnoresMissingProjects() {
        var tempDir = Path.Combine(Path.GetTempPath(), $"slntest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        
        try {
            var slnContent =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"MissingProject\", \"MissingProject\\MissingProject.csproj\", \"{GUID}\"\r\n" +
                "EndProject\r\n";
            
            var slnPath = Path.Combine(tempDir, "Test.sln");
            File.WriteAllText(slnPath, slnContent);
            
            var result = SolutionScanner.GetProjectPaths(slnPath);
            
            Assert.Empty(result);
        } finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void FilterOptedIn_ReturnsOnlyOptedInProjects() {
        var projects = new[] {
            "/path/a.csproj",
            "/path/b.csproj",
            "/path/c.csproj"
        };

        var optedIn = new[] {
            "/path/a.csproj",
            "/path/c.csproj"
        };

        var result = SolutionScanner.FilterOptedIn(projects, p => optedIn.Contains(p));

        Assert.Equal(2, result.Count);
        Assert.Contains("/path/a.csproj", result);
        Assert.Contains("/path/c.csproj", result);
    }
}
