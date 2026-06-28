#!/usr/bin/env bash
# DirtBowl local dev data helpers (Postgres in Docker).

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
COMPOSE="docker compose -f $ROOT/docker-compose.yml"

cmd="${1:-help}"

psql() {
  $COMPOSE exec -T postgres psql -U tawala_admin -d tawala "$@"
}

case "$cmd" in
  seed-admin)
    echo "Seeding AdminSetup for latest deployed project..."
    psql -f - < "$ROOT/scripts/seed-dirtbowl-admin-setup.sql"
    ;;
  cleanup-registrations)
    echo "Removing test Registration / RegStep2 submissions..."
    psql -f - < "$ROOT/scripts/cleanup-test-registrations.sql"
    ;;
  status)
    psql -c "SELECT form, COUNT(*) FROM submission GROUP BY form ORDER BY form;"
    ;;
  help|*)
    cat <<'EOF'
Usage: ./scripts/dev-data.sh <command>

  seed-admin              Insert/replace AdminSetup (fee, address, league)
  cleanup-registrations   Delete Registration + RegStep2 test rows
  status                  Show submission counts by form

Requires: docker compose up -d (postgres running)
EOF
    ;;
esac
