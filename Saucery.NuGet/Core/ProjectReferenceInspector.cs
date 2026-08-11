using System.Xml;

namespace Saucery.NuGet.Core;

public static class ProjectReferenceInspector {
    /// <summary>
    /// Returns <c>true</c> when the project at <paramref name="projectPath"/> declares a
    /// <c>&lt;PackageReference Include="..." /&gt;</c> whose id is contained in
    /// <paramref name="updatedPackageIds"/>.
    /// </summary>
    /// <remarks>
    /// This drives the CPM <c>--bump-own-version</c> fallback: in a pure Central Package
    /// Management solution the csproj carries no versioned PackageReferences, so the only 
    /// way to know a project consumed an updated package is to match its (unversioned)
    /// PackageReference includes against the ids updated in Directory.Packages.props. 
    /// Case sensitivity of the match follows the comparer of the supplied set.
    /// Unreadable or malformed project files are treated as "does not reference".
    /// </remarks>
    public static bool ReferencesAnyUpdatedPackage(
    string projectPath,
    IReadOnlySet<string> updatedPackageIds) {
        try {
            var doc = new XmlDocument();
            doc.Load(projectPath);
            
            var refs = doc.SelectNodes("//*[local-name()='PackageReference' and @Include]");
            if(refs is null) {
                return false;
            }

            foreach(XmlElement node in refs.Cast<XmlElement>()) {
                if(updatedPackageIds.Contains(node.GetAttribute("Include"))) {
                    return true;
                }
            }

            return false;
        } catch {
            return false;
        }
    }
}
