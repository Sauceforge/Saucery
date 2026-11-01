namespace Saucery.CoverageGapRemediator;

internal sealed record Options(
    string? RepositoryRoot,
    string ReportPath,
    bool Apply,
    bool AllowObsoleteRemoval,
    bool SkipValidation,
    bool KeepChangesOnValidationFailure,
    bool VerifyWithReporter,
    string PlanJsonPath,
    string PlanMarkdownPath)
{
    public static Options Parse(string[] args)
    {
        string? repoRoot = null;
        var report = "saucery-functional-platform-coverage-gaps.txt";
        var apply = false;
        var allowObsoleteRemoval = false;
        var skipValidation = false;
        var keepChanges = false;
        var verifyReporter = true;
        var planJson = "saucery-platform-remediation-plan.json";
        var planMarkdown = "saucery-platform-remediation-plan.md";

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--repo-root":
                    repoRoot = RequireValue(args, ref i, "--repo-root");
                    break;
                case "--report":
                    report = RequireValue(args, ref i, "--report");
                    break;
                case "--apply":
                    apply = true;
                    break;
                case "--allow-obsolete-removal":
                    allowObsoleteRemoval = true;
                    break;
                case "--skip-validation":
                    skipValidation = true;
                    break;
                case "--keep-on-validation-failure":
                    keepChanges = true;
                    break;
                case "--verify-with-reporter":
                    verifyReporter = bool.Parse(RequireValue(args, ref i, "--verify-with-reporter"));
                    break;
                case "--plan-json":
                    planJson = RequireValue(args, ref i, "--plan-json");
                    break;
                case "--plan-markdown":
                    planMarkdown = RequireValue(args, ref i, "--plan-markdown");
                    break;
                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        return new Options(
            repoRoot,
            report,
            apply,
            allowObsoleteRemoval,
            skipValidation,
            keepChanges,
            verifyReporter,
            planJson,
            planMarkdown);
    }

    private static string RequireValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"{option} requires a value.");
        }

        return args[++index];
    }

    private static void PrintHelp() => Console.WriteLine(
        """
        Saucery.CoverageGapRemediator

        Builds and optionally applies deterministic remediation plans for discrepancies emitted by
        Saucery.CoverageGapReporter.

        Usage:
          dotnet run -c Release -- [options]

        Options:
          --repo-root <path>                Saucery repository root. Default: auto-detect.
          --report <path>                   Reporter .txt or .json file.
                                            Default: saucery-functional-platform-coverage-gaps.txt
          --apply                           Apply supported automatic remediations.
          --allow-obsolete-removal          Permit destructive removal of obsolete candidates.
                                            Without this, obsolete items are plan-only/manual-review.
          --skip-validation                 Do not run syntax/build/test validation after applying.
          --keep-on-validation-failure      Do not roll back edits when validation fails.
          --verify-with-reporter <bool>     Re-run CoverageGapReporter after validation. Default: true.
          --plan-json <path>                JSON plan output path.
          --plan-markdown <path>            Markdown plan output path.
          --help                            Show help.

        Safe default:
          Running with no --apply argument NEVER edits Saucery.Core or Saucery.Core.Tests.
        """);
}
