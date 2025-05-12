#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2022 AS base
ARG TARGETPLATFORM
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
RUN if ($env:TARGETPLATFORM -eq 'windows/amd64') { ; \
        echo hallo amd64 ; \
    } elseif ($env:TARGETPLATFORM -eq 'windows/arm64') { \
        echo hallo arm64 ; \
    } else { \
        Write-Host "hello" ; \
        Write-Host "$env:TARGETPLATFORM" ; \
    }

WORKDIR /app
RUN \
    $webClient = [System.Net.WebClient]::new() ; \
    $webClient.DownloadFile('https://sourceforge.net/projects/nikkhokkho/files/latest/download', 'FileOptimizerSetup.exe') ; \
    $webClient.Dispose() ; \
    dir ; \
    & .\FileOptimizerSetup.exe /S | Out-Null ; \
    del .\FileOptimizerSetup.exe ;

# RUN wget -O /root/FileOptimizerSetup.exe https://sourceforge.net/projects/nikkhokkho/files/latest/download && wine /root/FileOptimizerSetup.exe /S && rm /root/FileOptimizerSetup.exe
# ENV FILEOPTIMIZERPATH="/root/.wine/drive_c/Program Files/FileOptimizer/FileOptimizer64.exe"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["DeveImageOptimizer.ConsoleApp/DeveImageOptimizer.ConsoleApp.csproj", "DeveImageOptimizer.ConsoleApp/"]
COPY ["DeveImageOptimizer/DeveImageOptimizer.csproj", "DeveImageOptimizer/"]
RUN dotnet restore "DeveImageOptimizer.ConsoleApp/DeveImageOptimizer.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/DeveImageOptimizer.ConsoleApp"
RUN dotnet build "DeveImageOptimizer.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_VERSION
ARG VER=${BUILD_VERSION:-1.0.0}
RUN dotnet publish "DeveImageOptimizer.ConsoleApp.csproj" -c Release -o /app/publish /p:Version=%VER%

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveImageOptimizer.ConsoleApp.dll"]
