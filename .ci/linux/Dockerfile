FROM debian:12

# basic debian installation with:
# - gcc and capstone (for ArmNative)
# - cmake for building
# - 7zip for packaging
# - multiple dotnet SDKs
RUN apt-get update \
    && apt-get install -y \
        locales ca-certificates curl git \
        build-essential gcc g++ cmake libcapstone-dev pkg-config p7zip-full \
    && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8 \
    && curl -LO https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh \
    && bash dotnet-install.sh --channel 6.0 \
    && bash dotnet-install.sh --channel 8.0 \
    && mv /root/.dotnet /usr/share/dotnet \
    && ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && dotnet nuget locals all --clear \
    && rm -f dotnet-install.sh
    # reserved for cache eviction: 2

ENV LANG en_US.utf8
