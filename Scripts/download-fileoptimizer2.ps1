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

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Add-Type -AssemblyName System.Net.Http
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

#Configure latest version from: https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/
$url = "https://sourceforge.net/projects/nikkhokkho/files/latest/download"
$path = Join-Path $scriptPath 'FileOptimizerFull.exe'
$extractPath = Join-path $scriptPath 'FileOptimizer'
$logFile = Join-Path $scriptPath 'setuplog.txt'

Write-Host "Downloading file..."
Using-Object ($httpClient = New-Object System.Net.Http.Httpclient) {
    Using-Object ($result = $httpClient.GetAsync($url).Result) {
        Using-Object ($contentStream = $result.Content.ReadAsStreamAsync().Result) {
            Using-object ($fs = New-Object IO.FileStream $path, 'Create', 'Write', 'None') {
                $contentStream.CopyToAsync($fs).Wait()

                Write-Host "File downloaded, extracting..."     
            }
        }
    }
}

#NSIS (nullsoft scriptable install system)
$p = Start-Process $path "/S" -Wait -Passthru
$p.WaitForExit()
if ($p.ExitCode -ne 0) {
    Write-Host "Extraction failed, exiting."
    $host.SetShouldExit($p.ExitCode)
}

Write-Host "Extraction completed"

Write-Host "Script completed, extracted path: $extractPath"