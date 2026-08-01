#!/usr/bin/env bash
# Read-only dump of all form submissions for a deployed project (by unique_random_id /
# :8080 /p/{id}/…), as a single JSON array on stdout between marker lines.
# Backs website-mock EXPORT (data only) and BACKUP (definition + this same data).
#
# Usage: ./scripts/export-project-responses.sh <uniqueId>
# Output: one line "EXPORT_JSON_BEGIN", one line of JSON (array of
#   {submission_id, form, contents, created_dt}), one line "EXPORT_JSON_END".
#   contents is the raw XStream `<linked-hash-map>` XML string (see FormSubmission.java);
#   the designer-web server parses it into flat field/value columns.

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
  # `</dev/null` avoids racing our own stdin with the -T reachability check (see
  # restore-project-responses.sh for the failure mode this prevents).
  if $COMPOSE exec -T postgres true </dev/null >/dev/null 2>&1; then
    $COMPOSE exec -T postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  else
    docker exec -i tawala-postgres psql -U tawala_admin -d tawala -v ON_ERROR_STOP=1 -P pager=off "$@"
  fi
}

found="$(psql -tAc "SELECT COUNT(*)::text FROM user_project WHERE unique_random_id = '${uid}';" </dev/null | tr -d '[:space:]')"
if [[ "$found" == "0" || -z "$found" ]]; then
  echo "No user_project row for uniqueId=${uid}." >&2
  exit 1
fi

echo "EXPORT_JSON_BEGIN"
psql -tAc "
SELECT COALESCE(json_agg(row_to_json(t)), '[]'::json)::text
FROM (
  SELECT s.submission_id, s.form, s.contents, s.created_dt
  FROM submission s
  JOIN user_project up ON s.project_id = up.project_id
  WHERE up.unique_random_id = '${uid}'
  ORDER BY s.submission_id
) t;
"
echo "EXPORT_JSON_END"
