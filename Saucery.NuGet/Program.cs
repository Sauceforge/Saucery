using System.CommandLine;
using Saucery.NuGet;
using Saucery.NuGet.Core;
using Saucery.NuGet.Models;
using Saucery.NuGet.Pipeline;

var solutionOption = new Option<FileInfo>(
    Constants.Cli.SolutionOption,
    [Constants.Cli.SolutionAlias]) {
    Required = true,
    Description = "Path to the.sln to process."
};

var includePrereleaseOption = new Option<bool>(
    Constants.Cli.IncludePrereleaseOption) {
    Description = "Include prerelease versions when resolving the next version.",
    DefaultValueFactory = _ => false
};

var dryRunOption = new Option<bool>(
    Constants.Cli.DryRunOption) {
    Description = "Print proposed changes without writing any files.",
    DefaultValueFactory = _ => false
};

var bumpOwnVersionOption = new Option<bool>(
    Constants.Cli.BumpOwnVersionOption) {
    Description = "When any PackageReference is updated, also increment the project's own <PackageVersion>.",
    DefaultValueFactory = _ => false
};

var versionSegmentOption = new Option<VersionSegment>(
    Constants.Cli.VersionSegmentOption) {
    Description = "The semver segment to increment when --bump-own-version is set (patch, minor, major).",
    DefaultValueFactory = _ => VersionSegment.Patch
};

var projectOption = new Option<string[]>(
    Constants.Cli.ProjectOption,
    [Constants.Cli.ProjectAlias]) {
    Description = "Optional: limit processing to one or more opted-in projects by project name or path.",
};

var syncWithOption = new Option<string?>(
    Constants.Cli.SyncWithOption,
    Constants.Cli.SyncWithAlias) {
    Description = "Optional: keep each processed project's <PackageVersion> in sync with the specified dependency package id (e.g. TUnit)."
};

var rootCommand = new RootCommand("Bumps each PackageReference in opted-in projects to its next available NuGet version.")
{
    solutionOption,
    includePrereleaseOption,
    dryRunOption,
    bumpOwnVersionOption,
    versionSegmentOption,
    projectOption,
    syncWithOption
};

rootCommand.SetAction(async (parseResult, cancellationToken) => {
    var solution = parseResult.GetValue(solutionOption);
    var includePrerelease = parseResult.GetValue(includePrereleaseOption);
    var dryRun = parseResult.GetValue(dryRunOption);
    var bumpOwnVersion = parseResult.GetValue(bumpOwnVersionOption);
    var versionSegment = parseResult.GetValue(versionSegmentOption);
    var syncWith = parseResult.GetValue(syncWithOption);

    if(solution is null || !solution.Exists) {
        Console.Error.WriteLine($"Error: Solution file not found: {solution?.FullName ?? "(null)"}.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Solution: {solution.FullName}");
    Console.WriteLine($"Prerelease: {(includePrerelease ? "included" : "stable only")}");
    Console.WriteLine($"Dry run: {dryRun}");
    Console.WriteLine($"Bump own version: {(bumpOwnVersion ? $"yes ({versionSegment})" : "no")}");
    if(!string.IsNullOrWhiteSpace(syncWith))
        Console.WriteLine($"Sync with: {syncWith}");
    Console.WriteLine();

    var allProjects = SolutionScanner.GetProjectPaths(solution.FullName);
    Console.WriteLine($"Found {allProjects.Count} projects in the solution.");

    var optedInProjects = SolutionScanner.FilterOptedIn(allProjects, CsprojUpdater.IsOptedIn).ToList();
    Console.WriteLine($"{optedInProjects.Count} projects are opted in for updates.");
    Console.WriteLine();

    var projectFilters = parseResult.GetValue(projectOption) ?? Array.Empty<string>();

    if(!string.IsNullOrWhiteSpace(syncWith) && projectFilters.Length == 0) {
        Console.Error.WriteLine("Error: --sync-with requires at least one --project to be specified. Use --project to limit processing to a single project to sync with a dependency.");
        Environment.Exit(1);
    }

    if(projectFilters.Length > 0) {
        var matched = SolutionScanner.FilterByRequestedProjects(optedInProjects, projectFilters);

        if(matched.Count == 0) {
            Console.Error.WriteLine($"No opted-in projects matched the requested filters: {string.Join(", ", projectFilters)}");
            Environment.Exit(1);
        }

        optedInProjects = matched;
        Console.WriteLine($"Processing limited to {optedInProjects.Count} opted-in project(s) by request.");
        Console.WriteLine();
    }

    // Ensure referenced projects are processed before dependents so --sync-with can read updated PackageVersion
    optedInProjects = SolutionScanner.TopologicallySortProjects(optedInProjects).ToList();

    if(optedInProjects.Count == 0) {
        Console.WriteLine($"No projects are opted in. Add <PackageReference Include=\"{Constants.Package.OptInPackageId}\" Version=\"...\" /> to opt in.");
        return;
    }

    using var apiClient = new NuGetApiClient();
    var updater = new CsprojUpdater(apiClient);

    var allResults = new List<UpdateResult>();

    foreach(var projectPath in optedInProjects) {
        Console.WriteLine($"Processing {Path.GetFileName(projectPath)}");

        var result = await updater.UpdateAsync(
            projectPath,
            includePrerelease,
            dryRun,
            bumpOwnVersion,
            versionSegment,
            syncWith,
            cancellationToken);

        allResults.Add(result);

        if(!result.Success) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ERROR: {result.Error}");
            Console.ResetColor();
            continue;
        }

        if(result.Updates.Count == 0) {
            Console.WriteLine("  No updates available.");
            continue;
        }

        foreach(var update in result.Updates) {
            Console.ForegroundColor = ConsoleColor.Green;

            if(string.Equals(update.PackageId, "PackageVersion", StringComparison.OrdinalIgnoreCase)) {
                var label = !string.IsNullOrWhiteSpace(syncWith)
                    ? "PackageVersion synced"
                    : "PackageVersion bumped";

                Console.WriteLine($"  {label}: {update.FromVersion} -> {update.ToVersion}{(dryRun ? " (dry run)" : "")}");
            } else {
                Console.WriteLine($"  {update.PackageId}: {update.FromVersion} -> {update.ToVersion}{(dryRun ? " (dry run)" : "")}");
            }

            Console.ResetColor();
        }
    }

    Console.WriteLine();

    var totalUpdates = allResults.Sum(r => r.Updates.Count);
    var errorCount = allResults.Count(r => !r.Success);

    if(dryRun)
        Console.WriteLine($"Dry run complete. {totalUpdates} update(s) proposed across {optedInProjects.Count} project(s).");
    else
        Console.WriteLine($"Done. {totalUpdates} updates applied across {optedInProjects.Count} projects with {errorCount} error(s).");

    if(errorCount > 0)
        Environment.Exit(1);

    await Task.CompletedTask;
});

return await rootCommand.Parse(args).InvokeAsync();