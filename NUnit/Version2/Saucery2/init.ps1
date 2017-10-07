param($installPath, $toolsPath, $package, $project)

$coreName = 'Saucery2'
$psMajorVersion = $PSVersionTable.PSVersion.Major

$dte.ExecuteCommand("File.SaveAll")

$scriptPath = If ($psMajorVersion -ge 3) { split-path -parent $MyInvocation.MyCommand.Definition } Else { $PSScriptRoot }

$dllPath = "$($scriptPath)\..\lib\net461\$($coreName).dll"
#$nugetPath = "$PSScriptRoot\..\lib\net461\$($nugetCore).dll"
#$xmlPath = "$PSScriptRoot\..\lib\net461\$($xmlTransform).dll"

[System.Reflection.Assembly]::LoadFrom($dllPath)
#[System.Reflection.Assembly]::LoadFrom($nugetPath)
#[System.Reflection.Assembly]::LoadFrom($xmlPath)

Write-Host "Checking $($coreName) Activation..."
$validatorClass = "$($coreName).Activation.ActivationValidator"
$validator = New-Object $validatorClass
$validator.CheckActivation()

#
# Copyright Andrew Gray, Full Circle Solutions
# Date: 12th November 2016
# 