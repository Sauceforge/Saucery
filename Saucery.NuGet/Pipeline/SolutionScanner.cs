using System.Text.RegularExpressions;
using System.Xml;

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
        => [.. projectPaths.Where(isOptedIn)];

    /// <summary>
    /// Filter an existing list of opted-in project paths down to those requested by the user via
    /// the --project / -p option. Matching supports:
    /// - project file name without extension (e.g. Saucery.Core)
    /// - project file name with extension (e.g. Saucery.Core.csproj)
    /// - absolute path to the project file
    /// </summary>
    public static List<string> FilterByRequestedProjects(
        IEnumerable<string> optedInProjectPaths,
        IEnumerable<string> requestedFilters)
    {
        var optedList = optedInProjectPaths.ToList();
        var filters = requestedFilters?.Select(f => f?.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? [];
        if(filters.Count == 0)
            return optedList;

        var matched = optedList.Where(pp => filters.Any(req =>
            string.Equals(Path.GetFileNameWithoutExtension(pp), req, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Path.GetFileName(pp), req, StringComparison.OrdinalIgnoreCase) ||
            (Path.IsPathRooted(req) && string.Equals(Path.GetFullPath(req), Path.GetFullPath(pp), StringComparison.OrdinalIgnoreCase))
        )).ToList();

        return matched;
    }

    /// <summary>
    /// Remove projects from the list that match any of the supplied exclude filters via
    /// the --exclude-projects option. Matching supports:
    /// - project file name without extension (e.g. Saucery.Core)
    /// - project file name with extension (e.g. Saucery.Core.csproj)
    /// - absolute path to the project file
    /// </summary>
    /// <param name="projectPaths"></param>
    /// <param name="excludedFilters"></param>
    /// <returns></returns>
    public static List<string> FilterExcludedProjects(
        IEnumerable<string> projectPaths,
        IEnumerable<string> excludedFilters)
    {
        var projectList = projectPaths.ToList();
        var filters = excludedFilters?.Select(f => f?.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? [];

        return filters.Count == 0
            ? projectList
            : [.. projectList.Where(pp => !filters.Any(req =>
            string.Equals(Path.GetFileNameWithoutExtension(pp), req, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Path.GetFileName(pp), req, StringComparison.OrdinalIgnoreCase) ||
            (Path.IsPathRooted(req) && string.Equals(Path.GetFullPath(req), Path.GetFullPath(pp), StringComparison.OrdinalIgnoreCase))))];
    }

    /// <summary>
    /// Topologically sort the supplied project paths so that projects referenced by others
    /// appear before dependents. If a cycle is detected, the original ordering is returned.
    /// </summary>
    public static List<string> TopologicallySortProjects(IEnumerable<string> projectPaths)
    {
        var fullPaths = projectPaths.Select(Path.GetFullPath).ToList();
        var set = fullPaths.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // build adjacency: referenced -> dependents
        var dependents = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var indegree = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach(var p in fullPaths) {
            dependents[p] = [];
            indegree[p] = 0;
        }

        foreach(var p in fullPaths) {
            try {
                var doc = new XmlDocument();
                doc.Load(p);
                var projectRefs = doc.SelectNodes("//*[local-name()='ProjectReference' and @Include]");
                if(projectRefs is null) continue;

                foreach(XmlElement projRef in projectRefs.Cast<XmlElement>()) {
                    var include = projRef.GetAttribute("Include");
                    if(string.IsNullOrWhiteSpace(include)) continue;
                    var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(p) ?? string.Empty, include.Replace('/', Path.DirectorySeparatorChar)));
                    if(set.Contains(resolved)) {
                        // edge: resolved -> p (resolved must come before p)
                        dependents[resolved].Add(p);
                        indegree[p] = indegree.GetValueOrDefault(p) + 1;
                    }
                }
            }
            catch {
                // ignore parse errors and continue
            }
        }

        // Kahn's algorithm
        var q = new Queue<string>(indegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var result = new List<string>();

        while(q.Count > 0) {
            var n = q.Dequeue();
            result.Add(n);
            foreach(var d in dependents[n]) {
                indegree[d] = indegree[d] - 1;
                if(indegree[d] == 0) q.Enqueue(d);
            }
        }

        if(result.Count != fullPaths.Count)
            return fullPaths; // cycle detected or parse issues - fall back

        return result;
    }

    public static IReadOnlyList<string> FindOrphanedCsprojs(string solutionPath) {
        var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionPath))
            ?? throw new ArgumentException($"Cannot determine directory of solution: {solutionPath}");
        
        var registeredPaths  = GetProjectPaths(solutionPath)
            .Select(Path.GetFullPath)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var orphans = new List<string>();

        foreach(var file in Directory.EnumerateFiles(solutionDir, "*.csproj", SearchOption.AllDirectories)) {
            var fullPath = Path.GetFullPath(file);
            var segments = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if(segments.Any(s => s.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                                 s.Equals("obj", StringComparison.OrdinalIgnoreCase)))
                continue;

            if(!registeredPaths.Contains(fullPath))
                orphans.Add(fullPath);
        }
        
        return orphans;
    }
}