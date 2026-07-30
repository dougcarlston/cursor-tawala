#!/usr/bin/env bash
# Purge all form submissions for a deployed project (by unique_random_id / :8080 /p/{id}/…).
# Mirrors Java Project Manager purgeProjectResponses for local Docker Postgres.
#
# Usage: ./scripts/purge-project-by-unique-id.sh <uniqueId>
# Example: ./scripts/purge-project-by-unique-id.sh gy1zssbrwm4fgfm

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

# Compose project name can lag a folder rename (containers here are still "ai-tawala"
# while this tree is Tawala/). Prefer env, then label on the named postgres container.
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

uid="${1:-}"
if [[ -z "$uid" ]]; then
  echo "Usage: $0 <uniqueId>" >&2
  exit 2
fi

# unique_random_id is VARCHAR(20); reject anything that is not safe for SQL literals.
if [[ ! "$uid" =~ ^[A-Za-z0-9]{1,20}$ ]]; then
  echo "Invalid uniqueId (expected 1–20 alphanumeric): $uid" >&2
  exit 2
fi

psql() {
  # Prefer compose exec; fall back to fixed container_name if project mapping drifts.
  if $COMPOSE exec -T postgres true >/dev/null 2>&1; then
    $COMPOSE exec -T postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  else
    docker exec -i tawala-postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  fi
}

echo "Purging submissions for unique_random_id=${uid}…"

found="$(psql -tAc "SELECT COUNT(*)::text FROM user_project WHERE unique_random_id = '${uid}';" | tr -d '[:space:]')"
if [[ "$found" != "1" && "$found" != "0" ]]; then
  # unexpected; continue carefully
  :
fi
if [[ "$found" == "0" || -z "$found" ]]; then
  echo "No user_project row for uniqueId=${uid}." >&2
  exit 1
fi

psql -c "
SELECT up.name AS project_name, up.unique_random_id, up.project_id,
       COUNT(s.submission_id) AS rows_before
FROM user_project up
LEFT JOIN submission s ON s.project_id = up.project_id
WHERE up.unique_random_id = '${uid}'
GROUP BY up.name, up.unique_random_id, up.project_id;
"

deleted="$(psql -tAc "
WITH gone AS (
  DELETE FROM submission s
  USING user_project up
  WHERE s.project_id = up.project_id
    AND up.unique_random_id = '${uid}'
  RETURNING s.submission_id
)
SELECT COUNT(*)::text FROM gone;
" | tr -d '[:space:]')"

echo "Deleted ${deleted:-0} submission row(s)."

psql -c "
SELECT up.name AS project_name, up.unique_random_id,
       COUNT(s.submission_id) AS rows_after
FROM user_project up
LEFT JOIN submission s ON s.project_id = up.project_id
WHERE up.unique_random_id = '${uid}'
GROUP BY up.name, up.unique_random_id;
"
