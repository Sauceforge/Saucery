using Saucery.NuGet.Models;
using System.Text;
using System.Xml;

namespace Saucery.NuGet.Core;

public sealed class CsprojUpdater(INuGetApiClient apiClient) {
    public static bool IsOptedIn(string projectPath) {
        var projectFileName = Path.GetFileNameWithoutExtension(projectPath);
        if(projectFileName != null && projectFileName.Equals("Saucery.NuGet.Tests", StringComparison.OrdinalIgnoreCase))
            return false;

        const string optInPropertyName = "SauceryNuGetOptIn";

        try {
            var doc = new XmlDocument();
            doc.Load(projectPath);

            var optInNodes = doc.SelectNodes($"//*[local-name()='{optInPropertyName}']");
            if(optInNodes is not null) {
                foreach(XmlElement node in optInNodes.Cast<XmlElement>()) {
                    var value = node.InnerText?.Trim();

                    if(string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
        } catch(Exception) {
            var text = File.ReadAllText(projectPath);

            if(text.Contains($"<{optInPropertyName}>true</{optInPropertyName}>", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public async Task<UpdateResult> UpdateAsync(
        string projectPath,
        bool includePrerelease = false,
        bool dryRun = false,
        bool bumpOwnVersion = false,
        VersionSegment versionSegment = VersionSegment.Patch,
        string? syncWithPackageId = null,
        CancellationToken ct = default) {
        var (rawText, encodingFactory) = ReadPreservingEncoding(projectPath);

        XmlDocument doc;
        try {
            doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(rawText);
        } catch(XmlException ex) {
            return new UpdateResult(projectPath, [], $"Failed to parse XML: {ex.Message}");
        }

        var packageRefs = doc.SelectNodes(
            $"//*[local-name()='{Constants.Xml.PackageReferenceElement}' and @{Constants.Xml.IncludeAttribute} and @{Constants.Xml.VersionAttribute}]");

        if(packageRefs is null || packageRefs.Count == 0)
            return new UpdateResult(projectPath, []);

        var updates = new List<PackageUpdate>();

        foreach(XmlElement node in packageRefs.Cast<XmlElement>()) {
            var id = node.GetAttribute(Constants.Xml.IncludeAttribute);
            var currentVersion = node.GetAttribute(Constants.Xml.VersionAttribute);

            // Skip if it's the opt-in marker itself
            if(id.Equals(Constants.Package.OptInPackageId, StringComparison.OrdinalIgnoreCase))
                continue;

            var available = await apiClient.GetAvailableVersionsAsync(id, ct).ConfigureAwait(false);
            var next = VersionResolver.FindNextVersion(currentVersion, available, includePrerelease);

            if(next is null || next == currentVersion)
                continue;

            updates.Add(new PackageUpdate(projectPath, id, currentVersion, next));

            if(!dryRun)
                node.SetAttribute(Constants.Xml.VersionAttribute, next);
        }

        // Build a lookup of proposed new versions for packages updated during this run
        var proposedUpdates = updates.ToDictionary(u => u.PackageId, u => u.ToVersion, StringComparer.OrdinalIgnoreCase);

        // Track whether we modified the in-memory XmlDocument and therefore need to persist it
        var docModified = false;

        if(!dryRun && updates.Count > 0) {
            var updated = SerializeDocument(doc);
            WritePreservingEncoding(projectPath, updated, encodingFactory);
        }

        string? newPackageVersion = null;

        // If syncWithPackageId is provided, set the project's PackageVersion to the dependency's version
        if(!string.IsNullOrWhiteSpace(syncWithPackageId)) {
            string? depVersion = null;

            // 1) Try PackageReference matching
            var syncPkgNode = doc.SelectSingleNode(
                $"//*[local-name()='{Constants.Xml.PackageReferenceElement}' and translate(@{Constants.Xml.IncludeAttribute}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{syncWithPackageId.ToLowerInvariant()}' and @{Constants.Xml.VersionAttribute}]")
                as XmlElement;

            if(syncPkgNode is not null) {
                var depId = syncPkgNode.GetAttribute(Constants.Xml.IncludeAttribute);
                if(!string.IsNullOrWhiteSpace(depId) && proposedUpdates.TryGetValue(depId, out var proposed)) {
                    depVersion = proposed;
                } else {
                    depVersion = syncPkgNode.GetAttribute(Constants.Xml.VersionAttribute);
                }
            }

            // 2) If not found as PackageReference, attempt ProjectReference matching
            if(depVersion is null) {
                var projectRefs = doc.SelectNodes("//*[local-name()='ProjectReference' and @Include]") as XmlNodeList;
                if(projectRefs is not null) {
                    foreach(XmlElement projRef in projectRefs.Cast<XmlElement>()) {
                        var include = projRef.GetAttribute("Include");
                        if(string.IsNullOrWhiteSpace(include))
                            continue;

                        var normalizedInclude = NormalizeProjectReferencePath(include);
                        var fileNameNoExt = Path.GetFileNameWithoutExtension(normalizedInclude);
                        var fileName = Path.GetFileName(normalizedInclude);

                        if(string.Equals(fileNameNoExt, syncWithPackageId, StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(fileName, syncWithPackageId + ".csproj", StringComparison.OrdinalIgnoreCase)) {
                            // resolve path relative to projectPath
                            var resolved = Path.GetFullPath(
                                Path.Combine(Path.GetDirectoryName(projectPath) ?? string.Empty, normalizedInclude));

                            if(File.Exists(resolved)) {
                                // Prefer reading PackageVersion from the referenced project's file
                                // (it may have been updated earlier in the run)
                                depVersion = PackageVersionBumper.ReadPackageVersion(resolved);
                                if(!string.IsNullOrWhiteSpace(depVersion))
                                    break;

                                // As a fallback, try to read <PackageId> from referenced project and continue searching
                                try {
                                    var refDoc = new XmlDocument();
                                    refDoc.Load(resolved);

                                    var pkgIdNode = refDoc.SelectSingleNode("//*[local-name()='PackageId']") as XmlElement;
                                    if(pkgIdNode is not null) {
                                        var pkgId = pkgIdNode.InnerText?.Trim();
                                        if(string.Equals(pkgId, syncWithPackageId, StringComparison.OrdinalIgnoreCase)) {
                                            depVersion = PackageVersionBumper.ReadPackageVersion(resolved);
                                            if(!string.IsNullOrWhiteSpace(depVersion))
                                                break;
                                        }
                                    }
                                } catch {
                                    // Ignore referenced project parse issues
                                }
                            }
                        }
                    }
                }
            }

            if(!string.IsNullOrWhiteSpace(depVersion)) {
                var currentPackageVersion = PackageVersionBumper.ReadPackageVersion(projectPath);
                if(!string.Equals(currentPackageVersion, depVersion, StringComparison.OrdinalIgnoreCase)) {
                    if(!dryRun) {
                        // Modify the in-memory XmlDocument to set PackageVersion
                        var pkgNode = doc.SelectSingleNode("//*[local-name()='PackageVersion']") as XmlElement;
                        if(pkgNode is not null) {
                            pkgNode.InnerText = depVersion;
                        } else {
                            var propGroup = doc.SelectSingleNode("//*[local-name()='PropertyGroup']") as XmlElement;
                            if(propGroup is not null) {
                                var newElem = doc.CreateElement("PackageVersion");
                                newElem.InnerText = depVersion;
                                propGroup.AppendChild(newElem);
                            } else {
                                var projectRoot = doc.DocumentElement;
                                if(projectRoot is not null) {
                                    var pg = doc.CreateElement("PropertyGroup");
                                    var newElem = doc.CreateElement("PackageVersion");
                                    newElem.InnerText = depVersion;
                                    pg.AppendChild(newElem);
                                    projectRoot.AppendChild(pg);
                                }
                            }
                        }

                        docModified = true;
                    }

                    // Record the PackageVersion sync as an update so result.Updates.Count reflects it
                    updates.Add(new PackageUpdate(
                        projectPath,
                        "PackageVersion",
                        currentPackageVersion ?? string.Empty,
                        depVersion));

                    newPackageVersion = depVersion;
                }
            }
        }

        // If syncing wasn't requested or didn't produce a new version,
        // fall back to bumping own version when requested
        var skipFinalWrite = false;
        if(newPackageVersion is null && bumpOwnVersion && updates.Count > 0) {
            var bumped = PackageVersionBumper.Bump(projectPath, versionSegment, dryRun);
            newPackageVersion = bumped;

            if(!dryRun && bumped is not null) {
                // PackageVersionBumper.Bump already wrote the file when not dryRun,
                // so skip the final write to avoid overwriting
                skipFinalWrite = true;
            }
        }

        // Persist the modified XmlDocument once if any modifications were made (and not a dry run)
        if(!dryRun && !skipFinalWrite && (docModified || updates.Count > 0)) {
            var final = SerializeDocument(doc);
            WritePreservingEncoding(projectPath, final, encodingFactory);
        }

        return new UpdateResult(projectPath, updates, NewPackageVersion: newPackageVersion);
    }

    private static string NormalizeProjectReferencePath(string include) =>
        include.Replace('\\', '/');

    private static (string Text, Func<Encoding> EncodingFactory) ReadPreservingEncoding(string path) {
        var bytes = File.ReadAllBytes(path);

        if(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return (new UTF8Encoding(true).GetString(bytes, 3, bytes.Length - 3), () => new UTF8Encoding(true));

        if(bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return (new UnicodeEncoding(false, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(false, true));

        if(bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return (new UnicodeEncoding(true, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(true, true));

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
            ConformanceLevel = ConformanceLevel.Document
        });

        doc.Save(xw);
        return sw.ToString();
    }
}