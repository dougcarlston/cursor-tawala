#!/usr/bin/env bash
# Start Postgres + Tomcat (Tawala Java backend). Builds ROOT.war first if missing.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

if [[ ! -f ROOT.war ]]; then
  echo "ROOT.war not found — building from sources (one-time, ~15s)..."
  (cd TawalaWebapp-build1700 && ./scripts/build-root-war.sh)
fi

echo "Starting Postgres and Tomcat..."
docker compose up -d --build

echo ""
echo "Waiting for services..."
for i in $(seq 1 60); do
  if curl -fsS http://localhost:8080/login >/dev/null 2>&1; then
    echo ""
    echo "Tawala backend is up."
    echo "  Java app:  http://localhost:8080/login"
    echo "  Postgres:  localhost:5432 (db tawala, user tawala_app / tawala)"
    echo ""
    echo "Point the Designer API at Java:"
    echo "  export TAWALA_JAVA_URL=http://localhost:8080"
    echo "  cd designer-web && npm run dev"
    exit 0
  fi
  sleep 3
done

echo "Tomcat did not become ready in time. Check logs:" >&2
echo "  docker compose logs tawala" >&2
exit 1
