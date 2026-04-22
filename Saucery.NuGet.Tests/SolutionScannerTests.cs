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

    private static string CreateTempRoot() {
        var tempDir = Path.Combine(Path.GetTempPath(), $"slntest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    [Fact]
    public void GetProjectPaths_ReturnsExistingCsprojPaths() {
        var tempDir = CreateTempRoot();

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
        var tempDir = CreateTempRoot();

        try {
            var missingRelPath = Path.Combine("MissingProject", "MissingProject.csproj");

            var slnContent =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"MissingProject\", \"" + missingRelPath + "\", \"{GUID}\"\r\n" +
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
        var root = CreateTempRoot();

        try {
            var projectA = Path.Combine(root, "a", "a.csproj");
            var projectB = Path.Combine(root, "b", "b.csproj");
            var projectC = Path.Combine(root, "c", "c.csproj");

            var projects = new[]
            {
                projectA,
                projectB,
                projectC
            };

            var optedIn = new[]
            {
                projectA,
                projectC
            };

            var result = SolutionScanner.FilterOptedIn(projects, p => optedIn.Contains(p));

            Assert.Equal(2, result.Count);
            Assert.Contains(projectA, result);
            Assert.Contains(projectC, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByNameWithoutExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");

            var opted = new[]
            {
                core,
                xunit
            };

            var result = SolutionScanner.FilterByRequestedProjects(opted, ["Saucery.Core"]);

            Assert.Single(result);
            Assert.Contains(core, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByFilenameWithExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");

            var opted = new[]
            {
                core,
                xunit
            };

            var result = SolutionScanner.FilterByRequestedProjects(opted, ["Saucery.XUnit.csproj"]);

            Assert.Single(result);
            Assert.Contains(xunit, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByFullPath() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");

            var opted = new[]
            {
                core,
                xunit
            };

            var path = Path.GetFullPath(core);
            var result = SolutionScanner.FilterByRequestedProjects(opted, [path]);

            Assert.Single(result);
            Assert.Contains(core, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MultipleFilters() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            var nuget = Path.Combine(root, "Saucery.NuGet", "Saucery.NuGet.csproj");

            var opted = new[]
            {
                core,
                xunit,
                nuget
            };

            var result = SolutionScanner.FilterByRequestedProjects(opted, ["Saucery.Core", "Saucery.NuGet"]);

            Assert.Equal(2, result.Count);
            Assert.Contains(core, result);
            Assert.Contains(nuget, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_NoMatches_ReturnsEmpty() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");

            var opted = new[]
            {
                core,
                xunit
            };

            var result = SolutionScanner.FilterByRequestedProjects(opted, ["Nonexistent"]);

            Assert.Empty(result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsOrphanNotInSolution() {
        var tempDir = CreateTempRoot();
        
        try {
            var registeredPath = CreateTempProject(tempDir, "Registered", "<Project />");
            var orphanPath = CreateTempProject(tempDir, "Orphan", "<Project />");
            var relPath = Path.Combine("Registered", "Registered.csproj");

            var slnContent =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Registered\", \"" + relPath + "\", \"{GUID}\"\r\n" +
                "EndProject\r\n";
            
            var slnPath = Path.Combine(tempDir, "Test.sln");
            File.WriteAllText(slnPath, slnContent);
            
            var result = SolutionScanner.FindOrphanedCsprojs(slnPath);
            
            Assert.Single(result);
            Assert.Equal(Path.GetFullPath(orphanPath), result[0]);
        } finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsEmpty_WhenAllRegistered() {
        var tempDir = CreateTempRoot();
        
        try {
            var projPath = CreateTempProject(tempDir, "MyProject", "<Project />");
            var relPath = Path.Combine("MyProject", "MyProject.csproj");
            
            
            var slnContent =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"MyProject\", \"" + relPath + "\", \"{GUID}\"\r\n" +
                "EndProject\r\n";
            
            var slnPath = Path.Combine(tempDir, "Test.sln");
            File.WriteAllText(slnPath, slnContent);
            
            var result = SolutionScanner.FindOrphanedCsprojs(slnPath);
            
            Assert.Empty(result);
        } finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void FindOphanedCsprojs_ExcludesBinAndObj() {
        var tempDir = CreateTempRoot();

        try {
            var slnContent = "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n";
            var slnPath = Path.Combine(tempDir, "Test.sln");
            File.WriteAllText(slnPath, slnContent);

            var binDir = Path.Combine(tempDir, "SomeProject", "bin", "Debug");
            Directory.CreateDirectory(binDir);
            File.WriteAllText(Path.Combine(binDir, "SomeProject.csproj"), "<Project />");

            var objDir = Path.Combine(tempDir, "SomeProject", "obj");
            Directory.CreateDirectory(objDir);
            File.WriteAllText(Path.Combine(objDir, "SomeProject.csproj"), "<Project />");

            var result = SolutionScanner.FindOrphanedCsprojs(slnPath);

            Assert.Empty(result);
        } finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByNameWithoutExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };

            var result = SolutionScanner.FilterByRequestedProjects(projects, ["Saucery.Core"]);

            Assert.Single(result);
            Assert.Contains(core, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByFilenameWithExtension() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };
            
            var result = SolutionScanner.FilterByRequestedProjects(projects, ["Saucery.XUnit.csproj"]);
            
            Assert.Single(result);
            Assert.Contains(xunit, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByFullPath() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };
            
            var result = SolutionScanner.FilterByRequestedProjects(projects, [Path.GetFullPath(core)]);
            
            Assert.Single(result);
            Assert.Contains(core, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_MultipleExclusions() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            var nuget = Path.Combine(root, "Saucery.NuGet", "Saucery.NuGet.csproj");
            
            var projects = new[] { core, xunit, nuget };
            
            var result = SolutionScanner.FilterExcludedProjects(projects, ["Saucery.Core", "Saucery.NuGet"]);
            
            Assert.Single(result);
            Assert.Contains(xunit, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_NoMatchingExclusions_ReturnsAll() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };
            
            var result = SolutionScanner.FilterExcludedProjects(projects, ["Nonexistent"]);
            
            Assert.Equal(2, result.Count);
            Assert.Contains(core, result);
            Assert.Contains(xunit, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_EmptyExclusions_ReturnsAll() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };
            
            var result = SolutionScanner.FilterExcludedProjects(projects, []);
            
            Assert.Equal(2, result.Count);
            Assert.Contains(core, result);
            Assert.Contains(xunit, result);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludeAll_ReturnsEmpty() {
        var root = CreateTempRoot();
        
        try {
            var core = Path.Combine(root, "Saucery.Core", "Saucery.Core.csproj");
            var xunit = Path.Combine(root, "Saucery.XUnit", "Saucery.XUnit.csproj");
            
            var projects = new[] { core, xunit };
            
            var result = SolutionScanner.FilterExcludedProjects(projects, ["Saucery.Core", "Saucery.XUnit"]);
            
            Assert.Empty(result);
        } finally {
            Directory.Delete(root, true);
        }
    }
}