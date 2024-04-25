# linux-specific base
FROM ubuntu:latest AS base-linux
ARG EXTENSION="x86_64"
USER $APP_UID

# windows-specific base
FROM mcr.microsoft.com/windows/nanoserver:ltsc2022 AS base-windows
ARG EXTENSION="exe"

# linux only build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-dependencies
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet-dependencies/dotnet-dependencies.csproj", "dotnet-dependencies/"]
RUN dotnet restore "dotnet-dependencies/dotnet-dependencies.csproj"
COPY . .
WORKDIR "/src/dotnet-dependencies"
RUN dotnet build "dotnet-dependencies.csproj" -c $BUILD_CONFIGURATION

FROM --platform=$BUILDPLATFORM unityci/editor:ubuntu-2022.3.16f1-linux-il2cpp-3.0.1 AS build-unity-base
WORKDIR "/src/unity-docker-build"
COPY --from=build-dependencies /src/unity-docker-build/ .
RUN --mount=type=secret,id=UNITY_USERNAME \
    --mount=type=secret,id=UNITY_PASSWORD \
    --mount=type=secret,id=UNITY_LICENSE \
    unity-editor -quit -batchmode -serial $(cat /run/secrets/UNITY_LICENSE) -username $(cat /run/secrets/UNITY_USERNAME) -password $(cat /run/secrets/UNITY_PASSWORD)

FROM --platform=$BUILDPLATFORM build-unity-base AS build-unity-linux
RUN unity-editor -quit -batchmode -nographics -buildTarget Linux64 -standaloneBuildSubtarget Server -executeMethod Builder.Build -projectPath .

FROM --platform=$BUILDPLATFORM build-unity-base AS build-unity-windows
RUN unity-editor -quit -batchmode -nographics -buildTarget win64 -standaloneBuildSubtarget Server -executeMethod Builder.Build -projectPath .

FROM --platform=$BUILDPLATFORM build-unity-${TARGETOS} AS build-unity
RUN --mount=type=secret,id=UNITY_USERNAME \
    --mount=type=secret,id=UNITY_PASSWORD \
    unity-editor -quit -batchmode -returnlicense -username $(cat /run/secrets/UNITY_USERNAME) -password $(cat /run/secrets/UNITY_PASSWORD) 

# platform-dynamic final
FROM base-${TARGETOS} AS final
EXPOSE 7777/udp
WORKDIR /app
COPY --from=build-unity /src/unity-docker-build/Build/ .
ENTRYPOINT ["./unity-docker-build.${EXTENSION}"]