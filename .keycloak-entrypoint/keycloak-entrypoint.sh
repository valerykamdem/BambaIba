#!/bin/bash
set -e

# Nom du conteneur Keycloak
KC_CONTAINER="bambaiba_idp"

# Nom du realm à exporter
REALM="bambaiba-realm"

echo "📦 Export du realm '$REALM' depuis Keycloak..."

docker exec $KC_CONTAINER /opt/keycloak/bin/kc.sh export \
  --dir=/opt/keycloak/data/export \
  --realm=$REALM \
  --users=all

echo "✅ Export terminé !"
echo "📁 Les fichiers sont disponibles dans le dossier local ./.keycloak-export/"
