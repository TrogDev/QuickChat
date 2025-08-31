# The context folder must be the repository root
# docker build -t <repo>/quickchat-chat-service -f deployment/services/chat/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy service code
COPY src/services/Chat ./services/Chat
# Copy shared libraries
COPY src/shared ./shared
RUN dotnet restore "./services/Chat/src/Chat.API/Chat.API.csproj"
WORKDIR "/src/services/Chat/src/Chat.API"
RUN dotnet build "./Chat.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chat.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickChat.Chat.API.dll"]
