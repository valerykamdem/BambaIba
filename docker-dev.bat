param(
    [string]$Action = "start"
)

$composeFile = "docker-compose.yml"
$composeOverride = "docker-compose.override.yml"
$projectName = "bambaiba_dev"

switch ($Action) {
    "start" {
        Write-Host "🚀 Démarrage de l'environnement Docker ($projectName)..."
        docker compose -p $projectName -f $composeFile -f $composeOverride up -d
        Write-Host "✅ Tous les conteneurs sont démarrés !"
    }
    "stop" {
        Write-Host "🛑 Arrêt de l'environnement Docker..."
        docker compose -p $projectName down
        Write-Host "✅ Environnement arrêté proprement."
    }
    "logs" {
        Write-Host "📜 Affichage des logs..."
        docker compose -p $projectName logs -f
    }
    "clean" {
        Write-Host "♻️ Nettoyage complet..."
        docker compose -p $projectName down -v --remove-orphans
        Write-Host "✅ Tout a été supprimé."
    }
    default {
        Write-Host "Utilisation : ./docker-dev.ps1 [start|stop|logs|clean]"
    }
}
