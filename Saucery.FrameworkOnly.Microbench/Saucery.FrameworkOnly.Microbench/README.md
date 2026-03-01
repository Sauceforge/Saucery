# Saucery.FrameworkOnly.Microbench

Framework-only microbenchmarks that run test assemblies **in-process** (no `dotnet test`, no VSTest/MTP host startup).

- xUnit runner utility package: `xunit.v3.runner.utility` citeturn0search0turn0search1
- NUnit in-process runner API: `NUnitLite.AutoRun` citeturn3search6

## Run

```bash
dotnet run -c Release --project Saucery.FrameworkOnly.Microbench\Saucery.FrameworkOnly.Microbench\Saucery.FrameworkOnly.Microbench.csproj -- --filter *FrameworkOnlyBenchmarks*
```
