@echo off
SET PROJECT_NAME=bambaiba_dev
echo ================================
echo 🚀 Lancement de %PROJECT_NAME%
echo ================================

docker compose -p %PROJECT_NAME% up -d

echo.
echo Attente 5 secondes pour que les services démarrent...
timeout /t 5 >nul

echo.
echo ===============================
echo 🌐 Lancement de l'API en mode Hot Reload...
echo ===============================
cd src\BambaIba.Api
dotnet watch run

pause
