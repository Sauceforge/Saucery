# PowerShell script to sync TUnit PackageReference version to PackageVersion in .csproj

param(
    [string]$CsprojPath = "Saucery.TUnit.csproj"
)

# Load XML
[xml]$xml = Get-Content $CsprojPath

# Find TUnit PackageReference version
$tunitNode = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "TUnit" }
if (-not $tunitNode) {
    Write-Error "TUnit PackageReference not found."
    exit 1
}
$tunitVersion = $tunitNode.Version

# Find PackageVersion property
$packageVersionNode = $xml.Project.PropertyGroup.PackageVersion
if (-not $packageVersionNode) {
    Write-Error "PackageVersion property not found."
    exit 1
}

# Update PackageVersion to match TUnit version
$packageVersionNode.'#text' = $tunitVersion

# Save changes
$xml.Save($CsprojPath)
Write-Host "Synchronized PackageVersion to $tunitVersion"
