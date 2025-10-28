@echo off
echo 🚀 Lancement de l'infrastructure Docker...
docker-compose up -d

echo ⏳ Attente 5 secondes pour que les services démarrent...
timeout /t 5 >nul

echo ▶️ Lancement de l'API en mode hot reload...
cd src\BambaIba.Api
dotnet watch run
