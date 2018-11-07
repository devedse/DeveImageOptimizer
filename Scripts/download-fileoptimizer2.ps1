$ErrorActionPreference = "Stop"

Write-Host "This script downloads the latest installer for FileOptimizer and installs it"

function Using-Object
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [AllowNull()]
        [Object]
        $InputObject,

        [Parameter(Mandatory = $true)]
        [scriptblock]
        $ScriptBlock
    )

    try
    {
        . $ScriptBlock
    }
    finally
    {
        if ($null -ne $InputObject -and $InputObject -is [System.IDisposable])
        {
            $InputObject.Dispose()
        }
    }
}


function Using-Object-Retry
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [AllowNull()]
        [Object]
        $InputObject,

        [Parameter(Mandatory = $true)]
        [scriptblock]
        $ScriptBlock
    )

    try
    {
        $success = $false
        $count = 0
        $retryMax = 5;

        do {
            . $ScriptBlock

            if ($InputObject -ne $null) {
                $success = $true
            } else {
                $count++
                Write-Host "Retrying: $count/$retryMax"
            }
        }
        until($count -eq $retryMax -or $success)

        if ($success -eq $false) {
            Write-Host "Failed when running: $scriptBlock"
        }
    }
    finally
    {
        if ($null -ne $InputObject -and $InputObject -is [System.IDisposable])
        {
            $InputObject.Dispose()
        }
    }
}



$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Add-Type -AssemblyName System.Net.Http
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

#Configure latest version from: https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/
$url = "https://sourceforge.net/projects/nikkhokkho/files/latest/download"
$path = Join-Path $scriptPath 'FileOptimizerSetup.exe'
$installPath = "C:\Program Files\FileOptimizer"
$logFile = Join-Path $scriptPath 'setuplog.txt'

Write-Host "Downloading file..."
Using-Object-Retry ($httpClient = New-Object System.Net.Http.Httpclient) {
    Using-Object-Retry ($result = $httpClient.GetAsync($url).Result) {
        Using-Object-Retry ($contentStream = $result.Content.ReadAsStreamAsync().Result) {
            Using-Object-Retry ($fs = New-Object IO.FileStream $path, 'Create', 'Write', 'None') {
                $contentStream.CopyToAsync($fs).Wait()

                Write-Host "File downloaded, installing..."     
            }
        }
    }
}

#NSIS (nullsoft scriptable install system)
$p = Start-Process $path "/S" -Wait -Passthru
$p.WaitForExit()
if ($p.ExitCode -ne 0) {
    Write-Host "Installation failed, exiting."
    $host.SetShouldExit($p.ExitCode)
}

Write-Host "Installation completed"

Write-Host "Script completed, installation path: $installPath"

Write-Host "Existing files:"
Get-ChildItem -Path $installPath