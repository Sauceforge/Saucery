namespace Saucery.TestFrameworkBenchmarks;

internal sealed class RepoLayout
{
    public required string RepoRoot { get; init; }

    public required string TUnitProject { get; init; }
    public required string NUnitProject { get; init; }
    public required string XUnitProject { get; init; }
    public required string XUnitV3Project { get; init; }

    public static RepoLayout DiscoverFromCurrentDirectory()
    {
        // BenchmarkDotNet runs from a generated harness directory, so we can't rely on relative paths
        // from the current working dir. Instead we walk upwards until we find the repo.
        var dir = new DirectoryInfo(Environment.CurrentDirectory);

        while (dir is not null)
        {
            // Repo-specific heuristics: these folders exist at repo root.
            var tunit = Path.Combine(dir.FullName, "Saucery.Core.Tests", "Saucery.Core.Tests.csproj");
            var nunit = Path.Combine(dir.FullName, "Saucery.Core.Tests.NUnit", "Saucery.Core.Tests.NUnit.csproj");
            var xunit = Path.Combine(dir.FullName, "Saucery.Core.Tests.XUnit", "Saucery.Core.Tests.XUnit.csproj");
            var xunitv3 = Path.Combine(dir.FullName, "Saucery.Core.Tests.XUnitv3", "Saucery.Core.Tests.XUnitv3.csproj");

            if (File.Exists(tunit) && File.Exists(nunit) && File.Exists(xunit) && File.Exists(xunitv3))
            {
                return new RepoLayout
                {
                    RepoRoot = dir.FullName,
                    TUnitProject = tunit,
                    NUnitProject = nunit,
                    XUnitProject = xunit,
                    XUnitV3Project = xunitv3
                };
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate Saucery repo root. Expected to find Saucery.Core.Tests*/ folders in an ancestor directory.");
    }

    public void BuildAllReleaseOnce()
    {
        // Build all four projects (and deps) once, so benchmark iterations can use --no-build.
        var projects = new[] { TUnitProject, NUnitProject, XUnitProject, XUnitV3Project };

        foreach (var project in projects)
        {
            DotnetCli.RunAndAssertSuccess(
                args: $"build \"{project}\" -c Release --nologo",
                workingDirectory: RepoRoot);
        }
    }
}
