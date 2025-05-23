#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
ARG TARGETPLATFORM

RUN apt-get update && apt-get install -y \
	wget tar htop p7zip-full ;

RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        echo Architecture: amd64 $TARGETPLATFORM ; \
		apt-get update && apt-get install -y \
			gnupg gnupg2 gnupg1 ; \
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
        # dpkg --add-architecture i386 && apt-get update && apt-get install wine wget -y && wine --version ; \
		dpkg --add-architecture i386 ; \
		wget -nc https://dl.winehq.org/wine-builds/winehq.key ; \
		apt-key add winehq.key ; \
		echo 'deb https://dl.winehq.org/wine-builds/debian/ buster main' >> /etc/apt/sources.list ; \
		wget https://download.opensuse.org/repositories/Emulators:/Wine:/Debian/xUbuntu_18.04/amd64/libfaudio0_19.07-0~bionic_amd64.deb ; \
		wget https://download.opensuse.org/repositories/Emulators:/Wine:/Debian/xUbuntu_18.04/i386/libfaudio0_19.07-0~bionic_i386.deb ; \
		dpkg -i libfaudio0_19.07-0~bionic_amd64.deb libfaudio0_19.07-0~bionic_i386.deb ; \
		apt-get update ; \
		apt --fix-broken install -y ; \
		apt install --install-recommends winehq-stable -y ; \
		wine --version ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
		wget -O /root/buildfolder_build_arm64.tar.gz --no-verbose https://github.com/devedse/hangover/releases/latest/download/buildfolder_build_arm64.tar.gz ; \
		tar -C /root/hangover -zxvf /root/buildfolder_build_arm64.tar.gz ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        echo "SKIPPING" ; \
    else \
        echo $TARGETPLATFORM ; \
	fi

RUN wget -O /root/FileOptimizerSetup.exe https://sourceforge.net/projects/nikkhokkho/files/latest/download

RUN 7z x /root/FileOptimizerSetup.exe -o"/root/.wine/drive_c/Program Files/FileOptimizer/"
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

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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

# We call the sleep64.exe here, it doesn't actually seem to work but it loads in wine, which makes all subsequent calls faster :)
RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        wget https://github.com/jackdp/sleep/releases/download/v1.0/sleep64.exe ; \
		echo '#!/bin/bash\n\
nohup wine /app/sleep64.exe 49d &\n\
dotnet DeveImageOptimizer.ConsoleApp.dll' > /app/go.sh && chmod +x /app/go.sh ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
		wget https://github.com/jackdp/sleep/releases/download/v1.0/sleep64.exe ; \
        echo '#!/bin/bash\n\
nohup /root/hangover/build/wine-host/loader/wine /root/hangover/build/qemu/x86_64-windows-user/qemu-x86_64.exe.so /app/sleep64.exe 49d &\n\
dotnet DeveImageOptimizer.ConsoleApp.dll' > /app/go.sh && chmod +x /app/go.sh ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        echo "SKIPPING" ; \
    else \
        echo $TARGETPLATFORM ; \
	fi

RUN chmod +x /app/go.sh
#ENTRYPOINT ["dotnet", "DeveImageOptimizer.ConsoleApp.dll"]
#ENTRYPOINT ["/root/hangover/build/wine-host/loader/wine", "/root/hangover/build/qemu/x86_64-windows-user/qemu-x86_64.exe.so", "/root/.wine/drive_c/Program Files/FileOptimizer/Plugins64/jpegoptim.exe", "-o", "--all-progressive", "/app/TestImage.jpg", "--", "&&", "dotnet", "DeveImageOptimizer.ConsoleApp.dll"]
ENTRYPOINT ["./go.sh"]
