using Saucery.NuGet.Models;
using System.Text;
using System.Xml;

namespace Saucery.NuGet.Core;

public sealed class DirectoryPackagePropsUpdater(INuGetApiClient apiClient) {
    public async Task<UpdateResult> UpdateAsync(
        string filePath, bool includePrerelease = false,
        bool dryRun = false,
        IReadOnlyList<string>? excludePackageIds = null,
        CancellationToken ct = default) {

        var (rawText, encodingFactory) = ReadPreservingEncoding(filePath);

        XmlDocument doc;
        try {
            doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(rawText);

        } catch(XmlException ex) {
            return new UpdateResult(filePath, [], $"Failed to parse XML: {ex.Message}");
        }

        var packageVersionNodes = doc.SelectNodes(
            $"//*[local-name()='{Constants.Xml.PackageVersionElement}' and @{Constants.Xml.IncludeAttribute} and @{Constants.Xml.VersionAttribute}]");

        if(packageVersionNodes is null || packageVersionNodes.Count == 0) {
            return new UpdateResult(filePath, []);
        }

        var fileExclusions = ReadFileExclusions(doc);
        var effectiveExclusions = MergeExclusions(excludePackageIds, fileExclusions);

        var updates = new List<PackageUpdate>();

        foreach(XmlElement node in packageVersionNodes.Cast<XmlElement>()) {
            var id = node.GetAttribute(Constants.Xml.IncludeAttribute);
            var currentVersion = node.GetAttribute(Constants.Xml.VersionAttribute);

            if(effectiveExclusions.Count > 0 && 
               effectiveExclusions.Any(e => string.Equals(e, id, StringComparison.OrdinalIgnoreCase))) {
                continue;
            }

            var available = await apiClient.GetAvailableVersionsAsync(id, ct).ConfigureAwait(false);
            var next = VersionResolver.FindNextVersion(currentVersion, available, includePrerelease);

            if(next is null || next == currentVersion) {
                continue;
            }

            updates.Add(new PackageUpdate(filePath, id, currentVersion, next));

            if(!dryRun) {
                node.SetAttribute(Constants.Xml.VersionAttribute, next);
            }
        }

        if(!dryRun && updates.Count > 0) {
            var updated = SerializeDocument(doc);
            WritePreservingEncoding(filePath, updated, encodingFactory);
        }

        return new UpdateResult(filePath, updates);
    }

    /// <summary>
    /// Reads all <c>&lt;PackageVersion Include="..." Version="..." /&gt;</c> entries from 
    /// a <c>Directory.Packages.props</c> file and returns them as a package-id -> version map.
    /// Returns an empty dictionary if the file cannot be read or parsed.
    /// </summary> 
    public static IReadOnlyDictionary<string, string> ReadAllPackageVersions(string filePath) {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if(!File.Exists(filePath)) {
            return result;
        }

        try {
            var doc = new XmlDocument();
            doc.Load(filePath);

            var nodes = doc.SelectNodes(
                $"//*[local-name()='{Constants.Xml.PackageVersionElement}' and @{Constants.Xml.IncludeAttribute} and @{Constants.Xml.VersionAttribute}]");

            if(nodes is null) { return result; }

            foreach(XmlElement node in nodes.Cast<XmlElement>()) {
                var id = node.GetAttribute(Constants.Xml.IncludeAttribute);
                var version = node.GetAttribute(Constants.Xml.VersionAttribute);

                if(!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(version)) {
                    result[id] = version;
                }
            }
        } catch {
            // Ignore unreadable or malformed files - callers treat missing entries gracefully.
        }
        
        return result;
    }

    private static IReadOnlyList<string> ReadFileExclusions(XmlDocument doc) {
        var nodes = doc.SelectNodes($"//*[local-name()='{Constants.Xml.SauceryNuGetExcludeElement}']");

        if(nodes is null || nodes.Count == 0) {
            return [];
        }

        return [.. nodes
            .Cast<XmlElement>()
            .Select(n => n.InnerText.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))];
    }

    private static IReadOnlyList<string> MergeExclusions(
        IReadOnlyList<string>? cliExclusions, 
        IReadOnlyList<string> fileExclusions) {

        if((cliExclusions is null || cliExclusions.Count == 0) &&
           (fileExclusions is null || fileExclusions.Count == 0)) {
            return [];
        }

        var merged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if(cliExclusions is not null) {
            foreach(var e in cliExclusions) {
                merged.Add(e);
            }
        }


        if(fileExclusions is not null) {
            foreach(var e in fileExclusions) {
                merged.Add(e);
            }
        }

        return [.. merged];
    }

    private static (string rawText, Func<Encoding> encodingFactory) ReadPreservingEncoding(string filePath) {
        var bytes = File.ReadAllBytes(filePath);

        if(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) {
            // UTF-8 BOM
            return (new UTF8Encoding(true).GetString(bytes, 3, bytes.Length - 3), () => new UTF8Encoding(true));
        }

        if(bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE) {
            // UTF-16 LE BOM
            return (new UnicodeEncoding(false, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(false, true));
        }

        if(bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF) {
            // UTF-16 BE BOM
            return (new UnicodeEncoding(true, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(true, true));
        }

        return (new UTF8Encoding(false).GetString(bytes), () => new UTF8Encoding(false));
    }

    private static void WritePreservingEncoding(string path, string text, Func<Encoding> encodingFactory) {
        var encoding = encodingFactory();
        File.WriteAllText(path, text, encoding);
    }

    private static string SerializeDocument(XmlDocument doc) {
        using var sw = new StringWriter();
        using var xw = XmlWriter.Create(sw, new XmlWriterSettings {
            Indent = false,
            OmitXmlDeclaration = doc.FirstChild is not XmlDeclaration,
            ConformanceLevel = ConformanceLevel.Document,
        });

        doc.Save(xw);
        return sw.ToString();
    }
}
