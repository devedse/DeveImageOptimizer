#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
ARG TARGETPLATFORM

RUN apt-get update && apt-get install -y \
	wget tar htop p7zip-full ;

RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        echo Architecture: amd64 $TARGETPLATFORM ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        echo Architecture: arm64 $TARGETPLATFORM ; \
        apt-get update && apt-get install -y \
			pkg-config ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        echo Architecture: armv7 $TARGETPLATFORM ; \
    else \
        echo $TARGETPLATFORM ; \
	fi

RUN if [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
		apt-get update && apt-get install -y --no-install-recommends \
			# ca-certificates \
			# python \
			# flex bison \
			libfreetype6 \
			libltdl7 \
			# libxcb1-dev \
			libx11-dev ; \
			# librsvg2-bin \
			# gcc-mingw-w64-x86-64 gcc-mingw-w64-i686 \
			# automake autoconf pkg-config libtool \
			# automake1.11 autoconf2.13 autoconf2.64 \
			# gtk-doc-tools git gperf groff p7zip-full \
			# gettext \
			# make \
			# clang \
			# dpkg-dev \
			# libglib2.0-dev:arm64 \
			# libfreetype6:arm64 \
			# libltdl7:arm64 \
			# # libxcb1-dev:arm64 \
			# libx11-dev:arm64 ; \
		# &&	apt clean \
		# &&	rm -rf /var/lib/apt/lists/* \
		# &&	ln -s /usr/bin/autoconf /usr/bin/autoconf-2.69 \
		# &&	ln -s /usr/bin/autoheader /usr/bin/autoheader-2.69 ; \
	fi

RUN if [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        git clone --depth 1 --recurse-submodules -j8 https://github.com/devedse/hangover.git /root/hangover ; \
    fi

RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        dpkg --add-architecture i386 && apt-get update && apt-get install wine wget -y ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
		wget -O /root/buildfolder_build_arm64.tar.gz --no-verbose https://github.com/devedse/hangover/releases/latest/download/buildfolder_build_arm64.tar.gz ; \
		tar -C /root/hangover -zxvf /root/buildfolder_build_arm64.tar.gz ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        echo "SKIPPING" ; \
    else \
        echo $TARGETPLATFORM ; \
	fi

RUN wget -O /root/FileOptimizerSetup.exe https://sourceforge.net/projects/nikkhokkho/files/latest/download

RUN 7z e FileOptimizerSetup.exe -o/root/.wine/drive_c/Program Files/FileOptimizer/
# RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
#         wine /root/FileOptimizerSetup.exe /S ; \
#     elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
#         /root/hangover/build/wine-host/loader/wine /root/hangover/build/qemu/x86_64-windows-user/qemu-x86_64.exe.so /root/FileOptimizerSetup.exe /S ; \
#     elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
#         echo Architecture: armv7 $TARGETPLATFORM ; \
#     else \
#         echo $TARGETPLATFORM ; \
# 	fi

RUN rm /root/FileOptimizerSetup.exe

ENV FILEOPTIMIZERPATH="/root/.wine/drive_c/Program Files/FileOptimizer/FileOptimizer64.exe"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
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
RUN dotnet publish "DeveImageOptimizer.ConsoleApp.csproj" -c Release -o /app/publish /p:Version=$VER

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveImageOptimizer.ConsoleApp.dll"]
