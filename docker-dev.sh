#!/bin/bash

set -e

echo "=========================================="
echo " 🚀 BambaIba Docker Manager (Linux/macOS) "
echo "=========================================="
echo "1️⃣  Démarrer tous les services (mode DEV)"
echo "2️⃣  Démarrer uniquement les dépendances (DB, Keycloak, Redis, MinIO)"
echo "3️⃣  Construire l'image API"
echo "4️⃣  Lancer l'API en mode PROD"
echo "5️⃣  Arrêter et supprimer tous les conteneurs"
echo "6️⃣  Voir les logs"
echo "0️⃣  Quitter"
echo "------------------------------------------"

read -p "👉 Que veux-tu faire ? (0-6) : " choice

case "$choice" in
  1)
    echo "▶️  Démarrage complet en mode DEV..."
    docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
    ;;
  2)
    echo "▶️  Démarrage des dépendances (DB, Keycloak, Redis, MinIO)..."
    docker compose up -d bambaiba_db bambaiba_idp_db bambaiba_idp redis minio
    ;;
  3)
    echo "🔨 Construction de l'image API..."
    docker compose -f docker-compose.build.yml build
    ;;
  4)
    echo "🚀 Lancement de l'API en mode PROD..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
    ;;
  5)
    echo "🧹 Nettoyage complet..."
    docker compose down -v
    ;;
  6)
    echo "📜 Affichage des logs..."
    docker compose logs -f
    ;;
  0)
    echo "👋 Sortie du script."
    exit 0
    ;;
  *)
    echo "❌ Choix invalide. Réessaye."
    ;;
esac
