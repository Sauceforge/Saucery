# Saucery.TestFrameworkBenchmarks

A BenchmarkDotNet harness that compares **end-to-end test-suite execution time** across the four Saucery test-framework projects:

- `Saucery.Core.Tests` (TUnit) **baseline** — executed via `dotnet run`
- `Saucery.Core.Tests.NUnit` — executed via `dotnet test`
- `Saucery.Core.Tests.XUnit` — executed via `dotnet test`
- `Saucery.Core.Tests.XUnitv3` — executed via `dotnet test`

## Usage (from the Saucery repo root)

1) Drop this folder alongside the other Saucery projects (repo root):

```
Saucery.TestFrameworkBenchmarks/
```

2) Run:

```bash
dotnet run -c Release --project Saucery.TestFrameworkBenchmarks/Saucery.TestFrameworkBenchmarks.csproj
```

Optional: filter benchmarks:

```bash
dotnet run -c Release --project Saucery.TestFrameworkBenchmarks/Saucery.TestFrameworkBenchmarks.csproj -- --filter *TestFrameworkBenchmarks*
```

## Notes

- This is an **E2E benchmark** (process start + discovery + execution + teardown).
- `[GlobalSetup]` builds all four projects in Release; iterations run with `--no-build`.
