$ErrorActionPreference = "Stop"
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$url = 'https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/11.10.2015/FileOptimizerFull.7z.exe'
$path = '.\FileOptimizerFull.7z.exe'

Write-Host "Downloading file..."
#Invoke-WebRequest -Uri $url -OutFile $path -UserAgent [Microsoft.PowerShell.Commands.PSUserAgent]::FireFox
$wc = New-Object net.webclient
$wc.Downloadfile($url, $path)

Write-Host "File downloaded, extracting..."
$p = Start-Process $path '-o".\FileOptimizer" -y' -Wait -Passthru
$p.WaitForExit()
if ($p.ExitCode -ne 0) {
	Write-Host "Extraction failed, exiting."
    $host.SetShouldExit($p.ExitCode)
}

Write-Host "Extraction completed, copying ini file."

$iniFile = Join-Path $scriptPath 'FileOptimizer64.ini'
cp $iniFile .\FileOptimizer\FileOptimizer64.ini

Write-Host "Script completed"