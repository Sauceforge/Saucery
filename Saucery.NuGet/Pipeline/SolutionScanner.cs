using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Saucery.NuGet.Pipeline;

public static class SolutionScanner {
    private static readonly Regex ProjectLineRegex =
        new(@"Project\(""\{[^}]+\}""\)\s*=\s*""[^""]+""\s*,\s*""([^""]+\.csproj)""",
            RegexOptions.IgnoreCase |
            RegexOptions.Compiled);

    public static IReadOnlyList<string> GetProjectPaths(string solutionPath)
    {
        var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionPath))
            ?? throw new ArgumentException($"Cannot determine directory of solution: {solutionPath}");

        var paths = new List<string>();

        foreach(var line in File.ReadLines(solutionPath))
        {
            var match = ProjectLineRegex.Match(line);
            if(!match.Success)
                continue;

            var relativePath = match.Groups[1].Value.Replace('\\', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(solutionDir, relativePath));

            if(File.Exists(fullPath))
                paths.Add(fullPath);
        }

        return paths;
    }

    public static IReadOnlyList<string> FilterOptedIn(
        IEnumerable<string> projectPaths,
        Func<string, bool> isOptedIn) 
        => projectPaths.Where(isOptedIn).ToList();
}