<#
.SYNOPSIS
    Builds Saucery.NuGet from source and reinstalls it as the global tool.
#>

[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ProjectFile = Join-Path $PSScriptRoot "Saucery.NuGet\Saucery.NuGet.csproj"
$NupkgOutDir = Join-Path $PSScriptRoot "nupkg-local"

dotnet pack $ProjectFile -c Release -o $NupkgOutDir
if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed." }

$version = ([xml](Get-Content $ProjectFile)).SelectSingleNode("//PackageVersion").'#text'
#    ForEach-Object { $_.PackageVersion } |
#    Where-Object { $_ } |
#	Select-Object -First 1
	
Write-Host "Installing Saucery.NuGet $version"

dotnet tool uninstall --global Saucery.NuGet 2>$null

dotnet tool install --global Saucery.NuGet --add-source $NupkgOutDir --version $version
if ($LASTEXITCODE -ne 0) { throw "dotnet tool install failed." }

Remove-Item -Recurse -Force $NupkgOutDir
Write-Host "Done. saucery-nuget $version installed."