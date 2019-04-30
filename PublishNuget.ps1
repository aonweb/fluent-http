param (
    [string]$source = "http://svd0nugetd01/nuget/",
	[string]$nuget = "tools\nuget.exe"
 )
 
 
function DisplayArgs()
{
	"Options"
    "    => source: $source"
    "    => nuget: $nuget"

    "examples"
    "  publish-nuget.ps1"
    "  publish-nuget.ps1 -source http://mynuget/nuget/"
    "  publish-nuget.ps1 -source http://mynuget/nuget/ -nuget c:\nuget\nuget.exe"
    ""

	if (-Not $nuget -eq "")
    {
        $global:nugetExe = $nuget
    }
    else
    {
        # Assumption, nuget.exe is in "..\tools\nuget.exe".
        $global:nugetExe = "tools\nuget.exe" 
    }

    $global:nugetExe

    if (!(Test-Path $global:nugetExe -PathType leaf))
    {
        ""
        "Nuget exe was not found. "
        ""
        throw;
    }
}

function Push()
{
	$directories = Get-ChildItem -Path . -File -Filter "*.nupkg" -Recurse | ? { $_.FullName -match '.*\\Release\\*.*' } | Select-Object -Property Directory -Unique

    foreach($directory in $directories)
    {
		$latest = Get-ChildItem -Path $directory.Directory -File -Filter "*.nupkg" | Sort-Object LastAccessTime -Descending | Select-Object -First 1
		
        &$nugetExe push ($latest.FullName) -Source $source

        ""
    }
}

$ErrorActionPreference = "Stop"
$global:nugetExe = ""

cls

DisplayArgs

Push