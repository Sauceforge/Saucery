using Saucery.NuGet.Models;
using System.Text;
using System.Xml;

namespace Saucery.NuGet.Core;

public sealed class CsprojUpdater(INuGetApiClient apiClient) {
    public static bool IsOptedIn(string projectPath) {
        // Exclude the test project for the tool itself to avoid self-updating during scans
        var projectFileName = Path.GetFileNameWithoutExtension(projectPath);
        if (projectFileName != null && projectFileName.Equals("Saucery.NuGet.Tests", StringComparison.OrdinalIgnoreCase))
            return false;
        try {
            var doc = new XmlDocument();
            doc.Load(projectPath);

            // Check PackageReference Include="Saucery.NuGet"
            var packageRefs = doc.SelectNodes($"//*[local-name()='{Constants.Xml.PackageReferenceElement}' and @{Constants.Xml.IncludeAttribute}]");
            if(packageRefs is not null) {
                foreach(XmlElement pr in packageRefs.Cast<XmlElement>()) {
                    var include = pr.GetAttribute(Constants.Xml.IncludeAttribute);
                    if(include.Equals(Constants.Package.OptInPackageId, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            // Check ProjectReference Include="..\Saucery.NuGet.csproj" or <ProjectReference> with <Name>Saucery.NuGet</Name>
            var projectRefs = doc.SelectNodes("//*[local-name()='ProjectReference' and @Include]");
            if(projectRefs is not null) {
                foreach(XmlElement projRef in projectRefs.Cast<XmlElement>()) {
                    var include = projRef.GetAttribute("Include");
                    if(!string.IsNullOrEmpty(include)) {
                        var fileName = Path.GetFileName(include.Replace('/', Path.DirectorySeparatorChar));
                        if(fileName.Equals(Constants.Package.OptInPackageId + ".csproj", StringComparison.OrdinalIgnoreCase))
                            return true;
                    }

                    var nameNode = projRef.SelectSingleNode("*[local-name()='Name']") as XmlElement;
                    if(nameNode is not null) {
                        var name = nameNode.InnerText?.Trim();
                        if(name is not null && name.Equals(Constants.Package.OptInPackageId, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
        }
        catch (Exception) {
            // Fall back to the old textual check if XML parsing fails for some reason
            var text = File.ReadAllText(projectPath);
            return text.Contains($"{Constants.Xml.IncludeAttribute}=\"{Constants.Package.OptInPackageId}\"", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public async Task<UpdateResult> UpdateAsync(
        string projectPath,
        bool includePrerelease = false,
        bool dryRun = false,
        bool bumpOwnVersion = false,
        VersionSegment versionSegment = VersionSegment.Patch,
        CancellationToken ct = default) 
    {
        var (rawText, encodingFactory) = ReadPreservingEncoding(projectPath);

        XmlDocument doc;
        try {
            doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(rawText);
        }
        catch (XmlException ex) {
            return new UpdateResult(projectPath, [], $"Failed to parse XML: {ex.Message}");
        }

        var packageRefs = doc.SelectNodes($"//*[local-name()='{Constants.Xml.PackageReferenceElement}' and @{Constants.Xml.IncludeAttribute} and @{Constants.Xml.VersionAttribute}]");
        if(packageRefs is null || packageRefs.Count == 0)
            return new UpdateResult(projectPath, []);

        var updates = new List<PackageUpdate>();

        foreach(XmlElement node in packageRefs.Cast<XmlElement>()) 
        {
            var id = node.GetAttribute(Constants.Xml.IncludeAttribute);
            var currentVersion = node.GetAttribute(Constants.Xml.VersionAttribute);

            //Skip if it's the opt-in marker itself
            if(id.Equals(Constants.Package.OptInPackageId, StringComparison.OrdinalIgnoreCase))
                continue;

            var avaiable = await apiClient.GetAvailableVersionsAsync(id, ct).ConfigureAwait(false);
            var next = VersionResolver.FindNextVersion(currentVersion, avaiable, includePrerelease);

            if(next is null || next == currentVersion)
                continue;

            updates.Add(new PackageUpdate(projectPath, id, currentVersion, next));

            if(!dryRun)
                node.SetAttribute(Constants.Xml.VersionAttribute, next);
        }

        if(!dryRun && updates.Count > 0) 
        {
            var updated = SerializeDocument(doc);
            WritePreservingEncoding(projectPath, updated, encodingFactory);
        }

        string? newPackageVersion = null;
        if(bumpOwnVersion && updates.Count > 0) 
        {
            newPackageVersion = PackageVersionBumper.Bump(projectPath, versionSegment, dryRun);
        }

        return new UpdateResult(projectPath, updates, NewPackageVersion: newPackageVersion);
    }

    private static (string Text, Func<Encoding> EncodingFactory) ReadPreservingEncoding(string path) 
    {
        var bytes = File.ReadAllBytes(path);
        if(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return (new UTF8Encoding(true).GetString(bytes, 3, bytes.Length - 3), () => new UTF8Encoding(true));

        if(bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return (new UnicodeEncoding(false, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(false, true));

        if(bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return (new UnicodeEncoding(true, true).GetString(bytes, 2, bytes.Length - 2), () => new UnicodeEncoding(true, true));

        return (new UTF8Encoding(false).GetString(bytes), () => new UTF8Encoding(false));
    }

    private static void WritePreservingEncoding(string path, string text, Func<Encoding> encodingFactory) 
    {
        var encoding = encodingFactory();
        File.WriteAllText(path, text, encoding);
    }

    private static string SerializeDocument(XmlDocument doc) 
    {
        using var sw = new StringWriter();
        using var xw = XmlWriter.Create(sw, new XmlWriterSettings 
        { 
            Indent = false,
            OmitXmlDeclaration = doc.FirstChild is not XmlDeclaration, 
            ConformanceLevel = ConformanceLevel.Document 
        });
        doc.Save(xw);
        return sw.ToString();
    }
}
