$ErrorActionPreference = "Stop"

$url = 'https://sourceforge.net/projects/nikkhokkho/files/FileOptimizer/11.10.2015/FileOptimizerFull.7z.exe'
$path = '.\FileOptimizerFull.7z.exe'

Invoke-WebRequest -Uri $url -OutFile $path -UserAgent [Microsoft.PowerShell.Commands.PSUserAgent]::FireFox

Start-Process $path '-o".\FileOptimizer" -y' -NoNewWindow -Wait
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode)  }

cp .\FileOptimizer64.ini .\FileOptimizer\FileOptimizer64.ini