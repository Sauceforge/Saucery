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
    Write-Host "timing metrics for each suite."
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
    Write-Host "                         Example: Saucery.Core.Tests.XUnitv3"
    Write-Host ""
    Write-Host "  -Help, -h, --help      Show this help message and exit."
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

    if ($null -ne $found) {
        return $found.FullName
    }

    throw "Project '$ProjectName' not found. Expected '$expected'."
}

function Invoke-TestSuite {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [ValidateSet("dotnet-test", "dotnet-run")]
        [string]$Mode,

        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,

        [Parameter(Mandatory = $true)]
        [int]$Iterations,

        [Parameter(Mandatory = $true)]
        [string]$Configuration
    )

    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "Running suite: $Name" -ForegroundColor Cyan
    Write-Host "Mode: $Mode" -ForegroundColor Cyan
    Write-Host "Project: $ProjectPath" -ForegroundColor Cyan
    Write-Host "Iterations: $Iterations" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""

    $durations = New-Object System.Collections.Generic.List[double]

    for ($i = 1; $i -le $Iterations; $i++) {
        Write-Host "[$Name] Iteration $i of $Iterations..." -ForegroundColor Yellow

        $sw = [System.Diagnostics.Stopwatch]::StartNew()

        if ($Mode -eq "dotnet-test") {
            $command = "dotnet test `"$ProjectPath`" --configuration $Configuration --nologo -v normal"
            Write-Host "Executing: $command" -ForegroundColor DarkGray
            & dotnet test $ProjectPath --configuration $Configuration --nologo -v normal
        }
        elseif ($Mode -eq "dotnet-run") {
            $command = "dotnet run --project `"$ProjectPath`" --configuration $Configuration --no-build"
            Write-Host "Executing: $command" -ForegroundColor DarkGray
            & dotnet run --project $ProjectPath --configuration $Configuration --no-build
        }
        else {
            throw "Unsupported mode '$Mode'."
        }

        $exitCode = $LASTEXITCODE
        $sw.Stop()

        $elapsedSeconds = $sw.Elapsed.TotalSeconds
        $elapsedRounded = [Math]::Round($elapsedSeconds, 3)
        $durations.Add($elapsedSeconds)

        if ($exitCode -ne 0) {
            throw @"
Suite '$Name' failed on iteration $i.
Command: $command
Exit code: $exitCode
Elapsed: $elapsedRounded s

The non-zero exit code came from the test process, not from PowerShell.
Scroll up to the test output above this error to find the real failure.
"@
        }

        Write-Host "[$Name] Iteration $i completed in $elapsedRounded s" -ForegroundColor Green
    }

    $average = ($durations | Measure-Object -Average).Average
    $minimum = ($durations | Measure-Object -Minimum).Minimum
    $maximum = ($durations | Measure-Object -Maximum).Maximum
    $total   = ($durations | Measure-Object -Sum).Sum

    $result = [pscustomobject]@{
        Name           = $Name
        Mode           = $Mode
        Iterations     = $Iterations
        AverageSeconds = [Math]::Round($average, 3)
        MinSeconds     = [Math]::Round($minimum, 3)
        MaxSeconds     = [Math]::Round($maximum, 3)
        TotalSeconds   = [Math]::Round($total, 3)
    }

    Write-Host ""
    Write-Host "-------------------- Suite Summary --------------------" -ForegroundColor Magenta
    Write-Host ("Suite      : {0}" -f $result.Name) -ForegroundColor Magenta
    Write-Host ("Mode       : {0}" -f $result.Mode) -ForegroundColor Magenta
    Write-Host ("Iterations : {0}" -f $result.Iterations) -ForegroundColor Magenta
    Write-Host ("Average    : {0} s" -f $result.AverageSeconds) -ForegroundColor Magenta
    Write-Host ("Minimum    : {0} s" -f $result.MinSeconds) -ForegroundColor Magenta
    Write-Host ("Maximum    : {0} s" -f $result.MaxSeconds) -ForegroundColor Magenta
    Write-Host ("Total      : {0} s" -f $result.TotalSeconds) -ForegroundColor Magenta
    Write-Host "-------------------------------------------------------" -ForegroundColor Magenta
    Write-Host ""

    return $result
}

$suites = @(
    @{ Name = "Saucery.Core.Tests";         Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.XUnit";   Mode = "dotnet-test" }
    @{ Name = "Saucery.Core.Tests.XUnitv3"; Mode = "dotnet-test" }
    @{ Name = "Saucery.Core.Tests.NUnit";   Mode = "dotnet-test" }
    @{ Name = "Saucery.Core.Tests.384";     Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.576";     Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.768";     Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.960";     Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.1152";    Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.1344";    Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.1536";    Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.1728";    Mode = "dotnet-run"  }
    @{ Name = "Saucery.Core.Tests.1920";    Mode = "dotnet-run"  }
)

if ($StartFrom) {
    $startIndex = -1

    for ($i = 0; $i -lt $suites.Count; $i++) {
        if ($suites[$i].Name -eq $StartFrom) {
            $startIndex = $i
            break
        }
    }

    if ($startIndex -lt 0) {
        $validNames = $suites | ForEach-Object { $_.Name }
        throw "StartFrom suite '$StartFrom' not found. Valid values: $($validNames -join ', ')"
    }

    $suites = $suites[$startIndex..($suites.Count - 1)]
}

Write-Host "Restoring solution..." -ForegroundColor Cyan
& dotnet restore
if ($LASTEXITCODE -ne 0) {
    throw "dotnet restore failed."
}

Write-Host "Building solution..." -ForegroundColor Cyan
& dotnet build --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed."
}

$results = New-Object System.Collections.Generic.List[object]

foreach ($suite in $suites) {
    $projectPath = Resolve-ProjectPath -ProjectName $suite.Name

    $result = Invoke-TestSuite `
        -Name $suite.Name `
        -Mode $suite.Mode `
        -ProjectPath $projectPath `
        -Iterations $Iterations `
        -Configuration $Configuration

    $results.Add($result)
}

Write-Host ""
Write-Host "==================== Average Runtime Summary ====================" -ForegroundColor Magenta
$results |
    Sort-Object AverageSeconds |
    Format-Table Name, Mode, Iterations, AverageSeconds, MinSeconds, MaxSeconds, TotalSeconds -AutoSize