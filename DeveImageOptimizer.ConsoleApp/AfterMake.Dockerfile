
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
ARG TARGETPLATFORM
RUN if [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        git clone --recurse-submodules -j8 https://github.com/devedse/hangover.git /root/hangover ; \
    fi
RUN if [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
    wget -O /root/buildfolder_build_arm64.tar.gz --no-verbose https://github.com/devedse/hangover/releases/latest/download/buildfolder_build_arm64.tar.gz ; \
    tar -C /root/hangover -zxvf /root/buildfolder_build_arm64.tar.gz ; \
fi
RUN wget -O /root/FileOptimizerSetup.exe https://sourceforge.net/projects/nikkhokkho/files/latest/download


ENV NOTESTS 1
ENV PKG_CONFIG aarch64-linux-gnu-pkg-config
ENV CROSS_TRIPLE aarch64-linux-gnu
ENV HANGOVER_WINE_CC aarch64-linux-gnu-clang
ENV HANGOVER_WINE_CXX aarch64-linux-gnu-clang++
RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        echo hallo amd64 ; \
        dpkg --add-architecture i386 && apt-get update && apt-get install wine wget -y ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        echo hallo arm64 ; \
		apt-get update && apt-get install -y \
			unzip wget python python3 bzip2 pkg-config xz-utils tar htop \
			binutils-aarch64-linux-gnu gcc-aarch64-linux-gnu \
		&&	apt clean \
		&&	rm -rf /var/lib/apt/lists/* ; \
		
		cat /etc/apt/sources.list | grep -v "^#"  | sed "s/^deb /deb [arch=amd64] /g" > /tmp/amd64.list && \
		cat /tmp/amd64.list | sed "s/\[arch=amd64\]/[arch=arm64]/g" > /tmp/arm64.list && \
		cat /tmp/amd64.list /tmp/arm64.list > /etc/apt/sources.list && \
		dpkg --add-architecture arm64 ; \
		
		cat /etc/apt/sources.list ; \

        apt-get update && apt-get install -y --no-install-recommends \
			ca-certificates \
			python \
			flex bison \
			libfreetype6-dev \
			libltdl-dev \
			libxcb1-dev \
			libx11-dev \
			librsvg2-bin \
			gcc-mingw-w64-x86-64 gcc-mingw-w64-i686 \
			automake autoconf pkg-config libtool \
			automake1.11 autoconf2.13 autoconf2.64 \
			gtk-doc-tools git gperf groff p7zip-full \
			gettext \
			make \
			clang \
			dpkg-dev \
			libglib2.0-dev:arm64 \
			libfreetype6-dev:arm64 \
			libltdl-dev:arm64 \
			libxcb1-dev:arm64 \
			libx11-dev:arm64 \
		&&	apt clean \
		&&	rm -rf /var/lib/apt/lists/* \
		&&	ln -s /usr/bin/autoconf /usr/bin/autoconf-2.69 \
		&&	ln -s /usr/bin/autoheader /usr/bin/autoheader-2.69 ; \

        ln -s /usr/bin/clang /usr/bin/aarch64-linux-gnu-clang; ln -s /usr/bin/clang /usr/bin/aarch64-linux-gnu-clang++ ; \
		#now this is quite hacky, but we use winegcc for qemu as compiler and it always defaults to gcc, whereas we need clang ; \
		rm -f /usr/bin/aarch64-linux-gnu-gcc /usr/bin/aarch64-linux-gnu-g++ ; \
		ln -s /usr/bin/clang /usr/bin/aarch64-linux-gnu-gcc; ln -s /usr/bin/clang /usr/bin/aarch64-linux-gnu-g++ ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        echo hallo armv7 ; \
    else \
        echo $TARGETPLATFORM ; \
    fi

RUN if [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        cd /root/hangover ; \
        make -j `nproc` -C /root/hangover -f Makefile ; \
    fi