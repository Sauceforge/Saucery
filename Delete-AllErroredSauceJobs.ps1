# Delete-AllErroredSauceTestResults.ps1
#
# Dry run:
#   .\Delete-AllErroredSauceTestResults.ps1 -From "2026-02-01" -DryRun
#
# Delete:
#   .\Delete-AllErroredSauceTestResults.ps1 -From "2026-02-01"
#
# Optional: include API status=failed too:
#   .\Delete-AllErroredSauceTestResults.ps1 -From "2026-02-01" -DryRun -IncludeFailed

param(
  [string]$Region = "us-west-1",
  [string]$SauceUser = $env:SAUCE_USER_NAME,
  [string]$SauceKey  = $env:SAUCE_API_KEY,

  [datetime]$From = "2026-02-01",
  [datetime]$To   = [datetime]::UtcNow,

  [int]$PageSize = 100,
  [int]$ChunkDays = 14,

  [switch]$DryRun,
  [switch]$IncludeFailed
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SauceUser)) {
  throw "SAUCE_USER_NAME is not set."
}

if ([string]::IsNullOrWhiteSpace($SauceKey)) {
  throw "SAUCE_API_KEY is not set."
}

$auth = [Convert]::ToBase64String(
  [Text.Encoding]::ASCII.GetBytes("$SauceUser`:$SauceKey")
)

$headers = @{
  Authorization = "Basic $auth"
  Accept        = "application/json"
}

$analyticsBase     = "https://api.$Region.saucelabs.com/v1/analytics/tests"
$virtualDeleteBase = "https://api.$Region.saucelabs.com/rest/v1/$SauceUser/jobs"
$rdcDeleteBase     = "https://api.$Region.saucelabs.com/v1/rdc/jobs"

$statuses = @("error")

if ($IncludeFailed) {
  $statuses += "failed"
}

function Get-ArrayFromResponse {
  param($Response)

  if ($null -eq $Response) {
    return @()
  }

  foreach ($name in @("items", "entities", "data", "results", "tests")) {
    if ($Response.PSObject.Properties.Name -contains $name) {
      return @($Response.$name)
    }
  }

  if ($Response -is [array]) {
    return @($Response)
  }

  return @()
}

function Get-TestResultId {
  param($Item)

  foreach ($name in @("id", "job_id", "jobId", "uuid")) {
    if ($Item.PSObject.Properties.Name -contains $name) {
      $value = [string]$Item.$name

      if (-not [string]::IsNullOrWhiteSpace($value)) {
        return $value
      }
    }
  }

  return $null
}

function Get-StringProperty {
  param(
    $Object,
    [string[]]$Names
  )

  foreach ($name in $Names) {
    if ($Object.PSObject.Properties.Name -contains $name) {
      $value = [string]$Object.$name

      if (-not [string]::IsNullOrWhiteSpace($value)) {
        return $value
      }
    }
  }

  return ""
}

function Invoke-SauceGet {
  param([string]$Url)

  try {
    return Invoke-RestMethod `
      -Uri $Url `
      -Headers $headers `
      -Method Get
  }
  catch {
    Write-Warning "GET failed: $Url"

    if ($_.ErrorDetails.Message) {
      Write-Warning $_.ErrorDetails.Message
    }
    else {
      Write-Warning $_.Exception.Message
    }

    return $null
  }
}

function Invoke-SauceDelete {
  param([string]$JobId)

  $virtualUrl = "$virtualDeleteBase/$JobId"
  $rdcUrl     = "$rdcDeleteBase/$JobId"

  if ($DryRun) {
    Write-Host "DRY RUN: would delete $JobId"
    Write-Host "  Virtual delete: $virtualUrl"
    Write-Host "  RDC delete:     $rdcUrl"
    return $false
  }

  try {
    Invoke-RestMethod `
      -Uri $virtualUrl `
      -Headers $headers `
      -Method Delete

    Write-Host "Deleted via virtual/jobs endpoint: $JobId"
    return $true
  }
  catch {
    Write-Warning "Virtual delete failed for $JobId. Trying RDC delete."
    Write-Warning $_.Exception.Message
  }

  try {
    Invoke-RestMethod `
      -Uri $rdcUrl `
      -Headers $headers `
      -Method Delete

    Write-Host "Deleted via RDC endpoint: $JobId"
    return $true
  }
  catch {
    Write-Warning "RDC delete failed for $JobId"
    Write-Warning $_.Exception.Message
    return $false
  }
}

function Get-TestResultsForWindowAndStatus {
  param(
    [datetime]$WindowStart,
    [datetime]$WindowEnd,
    [string]$Status
  )

  $start = $WindowStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
  $end   = $WindowEnd.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

  $all = @()
  $fromIndex = 0

  while ($true) {
    $url =
      "$analyticsBase" +
      "?start=$([uri]::EscapeDataString($start))" +
      "&end=$([uri]::EscapeDataString($end))" +
      "&size=$PageSize" +
      "&from=$fromIndex" +
      "&descending=true" +
      "&status=$Status"

    Write-Host "Fetching Test Results: $url"

    $response = Invoke-SauceGet -Url $url

    if ($null -eq $response) {
      break
    }

    $items = @(Get-ArrayFromResponse -Response $response)

    if ($items.Count -eq 0) {
      break
    }

    $all += $items

    if ($response.PSObject.Properties.Name -contains "has_more") {
      if ($response.has_more -ne $true) {
        break
      }
    }
    elseif ($items.Count -lt $PageSize) {
      break
    }

    $fromIndex += $PageSize
  }

  return @($all)
}

Write-Host ""
Write-Host "Sauce region:   $Region"
Write-Host "Sauce user:     $SauceUser"
Write-Host "From UTC:       $($From.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Host "To UTC:         $($To.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Host "Statuses:       $($statuses -join ', ')"
Write-Host "Dry run:        $DryRun"
Write-Host ""

$seen = @{}
$errored = @()

$cursor = $From

while ($cursor -lt $To) {
  $windowEnd = $cursor.AddDays($ChunkDays)

  if ($windowEnd -gt $To) {
    $windowEnd = $To
  }

  Write-Host ""
  Write-Host "Window: $($cursor.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")) -> $($windowEnd.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"

  foreach ($status in $statuses) {
    $items = @(Get-TestResultsForWindowAndStatus `
      -WindowStart $cursor `
      -WindowEnd $windowEnd `
      -Status $status)

    foreach ($item in $items) {
      $id = Get-TestResultId -Item $item

      if ([string]::IsNullOrWhiteSpace($id)) {
        continue
      }

      if ($seen.ContainsKey($id)) {
        continue
      }

      $seen[$id] = $true
      $errored += $item
    }
  }

  $cursor = $windowEnd
}

Write-Host ""
Write-Host "Errored Test Results found: $($errored.Count)"
Write-Host ""

$deleted = 0

foreach ($item in $errored) {
  $id      = Get-TestResultId -Item $item
  $name    = Get-StringProperty $item @("name", "test_name", "testName")
  $status  = Get-StringProperty $item @("status")
  $owner   = Get-StringProperty $item @("owner", "user", "username")
  $created = Get-StringProperty $item @("creation_time", "creationTime", "start_time", "startTime")
  $error   = Get-StringProperty $item @("error", "error_message", "errorMessage", "failure_reason", "failureReason")

  Write-Host "ERRORED TEST RESULT:"
  Write-Host "  Id:      $id"
  Write-Host "  Owner:   $owner"
  Write-Host "  Name:    $name"
  Write-Host "  Status:  $status"
  Write-Host "  Created: $created"
  Write-Host "  Error:   $error"

  if (Invoke-SauceDelete -JobId $id) {
    $deleted++
  }

  Write-Host ""
}

Write-Host "Done."
Write-Host "-----"
Write-Host "Errored Test Results found: $($errored.Count)"
Write-Host "Deleted:                    $deleted"

if ($DryRun) {
  Write-Host ""
  Write-Host "Dry run only. Nothing was deleted."
}