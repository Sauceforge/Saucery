<#
.SYNOPSIS
	Runs saucery-nuget for each configured project in order.

.PARAMETER DryRun
	Pass --dry-run to - saucery-nuget to preview changes without writing any files.
#>

[CmdletBinding()]
param(
	[switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$SolutionFile = Join-Path $PSScriptRoot "Saucery.sln"

$dotnetTools = Join-Path ([System.Environment]::GetFolderPath("UserProfile")) ".dotnet\tools"
if ($env:PATH -notlike "*$dotnetTools*") {
	$env:PATH = "$dotnetTools;$env:PATH"
}

$sauceryNuGet = Join-Path $dotnetTools "saucery-nuget.exe"
if (-not (Test-Path $sauceryNuGet)) {
	dotnet tool install --global Saucery.NuGet
	if ($LASTEXITCODE -ne 0) { throw "Failed to install Saucery.NuGet global tool."	}
}

function Invoke-SauceryNuGet([string[]]$passArgs) {
	Write-Host "=========================================================="
	Write-Host "Running saucery-nuget"
	Write-Host "Args: $passArgs"
	Write-Host "=========================================================="

	$allArgs = @("--solution", $SolutionFile) + $passArgs
	if ($DryRun) { $allArgs += "--dry-run" }

	Write-Host "Executing: $sauceryNuGet $allArgs"

	& $sauceryNuGet @allArgs
	if ($LASTEXITCODE -ne 0) { throw "saucery-nuget failed." }
}

Invoke-SauceryNuGet @("--project", "Saucery.Core", "--bump-own-version")
Invoke-SauceryNuGet @("--project", "Saucery.TUnit", "--sync-with", "TUnit")
Invoke-SauceryNuGet @("--project", "Saucery.XUnit3", "--sync-with", "xunit.v3.mtp-v2")
Invoke-SauceryNuGet @("--project", "Saucery.XUnit", "--sync-with", "Saucery.Core")
Invoke-SauceryNuGet @("--project", "Saucery", "--sync-with", "Saucery.Core")
Invoke-SauceryNuGet @("--project", "Saucery.Playwright.NUnit", "--sync-with", "Saucery.Core")
Invoke-SauceryNuGet @("--exclude-projects", "Saucery.Core", "Saucery.TUnit", "Saucery.XUnit3", "Saucery.XUnit", "Saucery", "Saucery.Playwright.NUnit")
Invoke-SauceryNuGet @("--scan-unregistered")