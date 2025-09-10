# The context folder must be the repository root
# docker build -t <repo>/quickchat-gateway -f deployment/gateway/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# The gRPC client won't build on Apple Silicon without this workaround.
RUN apt-get update \
  && DEBIAN_FRONTEND=noninteractive \
     apt-get install --no-install-recommends --assume-yes \
        protobuf-compiler-grpc
ENV PROTOBUF_PROTOC=/usr/bin/protoc

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy gateway code
COPY src/gateway/src/Gateway ./gateway/src/Gateway
# Copy shared libraries
COPY src/shared ./shared
RUN dotnet restore "./gateway/src/Gateway/Gateway.csproj"
WORKDIR "/src/gateway/src/Gateway"
RUN dotnet build "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickChat.Gateway.dll"]
