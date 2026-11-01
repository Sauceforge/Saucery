using System.Text;
using System.Text.Json;

namespace Saucery.CoverageGapRemediator;

internal static class Program
{
    internal const string Build = "coverage-remediator-v1";

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var options = Options.Parse(args);
            var repoRoot = RepositoryLocator.Resolve(options.RepositoryRoot);
            var reportPath = ResolvePath(repoRoot, options.ReportPath);

            Console.WriteLine($"Remediator build: {Build}");
            Console.WriteLine($"Repository: {repoRoot}");
            Console.WriteLine($"Report: {reportPath}");

            if (!File.Exists(reportPath))
            {
                throw new FileNotFoundException("Coverage report was not found.", reportPath);
            }

            var discrepancies = await CoverageReportReader.ReadAsync(reportPath);
            Console.WriteLine($"Discrepancies read: {discrepancies.Count:N0}");

            var context = new RepositoryContext(repoRoot);
            var engine = RemediationEngine.CreateDefault();
            var plan = engine.BuildPlan(context, reportPath, discrepancies, options);

            var planJsonPath = ResolvePath(repoRoot, options.PlanJsonPath);
            var planMarkdownPath = ResolvePath(repoRoot, options.PlanMarkdownPath);
            await PlanWriter.WriteAsync(plan, planJsonPath, planMarkdownPath);

            Console.WriteLine($"Automatic fixes available: {plan.AutoFixCount:N0}");
            Console.WriteLine($"Manual-review items: {plan.ManualReviewCount:N0}");
            Console.WriteLine($"Plan JSON: {planJsonPath}");
            Console.WriteLine($"Plan Markdown: {planMarkdownPath}");

            if (!options.Apply)
            {
                Console.WriteLine("Plan-only mode: no source files were changed.");
                return plan.ManualReviewCount == 0 ? 0 : 2;
            }

            var transaction = new RepositoryTransaction();
            try
            {
                var applyResult = await engine.ApplyAsync(context, plan, options, transaction);

                if (!options.SkipValidation)
                {
                    // The reporter currently refreshes the root README and its default text report.
                    // Capture those side effects as part of the same transaction so a failed
                    // validation can restore the repository completely.
                    transaction.Capture(Path.Combine(repoRoot, "README.md"));
                    transaction.Capture(Path.Combine(repoRoot, "saucery-functional-platform-coverage-gaps.txt"));

                    var validator = new RepositoryValidator();
                    var validationTargets = plan.Items
                        .Where(x => x.Disposition == RemediationDisposition.AutoFix)
                        .Select(x => x.Discrepancy)
                        .ToArray();
                    var validation = await validator.ValidateAsync(context, validationTargets, options);
                    foreach (var message in validation.Messages)
                    {
                        Console.WriteLine(message);
                    }

                    if (!validation.Success)
                    {
                        if (!options.KeepChangesOnValidationFailure)
                        {
                            transaction.Rollback();
                            Console.Error.WriteLine("Validation failed. Source changes were rolled back.");
                        }
                        else
                        {
                            Console.Error.WriteLine("Validation failed. Changes were retained because --keep-on-validation-failure was supplied.");
                        }

                        return 3;
                    }
                }

                transaction.Commit();
                PrintApplySummary(applyResult);
                return plan.ManualReviewCount == 0 ? 0 : 2;
            }
            catch
            {
                if (!options.KeepChangesOnValidationFailure)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static string ResolvePath(string repoRoot, string value) =>
        Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(repoRoot, value));

    private static void PrintApplySummary(ApplyResult result)
    {
        Console.WriteLine();
        Console.WriteLine("Remediation applied successfully.");
        Console.WriteLine($"Changed files: {result.ChangedFiles.Count:N0}");
        Console.WriteLine($"Created files: {result.CreatedFiles.Count:N0}");
        Console.WriteLine($"Deleted files: {result.DeletedFiles.Count:N0}");
    }
}

internal static class PlanWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteAsync(RemediationPlan plan, string jsonPath, string markdownPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(markdownPath)!);

        await File.WriteAllTextAsync(
            jsonPath,
            JsonSerializer.Serialize(plan, JsonOptions),
            new UTF8Encoding(false));

        var markdown = new StringBuilder();
        markdown.AppendLine("# Saucery Platform Coverage Remediation Plan");
        markdown.AppendLine();
        markdown.AppendLine($"Generated: `{plan.GeneratedUtc:O}`");
        markdown.AppendLine();
        markdown.AppendLine("| Discrepancy | Disposition | Strategy | Summary |");
        markdown.AppendLine("| --- | --- | --- | --- |");

        foreach (var item in plan.Items)
        {
            markdown.Append("| ")
                .Append(Escape(item.Discrepancy.DisplayName))
                .Append(" (" + item.Discrepancy.Type + ") | ")
                .Append(item.Disposition)
                .Append(" | ")
                .Append(Escape(item.Strategy))
                .Append(" | ")
                .Append(Escape(item.Summary))
                .AppendLine(" |");
        }

        foreach (var item in plan.Items)
        {
            markdown.AppendLine();
            markdown.AppendLine($"## {item.Discrepancy.Type}: {item.Discrepancy.DisplayName}");
            markdown.AppendLine();
            markdown.AppendLine($"Strategy: `{item.Strategy}`");
            markdown.AppendLine();
            markdown.AppendLine(item.Summary);

            if (item.PlannedChanges.Count > 0)
            {
                markdown.AppendLine();
                markdown.AppendLine("Planned changes:");
                foreach (var change in item.PlannedChanges)
                {
                    markdown.AppendLine($"- **{change.ChangeType}** `{change.Path}` — {change.Description}");
                }
            }

            if (item.Warnings.Count > 0)
            {
                markdown.AppendLine();
                markdown.AppendLine("Warnings:");
                foreach (var warning in item.Warnings)
                {
                    markdown.AppendLine($"- {warning}");
                }
            }
        }

        await File.WriteAllTextAsync(markdownPath, markdown.ToString(), new UTF8Encoding(false));
    }

    private static string Escape(string value) => value.Replace("|", "\\|", StringComparison.Ordinal);
}
