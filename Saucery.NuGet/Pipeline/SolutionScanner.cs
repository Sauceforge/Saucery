using Saucery.NuGet;
using System.Text.RegularExpressions;
using System.Xml;

namespace Saucery.NuGet.Pipeline;

public static class SolutionScanner {
    private static readonly Regex ProjectLineRegex = new(
        @"Project\(""\{[^}]+\}""\)\s*=\s*""[^""]+""\s*,\s*""([^""]+\.csproj)""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static IReadOnlyList<string> GetProjectPaths(string solutionPath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(solutionPath);

        var fullSolutionPath = Path.GetFullPath(solutionPath);

        if(!File.Exists(fullSolutionPath)) {
            throw new FileNotFoundException(
                $"Solution file was not found: {fullSolutionPath}",
                fullSolutionPath);
        }

        var solutionDirectory = Path.GetDirectoryName(fullSolutionPath)
            ?? throw new ArgumentException(
                $"Cannot determine directory of solution: {solutionPath}",
                nameof(solutionPath));

        var extension = Path.GetExtension(fullSolutionPath);

        return extension.ToLowerInvariant() switch {
            ".sln" => GetProjectPathsFromSln(
                fullSolutionPath,
                solutionDirectory),

            ".slnx" => GetProjectPathsFromSlnx(
                fullSolutionPath,
                solutionDirectory),

            _ => throw new NotSupportedException(
                $"Unsupported solution file extension '{extension}'. " +
                "Supported extensions are '.sln' and '.slnx'.")
        };
    }

    public static IReadOnlyList<string> FilterOptedIn(
        IEnumerable<string> projectPaths,
        Func<string, bool> isOptedIn) {
        ArgumentNullException.ThrowIfNull(projectPaths);
        ArgumentNullException.ThrowIfNull(isOptedIn);

        return
        [
            .. projectPaths.Where(isOptedIn)
        ];
    }

    /// <summary>
    /// Filters an existing list of opted-in project paths down to those requested
    /// by the user via the --project / -p option.
    /// </summary>
    /// <remarks>
    /// Matching supports:
    /// <list type="bullet">
    /// <item>Project file name without extension, for example Saucery.Core.</item>
    /// <item>Project file name with extension, for example Saucery.Core.csproj.</item>
    /// <item>Absolute path to the project file.</item>
    /// </list>
    /// </remarks>
    public static List<string> FilterByRequestedProjects(
        IEnumerable<string> optedInProjectPaths,
        IEnumerable<string>? requestedFilters) {
        ArgumentNullException.ThrowIfNull(optedInProjectPaths);

        var optedInProjects = optedInProjectPaths.ToList();
        var filters = NormalizeFilters(requestedFilters);

        if(filters.Count == 0) {
            return optedInProjects;
        }

        return
        [
            .. optedInProjects.Where(
                projectPath => filters.Any(
                    filter => ProjectMatchesFilter(projectPath, filter)))
        ];
    }

    /// <summary>
    /// Removes projects from the list that match any of the supplied
    /// --exclude-projects filters.
    /// </summary>
    /// <remarks>
    /// Matching supports:
    /// <list type="bullet">
    /// <item>Project file name without extension, for example Saucery.Core.</item>
    /// <item>Project file name with extension, for example Saucery.Core.csproj.</item>
    /// <item>Absolute path to the project file.</item>
    /// </list>
    /// </remarks>
    public static List<string> FilterExcludedProjects(
        IEnumerable<string> projectPaths,
        IEnumerable<string>? excludedFilters) {
        ArgumentNullException.ThrowIfNull(projectPaths);

        var projects = projectPaths.ToList();
        var filters = NormalizeFilters(excludedFilters);

        if(filters.Count == 0) {
            return projects;
        }

        return
        [
            .. projects.Where(
                projectPath => !filters.Any(
                    filter => ProjectMatchesFilter(projectPath, filter)))
        ];
    }

    /// <summary>
    /// Topologically sorts the supplied project paths so that projects referenced
    /// by other projects appear before their dependants.
    /// </summary>
    /// <remarks>
    /// If a dependency cycle is detected, the original ordering is returned.
    /// Invalid or unreadable project files are ignored while constructing the
    /// dependency graph.
    /// </remarks>
    public static List<string> TopologicallySortProjects(
        IEnumerable<string> projectPaths) {
        ArgumentNullException.ThrowIfNull(projectPaths);

        var fullPaths = projectPaths
            .Select(Path.GetFullPath)
            .ToList();

        var projectSet = fullPaths.ToHashSet(
            StringComparer.OrdinalIgnoreCase);

        var dependants = new Dictionary<string, List<string>>(
            StringComparer.OrdinalIgnoreCase);

        var indegree = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        foreach(var projectPath in fullPaths) {
            dependants[projectPath] = [];
            indegree[projectPath] = 0;
        }

        foreach(var projectPath in fullPaths) {
            AddProjectReferenceEdges(
                projectPath,
                projectSet,
                dependants,
                indegree);
        }

        var queue = new Queue<string>(
            indegree
                .Where(entry => entry.Value == 0)
                .Select(entry => entry.Key));

        var result = new List<string>(fullPaths.Count);

        while(queue.Count > 0) {
            var projectPath = queue.Dequeue();

            result.Add(projectPath);

            foreach(var dependant in dependants[projectPath]) {
                indegree[dependant]--;

                if(indegree[dependant] == 0) {
                    queue.Enqueue(dependant);
                }
            }
        }

        return result.Count == fullPaths.Count
            ? result
            : fullPaths;
    }

    public static IReadOnlyList<string> FindDirectoryPackagesProps(string solutionPath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(solutionPath);

        var fullSolutionPath = Path.GetFullPath(solutionPath);
        
        if(!File.Exists(fullSolutionPath)) {
            throw new FileNotFoundException(
                $"Solution file was not found: {fullSolutionPath}",
                fullSolutionPath);
        }

        var solutionDirectory = Path.GetDirectoryName(fullSolutionPath)
            ?? throw new ArgumentException(
                $"Cannot determine directory of solution: {solutionPath}",
                nameof(solutionPath));
        
        var results = new List<string>();
        
        foreach(var filePath in Directory.EnumerateFiles(
                     solutionDirectory,
                     Constants.Files.DirectoryPackagesProps,
                     SearchOption.AllDirectories)) {
            var fullPath = Path.GetFullPath(filePath);

            if(IsBuildOutputPath(fullPath, solutionDirectory)) {
                continue;
            }

            results.Add(fullPath);
        }

        return results;
    }

    public static IReadOnlyList<string> FindOrphanedCsprojs(
        string solutionPath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(solutionPath);

        var fullSolutionPath = Path.GetFullPath(solutionPath);

        if(!File.Exists(fullSolutionPath)) {
            throw new FileNotFoundException(
                $"Solution file was not found: {fullSolutionPath}",
                fullSolutionPath);
        }

        var solutionDirectory = Path.GetDirectoryName(fullSolutionPath)
            ?? throw new ArgumentException(
                $"Cannot determine directory of solution: {solutionPath}",
                nameof(solutionPath));

        var registeredPaths = GetProjectPaths(fullSolutionPath)
            .Select(Path.GetFullPath)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var orphanedProjects = new List<string>();

        foreach(var projectPath in Directory.EnumerateFiles(
                     solutionDirectory,
                     "*.csproj",
                     SearchOption.AllDirectories)) {
            var fullProjectPath = Path.GetFullPath(projectPath);

            if(IsBuildOutputPath(
                    fullProjectPath,
                    solutionDirectory)) {
                continue;
            }

            if(!registeredPaths.Contains(fullProjectPath)) {
                orphanedProjects.Add(fullProjectPath);
            }
        }

        return orphanedProjects;
    }

    private static IReadOnlyList<string> GetProjectPathsFromSln(
        string solutionPath,
        string solutionDirectory) {
        var projectPaths = new List<string>();

        foreach(var line in File.ReadLines(solutionPath)) {
            var match = ProjectLineRegex.Match(line);

            if(!match.Success) {
                continue;
            }

            AddProjectPath(
                projectPaths,
                solutionDirectory,
                match.Groups[1].Value);
        }

        return projectPaths;
    }

    private static IReadOnlyList<string> GetProjectPathsFromSlnx(
        string solutionPath,
        string solutionDirectory) {
        var document = new XmlDocument {
            PreserveWhitespace = true
        };

        document.Load(solutionPath);

        var projectNodes = document.SelectNodes(
            "//*[local-name()='Project' and @Path]");

        if(projectNodes is null) {
            return [];
        }

        var projectPaths = new List<string>();

        foreach(var projectElement in projectNodes
                     .OfType<XmlElement>()) {
            var relativePath = projectElement.GetAttribute("Path");

            if(string.IsNullOrWhiteSpace(relativePath)) {
                continue;
            }

            AddProjectPath(
                projectPaths,
                solutionDirectory,
                relativePath);
        }

        return projectPaths;
    }

    private static void AddProjectPath(
        ICollection<string> projectPaths,
        string solutionDirectory,
        string projectPath) {
        if(!projectPath.EndsWith(
                ".csproj",
                StringComparison.OrdinalIgnoreCase)) {
            return;
        }

        var normalizedPath = NormalizeDirectorySeparators(projectPath);

        var fullPath = Path.GetFullPath(
            Path.Combine(solutionDirectory, normalizedPath));

        if(File.Exists(fullPath)) {
            projectPaths.Add(fullPath);
        }
    }

    private static void AddProjectReferenceEdges(
        string projectPath,
        IReadOnlySet<string> projectSet,
        IDictionary<string, List<string>> dependants,
        IDictionary<string, int> indegree) {
        try {
            var document = new XmlDocument();

            document.Load(projectPath);

            var projectReferenceNodes = document.SelectNodes(
                "//*[local-name()='ProjectReference' and @Include]");

            if(projectReferenceNodes is null) {
                return;
            }

            var projectDirectory = Path.GetDirectoryName(projectPath)
                ?? string.Empty;

            foreach(var projectReference in projectReferenceNodes
                         .OfType<XmlElement>()) {
                var include = projectReference.GetAttribute("Include");

                if(string.IsNullOrWhiteSpace(include)) {
                    continue;
                }

                var referencedProjectPath = Path.GetFullPath(
                    Path.Combine(
                        projectDirectory,
                        NormalizeDirectorySeparators(include)));

                if(!projectSet.Contains(referencedProjectPath)) {
                    continue;
                }

                dependants[referencedProjectPath].Add(projectPath);
                indegree[projectPath]++;
            }
        } catch(IOException) {
            // Ignore unreadable project files and preserve the remaining graph.
        } catch(UnauthorizedAccessException) {
            // Ignore inaccessible project files and preserve the remaining graph.
        } catch(XmlException) {
            // Ignore malformed project files and preserve the remaining graph.
        }
    }

    private static List<string> NormalizeFilters(
        IEnumerable<string>? filters) {
        return filters?
            .Select(filter => filter?.Trim())
            .Where(filter => !string.IsNullOrWhiteSpace(filter))
            .Select(filter => filter!)
            .ToList()
            ?? [];
    }

    private static bool ProjectMatchesFilter(
        string projectPath,
        string filter) {
        if(string.Equals(
                Path.GetFileNameWithoutExtension(projectPath),
                filter,
                StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if(string.Equals(
                Path.GetFileName(projectPath),
                filter,
                StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if(!Path.IsPathRooted(filter)) {
            return false;
        }

        return string.Equals(
            Path.GetFullPath(projectPath),
            Path.GetFullPath(filter),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBuildOutputPath(
        string projectPath,
        string solutionDirectory) {
        var relativePath = Path.GetRelativePath(
            solutionDirectory,
            projectPath);

        var segments = relativePath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        return segments.Any(
            segment =>
                segment.Equals(
                    "bin",
                    StringComparison.OrdinalIgnoreCase)
                || segment.Equals(
                    "obj",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeDirectorySeparators(string path) {
        return path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }
}