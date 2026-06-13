#!/usr/bin/env bash
# make-dep-zip.sh – erstellt die Abgabe-ZIP für DEP (Linux vmLP1).
# Ausführen aus dem Repo-Wurzelverzeichnis:  bash scripts/make-dep-zip.sh
# Die ZIP enthält den vollständigen Stack inkl. Quellcode, ohne Build-Artefakte,
# Secrets und Zertifikate. dokumentation.md liegt im ZIP-Wurzelverzeichnis.

set -euo pipefail
repo="$(cd "$(dirname "$0")/.." && pwd)"
cd "$repo"

out="$HOME/Schreibtisch/Abgabe_DEP.zip"
[ -d "$HOME/Schreibtisch" ] || out="$HOME/Desktop/Abgabe_DEP.zip"
[ -d "$(dirname "$out")" ] || out="$repo/Abgabe_DEP.zip"

rm -f "$out"

zip -r "$out" \
  docker-compose.yml \
  .env.example \
  dokumentation.md \
  deploy \
  backend/UBahnApi \
  frontend \
  -x "*/bin/*" "*/obj/*" "*/.vs/*" \
     "*/node_modules/*" "*/dist/*" \
     "*/appsettings.Development.json" \
     "frontend/.env.local" \
     "deploy/certs/*.pem" \
  >/dev/null

echo "Fertig: $out"
unzip -l "$out" | tail -5
