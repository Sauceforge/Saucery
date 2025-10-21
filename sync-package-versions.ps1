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

#Core

.\Update-NuGetNext-All.ps1 -PackageId "Appium.WebDriver" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "Castle.Core" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "DotNetSeleniumExtras.PageObjects.Core" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "DotNetSeleniumExtras.WaitHelpers" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "RestSharp" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "Selenium.Support" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "Selenium.WebDriver" -Root "C:\gitrepos\Saucery\Saucery.Core\"
.\Update-NuGetNext-All.ps1 -PackageId "Shouldly" -Root "C:\gitrepos\Saucery\Saucery.Core\"

# Core Tests

.\Update-NuGetNext-All.ps1 -PackageId "TUnit" -Root "C:\gitrepos\Saucery\Saucery.Core.Tests\"
#.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Testing.Extensions.CodeCoverage" -Root "C:\gitrepos\Saucery\Saucery.Core.Tests\"
#.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Testing.Extensions.TrxReport" -Root "C:\gitrepos\Saucery\Saucery.Core.Tests\"

# FrontEnd

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

#MERLINS
#NUnit
.\Update-NuGetNext-All.ps1 -PackageId "DotNetSeleniumExtras.PageObjects.Core" -Root "C:\gitrepos\Saucery\Merlin.NUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.NUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Web.Xdt" -Root "C:\gitrepos\Saucery\Merlin.NUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "NUnit3TestAdapter" -Root "C:\gitrepos\Saucery\Merlin.NUnit\"

#NUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.msbuild" -Root "C:\gitrepos\Saucery\Merlin.NUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "DotNetSeleniumExtras.PageObjects.Core" -Root "C:\gitrepos\Saucery\Merlin.NUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.NUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Web.Xdt" -Root "C:\gitrepos\Saucery\Merlin.NUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "NUnit3TestAdapter" -Root "C:\gitrepos\Saucery\Merlin.NUnit.RealDevices\"

#XUnit
.\Update-NuGetNext-All.ps1 -PackageId "Meziantou.Xunit.ParallelTestFramework" -Root "C:\gitrepos\Saucery\Merlin.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.collector" -Root "C:\gitrepos\Saucery\Merlin.XUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.msbuild" -Root "C:\gitrepos\Saucery\Merlin.XUnit\"

#XUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "Meziantou.Xunit.ParallelTestFramework" -Root "C:\gitrepos\Saucery\Merlin.XUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.XUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.collector" -Root "C:\gitrepos\Saucery\Merlin.XUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.msbuild" -Root "C:\gitrepos\Saucery\Merlin.XUnit.RealDevices\"

#TUnit
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.collector" -Root "C:\gitrepos\Saucery\Merlin.TUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.msbuild" -Root "C:\gitrepos\Saucery\Merlin.TUnit\"

#TUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.collector" -Root "C:\gitrepos\Saucery\Merlin.TUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "coverlet.msbuild" -Root "C:\gitrepos\Saucery\Merlin.TUnit.RealDevices\"

#XUnit3
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.XUnit3\"
#.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Testing.Extensions.CodeCoverage" -Root "C:\gitrepos\Saucery\Merlin.XUnit3\"

#XUnit3.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.NET.Test.Sdk" -Root "C:\gitrepos\Saucery\Merlin.XUnit3.RealDevices\"
#.\Update-NuGetNext-All.ps1 -PackageId "Microsoft.Testing.Extensions.CodeCoverage" -Root "C:\gitrepos\Saucery\Merlin.XUnit3.RealDevices\"

#EXTERNALMERLINS
#NUnit
.\Update-NuGetNext-All.ps1 -PackageId "NUnit3TestAdapter" -Root "C:\gitrepos\Saucery\ExternalMerlin.NUnit\"
.\Update-NuGetNext-All.ps1 -PackageId "Saucery" -Root "C:\gitrepos\Saucery\ExternalMerlin.NUnit\"

#NUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "NUnit3TestAdapter" -Root "C:\gitrepos\Saucery\ExternalMerlin.NUnit.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "Saucery" -Root "C:\gitrepos\Saucery\ExternalMerlin.NUnit.RealDevices\"

#XUnit
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.XUnit" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit\"

#XUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.XUnit" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit.RealDevices\"

#TUnit
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.TUnit" -Root "C:\gitrepos\Saucery\ExternalMerlin.TUnit\"

#TUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.TUnit" -Root "C:\gitrepos\Saucery\ExternalMerlin.TUnit.RealDevices\"

#XUnit3
.\Update-NuGetNext-All.ps1 -PackageId "xunit.runner.visualstudio" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit3\"
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.XUnit.v3" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit3\"

#XUnit3.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId "xunit.runner.visualstudio" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit3.RealDevices\"
.\Update-NuGetNext-All.ps1 -PackageId "Saucery.XUnit.v3" -Root "C:\gitrepos\Saucery\ExternalMerlin.XUnit3.RealDevices\"

Write-Host "Running syncs from $Root ..."
foreach ($p in $Projects) {
    Invoke-Sync -Project $p -Root $Root -ScriptName $ScriptName
}
Write-Host "All done."
