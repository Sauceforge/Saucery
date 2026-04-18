using System.CommandLine;
using Saucery.NuGet;
using Saucery.NuGet.Core;
using Saucery.NuGet.Models;
using Saucery.NuGet.Pipeline;

var solutionOption = new Option<FileInfo>(
    Constants.Cli.SolutionOption, 
    [Constants.Cli.SolutionAlias])
{ 
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

var rootCommand = new RootCommand("Bumps each PackageReference in opted-in projects to its next available NuGet version.")
{
    solutionOption,
    includePrereleaseOption,
    dryRunOption,
    bumpOwnVersionOption,
    versionSegmentOption
};

rootCommand.SetAction(async (parseResult, cancellationToken) => {
    var solution = parseResult.GetValue(solutionOption);
    var includePrerelease = parseResult.GetValue(includePrereleaseOption);
    var dryRun = parseResult.GetValue(dryRunOption);
    var bumpOwnVersion = parseResult.GetValue(bumpOwnVersionOption);
    var versionSegment = parseResult.GetValue(versionSegmentOption);

    if(!solution.Exists) {
        Console.Error.WriteLine($"Error: Solution file not found: {solution.FullName}.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Solution: {solution.FullName}");
    Console.WriteLine($"Prerelease: {(includePrerelease ? "included" : "stable only")}");
    Console.WriteLine($"Dry run: {dryRun}");
    Console.WriteLine($"Bump own version: {(bumpOwnVersion ? $"yes ({versionSegment})" : "no")}");
    Console.WriteLine();

    var allProjects = SolutionScanner.GetProjectPaths(solution.FullName);
    Console.WriteLine($"Found {allProjects.Count} projects in the solution.");

    var optedInProjects = SolutionScanner.FilterOptedIn(allProjects, CsprojUpdater.IsOptedIn);
    Console.WriteLine($"{optedInProjects.Count} projects are opted in for updates.");
    Console.WriteLine();

    if(optedInProjects.Count == 0) {
        Console.WriteLine($"No projects are opted in. Add <PackageReference Include=\"{Constants.Package.OptInPackageId}\" Version\"...\" /> to opt in.");
        return;
    }

    using var apiClient = new NuGetApiClient();
    var updater = new CsprojUpdater(apiClient);

    var allResults = new List<UpdateResult>();

    foreach(var projectPath in optedInProjects) 
    {
        Console.WriteLine($"Processing {Path.GetFileName(projectPath)}");
        var result = await updater.UpdateAsync(
            projectPath, 
            includePrerelease, 
            dryRun, 
            bumpOwnVersion, 
            versionSegment);
        allResults.Add(result);

        if(!result.Success) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" ERROR: {result.Error}");
            Console.ResetColor();
            continue;
        }

        if(result.Updates.Count == 0) 
        {
            Console.WriteLine("  No updates available.");
            continue;
        }

        foreach(var update in result.Updates) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {update.PackageId}: {update.FromVersion} -> {update.ToVersion}{(dryRun ? " (dry run)" : "")}");
            Console.ResetColor();
        }

        if(!string.IsNullOrEmpty(result.NewPackageVersion)) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  <PackageVersion> bumped to {result.NewPackageVersion}{(dryRun ? " (dry run)" : "")}");
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

    if(errorCount > 0) {
        Environment.Exit(1);
    }

    await Task.CompletedTask;
});

return await rootCommand.Parse(args).InvokeAsync();