$ErrorActionPreference = "Stop"

$url = "https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/13.30.2393/FileOptimizerFull.7z.exe"
Write-Host "This script downloads a specific version of FileOptimizer and extracts it: $url"

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

$path = Join-Path $scriptPath 'FileOptimizerFull.7z.exe'
$extractPath = Join-path $scriptPath 'FileOptimizer'

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

$p = Start-Process $path "-o""$extractPath"" -y" -Wait -Passthru
$p.WaitForExit()
if ($p.ExitCode -ne 0) {
    Write-Host "Extraction failed, exiting."
    $host.SetShouldExit($p.ExitCode)
}

Write-Host "Extraction completed"

Write-Host "Script completed, extracted path: $extractPath"