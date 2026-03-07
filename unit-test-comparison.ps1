[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [ValidateRange(1, 100000)]
    [int]$Iterations = 1,

    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [string]$StartFrom = "",

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

function Show-Help {
    Write-Host ""
    Write-Host "Unit Test Comparison Runner"
    Write-Host "------------------------------------------------------------"
    Write-Host "Runs all configured test suites multiple times and reports"
    Write-Host "average execution time for each suite."
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\Run-UnitTestComparison.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Iterations <int>      Number of times each suite runs."
    Write-Host "                         Default: 1"
    Write-Host ""
    Write-Host "  -Configuration <str>   Build configuration."
    Write-Host "                         Default: Release"
    Write-Host ""
    Write-Host "  -StartFrom <name>      Start execution from this suite."
    Write-Host "                         Example: Saucery.Core.Tests.XUnit"
    Write-Host ""
    Write-Host "  -Help, -h, --help      Show this help message."
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  Run everything once:"
    Write-Host "    .\Run-UnitTestComparison.ps1"
    Write-Host ""
    Write-Host "  Run each suite 5 times:"
    Write-Host "    .\Run-UnitTestComparison.ps1 -Iterations 5"
    Write-Host ""
    Write-Host "  Start from XUnitv3:"
    Write-Host "    .\Run-UnitTestComparison.ps1 -StartFrom Saucery.Core.Tests.XUnitv3"
    Write-Host ""
    exit 0
}

if ($Help -or $args -contains "--help" -or $args -contains "/?") {
    Show-Help
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-ProjectPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectName
    )

    $expected = Join-Path $PSScriptRoot "$ProjectName\$ProjectName.csproj"

    if (Test-Path $expected) {
        return $expected
    }

    $found = Get-ChildItem -Path $PSScriptRoot -Recurse -Filter "$ProjectName.csproj" -File |
        Select-Object -First 1

    if ($found) {
        return $found.FullName
    }

    throw "Project '$ProjectName' not found."
}

function Invoke-TestSuite {
    param(
        [string]$Name,
        [string]$Mode,
        [string]$ProjectPath,
        [int]$Iterations,
        [string]$Configuration
    )

    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "Running suite: $Name"
    Write-Host "============================================================" -ForegroundColor Cyan

    $durations = @()

    for ($i = 1; $i -le $Iterations; $i++) {

        Write-Host "[$Name] Iteration $i of $Iterations" -ForegroundColor Yellow

        $sw = [System.Diagnostics.Stopwatch]::StartNew()

        if ($Mode -eq "dotnet-test") {
            & dotnet test $ProjectPath --configuration $Configuration --nologo -v normal
        }
        else {
            & dotnet run --project $ProjectPath --configuration $Configuration --no-build
        }

        $exitCode = $LASTEXITCODE
        $sw.Stop()

        $seconds = $sw.Elapsed.TotalSeconds
        $durations += $seconds

        if ($exitCode -ne 0) {
            throw "Suite '$Name' failed on iteration $i with exit code $exitCode after $seconds s"
        }

        Write-Host "Completed in $([Math]::Round($seconds,3)) s" -ForegroundColor Green
    }

    $avg = ($durations | Measure-Object -Average).Average
    $min = ($durations | Measure-Object -Minimum).Minimum
    $max = ($durations | Measure-Object -Maximum).Maximum
    $sum = ($durations | Measure-Object -Sum).Sum

    return [pscustomobject]@{
        Name = $Name
        AverageSeconds = [Math]::Round($avg,3)
        MinSeconds = [Math]::Round($min,3)
        MaxSeconds = [Math]::Round($max,3)
        TotalSeconds = [Math]::Round($sum,3)
    }
}

$suites = @(
    @{ Name="Saucery.Core.Tests"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.XUnit"; Mode="dotnet-test" }
    @{ Name="Saucery.Core.Tests.XUnitv3"; Mode="dotnet-test" }
    @{ Name="Saucery.Core.Tests.NUnit"; Mode="dotnet-test" }
    @{ Name="Saucery.Core.Tests.384"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.576"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.768"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.960"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.1152"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.1344"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.1536"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.1728"; Mode="dotnet-run" }
    @{ Name="Saucery.Core.Tests.1920"; Mode="dotnet-run" }
)

if ($StartFrom) {

    $index = $suites.FindIndex({ param($s) $s.Name -eq $StartFrom })

    if ($index -lt 0) {
        throw "StartFrom suite '$StartFrom' not found."
    }

    $suites = $suites[$index..($suites.Count-1)]
}

Write-Host "Restoring solution..." -ForegroundColor Cyan
dotnet restore

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build --configuration $Configuration --no-restore

$results = @()

foreach ($suite in $suites) {

    $path = Resolve-ProjectPath $suite.Name

    $result = Invoke-TestSuite `
        -Name $suite.Name `
        -Mode $suite.Mode `
        -ProjectPath $path `
        -Iterations $Iterations `
        -Configuration $Configuration

    $results += $result
}

Write-Host ""
Write-Host "==================== Average Runtime Summary ====================" -ForegroundColor Magenta

$results |
    Sort-Object AverageSeconds |
    Format-Table Name, AverageSeconds, MinSeconds, MaxSeconds, TotalSeconds -AutoSize