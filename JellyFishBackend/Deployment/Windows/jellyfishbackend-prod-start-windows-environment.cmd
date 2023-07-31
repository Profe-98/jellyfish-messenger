@echo off
color 2
set /p JellyfishPublishFolder=Input the folderpath of jellyfish backend:
cd %JellyfishPublishFolder%
set ASPNETCORE_ENVIRONMENT=Production
set DOTNET_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://jellyfish-backend01:5030
color d
echo "[JELLYFISH-BACKEND]: Start script for Windows"
echo "[JELLYFISH-BACKEND]: Try to start jellyfish-backend"
echo "[JELLYFISH-BACKEND]: Minimal infrastructure requirments on host:"
color 7
echo "                     mysql (schemas: rest_api,jellyfish), rabbitmq"
timeout 5 > NUL
color 7
dotnet JellyFishBackend.dll
color 4
echo "[JELLYFISH-BACKEND]: jellyfish-backend process exit, press any key to exit console"
@echo off
pause