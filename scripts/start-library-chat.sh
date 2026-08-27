#!/usr/bin/env bash
# Prep for the Library Cursor chat: health-check services, fix common Docker
# network splits, optionally start stack, print copy-paste opener + punch list.
#
# Usage (from repo root):
#   ./scripts/start-library-chat.sh              # check + print opener
#   ./scripts/start-library-chat.sh --start      # start missing services, then print
#   ./scripts/start-library-chat.sh --copy       # also copy opener to clipboard (macOS)
#   ./scripts/start-library-chat.sh --start --copy
#
# See also: docs/START_LIBRARY_CHAT.md · docs/CHAT_HANDOFF.md Chat 3
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

DO_START=0
DO_COPY=0
for arg in "$@"; do
  case "$arg" in
    --start) DO_START=1 ;;
    --copy) DO_COPY=1 ;;
    -h|--help)
      sed -n '2,12p' "$0" | sed 's/^# \?//'
      exit 0
      ;;
    *)
      echo "Unknown option: $arg (try --help)" >&2
      exit 2
      ;;
  esac
done

CACHE_BUST="${LIBRARY_CACHE_BUST:-20260824-lib1}"
HOST="${LIBRARY_MOCK_HOST:-localhost}"
MOCK_BASE="http://${HOST}:5500"
JAVA_BASE="http://localhost:8080"
API_BASE="http://localhost:3001"

RED=$'\033[31m'
GRN=$'\033[32m'
YLW=$'\033[33m'
BLD=$'\033[1m'
RST=$'\033[0m'

ok()   { printf '%s✔%s %s\n' "$GRN" "$RST" "$*"; }
warn() { printf '%s⚠%s %s\n' "$YLW" "$RST" "$*"; }
bad()  { printf '%s✖%s %s\n' "$RED" "$RST" "$*"; }
hdr()  { printf '\n%s%s%s\n' "$BLD" "$*" "$RST"; }

http_code() {
  curl -s -o /dev/null -w '%{http_code}' --connect-timeout 3 "$1" 2>/dev/null || echo '000'
}

docker_running() {
  docker info >/dev/null 2>&1
}

container_exists() {
  docker inspect "$1" >/dev/null 2>&1
}

container_networks() {
  docker inspect "$1" --format '{{range $k,$v := .NetworkSettings.Networks}}{{$k}} {{end}}' 2>/dev/null || true
}

fix_tomcat_postgres_network() {
  container_exists tawala-tomcat || return 0
  container_exists tawala-postgres || return 0

  local pg_net tomcat_nets
  pg_net="$(container_networks tawala-postgres | awk '{print $1}')"
  tomcat_nets="$(container_networks tawala-tomcat)"
  [[ -n "$pg_net" ]] || return 0
  if [[ "$tomcat_nets" == *"$pg_net"* ]]; then
    return 0
  fi

  warn "Tomcat and Postgres on different Docker networks — connecting tawala-tomcat → ${pg_net}"
  docker network connect "$pg_net" tawala-tomcat 2>/dev/null || true
  docker restart tawala-tomcat >/dev/null
  printf '  Waiting for Tomcat after network fix…'
  local i code
  for i in $(seq 1 20); do
    code="$(http_code "${JAVA_BASE}/home")"
    if [[ "$code" == "200" ]]; then
      printf ' ok\n'
      ok "Tomcat /home → 200 after network fix"
      return 0
    fi
    sleep 1
    printf '.'
  done
  printf '\n'
  bad "Tomcat still not healthy after network fix (last /home → ${code})"
}

start_docker_stack() {
  hdr "Starting Docker (Postgres + Tomcat)"
  if [[ ! -f "$ROOT/ROOT.war" ]]; then
    warn "ROOT.war missing — building (one-time)…"
    (cd "$ROOT/TawalaWebapp-build1700" && ./scripts/build-root-war.sh)
  fi

  if container_exists tawala-postgres && ! container_exists tawala-tomcat; then
    warn "Postgres already running — starting Tomcat only (--no-deps)"
    docker compose up -d --no-deps tawala
  elif container_exists tawala-postgres; then
    warn "Postgres container already exists — trying compose up (may no-op postgres)"
    docker compose up -d tawala 2>/dev/null || docker compose up -d --no-deps tawala
  else
    docker compose up -d --build
  fi

  fix_tomcat_postgres_network
}

start_website_mock() {
  hdr "Starting website mock (:5500)"
  local code
  code="$(http_code "${MOCK_BASE}/library.html")"
  if [[ "$code" == "200" ]]; then
    ok "Website mock already up (${MOCK_BASE})"
    return 0
  fi

  if [[ ! -x "$ROOT/website-mock/serve.sh" ]]; then
    bad "website-mock/serve.sh not found or not executable"
    return 1
  fi

  (cd "$ROOT/website-mock" && nohup ./serve.sh >> .serve-supervisor.out 2>&1 &)
  sleep 1
  for i in $(seq 1 15); do
    code="$(http_code "${MOCK_BASE}/library.html")"
    if [[ "$code" == "200" ]]; then
      ok "Website mock started (${MOCK_BASE})"
      return 0
    fi
    sleep 0.5
  done
  bad "Website mock did not respond on :5500 — check website-mock/.serve.log"
}

start_designer_api() {
  hdr "Ensuring Designer API (:3001)"
  if [[ -x "$ROOT/designer-web/scripts/ensure-dev-api.sh" ]]; then
    "$ROOT/designer-web/scripts/ensure-dev-api.sh" || warn "Designer API ensure script failed"
  else
    warn "ensure-dev-api.sh not found — skip :3001 (Purge/Records need it)"
  fi
}

run_checks() {
  hdr "Environment"
  printf '  Repo:    %s\n' "$ROOT"
  printf '  Branch:  %s\n' "$(git -C "$ROOT" branch --show-current 2>/dev/null || echo '?')"
  printf '  Commit:  %s\n' "$(git -C "$ROOT" log -1 --oneline 2>/dev/null || echo '?')"

  hdr "Service health"
  local java_code mock_code api_code

  if docker_running; then
    ok "Docker daemon reachable"
  else
    bad "Docker not running — start Docker Desktop, then re-run with --start"
  fi

  java_code="$(http_code "${JAVA_BASE}/home")"
  if [[ "$java_code" == "200" ]]; then
    ok "Tomcat ${JAVA_BASE}/home → 200"
  else
    bad "Tomcat ${JAVA_BASE}/home → ${java_code} (forms/Test Drive need 200)"
    if docker_running && container_exists tawala-tomcat; then
      warn "If code is 500, run: docker network connect ai-tawala_default tawala-tomcat && docker restart tawala-tomcat"
      warn "Or re-run: ./scripts/start-library-chat.sh --start"
    fi
  fi

  mock_code="$(http_code "${MOCK_BASE}/library.html")"
  if [[ "$mock_code" == "200" ]]; then
    ok "Website mock ${MOCK_BASE}/library.html → 200"
  else
    bad "Website mock → ${mock_code} — run: cd website-mock && ./serve.sh"
    warn "Use http://${HOST}:5500 (not 127.0.0.1) — separate localStorage"
  fi

  api_code="$(http_code "${API_BASE}/api/health")"
  if [[ "$api_code" == "200" ]]; then
    ok "Designer API ${API_BASE}/api/health → 200"
  else
    warn "Designer API → ${api_code} (optional for browsing; needed for Purge/Records/Export)"
  fi

  if container_exists tawala-postgres && container_exists tawala-tomcat; then
    local pg_net tc_nets
    pg_net="$(container_networks tawala-postgres | awk '{print $1}')"
    tc_nets="$(container_networks tawala-tomcat)"
    if [[ -n "$pg_net" && "$tc_nets" != *"$pg_net"* ]]; then
      warn "Network split: postgres on ${pg_net}, tomcat on ${tc_nets} — use --start to auto-fix"
    fi
  fi
}

if [[ "$DO_START" == "1" ]]; then
  if docker_running; then
    start_docker_stack
  fi
  start_website_mock || true
  start_designer_api || true
fi

run_checks

GIT_COMMIT="$(git -C "$ROOT" rev-parse --short HEAD 2>/dev/null || echo unknown)"
GIT_BRANCH="$(git -C "$ROOT" branch --show-current 2>/dev/null || echo unknown)"

read -r -d '' OPENER <<EOF || true
Project: Tawala (${ROOT})
Track: Website mock — website-mock/ (Phase 3) — chat title: Library thread
Branch: ${GIT_BRANCH} @ ${GIT_COMMIT}
Goal: Library-only ops — harden Delete/Purge, EXPORT/IMPORT, BACKUP/RESTORE; Test Drive stability; Publish/stub readiness. Do NOT open Designer canvas work unless a Test Drive URL truly requires deploy wiring.
Read first (in order):
1. docs/START_LIBRARY_CHAT.md
2. docs/CHAT_HANDOFF.md (Chat 3 + Aug 24 checkpoint)
3. website-mock/README.md § Task List (Aug 9) + § Aug 9 decisions
4. .cursor/rules/tawala-work-scopes.mdc (Library track only)
Constraints:
- Use http://${HOST}:5500 for review (NOT 127.0.0.1 — separate localStorage)
- website-mock/ + :5500 + :8080 Test Drive; defer parked Designer polish (.cursor/rules/tawala-designer-parked-post-website.mdc)
- No commit unless I explicitly ask
- If Test Drive shows "We are very sorry": ./scripts/start-library-chat.sh --start
Review URLs (cache-bust ${CACHE_BUST}):
- Library: ${MOCK_BASE}/library.html?v=${CACHE_BUST}
- My Tawala: ${MOCK_BASE}/mytawala.html?v=${CACHE_BUST}
- Simple Survey Test Drive: ${JAVA_BASE}/p/gy1zssbrwm4fgfm/npwtqlg.Survey
First task: P0 Task #26 — private uniqueId on Copy to MyTawala (stop sharing Library :8080 ids). Then confirm health and remaining Task List (#14 leave/wipe, #10 Deploy/share, EXPORT/IMPORT smoked Aug 24).
EOF

read -r -d '' PUNCH <<'EOF' || true
Library punch list (open / partial — Aug 24):
  0. P0 — Private uniqueId on Copy to MyTawala (#26) — retire mockSharedLibraryRuntime; acquire must not mutate Library Test Drive
  1. EXPORT / IMPORT — Excel response-data spine; field-mismatch fails cleanly (owner smoked Aug 24)
  2. BACKUP / RESTORE — paired definition+data .backup ZIP (HOLD on default UX copy — confirm with owner before expanding)
  3. Delete / Purge — scoped confirms; Records refresh; whole-project vs per-form Purge
  4. Test Drive (#14) — honesty copy DONE Aug 24; real leave/wipe + per-drive uniqueId waits for Library Live
  5. Deploy / share panel (#10) — start picker, Copy link, embed; uniqueId hardening
  6. Usage stats (#13) — Times used / Last used increment on My Tawala Use (localStorage mock)
  7. Library stub cleanup (#18) — Publish real replacement, retire stub (agent-run)
Parked (do not start): Auth (#21), Payments (#22), end page (#23), ratings (#17), Designer Document P0s
Designer deferred for later pass: .tawala convert batch C2/C3, Document caret epic, post-website polish rule
EOF

hdr "Cursor — new chat steps"
printf '  1. New Chat in Cursor (Cmd+L → New Chat)\n'
printf '  2. Rename chat: %sLibrary thread%s\n' "$BLD" "$RST"
printf '  3. Paste the START SCRIPT below as your first message\n'
printf '  4. Optional @-attach: docs/START_LIBRARY_CHAT.md, website-mock/README.md\n'

hdr "START SCRIPT (paste as first message)"
printf '%s\n' "$OPENER"

hdr "Library punch list (reference)"
printf '%s\n' "$PUNCH"

if [[ "$DO_COPY" == "1" ]]; then
  if command -v pbcopy >/dev/null 2>&1; then
    printf '%s' "$OPENER" | pbcopy
    ok "Opener copied to clipboard (pbcopy)"
  else
    warn "pbcopy not available — copy the START SCRIPT block manually"
  fi
fi

printf '\n'
ok "Prep complete. Run with --start --copy to auto-start services and copy the opener."
