using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal static class RepositoryLocator
{
    public static string Resolve(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            var explicitRoot = Path.GetFullPath(configured);
            Validate(explicitRoot);
            return explicitRoot;
        }

        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory }
                     .Select(Path.GetFullPath)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var current = new DirectoryInfo(start);
            while (current is not null)
            {
                if (IsRepositoryRoot(current.FullName))
                {
                    return current.FullName;
                }
                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate a Saucery repository root containing Saucery.Core and Saucery.Core.Tests. " +
            "Supply --repo-root <path>.");
    }

    private static void Validate(string root)
    {
        if (!IsRepositoryRoot(root))
        {
            throw new DirectoryNotFoundException(
                $"'{root}' does not contain Saucery.Core and Saucery.Core.Tests.");
        }
    }

    private static bool IsRepositoryRoot(string root) =>
        Directory.Exists(Path.Combine(root, "Saucery.Core")) &&
        Directory.Exists(Path.Combine(root, "Saucery.Core.Tests"));
}

internal sealed class RepositoryContext(string root)
{
    public string Root { get; } = root;
    public string Core => Path.Combine(Root, "Saucery.Core");
    public string Tests => Path.Combine(Root, "Saucery.Core.Tests");
    public string ReporterProject => Path.Combine(Root, "Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj");

    public string PathFromRoot(string relative) => Path.GetFullPath(Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar)));

    public string Relative(string path) => Path.GetRelativePath(Root, path);

    public string RequireFile(string relative)
    {
        var path = PathFromRoot(relative);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Required Saucery source file does not exist: {relative}", path);
        }
        return path;
    }
}

internal sealed class RepositoryTransaction
{
    private readonly Dictionary<string, byte[]?> _originals = new(StringComparer.OrdinalIgnoreCase);
    private bool _completed;

    public void Capture(string path)
    {
        path = Path.GetFullPath(path);
        if (_originals.ContainsKey(path)) return;
        _originals[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
    }

    public void WriteText(string path, string text)
    {
        Capture(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text, new UTF8Encoding(false));
    }

    public void Delete(string path)
    {
        Capture(path);
        if (File.Exists(path)) File.Delete(path);
    }

    public void Rollback()
    {
        if (_completed) return;
        foreach (var pair in _originals.Reverse())
        {
            if (pair.Value is null)
            {
                if (File.Exists(pair.Key)) File.Delete(pair.Key);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pair.Key)!);
                File.WriteAllBytes(pair.Key, pair.Value);
            }
        }
        _completed = true;
    }

    public void Commit() => _completed = true;
}

internal static class CSharpTextEditor
{
    public static string AddSwitchArmBeforeDefault(
        string source,
        string containingMethod,
        string armText,
        string uniqueNeedle)
    {
        if (source.Contains(uniqueNeedle, StringComparison.Ordinal))
        {
            return source;
        }

        var methodIndex = source.IndexOf(containingMethod, StringComparison.Ordinal);
        if (methodIndex < 0)
        {
            throw new InvalidDataException($"Could not locate method marker '{containingMethod}'.");
        }

        var defaultIndex = source.IndexOf("_ => null", methodIndex, StringComparison.Ordinal);
        if (defaultIndex < 0)
        {
            throw new InvalidDataException($"Could not locate switch default after '{containingMethod}'.");
        }

        var lineStart = source.LastIndexOf('\n', defaultIndex);
        lineStart = lineStart < 0 ? 0 : lineStart + 1;
        var indentation = source[lineStart..defaultIndex];
        var insertion = indentation + armText.Trim() + Environment.NewLine;
        return source.Insert(lineStart, insertion);
    }

    public static string RemoveLinesContaining(string source, params string[] needles)
    {
        var normalized = source.Replace("\r\n", "\n", StringComparison.Ordinal);
        var lines = normalized.Split('\n').ToList();
        var removed = 0;

        lines.RemoveAll(line =>
        {
            var match = needles.All(n => line.Contains(n, StringComparison.Ordinal));
            if (match) removed++;
            return match;
        });

        if (removed == 0)
        {
            return source;
        }

        return string.Join(Environment.NewLine, lines);
    }

    public static string AddConstantAfter(string source, string afterNeedle, string constantLine, string uniqueNeedle)
    {
        if (source.Contains(uniqueNeedle, StringComparison.Ordinal)) return source;

        var index = source.IndexOf(afterNeedle, StringComparison.Ordinal);
        if (index < 0) throw new InvalidDataException($"Could not locate '{afterNeedle}'.");
        var end = source.IndexOf('\n', index);
        if (end < 0) end = source.Length;
        else end++;
        return source.Insert(end, constantLine + Environment.NewLine);
    }

    public static string AddUsingAfter(string source, string afterUsing, string usingLine)
    {
        if (source.Contains(usingLine, StringComparison.Ordinal)) return source;
        var index = source.IndexOf(afterUsing, StringComparison.Ordinal);
        if (index < 0) throw new InvalidDataException($"Could not locate using '{afterUsing}'.");
        var end = source.IndexOf('\n', index);
        if (end < 0) end = source.Length;
        else end++;
        return source.Insert(end, usingLine + Environment.NewLine);
    }

    public static string AddArrayItemBeforeClosing(
        string source,
        string arrayMarker,
        string itemText,
        string uniqueNeedle)
    {
        if (source.Contains(uniqueNeedle, StringComparison.Ordinal)) return source;
        var markerIndex = source.IndexOf(arrayMarker, StringComparison.Ordinal);
        if (markerIndex < 0) throw new InvalidDataException($"Could not locate array marker '{arrayMarker}'.");
        var open = source.IndexOf('[', markerIndex);
        var close = source.IndexOf("]", open, StringComparison.Ordinal);
        if (open < 0 || close < 0) throw new InvalidDataException($"Could not locate array body for '{arrayMarker}'.");

        var before = source[..close];
        var lastNonWhitespace = before.Length - 1;
        while (lastNonWhitespace >= 0 && char.IsWhiteSpace(before[lastNonWhitespace])) lastNonWhitespace--;
        var needsComma = lastNonWhitespace >= 0 && before[lastNonWhitespace] != '[' && before[lastNonWhitespace] != ',';
        var indent = DetectArrayIndent(source, open, close);
        var insertion = (needsComma ? "," : string.Empty) + Environment.NewLine + indent + itemText.Trim();
        return source.Insert(close, insertion);
    }

    public static string RemoveArrayLineContaining(string source, string arrayMarker, string needle)
    {
        var markerIndex = source.IndexOf(arrayMarker, StringComparison.Ordinal);
        if (markerIndex < 0) return source;
        var open = source.IndexOf('[', markerIndex);
        var close = source.IndexOf("]", open, StringComparison.Ordinal);
        if (open < 0 || close < 0) return source;

        var prefix = source[..open];
        var body = source[(open + 1)..close];
        var suffix = source[close..];
        var lines = body.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
        var removed = lines.RemoveAll(x => x.Contains(needle, StringComparison.Ordinal));
        if (removed == 0) return source;

        // Repair a dangling comma pattern conservatively.
        var rebuilt = string.Join(Environment.NewLine, lines)
            .Replace("," + Environment.NewLine + ",", "," + Environment.NewLine, StringComparison.Ordinal);
        return prefix + "[" + rebuilt + suffix;
    }

    public static string CloneAndRename(string source, params (string From, string To)[] replacements)
    {
        var result = source;
        foreach (var (from, to) in replacements.OrderByDescending(x => x.From.Length))
        {
            result = result.Replace(from, to, StringComparison.Ordinal);
        }
        return result;
    }

    public static void AssertValidCSharp(string source, string filePath)
    {
        var tree = CSharpSyntaxTree.ParseText(source, path: filePath);
        var errors = tree.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToArray();
        if (errors.Length > 0)
        {
            throw new InvalidDataException(
                $"Generated invalid C# in {filePath}:{Environment.NewLine}" +
                string.Join(Environment.NewLine, errors.Select(x => x.ToString())));
        }
    }

    private static string DetectArrayIndent(string source, int open, int close)
    {
        var segment = source[(open + 1)..close];
        var match = Regex.Match(segment, @"\n(?<indent>\s+)\S");
        return match.Success ? match.Groups["indent"].Value : "            ";
    }
}

internal static class PlatformNaming
{
    public static int Major(string version)
    {
        var match = Regex.Match(version, @"^(?<major>\d+)");
        if (!match.Success) throw new InvalidDataException($"Cannot parse major version from '{version}'.");
        return int.Parse(match.Groups["major"].Value);
    }

    public static string ExactVersion(string version)
    {
        var match = Regex.Match(version, @"^(?<major>\d+)(?:\.(?<minor>\d+))?$");
        if (!match.Success) throw new InvalidDataException($"Cannot parse exact version '{version}'.");
        return $"{int.Parse(match.Groups["major"].Value)}.{(match.Groups["minor"].Success ? int.Parse(match.Groups["minor"].Value) : 0)}";
    }

    public static string ClassDigits(string version)
    {
        var exact = ExactVersion(version);
        var parts = exact.Split('.');
        return parts[1] == "0" ? parts[0] : parts[0] + parts[1];
    }

    public static string WildcardMajor(string version) => Major(version).ToString();

    public static string ConstantSuffix(string version) =>
        version.Replace('.', '_').Replace(".x", string.Empty, StringComparison.OrdinalIgnoreCase);
}
