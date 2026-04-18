using System.Text;
using System.Text.RegularExpressions;

namespace Saucery.NuGet.Core;

public static class PackageVersionBumper {
    private static readonly Regex PackageVersionRegex = new(
        @"(?ms)(<PackageVersion\b[^>]*>)\s*([^<\r\n]+?)\s*(</PackageVersion>)",
        RegexOptions.Compiled);

    public static string? ReadPackageVersion(string projectPath) {
        var text = ReadText(projectPath);
        var m = PackageVersionRegex.Match(text);
        return m.Success ? m.Groups[2].Value.Trim() : null;
    }

    public static string? Bump(
        string projectPath,
        VersionSegment segment = VersionSegment.Patch,
        bool dryRun = false) {
        var (text, encodingFactory) = ReadPreservingEncoding(projectPath);

        var m = PackageVersionRegex.Match(text);
        if(!m.Success)
            return null;

        var current = m.Groups[2].Value.Trim();
        var bumped = IncrementVersion(current, segment);
        if(bumped is null)
            return null;

        if(!dryRun) {
            var newText = text[..m.Groups[2].Index]
                        + bumped
                        + text[(m.Groups[2].Index + m.Groups[2].Length)..];
            WritePreservingEncoding(projectPath, newText, encodingFactory);
        }

        return bumped;
    }

    public static string? IncrementVersion(string version, VersionSegment segment) {
        if(string.IsNullOrWhiteSpace(version))
            return null;

        var corePart = version.Split('-', '+')[0].Trim();
        var parts = corePart.Split('.');

        if(parts.Length < 1)
            return null;

        var segs = new int[Math.Max(parts.Length, 3)];
        for(int i = 0; i < parts.Length; i++) {
            if(!int.TryParse(parts[i], out segs[i]))
                return null;
        }

        switch(segment) {
            case VersionSegment.Major:
                segs[0]++;
                segs[1] = 0;
                segs[2] = 0;
                break;
            case VersionSegment.Minor:
                segs[1]++;
                segs[2] = 0;
                break;
            case VersionSegment.Patch:
                segs[2]++;
                break;
        }

        var outParts = new string[parts.Length];
        for(int i = 0; i < parts.Length; i++) {
            outParts[i] = segs[i].ToString();
        }

        return string.Join('.', outParts);
    }

    private static string ReadText(string path)
        => ReadPreservingEncoding(path).Text;

    private static (string Text, Func<Encoding> EncodingFactory) ReadPreservingEncoding(string path) {
        var bytes = File.ReadAllBytes(path);

        if(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return (new UTF8Encoding(true).GetString(bytes, 3, bytes.Length - 3), () => new UTF8Encoding(true));

        if(bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return (new UTF8Encoding(false, true).GetString(bytes, 2, bytes.Length - 2), () => new UTF8Encoding(false,true));

        if(bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return (new UTF8Encoding(true, true).GetString(bytes, 2, bytes.Length - 2), () => new UTF8Encoding(true, true));

        return (new UTF8Encoding(false).GetString(bytes), () => new UTF8Encoding(false));
    }

    private static void WritePreservingEncoding(string path, string text, Func<Encoding> encodingFactory) =>
        File.WriteAllText(path, text, encodingFactory());
}
