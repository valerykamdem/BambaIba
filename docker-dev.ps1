# ============================================================
# Docker Dev Script - BambaIba (version sans accent ni emoji)
# ============================================================

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "============================================================"
Write-Host "Lancement et gestion Docker - Projet BambaIba (DEV)"
Write-Host "============================================================`n"

Write-Host "1 - Lancer toute l'infrastructure + API (Hot Reload)"
Write-Host "2 - Lancer seulement l'API (Hot Reload)"
Write-Host "3 - Rebuild complet des images"
Write-Host "4 - Arreter tous les conteneurs"
Write-Host "5 - Nettoyer tout (volumes, images, reseaux)"
Write-Host "0 - Quitter`n"

$choice = Read-Host "Que veux-tu faire ? (0-5)"
Write-Host ""

function Run-Command($cmd, $msg) {
    Write-Host ">>> $msg"
    Invoke-Expression $cmd
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Erreur pendant : $msg" -ForegroundColor Red
        exit 1
    }
    Write-Host "$msg termine avec succes.`n" -ForegroundColor Green
}

switch ($choice) {

    1 {
        Run-Command "docker compose -f docker-compose.yml -f docker-compose.override.yml up -d" "Lancement de l'infrastructure"
        Start-Sleep -Seconds 5
        Write-Host "API Hot Reload disponible sur http://localhost:7000"
    }

    2 {
        Run-Command "docker compose -f docker-compose.yml -f docker-compose.override.yml up -d bambaiba_db bambaiba_idp_db bambaiba_idp redis minio" "Lancement des services de base"
        Run-Command "docker compose -f docker-compose.yml -f docker-compose.override.yml up bambaiba_api" "Lancement de l'API en Hot Reload"
    }

    3 {
        Run-Command "docker compose down --volumes --remove-orphans" "Arret des conteneurs existants"
        Run-Command "docker compose build --no-cache" "Reconstruction complete des images"
        Run-Command "docker compose up -d" "Relancement complet"
    }

    4 {
        Run-Command "docker compose down" "Arret des conteneurs"
    }

    5 {
        Run-Command "docker compose down --volumes --remove-orphans" "Arret et suppression des conteneurs et volumes"
        Run-Command "docker system prune -a -f" "Nettoyage complet Docker"
    }

    0 {
        Write-Host "Fermeture du script..."
        exit 0
    }

    default {
        Write-Host "Choix invalide. Reessaie avec un nombre entre 0 et 5." -ForegroundColor Yellow
    }
}
