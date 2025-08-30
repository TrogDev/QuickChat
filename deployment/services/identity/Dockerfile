# The context folder must be the repository root
# docker build -t <repo>/quickchat-identity-service -f deployment/services/identity/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy service code
COPY src/services/Identity ./services/Identity
# Copy shared libraries
COPY src/shared ./shared
RUN dotnet restore "./services/Identity/src/Identity.API/Identity.API.csproj"
WORKDIR "/src/services/Identity/src/Identity.API"
RUN dotnet build "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickChat.Identity.API.dll"]
