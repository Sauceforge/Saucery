param($installPath, $toolsPath, $package, $project)

#Write-Host "PWD: $($PWD)"

$coreName = 'Saucery'

#$TargetProject = ''

#foreach ($proj in get-project) {
#	Write-Host "Project $($proj.Name) is in this solution."
#	foreach ($ref in $proj.Object.References) {
#		Write-Host "$($proj.Name) Reference: $($ref.Name)"
#		if($($ref.Name) -eq $($coreName))
#		{
#			$TargetProject = $($coreName)
#			break
#		}
#	}
#}

$dte.ExecuteCommand("File.SaveAll")


#If ($TargetProject)
#{
	If ($PSVersionTable.PSVersion.Major -lt 3)
	{
	  $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
	}

	Write-Host "Checking $($coreName) Activation..."
	$dllpath = "$PSScriptRoot\..\lib\net45\$($coreName).dll"
	[System.Reflection.Assembly]::LoadFrom($dllpath)
	
	#$PSVersionTable
	$validatorClass = "$($coreName).Activation.ActivationValidator"
	$validator = New-Object $validatorClass
	$validator.CheckActivation()
#}

#
# Copyright Andrew Gray, Full Circle Solutions
# Date: 25th October 2014
# 