# PowerShell script to sync TUnit PackageReference version to PackageVersion in .csproj

param(
    [string]$CsprojPath = "Saucery.XUnit3.csproj"
)

# Load XML
[xml]$xml = Get-Content $CsprojPath

# Find TUnit PackageReference version
$tunitNode = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "xunit.v3" }
if (-not $tunitNode) {
    Write-Error "XUnit.v3 PackageReference not found."
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
