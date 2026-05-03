# Delete-AllErroredSauceJobs.ps1

param(
  [string]$Region = "us-west-1",
  [string]$SauceUser = $env:SAUCE_USER_NAME,
  [string]$SauceKey  = $env:SAUCE_API_KEY,

  [datetime]$From = "2026-02-01",
  [datetime]$To   = [datetime]::UtcNow,

  [int]$PageSize = 100,
  [int]$ChunkDays = 14,
  [int]$TimeoutSec = 20,

  [switch]$DryRun,
  [switch]$IncludeFailed,
  [switch]$VerboseJobDump
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SauceUser)) { throw "SAUCE_USER_NAME is not set." }
if ([string]::IsNullOrWhiteSpace($SauceKey))  { throw "SAUCE_API_KEY is not set." }

$auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$SauceUser`:$SauceKey"))

$headers = @{
  Authorization = "Basic $auth"
  Accept        = "application/json"
}

$analyticsBase   = "https://api.$Region.saucelabs.com/v1/analytics/tests"
$virtualJobsBase = "https://api.$Region.saucelabs.com/rest/v1/$SauceUser/jobs"
$rdcJobsBase     = "https://api.$Region.saucelabs.com/v1/rdc/jobs"

$statuses = @("error")
if ($IncludeFailed) { $statuses += "failed" }

function Invoke-SauceWeb {
  param(
    [ValidateSet("GET", "DELETE")]
    [string]$Method,
    [string]$Url
  )

  try {
    $response = Invoke-WebRequest `
      -Uri $Url `
      -Headers $headers `
      -Method $Method `
      -TimeoutSec $TimeoutSec `
      -SkipHttpErrorCheck

    return [pscustomobject]@{
      StatusCode = [int]$response.StatusCode
      Content    = [string]$response.Content
      Success    = ([int]$response.StatusCode -ge 200 -and [int]$response.StatusCode -lt 300)
    }
  }
  catch {
    return [pscustomobject]@{
      StatusCode = 0
      Content    = $_.Exception.Message
      Success    = $false
    }
  }
}

function Invoke-SauceJsonGet {
  param([string]$Url)

  $result = Invoke-SauceWeb -Method GET -Url $Url

  if (-not $result.Success) {
    Write-Warning "GET failed [$($result.StatusCode)]: $Url"
    return $null
  }

  if ([string]::IsNullOrWhiteSpace($result.Content)) {
    return $null
  }

  return $result.Content | ConvertFrom-Json
}

function Get-ArrayFromResponse {
  param($Response)

  if ($null -eq $Response) { return @() }

  foreach ($name in @("items", "entities", "data", "results", "tests")) {
    if ($Response.PSObject.Properties.Name -contains $name) {
      return @($Response.$name)
    }
  }

  if ($Response -is [array]) { return @($Response) }

  return @()
}

function Get-StringProperty {
  param($Object, [string[]]$Names)

  if ($null -eq $Object) { return "" }

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

function Add-UniqueValue {
  param(
    [System.Collections.Generic.List[string]]$List,
    [string]$Value
  )

  if ([string]::IsNullOrWhiteSpace($Value)) { return }

  if (-not $List.Contains($Value)) {
    [void]$List.Add($Value)
  }
}

function Get-CandidateJobIds {
  param($Item)

  $ids = [System.Collections.Generic.List[string]]::new()

  foreach ($name in @(
    "job_id",
    "jobId",
    "job_uuid",
    "jobUuid",
    "session_id",
    "sessionId",
    "selenium_session_id",
    "seleniumSessionId",
    "appium_session_id",
    "appiumSessionId",
    "device_session_id",
    "deviceSessionId",
    "id",
    "uuid"
  )) {
    if ($Item.PSObject.Properties.Name -contains $name) {
      Add-UniqueValue -List $ids -Value ([string]$Item.$name)
    }
  }

  foreach ($nestedName in @("job", "session", "metadata", "details")) {
    if (-not ($Item.PSObject.Properties.Name -contains $nestedName)) { continue }

    $nested = $Item.$nestedName
    if ($null -eq $nested) { continue }

    foreach ($name in @(
      "job_id",
      "jobId",
      "id",
      "uuid",
      "session_id",
      "sessionId",
      "appium_session_id",
      "appiumSessionId",
      "device_session_id",
      "deviceSessionId"
    )) {
      if ($nested.PSObject.Properties.Name -contains $name) {
        Add-UniqueValue -List $ids -Value ([string]$nested.$name)
      }
    }
  }

  return @($ids)
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

    Write-Host "Fetching Analytics Test Results: $url"

    $response = Invoke-SauceJsonGet -Url $url
    if ($null -eq $response) { break }

    $items = @(Get-ArrayFromResponse -Response $response)
    if ($items.Count -eq 0) { break }

    $all += $items

    if ($response.PSObject.Properties.Name -contains "has_more") {
      if ($response.has_more -ne $true) { break }
    }
    elseif ($items.Count -lt $PageSize) {
      break
    }

    $fromIndex += $PageSize
  }

  return @($all)
}

function Resolve-DeletableJob {
  param([string[]]$CandidateIds)

  foreach ($id in $CandidateIds) {
    if ([string]::IsNullOrWhiteSpace($id)) { continue }

    $virtualUrl = "$virtualJobsBase/$id"
    $rdcUrl     = "$rdcJobsBase/$id"

    $virtual = Invoke-SauceWeb -Method GET -Url $virtualUrl
    if ($virtual.Success) {
      return [pscustomobject]@{
        Id        = $id
        Kind      = "virtual"
        DeleteUrl = $virtualUrl
      }
    }

    $rdc = Invoke-SauceWeb -Method GET -Url $rdcUrl
    if ($rdc.Success) {
      return [pscustomobject]@{
        Id        = $id
        Kind      = "rdc"
        DeleteUrl = $rdcUrl
      }
    }
  }

  return $null
}

Write-Host ""
Write-Host "Sauce region:   $Region"
Write-Host "Sauce user:     $SauceUser"
Write-Host "From UTC:       $($From.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Host "To UTC:         $($To.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Host "Statuses:       $($statuses -join ', ')"
Write-Host "Dry run:        $DryRun"
Write-Host ""

$analyticsSeen = @{}
$analyticsRecords = @()

$cursor = $From

while ($cursor -lt $To) {
  $windowEnd = $cursor.AddDays($ChunkDays)
  if ($windowEnd -gt $To) { $windowEnd = $To }

  Write-Host ""
  Write-Host "Window: $($cursor.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")) -> $($windowEnd.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"

  foreach ($status in $statuses) {
    $items = @(Get-TestResultsForWindowAndStatus -WindowStart $cursor -WindowEnd $windowEnd -Status $status)

    foreach ($item in $items) {
      $candidateIds = @(Get-CandidateJobIds -Item $item)
      if ($candidateIds.Count -eq 0) { continue }

      $key = $candidateIds -join "|"
      if ($analyticsSeen.ContainsKey($key)) { continue }

      $analyticsSeen[$key] = $true
      $analyticsRecords += $item
    }
  }

  $cursor = $windowEnd
}

Write-Host ""
Write-Host "Analytics errored records found: $($analyticsRecords.Count)"
Write-Host "Resolving deletable jobs..."
Write-Host ""

$deletable = @()
$notDeletable = @()
$count = 0

foreach ($item in $analyticsRecords) {
  $count++
  $candidateIds = @(Get-CandidateJobIds -Item $item)

  Write-Host "Resolving [$count / $($analyticsRecords.Count)]: $($candidateIds -join ', ')"

  $resolved = Resolve-DeletableJob -CandidateIds $candidateIds

  $record = [pscustomobject]@{
    CandidateIds = ($candidateIds -join ",")
    Owner        = Get-StringProperty $item @("owner", "user", "username")
    Name         = Get-StringProperty $item @("name", "test_name", "testName")
    Status       = Get-StringProperty $item @("status")
    Created      = Get-StringProperty $item @("creation_time", "creationTime", "start_time", "startTime")
    Error        = Get-StringProperty $item @("error", "error_message", "errorMessage", "failure_reason", "failureReason")
    Raw          = $item
  }

  if ($null -eq $resolved) {
    $notDeletable += $record
    continue
  }

  $deletable += [pscustomobject]@{
    JobId        = $resolved.Id
    Kind         = $resolved.Kind
    DeleteUrl    = $resolved.DeleteUrl
    CandidateIds = $record.CandidateIds
    Owner        = $record.Owner
    Name         = $record.Name
    Status       = $record.Status
    Created      = $record.Created
    Error        = $record.Error
    Raw          = $item
  }
}

Write-Host ""
Write-Host "Accurate summary"
Write-Host "----------------"
Write-Host "Analytics errored records found: $($analyticsRecords.Count)"
Write-Host "Deletable errored jobs found:   $($deletable.Count)"
Write-Host "Analytics-only / not deletable: $($notDeletable.Count)"
Write-Host ""

$deleted = 0
$failedDeletes = @()

foreach ($job in $deletable) {
  Write-Host "DELETABLE ERRORED JOB:"
  Write-Host "  Job Id:     $($job.JobId)"
  Write-Host "  Kind:       $($job.Kind)"
  Write-Host "  Name:       $($job.Name)"
  Write-Host "  Status:     $($job.Status)"
  Write-Host "  Created:    $($job.Created)"
  Write-Host "  Error:      $($job.Error)"

  if ($VerboseJobDump) {
    Write-Host "  Raw:"
    $job.Raw | ConvertTo-Json -Depth 30
  }

  if ($DryRun) {
    Write-Host "DRY RUN: would delete $($job.DeleteUrl)"
    Write-Host ""
    continue
  }

  $deleteResult = Invoke-SauceWeb -Method DELETE -Url $job.DeleteUrl

  if ($deleteResult.Success -or $deleteResult.StatusCode -eq 404) {
    Write-Host "Deleted or already gone: $($job.JobId)"
    $deleted++
  }
  else {
    Write-Warning "Delete failed [$($deleteResult.StatusCode)]: $($job.DeleteUrl)"
    $failedDeletes += $job
  }

  Write-Host ""
}

Write-Host ""
Write-Host "Done."
Write-Host "-----"
Write-Host "Analytics errored records found: $($analyticsRecords.Count)"
Write-Host "Deletable errored jobs found:   $($deletable.Count)"
Write-Host "Deleted:                       $deleted"
Write-Host "Analytics-only / not deletable: $($notDeletable.Count)"
Write-Host "Failed deletes:                 $($failedDeletes.Count)"

if ($DryRun) {
  Write-Host ""
  Write-Host "Dry run only. Nothing was deleted."
}