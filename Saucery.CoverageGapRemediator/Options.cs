namespace Saucery.CoverageGapRemediator;

internal sealed record Options(
    string? RepositoryRoot,
    string ReportPath,
    bool Apply,
    bool AllowObsoleteRemoval,
    bool SkipValidation,
    bool KeepChangesOnValidationFailure,
    bool VerifyWithReporter,
    int MaxRepairPasses,
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
        var maxRepairPasses = 3;
        var planJson = "saucery-platform-remediation-plan.json";
        var planMarkdown = "saucery-platform-remediation-plan.md";

        for(var i = 0; i < args.Length; i++)
        {
            switch(args[i])
            {
                case "--repo-root": repoRoot = Value(args, ref i, "--repo-root"); break;
                case "--report": report = Value(args, ref i, "--report"); break;
                case "--apply": apply = true; break;
                case "--allow-obsolete-removal": allowObsoleteRemoval = true; break;
                case "--skip-validation": skipValidation = true; break;
                case "--keep-on-validation-failure": keepChanges = true; break;
                case "--verify-with-reporter": verifyReporter = bool.Parse(Value(args, ref i, "--verify-with-reporter")); break;
                case "--max-repair-passes": maxRepairPasses = int.Parse(Value(args, ref i, "--max-repair-passes")); break;
                case "--plan-json": planJson = Value(args, ref i, "--plan-json"); break;
                case "--plan-markdown": planMarkdown = Value(args, ref i, "--plan-markdown"); break;
                case "--help":
                case "-h": PrintHelp(); Environment.Exit(0); break;
                default: throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        if(maxRepairPasses < 1) throw new ArgumentOutOfRangeException(nameof(maxRepairPasses));
        return new(repoRoot, report, apply, allowObsoleteRemoval, skipValidation, keepChanges,
            verifyReporter, maxRepairPasses, planJson, planMarkdown);
    }

    private static string Value(string[] args, ref int index, string option) =>
        index + 1 < args.Length ? args[++index] : throw new ArgumentException($"{option} requires a value.");

    private static void PrintHelp() => Console.WriteLine(
        """
        Saucery.CoverageGapRemediator

        Repository-driven remediation for discrepancies emitted by Saucery.CoverageGapReporter.
        The remediator discovers existing platform implementation patterns instead of using
        platform-specific strategy classes.

        Options:
          --repo-root <path>                Saucery repository root. Default: auto-detect.
          --report <path>                   Reporter .txt or .json file.
          --apply                           Apply inferred remediations.
          --allow-obsolete-removal          Allow destructive obsolete-candidate remediation.
          --skip-validation                 Skip build/test/reporter validation.
          --keep-on-validation-failure      Keep edits after failed validation.
          --verify-with-reporter <bool>     Re-run reporter. Default: true.
          --max-repair-passes <n>           Iterative synthesis passes. Default: 3.
          --plan-json <path>                JSON plan output.
          --plan-markdown <path>            Markdown plan output.
        """);
}
