# Verwende das offizielle .NET SDK-Basisimage aus dem Docker Hub für das Build
# diese Datei muss im Stammverzeichnis liegen, also da wo die .csproj vom Backendprojekt liegt
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Setze das Arbeitsverzeichnis im Container
WORKDIR /src

# Kopiere das Projekt-Datei und den Rest der App in den Container
COPY ["jellyfish-messenger/Application.Web.Api.JellyFishBackend/.","jellyfish-messenger/Application.Web.Api.JellyFishBackend/"]
COPY ["generic-aspnet-webapi/Application.Shared.Web.Api.Shared/.","generic-aspnet-webapi/Application.Shared.Web.Api.Shared/"]
COPY ["generic-aspnet-webapi/Application.Shared.Kernel/.","generic-aspnet-webapi/Application.Shared.Kernel/"]

# Build der ASP.NET Core App
WORKDIR /src/jellyfish-messenger/Application.Web.Api.JellyFishBackend
RUN dotnet restore "Application.Web.Api.JellyFishBackend.csproj"
RUN dotnet build "Application.Web.Api.JellyFishBackend.csproj" -c Release -o /app
RUN dotnet publish "Application.Web.Api.JellyFishBackend.csproj" -c Release -o /app

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
ENTRYPOINT ["dotnet", "Application.Web.Api.JellyFishBackend.dll"]