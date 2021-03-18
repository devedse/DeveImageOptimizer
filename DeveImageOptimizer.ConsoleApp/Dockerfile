#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DeveImageOptimizer.ConsoleApp/DeveImageOptimizer.ConsoleApp.csproj", "DeveImageOptimizer.ConsoleApp/"]
COPY ["DeveImageOptimizer/DeveImageOptimizer.csproj", "DeveImageOptimizer/"]
RUN dotnet restore "DeveImageOptimizer.ConsoleApp/DeveImageOptimizer.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/DeveImageOptimizer.ConsoleApp"
RUN dotnet build "DeveImageOptimizer.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_VERSION
ARG VER=${BUILD_VERSION:-1.0.0}
RUN dotnet publish "DeveImageOptimizer.ConsoleApp.csproj" -c Release -o /app/publish /p:Version=$VER

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveMazeGenerator.ConsoleApp.dll"]
