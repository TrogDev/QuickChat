# The context folder must be the repository root
# docker build -t <repo>/quickchat-message-service -f deployment/services/message/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy service code
COPY src/services/Message ./services/Message
# Copy shared libraries
COPY src/shared ./shared
RUN dotnet restore "./services/Message/src/Message.API/Message.API.csproj"
WORKDIR "/src/services/Message/src/Message.API"
RUN dotnet build "./Message.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Message.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickChat.Message.API.dll"]
