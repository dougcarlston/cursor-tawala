#!/usr/bin/env bash
# Replace all form submissions for a deployed project (by unique_random_id) with a caller-
# supplied set. Backs website-mock RESTORE (paired snapshot data) and IMPORT (response-data
# only) — both are "replace current submissions with this set" at the DB layer; the product-
# level distinction (paired definition+data vs data-only, field-mismatch checks) lives in
# website-mock/js/transfer.js + project-ops.js, not here.
#
# Usage: ./scripts/restore-project-responses.sh <uniqueId>
#   Reads a full SQL script on stdin (DELETE + INSERT statements prepared by the caller,
#   already using dollar-quoted string literals — see designer-web/server/projectResponses.mjs)
#   and runs it as one psql invocation (transaction wraps BEGIN/COMMIT in the supplied SQL).
# Output: "No user_project row…" + exit 1 if uniqueId is unknown; otherwise psql's own output
#   for the supplied script (the caller embeds `SELECT 'INSERTED_COUNT:' || count(*) ...` etc.
#   as the final statement to report counts back to Node).

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

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
  echo "Usage: $0 <uniqueId>  (reads SQL body on stdin)" >&2
  exit 2
fi

if [[ ! "$uid" =~ ^[A-Za-z0-9]{1,20}$ ]]; then
  echo "Invalid uniqueId (expected 1–20 alphanumeric): $uid" >&2
  exit 2
fi

psql() {
  # Reachability check must not read our real stdin (the SQL script) — `docker compose exec -T`
  # forwards fd 0 into the container even for `true`, and without `</dev/null` it races the
  # caller's piped stdin, silently swallowing part/all of the SQL below.
  if $COMPOSE exec -T postgres true </dev/null >/dev/null 2>&1; then
    $COMPOSE exec -T postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  else
    docker exec -i tawala-postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  fi
}

## Reads the SQL body on stdin below — every psql call before that point (including this
## existence check) MUST redirect its own stdin from /dev/null. `docker compose exec -T`
## attaches and drains local stdin even for queries that never read it, so without
## `</dev/null` here this check silently eats the real script meant for `psql -f -`,
## leaving it a no-op EOF (no error, no output, no writes) — the exact failure this comment
## exists to prevent regressing.
found="$(psql -tAc "SELECT COUNT(*)::text FROM user_project WHERE unique_random_id = '${uid}';" </dev/null | tr -d '[:space:]')"
if [[ "$found" == "0" || -z "$found" ]]; then
  echo "No user_project row for uniqueId=${uid}." >&2
  exit 1
fi

# SQL body (already parameterized with this same ${uid} by the caller) arrives on stdin.
psql -f -
