#!/usr/bin/env bash
# DirtBowl local dev data helpers (Postgres in Docker).

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

# Match purge-project-by-unique-id.sh — project name may be "ai-tawala" after a folder rename.
compose_project() {
  if [[ -n "${COMPOSE_PROJECT_NAME:-}" ]]; then
    echo "$COMPOSE_PROJECT_NAME"
    return
  fi
  local from_ctr
  from_ctr="$(
    docker inspect -f '{{index .Config.Labels "com.docker.compose.project"}}' tawala-postgres 2>/dev/null || true
  )"
  if [[ -n "$from_ctr" ]]; then
    echo "$from_ctr"
    return
  fi
  basename "$ROOT"
}

COMPOSE="docker compose -p $(compose_project) -f $ROOT/docker-compose.yml"

cmd="${1:-help}"

psql() {
  if $COMPOSE exec -T postgres true >/dev/null 2>&1; then
    $COMPOSE exec -T postgres psql -U tawala_admin -d tawala "$@"
  else
    docker exec -i tawala-postgres psql -U tawala_admin -d tawala "$@"
  fi
}

case "$cmd" in
  seed-admin)
    echo "Seeding AdminSetup for latest deployed project..."
    psql -f - < "$ROOT/scripts/seed-dirtbowl-admin-setup.sql"
    ;;
  seed-divisions)
    echo "Seeding Divisions for latest deployed project..."
    psql -f - < "$ROOT/scripts/seed-dirtbowl-divisions.sql"
    ;;
  cleanup-registrations)
    echo "Removing test Registration / RegStep2 submissions..."
    psql -f - < "$ROOT/scripts/cleanup-test-registrations.sql"
    ;;
  purge-by-unique-id)
    uid="${2:-}"
    if [[ -z "$uid" ]]; then
      echo "Usage: ./scripts/dev-data.sh purge-by-unique-id <uniqueId>" >&2
      exit 2
    fi
    exec "$ROOT/scripts/purge-project-by-unique-id.sh" "$uid"
    ;;
  status)
    psql -c "SELECT form, COUNT(*) FROM submission GROUP BY form ORDER BY form;"
    ;;
  help|*)
    cat <<'EOF'
Usage: ./scripts/dev-data.sh <command>

  seed-admin              Insert/replace AdminSetup (fee, address, league)
  seed-divisions          Insert/replace Divisions (Q5/Q6 pickers)
  cleanup-registrations   Delete Registration + RegStep2 test rows
  purge-by-unique-id ID   Delete all submissions for /p/{ID}/… (any project)
  status                  Show submission counts by form

Requires: docker compose up -d (postgres running)
EOF
    ;;
esac
