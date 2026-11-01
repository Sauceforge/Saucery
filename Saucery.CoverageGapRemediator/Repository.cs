using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal static class RepositoryLocator {
    public static string Resolve(string? configured) {
        if(!string.IsNullOrWhiteSpace(configured)) {
            var root = Path.GetFullPath(configured);
            Validate(root);
            return root;
        }

        foreach(var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory }.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase)) {
            for(var dir = new DirectoryInfo(start); dir is not null; dir = dir.Parent) {
                if(IsRepositoryRoot(dir.FullName)) return dir.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate a Saucery repository root containing Saucery.Core and Saucery.Core.Tests. Supply --repo-root <path>.");
    }

    private static void Validate(string root) {
        if(!IsRepositoryRoot(root))
            throw new DirectoryNotFoundException($"'{root}' does not contain Saucery.Core and Saucery.Core.Tests.");
    }

    private static bool IsRepositoryRoot(string root) =>
        Directory.Exists(Path.Combine(root, "Saucery.Core")) &&
        Directory.Exists(Path.Combine(root, "Saucery.Core.Tests"));
}

internal sealed class RepositoryContext(string root) {
    public string Root { get; } = root;
    public string Core => Path.Combine(Root, "Saucery.Core");
    public string Tests => Path.Combine(Root, "Saucery.Core.Tests");
    public string ReporterProject => Path.Combine(Root, "Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj");
    public string PathFromRoot(string relative) => Path.GetFullPath(Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar)));
    public string Relative(string path) => Path.GetRelativePath(Root, path);

    public string RequireFile(string relative) {
        var path = PathFromRoot(relative);
        return File.Exists(path) ? path : throw new FileNotFoundException($"Required Saucery source file does not exist: {relative}", path);
    }
}

internal sealed class RepositoryTransaction {
    private readonly Dictionary<string, byte[]?> _originals = new(StringComparer.OrdinalIgnoreCase);
    private bool _completed;

    public void Capture(string path) {
        path = Path.GetFullPath(path);
        if(!_originals.ContainsKey(path))
            _originals[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
    }

    public void WriteText(string path, string text) {
        Capture(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text, new UTF8Encoding(false));
    }

    public void Delete(string path) {
        Capture(path);
        if(File.Exists(path)) File.Delete(path);
    }

    public void Rollback() {
        if(_completed) return;

        foreach(var (path, contents) in _originals.Reverse()) {
            if(contents is null) {
                if(File.Exists(path)) File.Delete(path);
            } else {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllBytes(path, contents);
            }
        }

        _completed = true;
    }

    public void Commit() => _completed = true;
}

internal static class CSharpTextEditor {
    public static string AddSwitchArmBeforeDefault(string source, string containingMethod, string armText, string uniqueNeedle) {
        if(source.Contains(uniqueNeedle, StringComparison.Ordinal)) return source;

        var method = source.IndexOf(containingMethod, StringComparison.Ordinal);
        if(method < 0) throw new InvalidDataException($"Could not locate method marker '{containingMethod}'.");

        var @default = source.IndexOf("_ => null", method, StringComparison.Ordinal);
        if(@default < 0) throw new InvalidDataException($"Could not locate switch default after '{containingMethod}'.");

        var start = source.LastIndexOf('\n', @default) + 1;
        return source.Insert(start, source[start..@default] + armText.Trim() + Environment.NewLine);
    }

    public static string RemoveLinesContaining(string source, params string[] needles) {
        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
        return lines.RemoveAll(line => needles.All(n => line.Contains(n, StringComparison.Ordinal))) == 0
            ? source
            : string.Join(Environment.NewLine, lines);
    }

    public static string AddConstantAfter(string source, string afterNeedle, string constantLine, string uniqueNeedle) {
        if(source.Contains(uniqueNeedle, StringComparison.Ordinal)) return source;

        var index = source.IndexOf(afterNeedle, StringComparison.Ordinal);
        if(index < 0) throw new InvalidDataException($"Could not locate '{afterNeedle}'.");

        var end = source.IndexOf('\n', index);
        return source.Insert(end < 0 ? source.Length : end + 1, constantLine + Environment.NewLine);
    }

    public static string AddUsingAfter(string source, string afterUsing, string usingLine) {
        if(source.Contains(usingLine, StringComparison.Ordinal)) return source;

        var index = source.IndexOf(afterUsing, StringComparison.Ordinal);
        if(index < 0) throw new InvalidDataException($"Could not locate using '{afterUsing}'.");

        var end = source.IndexOf('\n', index);
        return source.Insert(end < 0 ? source.Length : end + 1, usingLine + Environment.NewLine);
    }

    public static string AddArrayItemBeforeClosing(string source, string arrayMarker, string itemText, string uniqueNeedle) {
        if(source.Contains(uniqueNeedle, StringComparison.Ordinal)) return source;

        var marker = source.IndexOf(arrayMarker, StringComparison.Ordinal);
        if(marker < 0) throw new InvalidDataException($"Could not locate array marker '{arrayMarker}'.");

        var open = source.IndexOf('[', marker);
        var close = source.IndexOf(']', open);
        if(open < 0 || close < 0) throw new InvalidDataException($"Could not locate array body for '{arrayMarker}'.");

        var p = close - 1;
        while(p >= 0 && char.IsWhiteSpace(source[p])) p--;

        var comma = p >= 0 && source[p] is not '[' and not ',' ? "," : "";
        return source.Insert(close, comma + Environment.NewLine + DetectArrayIndent(source, open, close) + itemText.Trim());
    }

    public static string RemoveArrayLineContaining(string source, string arrayMarker, string needle) {
        var marker = source.IndexOf(arrayMarker, StringComparison.Ordinal);
        if(marker < 0) return source;

        var open = source.IndexOf('[', marker);
        var close = source.IndexOf(']', open);
        if(open < 0 || close < 0) return source;

        var lines = source[(open + 1)..close].Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
        if(lines.RemoveAll(line => line.Contains(needle, StringComparison.Ordinal)) == 0) return source;

        var body = string.Join(Environment.NewLine, lines)
            .Replace("," + Environment.NewLine + ",", "," + Environment.NewLine, StringComparison.Ordinal);

        return source[..(open + 1)] + body + source[close..];
    }

    public static string CloneAndRename(string source, params (string From, string To)[] replacements) {
        foreach(var (from, to) in replacements.OrderByDescending(x => x.From.Length))
            source = source.Replace(from, to, StringComparison.Ordinal);

        return source;
    }

    public static void AssertValidCSharp(string source, string filePath) {
        var errors = CSharpSyntaxTree.ParseText(source, path: filePath)
            .GetDiagnostics()
            .Where(x => x.Severity == DiagnosticSeverity.Error)
            .ToArray();

        if(errors.Length > 0)
            throw new InvalidDataException($"Generated invalid C# in {filePath}:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
    }

    private static string DetectArrayIndent(string source, int open, int close) {
        var match = Regex.Match(source[(open + 1)..close], @"\n(?<indent>\s+)\S");
        return match.Success ? match.Groups["indent"].Value : "            ";
    }
}

internal static class PlatformNaming {
    public static int Major(string version) {
        var match = Regex.Match(version, @"^(?<major>\d+)");
        return match.Success
            ? int.Parse(match.Groups["major"].Value)
            : throw new InvalidDataException($"Cannot parse major version from '{version}'.");
    }

    public static string ExactVersion(string version) {
        var match = Regex.Match(version, @"^(?<major>\d+)(?:\.(?<minor>\d+))?$");
        if(!match.Success) throw new InvalidDataException($"Cannot parse exact version '{version}'.");

        var major = int.Parse(match.Groups["major"].Value);
        var minor = match.Groups["minor"].Success ? int.Parse(match.Groups["minor"].Value) : 0;
        return $"{major}.{minor}";
    }

    public static string ClassDigits(string version) {
        var parts = ExactVersion(version).Split('.');
        return parts[1] == "0" ? parts[0] : parts[0] + parts[1];
    }

    public static string WildcardMajor(string version) => Major(version).ToString();

    public static string ConstantSuffix(string version) =>
        version.Replace('.', '_').Replace(".x", "", StringComparison.OrdinalIgnoreCase);
}