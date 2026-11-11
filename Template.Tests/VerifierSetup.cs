using Microsoft.TemplateEngine.Authoring.TemplateVerifier;

namespace Template.Tests;

public static class VerifierSetup {
    private static readonly string[] _buildOutputFolders = ["bin", "obj"];

    [Before(Assembly)]
    public static void ConfigureVerification() {
        RemoveBuildOutputFromSnapshots();

        // TemplateVerifier 11.x no longer bundles a framework-specific snapshot verifier;
        // it must be told which test framework's VerifyDirectory to use.
        // A lambda is used instead of a method group because Verify.TUnit 33.x adds a
        // trailing optional lineNumber parameter that the delegate doesn't declare.
        VerificationEngine.DirectoryVerifier =
            (path, include, pattern, options, settings, info, fileScrubber, sourceFile) =>
                VerifyTUnit.Verifier.VerifyDirectory(path, include, pattern, options, settings, info, fileScrubber, sourceFile);
    }

    // The verified snapshots contain real .csproj files, so IDE design-time builds can drop
    // (gitignored) bin/obj folders into them. Verify treats any extra file in a .verified
    // directory as a failure, so clear them out before verifying.
    private static void RemoveBuildOutputFromSnapshots() {
        var snapshotsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Snapshots"));
        if (!Directory.Exists(snapshotsDir)) {
            return;
        }

        var strayDirs = Directory.EnumerateDirectories(snapshotsDir, "*", SearchOption.AllDirectories)
            .Where(d => _buildOutputFolders.Contains(Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var dir in strayDirs.Where(Directory.Exists)) {
            Directory.Delete(dir, recursive: true);
        }
    }
}
