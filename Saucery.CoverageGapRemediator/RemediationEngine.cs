namespace Saucery.CoverageGapRemediator;

internal interface IRemediationStrategy
{
    string Name { get; }
    bool CanHandle(CoverageDiscrepancy discrepancy);
    RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy discrepancy, Options options);
    Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy discrepancy,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes);
}

internal sealed class RemediationEngine(IReadOnlyList<IRemediationStrategy> strategies)
{
    public static RemediationEngine CreateDefault() => new([
        new ChromiumOsMissingStrategy(),
        new MobileMissingStrategy(),
        new DesktopVersionMissingStrategy(),
        new ObsoletePlatformStrategy(),
        new ManualReviewStrategy()
    ]);

    public RemediationPlan BuildPlan(
        RepositoryContext context,
        string reportPath,
        IReadOnlyList<CoverageDiscrepancy> discrepancies,
        Options options)
    {
        var items = discrepancies
            .Select(discrepancy => Find(discrepancy).Plan(context, discrepancy, options))
            .ToArray();

        return new RemediationPlan(
            context.Root,
            reportPath,
            DateTimeOffset.UtcNow,
            items);
    }

    public async Task<ApplyResult> ApplyAsync(
        RepositoryContext context,
        RemediationPlan plan,
        Options options,
        RepositoryTransaction transaction)
    {
        var changes = new ChangeCollector(context, transaction);

        foreach (var item in plan.Items)
        {
            if (item.Disposition != RemediationDisposition.AutoFix)
            {
                continue;
            }

            var strategy = Find(item.Discrepancy);
            Console.WriteLine($"Applying {strategy.Name}: {item.Discrepancy.DisplayName}");
            await strategy.ApplyAsync(context, item.Discrepancy, options, transaction, changes);
        }

        return new ApplyResult(
            changes.ChangedFiles.OrderBy(x => x).ToArray(),
            changes.CreatedFiles.OrderBy(x => x).ToArray(),
            changes.DeletedFiles.OrderBy(x => x).ToArray(),
            Array.Empty<string>());
    }

    private IRemediationStrategy Find(CoverageDiscrepancy discrepancy) =>
        strategies.First(x => x.CanHandle(discrepancy));
}

internal sealed class ChangeCollector(RepositoryContext context, RepositoryTransaction transaction)
{
    private readonly HashSet<string> _changed = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _created = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _deleted = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> ChangedFiles => _changed;
    public IReadOnlyCollection<string> CreatedFiles => _created;
    public IReadOnlyCollection<string> DeletedFiles => _deleted;

    public string Read(string relative) => File.ReadAllText(context.RequireFile(relative));

    public void Write(string relative, string content)
    {
        var path = context.PathFromRoot(relative);
        var existed = File.Exists(path);
        if (Path.GetExtension(path).Equals(".cs", StringComparison.OrdinalIgnoreCase))
        {
            CSharpTextEditor.AssertValidCSharp(content, path);
        }

        transaction.WriteText(path, content);
        if (existed) _changed.Add(relative); else _created.Add(relative);
    }

    public void Delete(string relative)
    {
        var path = context.PathFromRoot(relative);
        if (!File.Exists(path)) return;
        transaction.Delete(path);
        _deleted.Add(relative);
    }

    public void Modify(string relative, Func<string, string> change)
    {
        var current = Read(relative);
        var updated = change(current);
        if (updated == current) return;
        Write(relative, updated);
    }
}
