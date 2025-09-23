param(
    [string]$Root = (Split-Path -Parent $MyInvocation.MyCommand.Path),
    [string]$ScriptName = "sync-package-version.ps1"
)

# Do NOT set global StrictMode here; keep parent environment inert.

function Get-PowerShellExe {
    $pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
    if ($pwsh) { return $pwsh.Path }
    (Get-Command powershell).Path
}

function Invoke-Sync([string]$Project, [string]$Root, [string]$ScriptName) {
    $projectDir = Join-Path $Root $Project
    $scriptPath = Join-Path $projectDir $ScriptName

    if (-not (Test-Path -LiteralPath $projectDir)) { Write-Error "Project dir not found: $projectDir"; return }
    if (-not (Test-Path -LiteralPath $scriptPath)) { Write-Error "Script not found: $scriptPath";   return }

    Write-Host "→ $Project"
    Write-Host "   Script : $scriptPath"

    $exe  = Get-PowerShellExe
    $args = @('-NoProfile','-ExecutionPolicy','Bypass','-File', $scriptPath)

    try {
        # Launch child in its OWN PROCESS with the project as the working directory.
        Push-Location $projectDir
        & $exe @args
        $code = $LASTEXITCODE
    }
    catch {
        Write-Error "✗ Failed ${Project}: $($_.Exception.Message)"
        return
    }
    finally {
        Pop-Location
    }

    if ($code -ne 0) {
        Write-Error "✗ Failed ${Project}: child exited with code $code"
        return
    }

    Write-Host "✓ Synced $Project"
}

$Projects = @(
    "Saucery",
    "Saucery.Playwright.NUnit",
    "Saucery.XUnit",
    "Saucery.XUnit3",
    "Saucery.TUnit"
)

cls

.\Update-NuGetNext-All.ps1 -PackageId "NUnit" -Root "C:\gitrepos\Saucery\Saucery\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Saucery\"
.\Update-NuGetNext-All.ps1 -PackageId "NUnit3TestAdapter" -Root "C:\gitrepos\Saucery\Saucery\"

.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Saucery.TUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.CodeCoverage" -Root "C:\gitrepos\Saucery\Saucery.TUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "TUnit" -Root "C:\gitrepos\Saucery\Saucery.TUnit\"

.\Update-NuGetNext-All.ps1 -PackageId "Meziantou.Xunit.ParallelTestFramework" -Root "C:\gitrepos\Saucery\Saucery.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Saucery.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "xunit" -Root "C:\gitrepos\Saucery\Saucery.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "xunit.runner.visualstudio" -Root "C:\gitrepos\Saucery\Saucery.XUnit\"

.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Saucery.XUnit3\"
.\Update-NuGetNext-All.ps1 -PackageId "xunit.v3" -Root "C:\gitrepos\Saucery\Saucery.XUnit3\"
.\Update-NuGetNext-All.ps1 -PackageId "xunit.runner.visualstudio" -Root "C:\gitrepos\Saucery\Saucery.XUnit3\"

Write-Host "Running syncs from $Root ..."
foreach ($p in $Projects) {
    Invoke-Sync -Project $p -Root $Root -ScriptName $ScriptName
}
Write-Host "All done."
