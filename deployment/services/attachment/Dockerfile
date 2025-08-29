# The context folder must be the repository root
# docker build -t <repo>/quickchat-attachment-service -f deployment/services/attachment/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy service code
COPY src/services/Attachment ./services/Attachment
# Copy shared libraries
COPY src/shared ./shared
RUN dotnet restore "./services/Attachment/src/Attachment.API/Attachment.API.csproj"
WORKDIR "/src/services/Attachment/src/Attachment.API"
RUN dotnet build "./Attachment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Attachment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickChat.Attachment.API.dll"]
