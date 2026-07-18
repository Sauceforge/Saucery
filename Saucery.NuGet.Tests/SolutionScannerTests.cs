using Saucery.NuGet.Pipeline;

namespace Saucery.NuGet.Tests;

public class SolutionScannerTests {
    private const string CSharpProjectTypeGuid =
        "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

    [Fact]
    public void GetProjectPaths_ReturnsExistingCsprojPathsFromSln() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var solutionPath = CreateSln(
                tempDirectory,
                ("MyProject", projectPath));

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(projectPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ReturnsMultipleExistingCsprojPathsFromSln() {
        var tempDirectory = CreateTempRoot();

        try {
            var firstProjectPath = CreateTempProject(
                tempDirectory,
                "FirstProject");

            var secondProjectPath = CreateTempProject(
                tempDirectory,
                "SecondProject");

            var solutionPath = CreateSln(
                tempDirectory,
                ("FirstProject", firstProjectPath),
                ("SecondProject", secondProjectPath));

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Equal(2, result.Count);
            Assert.Equal(
                Path.GetFullPath(firstProjectPath),
                result[0]);
            Assert.Equal(
                Path.GetFullPath(secondProjectPath),
                result[1]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_IgnoresMissingProjectsFromSln() {
        var tempDirectory = CreateTempRoot();

        try {
            var missingProjectPath = Path.Combine(
                tempDirectory,
                "MissingProject",
                "MissingProject.csproj");

            var solutionPath = CreateSln(
                tempDirectory,
                ("MissingProject", missingProjectPath));

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ReturnsExistingCsprojPathsFromSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var solutionPath = CreateSlnx(
                tempDirectory,
                projectPath);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(projectPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ReturnsMultipleExistingCsprojPathsFromSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var firstProjectPath = CreateTempProject(
                tempDirectory,
                "FirstProject");

            var secondProjectPath = CreateTempProject(
                tempDirectory,
                "SecondProject");

            var solutionPath = CreateSlnx(
                tempDirectory,
                firstProjectPath,
                secondProjectPath);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Equal(2, result.Count);
            Assert.Equal(
                Path.GetFullPath(firstProjectPath),
                result[0]);
            Assert.Equal(
                Path.GetFullPath(secondProjectPath),
                result[1]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ReturnsProjectsNestedInsideSlnxFolders() {
        var tempDirectory = CreateTempRoot();

        try {
            var coreProjectPath = CreateTempProject(
                tempDirectory,
                Path.Combine("src", "Saucery.Core"),
                "Saucery.Core");

            var testProjectPath = CreateTempProject(
                tempDirectory,
                Path.Combine("tests", "Saucery.Core.Tests"),
                "Saucery.Core.Tests");

            var coreRelativePath = Path.GetRelativePath(
                tempDirectory,
                coreProjectPath);

            var testRelativePath = Path.GetRelativePath(
                tempDirectory,
                testProjectPath);

            var solutionPath = Path.Combine(
                tempDirectory,
                "Test.slnx");

            File.WriteAllText(
                solutionPath,
                $"""
                 <Solution>
                   <Folder Name="/src/">
                     <Project Path="{ToSlnxPath(coreRelativePath)}" />
                   </Folder>
                   <Folder Name="/tests/">
                     <Project Path="{ToSlnxPath(testRelativePath)}" />
                   </Folder>
                 </Solution>
                 """);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Equal(2, result.Count);
            Assert.Equal(
                Path.GetFullPath(coreProjectPath),
                result[0]);
            Assert.Equal(
                Path.GetFullPath(testProjectPath),
                result[1]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_HandlesNamespacedSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var relativePath = Path.GetRelativePath(
                tempDirectory,
                projectPath);

            var solutionPath = Path.Combine(
                tempDirectory,
                "Test.slnx");

            File.WriteAllText(
                solutionPath,
                $"""
                 <Solution xmlns="urn:example:slnx">
                   <Project Path="{ToSlnxPath(relativePath)}" />
                 </Solution>
                 """);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(projectPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_IgnoresMissingProjectsFromSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var missingProjectPath = Path.Combine(
                tempDirectory,
                "MissingProject",
                "MissingProject.csproj");

            var solutionPath = CreateSlnx(
                tempDirectory,
                missingProjectPath);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_IgnoresNonCsprojEntriesFromSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var relativePath = Path.GetRelativePath(
                tempDirectory,
                projectPath);

            var solutionPath = Path.Combine(
                tempDirectory,
                "Test.slnx");

            File.WriteAllText(
                solutionPath,
                $"""
                 <Solution>
                   <Project Path="{ToSlnxPath(relativePath)}" />
                   <Project Path="OtherProject/OtherProject.vbproj" />
                   <Project Path="AnotherProject/AnotherProject.fsproj" />
                 </Solution>
                 """);

            var result = SolutionScanner.GetProjectPaths(solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(projectPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ThrowsWhenSolutionDoesNotExist() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = Path.Combine(
                tempDirectory,
                "Missing.slnx");

            var exception = Assert.Throws<FileNotFoundException>(
                () => SolutionScanner.GetProjectPaths(solutionPath));

            Assert.Equal(
                Path.GetFullPath(solutionPath),
                exception.FileName);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void GetProjectPaths_ThrowsForUnsupportedExtension() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = Path.Combine(
                tempDirectory,
                "Test.txt");

            File.WriteAllText(
                solutionPath,
                "Not a solution.");

            var exception = Assert.Throws<NotSupportedException>(
                () => SolutionScanner.GetProjectPaths(solutionPath));

            Assert.Contains(
                ".txt",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            Assert.Contains(
                ".sln",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            Assert.Contains(
                ".slnx",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FilterOptedIn_ReturnsOnlyOptedInProjects() {
        var root = CreateTempRoot();

        try {
            var projectA = Path.Combine(
                root,
                "a",
                "a.csproj");

            var projectB = Path.Combine(
                root,
                "b",
                "b.csproj");

            var projectC = Path.Combine(
                root,
                "c",
                "c.csproj");

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

            var result = SolutionScanner.FilterOptedIn(
                projects,
                optedIn.Contains);

            Assert.Equal(2, result.Count);
            Assert.Contains(projectA, result);
            Assert.Contains(projectC, result);
            Assert.DoesNotContain(projectB, result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByNameWithoutExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                ["Saucery.Core"]);

            Assert.Single(result);
            Assert.Equal(core, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByFilenameWithExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                ["Saucery.XUnit.csproj"]);

            Assert.Single(result);
            Assert.Equal(xunit, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesByFullPath() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                [Path.GetFullPath(core)]);

            Assert.Single(result);
            Assert.Equal(core, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MatchesCaseInsensitively() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core],
                ["saucery.core"]);

            Assert.Single(result);
            Assert.Equal(core, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_MultipleFilters() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var nuget = Path.Combine(
                root,
                "Saucery.NuGet",
                "Saucery.NuGet.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit, nuget],
                [
                    "Saucery.Core",
                    "Saucery.NuGet"
                ]);

            Assert.Equal(2, result.Count);
            Assert.Contains(core, result);
            Assert.Contains(nuget, result);
            Assert.DoesNotContain(xunit, result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_NoMatches_ReturnsEmpty() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                ["Nonexistent"]);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_EmptyFilters_ReturnsAll() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                []);

            Assert.Equal(
                [core, xunit],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterByRequestedProjects_NullFilters_ReturnsAll() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterByRequestedProjects(
                [core, xunit],
                null);

            Assert.Equal(
                [core, xunit],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByNameWithoutExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                ["Saucery.Core"]);

            Assert.Single(result);
            Assert.Equal(xunit, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByFilenameWithExtension() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                ["Saucery.XUnit.csproj"]);

            Assert.Single(result);
            Assert.Equal(core, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludesByFullPath() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                [Path.GetFullPath(core)]);

            Assert.Single(result);
            Assert.Equal(xunit, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_MultipleExclusions() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var nuget = Path.Combine(
                root,
                "Saucery.NuGet",
                "Saucery.NuGet.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit, nuget],
                [
                    "Saucery.Core",
                    "Saucery.NuGet"
                ]);

            Assert.Single(result);
            Assert.Equal(xunit, result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_NoMatchingExclusions_ReturnsAll() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                ["Nonexistent"]);

            Assert.Equal(
                [core, xunit],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_EmptyExclusions_ReturnsAll() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                []);

            Assert.Equal(
                [core, xunit],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_NullExclusions_ReturnsAll() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                null);

            Assert.Equal(
                [core, xunit],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FilterExcludedProjects_ExcludeAll_ReturnsEmpty() {
        var root = CreateTempRoot();

        try {
            var core = Path.Combine(
                root,
                "Saucery.Core",
                "Saucery.Core.csproj");

            var xunit = Path.Combine(
                root,
                "Saucery.XUnit",
                "Saucery.XUnit.csproj");

            var result = SolutionScanner.FilterExcludedProjects(
                [core, xunit],
                [
                    "Saucery.Core",
                    "Saucery.XUnit"
                ]);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_PutsDependencyBeforeDependant() {
        var root = CreateTempRoot();

        try {
            var core = CreateTempProject(
                root,
                "Core");

            var applicationReference = Path.GetRelativePath(
                Path.Combine(root, "Application"),
                core);

            var application = CreateTempProjectWithContent(
                root,
                "Application",
                CreateProjectContent(applicationReference));

            var result = SolutionScanner.TopologicallySortProjects(
                [
                    application,
                    core
                ]);

            Assert.Equal(
                [
                    Path.GetFullPath(core),
                    Path.GetFullPath(application)
                ],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_SortsDependencyChain() {
        var root = CreateTempRoot();

        try {
            var core = CreateTempProject(
                root,
                "Core");

            var applicationReference = Path.GetRelativePath(
                Path.Combine(root, "Application"),
                core);

            var application = CreateTempProjectWithContent(
                root,
                "Application",
                CreateProjectContent(applicationReference));

            var webReference = Path.GetRelativePath(
                Path.Combine(root, "Web"),
                application);

            var web = CreateTempProjectWithContent(
                root,
                "Web",
                CreateProjectContent(webReference));

            var result = SolutionScanner.TopologicallySortProjects(
                [
                    web,
                    application,
                    core
                ]);

            Assert.Equal(
                [
                    Path.GetFullPath(core),
                    Path.GetFullPath(application),
                    Path.GetFullPath(web)
                ],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_PreservesIndependentProjectOrder() {
        var root = CreateTempRoot();

        try {
            var first = CreateTempProject(
                root,
                "First");

            var second = CreateTempProject(
                root,
                "Second");

            var third = CreateTempProject(
                root,
                "Third");

            var result = SolutionScanner.TopologicallySortProjects(
                [
                    first,
                    second,
                    third
                ]);

            Assert.Equal(
                [
                    Path.GetFullPath(first),
                    Path.GetFullPath(second),
                    Path.GetFullPath(third)
                ],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_IgnoresReferencesOutsideProvidedProjects() {
        var root = CreateTempRoot();

        try {
            var external = CreateTempProject(
                root,
                "External");

            var applicationReference = Path.GetRelativePath(
                Path.Combine(root, "Application"),
                external);

            var application = CreateTempProjectWithContent(
                root,
                "Application",
                CreateProjectContent(applicationReference));

            var result = SolutionScanner.TopologicallySortProjects(
                [application]);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(application),
                result[0]);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_ReturnsOriginalOrderForCycle() {
        var root = CreateTempRoot();

        try {
            var firstDirectory = Path.Combine(
                root,
                "First");

            var secondDirectory = Path.Combine(
                root,
                "Second");

            Directory.CreateDirectory(firstDirectory);
            Directory.CreateDirectory(secondDirectory);

            var first = Path.Combine(
                firstDirectory,
                "First.csproj");

            var second = Path.Combine(
                secondDirectory,
                "Second.csproj");

            File.WriteAllText(
                first,
                CreateProjectContent(
                    Path.GetRelativePath(
                        firstDirectory,
                        second)));

            File.WriteAllText(
                second,
                CreateProjectContent(
                    Path.GetRelativePath(
                        secondDirectory,
                        first)));

            var result = SolutionScanner.TopologicallySortProjects(
                [
                    second,
                    first
                ]);

            Assert.Equal(
                [
                    Path.GetFullPath(second),
                    Path.GetFullPath(first)
                ],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void TopologicallySortProjects_IgnoresMalformedProjectFiles() {
        var root = CreateTempRoot();

        try {
            var malformed = CreateTempProjectWithContent(
                root,
                "Malformed",
                "<Project>");

            var valid = CreateTempProject(
                root,
                "Valid");

            var result = SolutionScanner.TopologicallySortProjects(
                [
                    malformed,
                    valid
                ]);

            Assert.Equal(
                [
                    Path.GetFullPath(malformed),
                    Path.GetFullPath(valid)
                ],
                result);
        } finally {
            DeleteDirectory(root);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsOrphanNotInSln() {
        var tempDirectory = CreateTempRoot();

        try {
            var registeredPath = CreateTempProject(
                tempDirectory,
                "Registered");

            var orphanPath = CreateTempProject(
                tempDirectory,
                "Orphan");

            var solutionPath = CreateSln(
                tempDirectory,
                ("Registered", registeredPath));

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(orphanPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsOrphanNotInSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var registeredPath = CreateTempProject(
                tempDirectory,
                "Registered");

            var orphanPath = CreateTempProject(
                tempDirectory,
                "Orphan");

            var solutionPath = CreateSlnx(
                tempDirectory,
                registeredPath);

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Single(result);
            Assert.Equal(
                Path.GetFullPath(orphanPath),
                result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsEmptyWhenAllRegisteredInSln() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var solutionPath = CreateSln(
                tempDirectory,
                ("MyProject", projectPath));

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ReturnsEmptyWhenAllRegisteredInSlnx() {
        var tempDirectory = CreateTempRoot();

        try {
            var projectPath = CreateTempProject(
                tempDirectory,
                "MyProject");

            var solutionPath = CreateSlnx(
                tempDirectory,
                projectPath);

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_ExcludesBinAndObj() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var binDirectory = Path.Combine(
                tempDirectory,
                "SomeProject",
                "bin",
                "Debug");

            Directory.CreateDirectory(binDirectory);

            File.WriteAllText(
                Path.Combine(
                    binDirectory,
                    "SomeProject.csproj"),
                "<Project />");

            var objDirectory = Path.Combine(
                tempDirectory,
                "SomeProject",
                "obj");

            Directory.CreateDirectory(objDirectory);

            File.WriteAllText(
                Path.Combine(
                    objDirectory,
                    "SomeProject.csproj"),
                "<Project />");

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindOrphanedCsprojs_DoesNotExcludeDirectoriesContainingBinOrObj() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var binaryProject = CreateTempProject(
                tempDirectory,
                "BinaryProject");

            var objectModelProject = CreateTempProject(
                tempDirectory,
                "ObjectModel");

            var result = SolutionScanner.FindOrphanedCsprojs(
                solutionPath);

            Assert.Equal(2, result.Count);
            Assert.Contains(
                Path.GetFullPath(binaryProject),
                result);
            Assert.Contains(
                Path.GetFullPath(objectModelProject),
                result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    private static string CreateTempRoot() {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            $"slntest_{Guid.NewGuid():N}");

        Directory.CreateDirectory(tempDirectory);

        return tempDirectory;
    }

    private static string CreateTempProject(
        string rootDirectory,
        string directoryName) {
        var projectName = Path.GetFileName(directoryName);

        return CreateTempProjectCore(
            rootDirectory,
            directoryName,
            projectName,
            "<Project />");
    }

    private static string CreateTempProject(
        string rootDirectory,
        string directoryName,
        string projectName) {
        return CreateTempProjectCore(
            rootDirectory,
            directoryName,
            projectName,
            "<Project />");
    }

    private static string CreateTempProjectWithContent(
        string rootDirectory,
        string directoryName,
        string projectContent) {
        var projectName = Path.GetFileName(directoryName);

        return CreateTempProjectCore(
            rootDirectory,
            directoryName,
            projectName,
            projectContent);
    }

    private static string CreateTempProjectCore(
        string rootDirectory,
        string directoryName,
        string projectName,
        string projectContent) {
        var projectDirectory = Path.Combine(
            rootDirectory,
            directoryName);

        Directory.CreateDirectory(projectDirectory);

        var projectPath = Path.Combine(
            projectDirectory,
            $"{projectName}.csproj");

        File.WriteAllText(
            projectPath,
            projectContent);

        return projectPath;
    }

    private static string CreateSln(
        string rootDirectory,
        params (string Name, string ProjectPath)[] projects) {
        var solutionPath = Path.Combine(
            rootDirectory,
            "Test.sln");

        var lines = new List<string>
        {
            "Microsoft Visual Studio Solution File, Format Version 12.00",
            "# Visual Studio Version 17"
        };

        foreach(var project in projects) {
            var relativePath = Path.GetRelativePath(
                rootDirectory,
                project.ProjectPath);

            lines.Add(
                $"Project(\"{CSharpProjectTypeGuid}\") = " +
                $"\"{project.Name}\", " +
                $"\"{ToSlnPath(relativePath)}\", " +
                $"\"{{{Guid.NewGuid().ToString().ToUpperInvariant()}}}\"");

            lines.Add("EndProject");
        }

        lines.Add("Global");
        lines.Add("EndGlobal");

        File.WriteAllLines(
            solutionPath,
            lines);

        return solutionPath;
    }

    private static string CreateSlnx(
        string rootDirectory,
        params string[] projectPaths) {
        var solutionPath = Path.Combine(
            rootDirectory,
            "Test.slnx");

        var projectElements = projectPaths.Select(
            projectPath => {
                var relativePath = Path.GetRelativePath(
                    rootDirectory,
                    projectPath);

                return
                    $"  <Project Path=\"{ToSlnxPath(relativePath)}\" />";
            });

        var lines = new List<string>
        {
            "<Solution>"
        };

        lines.AddRange(projectElements);
        lines.Add("</Solution>");

        File.WriteAllLines(
            solutionPath,
            lines);

        return solutionPath;
    }

    private static string CreateProjectContent(
        string projectReference) {
        return
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <ItemGroup>
                 <ProjectReference Include="{projectReference}" />
               </ItemGroup>
             </Project>
             """;
    }

    private static string ToSlnPath(string path) {
        return path
            .Replace(
                Path.DirectorySeparatorChar,
                '\\')
            .Replace(
                Path.AltDirectorySeparatorChar,
                '\\');
    }

    private static string ToSlnxPath(string path) {
        return path
            .Replace(
                Path.DirectorySeparatorChar,
                '/')
            .Replace(
                Path.AltDirectorySeparatorChar,
                '/');
    }

    [Fact]
    public void FindDirectoryPackagesProps_FindsFileInSolutionRoot() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var propsPath = Path.Combine(
                tempDirectory,
                "Directory.Packages.props");
            File.WriteAllText(propsPath, "<Project />");

            var result = SolutionScanner.FindDirectoryPackagesProps(solutionPath);

            Assert.Single(result);
            Assert.Equal(Path.GetFullPath(propsPath), result[0]);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindDirectoryPackagesProps_FindsFilesRecursively() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var rootPropsPath = Path.Combine(
                tempDirectory,
                "Directory.Packages.props");

            var subDirectory = Path.Combine(tempDirectory, "src");
            Directory.CreateDirectory(subDirectory);

            var subPropsPath = Path.Combine(subDirectory, "Directory.Packages.props");

            File.WriteAllText(rootPropsPath, "<Project />");
            File.WriteAllText(subPropsPath, "<Project />");

            var result = SolutionScanner.FindDirectoryPackagesProps(solutionPath);

            Assert.Equal(2, result.Count);
            Assert.Contains(Path.GetFullPath(rootPropsPath), result);
            Assert.Contains(Path.GetFullPath(subPropsPath), result);
        } finally { 
            DeleteDirectory(tempDirectory); 
        }
    }

    [Fact]
    public void FindDirectoryPackagesProps_ReturnsEmpty_WhenFilesExist() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var result = SolutionScanner.FindDirectoryPackagesProps(solutionPath);

            Assert.Empty(result);
        } finally { 
            DeleteDirectory(tempDirectory); 
        }
    }

    [Fact]
    public void FindDirectoryPackagesProps_SkipsFilesUnderBinAndObj() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = CreateSlnx(tempDirectory);

            var binDirectory = Path.Combine(tempDirectory, "SomeProject", "bin", "Debug");
            var objDirectory = Path.Combine(tempDirectory, "SomeProject", "obj");

            Directory.CreateDirectory(binDirectory);
            Directory.CreateDirectory(objDirectory);

            File.WriteAllText(
                Path.Combine(binDirectory, "Directory.Packages.props"),
                "<Project />");

            File.WriteAllText(
                Path.Combine(objDirectory, "Directory.Packages.props"),
                "<Project />");

            var result = SolutionScanner.FindDirectoryPackagesProps(solutionPath);

            Assert.Empty(result);
        } finally {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FindDirectoryPackagesProps_ThrowsWhenSolutionDoesNotExist() {
        var tempDirectory = CreateTempRoot();

        try {
            var solutionPath = Path.Combine(tempDirectory, "Missing.slnx");

            var exception = Assert.Throws<FileNotFoundException>(
                () => SolutionScanner.FindDirectoryPackagesProps(solutionPath));

            Assert.Equal(Path.GetFullPath(solutionPath), exception.FileName);
        } finally { 
            DeleteDirectory(tempDirectory); 
        }
    }

    private static void DeleteDirectory(string directory) {
        if(Directory.Exists(directory)) {
            Directory.Delete(
                directory,
                recursive: true);
        }
    }
}