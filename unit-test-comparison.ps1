[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [ValidateRange(1, 100000)]
    [int]$Iterations = 1,

    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release"
)

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
            & dotnet test $ProjectPath --configuration $Configuration --nologo -v normal
        }
        elseif ($Mode -eq "dotnet-run") {
            & dotnet run --project $ProjectPath --configuration $Configuration --no-build
        }
        else {
            throw "Unsupported mode '$Mode'."
        }

        $exitCode = $LASTEXITCODE
        $sw.Stop()

        $elapsedSeconds = [Math]::Round($sw.Elapsed.TotalSeconds, 3)
        $durations.Add($sw.Elapsed.TotalSeconds)

        if ($exitCode -ne 0) {
            throw "Suite '$Name' failed on iteration $i with exit code $exitCode after $elapsedSeconds s."
        }

        Write-Host "[$Name] Iteration $i completed in $elapsedSeconds s" -ForegroundColor Green
    }

    $average = ($durations | Measure-Object -Average).Average
    $minimum = ($durations | Measure-Object -Minimum).Minimum
    $maximum = ($durations | Measure-Object -Maximum).Maximum
    $total = ($durations | Measure-Object -Sum).Sum

    [pscustomobject]@{
        Name           = $Name
        Mode           = $Mode
        Iterations     = $Iterations
        AverageSeconds = [Math]::Round($average, 3)
        MinSeconds     = [Math]::Round($minimum, 3)
        MaxSeconds     = [Math]::Round($maximum, 3)
        TotalSeconds   = [Math]::Round($total, 3)
    }
}

# Match the pipeline ordering exactly.
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