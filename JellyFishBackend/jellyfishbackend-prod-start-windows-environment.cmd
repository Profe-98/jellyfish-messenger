@echo off
C:
cd C:\Users\Mika\Desktop\Projekte\jellyfish-messenger\JellyFishBackend\bin\Release\net7.0
set ASPNETCORE_ENVIRONMENT=Production
set DOTNET_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://jellyfish-backend01:5030
@echo on
echo "[JELLYFISH-BACKEND]: Start script for Windows"
echo "[JELLYFISH-BACKEND]: Try to start jellyfish-backend"
echo "[JELLYFISH-BACKEND]: Minimal infrastructure requirments on host:"
echo "                     mysql (schemas: rest_api,jellyfish), rabbitmq"
dotnet JellyFishBackend.dll
echo "[JELLYFISH-BACKEND]: jellyfish-backend process exit, press any key to exit console"
@echo off
pause