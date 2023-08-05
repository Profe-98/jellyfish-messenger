# Verwende das offizielle .NET SDK-Basisimage aus dem Docker Hub für das Build
# diese Datei muss im Stammverzeichnis liegen, also da wo die .csproj vom Backendprojekt liegt
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Setze das Arbeitsverzeichnis im Container
WORKDIR /src

# Kopiere das Projekt-Datei und den Rest der App in den Container
COPY ["jellyfish-messenger/JellyFishBackend/.","jellyfish-messenger/JellyFishBackend/"]
COPY ["generic-aspnet-webapi/WebApiApplicationServiceV2/.","generic-aspnet-webapi/WebApiApplicationServiceV2/"]
COPY ["generic-aspnet-webapi/WebApiFunction/.","generic-aspnet-webapi/WebApiFunction/"]

# Build der ASP.NET Core App
WORKDIR /src/jellyfish-messenger/JellyFishBackend
RUN dotnet restore "JellyFishBackend.csproj"
RUN dotnet build "JellyFishBackend.csproj" -c Release -o /app
RUN dotnet publish "JellyFishBackend.csproj" -c Release -o /app

# Verwende das offizielle ASP.NET Core Runtime-Basisimage aus dem Docker Hub für das Deployment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Setze das Arbeitsverzeichnis im Container
WORKDIR /app

# Kopiere die veröffentlichten Dateien aus dem Build-Container in den Runtime-Container
COPY --from=build /app ./
ENV ASPNETCORE_ENVIRONMENT = Production
ENV DOTNET_ENVIRONMENT = Production
EXPOSE 80

# Starte die ASP.NET Core App im Container
ENTRYPOINT ["dotnet", "JellyFishBackend.dll"]