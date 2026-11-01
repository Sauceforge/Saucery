using System.Text;
using System.Text.Json;

namespace Saucery.CoverageGapRemediator;

internal static class Program
{
    internal const string Build = "coverage-remediator-v18-delete-product-and-creator-only";

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var options = Options.Parse(args);
            var repoRoot = RepositoryLocator.Resolve(options.RepositoryRoot);
            var reportPath = Resolve(repoRoot, options.ReportPath);
            Console.WriteLine($"Remediator build: {Build}");
            Console.WriteLine($"Repository: {repoRoot}");
            Console.WriteLine($"Report: {reportPath}");
            if(!File.Exists(reportPath)) throw new FileNotFoundException("Coverage report was not found.", reportPath);

            var discrepancies = await CoverageReportReader.ReadAsync(reportPath);
            Console.WriteLine($"Discrepancies read: {discrepancies.Count:N0}");
            var context = new RepositoryContext(repoRoot);
            var engine = new GenericRemediationEngine();
            var plan = engine.BuildPlan(context, reportPath, discrepancies, options);
            await PlanWriter.WriteAsync(plan, Resolve(repoRoot, options.PlanJsonPath), Resolve(repoRoot, options.PlanMarkdownPath));
            Console.WriteLine($"Automatic fixes inferred: {plan.AutoFixCount:N0}");
            Console.WriteLine($"Manual-review items: {plan.ManualReviewCount:N0}");

            if(!options.Apply)
            {
                Console.WriteLine("Plan-only mode: no source files were changed.");
                return 0;
            }

            var transaction = new RepositoryTransaction();
            try
            {
                var result = await engine.ApplyAsync(context, plan, options, transaction);
                transaction.Commit();
                Console.WriteLine("Remediation applied successfully.");
                Console.WriteLine($"Changed files: {result.ChangedFiles.Count:N0}");
                Console.WriteLine($"Created files: {result.CreatedFiles.Count:N0}");
                Console.WriteLine($"Deleted files: {result.DeletedFiles.Count:N0}");
                return 0;
            }
            catch
            {
                if(!options.KeepChangesOnValidationFailure) transaction.Rollback();
                throw;
            }
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static string Resolve(string root, string value) => Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(root, value));
}

internal static class PlanWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task WriteAsync(RemediationPlan plan, string jsonPath, string markdownPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(markdownPath)!);
        await File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(plan, JsonOptions), new UTF8Encoding(false));

        var md = new StringBuilder("# Saucery Platform Coverage Remediation Plan\n\n");
        md.AppendLine("This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.\n");
        md.AppendLine("| Discrepancy | Disposition | Inference |\n| --- | --- | --- |");
        foreach(var item in plan.Items)
            md.AppendLine($"| {Escape(item.Discrepancy.DisplayName)} ({item.Discrepancy.Type}) | {item.Disposition} | {Escape(item.Summary)} |");
        foreach(var item in plan.Items)
        {
            md.AppendLine($"\n## {item.Discrepancy.Type}: {item.Discrepancy.DisplayName}\n\n{item.Summary}\n");
            foreach(var change in item.PlannedChanges) md.AppendLine($"- **{change.ChangeType}** `{change.Path}` — {change.Description}");
            foreach(var warning in item.Warnings) md.AppendLine($"- ⚠ {warning}");
        }
        await File.WriteAllTextAsync(markdownPath, md.ToString(), new UTF8Encoding(false));
    }

    private static string Escape(string value) => value.Replace("|", "\\|", StringComparison.Ordinal);
}
