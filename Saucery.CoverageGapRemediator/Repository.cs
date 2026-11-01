using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace Saucery.CoverageGapRemediator;

internal static class RepositoryLocator
{
    public static string Resolve(string? configured)
    {
        if(!string.IsNullOrWhiteSpace(configured))
        {
            var root = Path.GetFullPath(configured);
            Validate(root);
            return root;
        }

        foreach(var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory }.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase))
            for(var dir = new DirectoryInfo(start); dir is not null; dir = dir.Parent)
                if(IsRoot(dir.FullName)) return dir.FullName;

        throw new DirectoryNotFoundException("Could not locate a Saucery repository root containing Saucery.Core and Saucery.Core.Tests. Supply --repo-root <path>.");
    }

    private static void Validate(string root)
    {
        if(!IsRoot(root)) throw new DirectoryNotFoundException($"'{root}' does not contain Saucery.Core and Saucery.Core.Tests.");
    }

    private static bool IsRoot(string root) => Directory.Exists(Path.Combine(root, "Saucery.Core")) && Directory.Exists(Path.Combine(root, "Saucery.Core.Tests"));
}

internal sealed class RepositoryContext(string root)
{
    public string Root { get; } = root;
    public string Core => Path.Combine(Root, "Saucery.Core");
    public string Tests => Path.Combine(Root, "Saucery.Core.Tests");
    public string ReporterProject => Path.Combine(Root, "Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj");
    public string PathFromRoot(string relative) => Path.GetFullPath(Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar)));
    public string Relative(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');
}

internal sealed class RepositoryTransaction
{
    private readonly Dictionary<string, byte[]?> originals = new(StringComparer.OrdinalIgnoreCase);
    private bool completed;

    public void Capture(string path)
    {
        path = Path.GetFullPath(path);
        if(!originals.ContainsKey(path)) originals[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
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
        if(File.Exists(path)) File.Delete(path);
    }

    public void Rollback()
    {
        if(completed) return;
        foreach(var (path, bytes) in originals.Reverse())
        {
            if(bytes is null) { if(File.Exists(path)) File.Delete(path); }
            else { Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllBytes(path, bytes); }
        }
        completed = true;
    }

    public void Commit() => completed = true;
}

internal sealed class ChangeCollector(RepositoryContext context, RepositoryTransaction transaction)
{
    private readonly HashSet<string> changed = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> created = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> deleted = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> ChangedFiles => changed;
    public IReadOnlyCollection<string> CreatedFiles => created;
    public IReadOnlyCollection<string> DeletedFiles => deleted;

    public string Read(string relative) => File.ReadAllText(context.PathFromRoot(relative));

    public void Write(string relative, string content)
    {
        var path = context.PathFromRoot(relative);
        var existed = File.Exists(path);
        if(path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) CSharpGuard.AssertValid(content, path);
        var old = existed ? File.ReadAllText(path) : null;
        if(old == content) return;
        transaction.WriteText(path, content);
        (existed ? changed : created).Add(context.Relative(path));
    }

    public void Modify(string relative, Func<string, string> edit)
    {
        var path = context.PathFromRoot(relative);
        if(!File.Exists(path)) throw new FileNotFoundException($"Required source file was not found: {relative}", path);
        Write(relative, edit(File.ReadAllText(path)));
    }

    public void Delete(string relative)
    {
        var path = context.PathFromRoot(relative);
        if(!File.Exists(path)) return;
        transaction.Delete(path);
        deleted.Add(context.Relative(path));
    }
}

internal static class CSharpGuard
{
    public static void AssertValid(string source, string file)
    {
        var errors = CSharpSyntaxTree.ParseText(source, path: file).GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToArray();
        if(errors.Length > 0) throw new InvalidDataException($"Generated invalid C# in {file}:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
    }
}
