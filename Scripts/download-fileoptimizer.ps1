$ErrorActionPreference = "Stop"
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$url = 'https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/11.20.2033/FileOptimizerFull.7z.exe'
$path = Join-Path $scriptPath 'FileOptimizerFull.7z.exe'
$extractPath = Join-path $scriptPath 'FileOptimizer'

Write-Host "Downloading file..."
#Invoke-WebRequest -Uri $url -OutFile $path -UserAgent [Microsoft.PowerShell.Commands.PSUserAgent]::FireFox
$wc = New-Object net.webclient
$wc.Downloadfile($url, $path)

Write-Host "File downloaded, extracting..."
$p = Start-Process $path "-o""$extractPath"" -y" -Wait -Passthru
$p.WaitForExit()
if ($p.ExitCode -ne 0) {
	Write-Host "Extraction failed, exiting."
    $host.SetShouldExit($p.ExitCode)
}

Write-Host "Extraction completed, copying ini file."

$iniFile = Join-Path $scriptPath 'FileOptimizer64.ini'
$iniFileDest = Join-Path $extractPath 'FileOptimizer64.ini'
cp $iniFile $iniFileDest

Write-Host "Script completed, extracted path: $extractPath"