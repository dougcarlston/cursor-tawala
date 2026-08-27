#!/usr/bin/env bash
# Prep for the Designer Cursor chat: health-check Vite :5173 + API :3001,
# optionally start them (and Tomcat for Push), print copy-paste opener.
#
# Usage (from repo root):
#   ./scripts/start-designer-chat.sh              # check + print opener
#   ./scripts/start-designer-chat.sh --start      # start missing services, then print
#   ./scripts/start-designer-chat.sh --copy       # also copy opener to clipboard (macOS)
#   ./scripts/start-designer-chat.sh --start --copy
#
# Prefer a Terminal you leave open for the UI:
#   cd designer-web && npm run keep
# This script can background Vite if :5173 is down; `npm run keep` is more reliable.
#
# See also: docs/START_DESIGNER_CHAT.md · docs/CHAT_HANDOFF.md Chat 1
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
      sed -n '2,16p' "$0" | sed 's/^# \?//'
      exit 0
      ;;
    *)
      echo "Unknown option: $arg (try --help)" >&2
      exit 2
      ;;
  esac
done

DESIGNER_BASE="http://localhost:5173"
API_BASE="http://localhost:3001"
JAVA_BASE="http://localhost:8080"

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
  hdr "Starting Docker (Postgres + Tomcat) — needed for Push checks"
  if ! docker_running; then
    bad "Docker not running — start Docker Desktop, then re-run with --start"
    return 1
  fi
  if [[ ! -f "$ROOT/ROOT.war" ]]; then
    warn "ROOT.war missing — building (one-time)…"
    (cd "$ROOT/TawalaWebapp-build1700" && ./scripts/build-root-war.sh)
  fi

  if container_exists tawala-postgres && ! container_exists tawala-tomcat; then
    warn "Postgres already running — starting Tomcat only (--no-deps)"
    docker compose up -d --no-deps tawala
  elif container_exists tawala-postgres; then
    docker compose up -d tawala 2>/dev/null || docker compose up -d --no-deps tawala
  else
    docker compose up -d --build
  fi

  fix_tomcat_postgres_network
}

start_designer_api() {
  hdr "Ensuring Designer API (:3001)"
  if [[ -x "$ROOT/designer-web/scripts/ensure-dev-api.sh" ]]; then
    "$ROOT/designer-web/scripts/ensure-dev-api.sh" || warn "Designer API ensure script failed"
  else
    bad "ensure-dev-api.sh not found"
    return 1
  fi
}

start_designer_ui() {
  hdr "Ensuring Designer UI (:5173)"
  local code
  code="$(http_code "${DESIGNER_BASE}/")"
  if [[ "$code" == "200" ]]; then
    ok "Designer UI already up (${DESIGNER_BASE})"
    return 0
  fi

  warn "Starting Vite in the background — for a day of work, prefer a Terminal with: cd designer-web && npm run keep"
  (
    cd "$ROOT/designer-web"
    nohup npx vite >> .vite-keep.out 2>&1 &
  )
  local i
  for i in $(seq 1 20); do
    code="$(http_code "${DESIGNER_BASE}/")"
    if [[ "$code" == "200" ]]; then
      ok "Designer UI started (${DESIGNER_BASE})"
      return 0
    fi
    sleep 0.5
  done
  bad "Designer UI did not respond on :5173 — in a Terminal: cd designer-web && npm run keep"
  return 1
}

run_checks() {
  hdr "Environment"
  printf '  Repo:    %s\n' "$ROOT"
  printf '  Branch:  %s\n' "$(git -C "$ROOT" branch --show-current 2>/dev/null || echo '?')"
  printf '  Commit:  %s\n' "$(git -C "$ROOT" log -1 --oneline 2>/dev/null || echo '?')"

  hdr "Service health"
  local ui_code api_code java_code

  ui_code="$(http_code "${DESIGNER_BASE}/")"
  if [[ "$ui_code" == "200" ]]; then
    ok "Designer UI ${DESIGNER_BASE} → 200"
  else
    bad "Designer UI → ${ui_code} — in a Terminal you leave open: cd designer-web && npm run keep"
  fi

  api_code="$(http_code "${API_BASE}/api/health")"
  if [[ "$api_code" == "200" ]]; then
    ok "Designer API ${API_BASE}/api/health → 200"
  else
    bad "Designer API → ${api_code} — run: designer-web/scripts/ensure-dev-api.sh"
    warn "Vite on :5173 often stays up while :3001 dies → Push/Preview 'failed to fetch'"
  fi

  java_code="$(http_code "${JAVA_BASE}/home")"
  if [[ "$java_code" == "200" ]]; then
    ok "Tomcat ${JAVA_BASE}/home → 200 (Push / live form check)"
  else
    warn "Tomcat ${JAVA_BASE}/home → ${java_code} (optional until you Push; --start will try Docker)"
  fi
}

if [[ "$DO_START" == "1" ]]; then
  start_docker_stack || true
  start_designer_api || true
  start_designer_ui || true
fi

run_checks

GIT_COMMIT="$(git -C "$ROOT" rev-parse --short HEAD 2>/dev/null || echo unknown)"
GIT_BRANCH="$(git -C "$ROOT" branch --show-current 2>/dev/null || echo unknown)"

read -r -d '' OPENER <<EOF || true
Project: Tawala (${ROOT})
Track: Browser Designer — designer-web/ — chat title: Designer thread
Branch: ${GIT_BRANCH} @ ${GIT_COMMIT}
Goal: Designer canvas pass. FIRST: Document two links on one line merge underlines (Shared To-Do → Document - User Menu). Then parked Document smoothness P0s if we get there. Design canvas ≫ Deploy/Push ≫ Preview. Do NOT mix website-mock / Library look-and-feel in this chat.
Read first (in order):
1. docs/START_DESIGNER_CHAT.md
2. Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md § Document — two links on one line merge underlines
3. Tawala_Key_Documents/DESIGNER_DOCUMENT_EDITOR.md + DESIGNER_INSERT_MENU_AND_FUNCTIONS.md § Smoke — Link…
4. .cursor/rules/tawala-work-scopes.mdc (Designer track — Design canvas first)
5. .cursor/rules/tawala-designer-parked-post-website.mdc (parked polish; Link/Push/Theme MUST DOs already done)
Constraints:
- Work in designer-web/ (and C#/specs for truth). Do not refactor website-mock Library listing cosmetics here.
- Preview/runtime fixes stay in designer-web/server/ — do not change Design canvas to match Preview.
- Push checks use http://localhost:5173 + :3001 + :8080. Never www.tawala.com.
- No commit unless I explicitly ask
Review URLs:
- Designer: ${DESIGNER_BASE}
- API health: ${API_BASE}/api/health
- Tomcat (Push): ${JAVA_BASE}/home
First task: Shared To-Do → Document - User Menu — two Form links on one line with “or” between them must not share one underline (Design and Push). Confirm whether click targets are merged too.
EOF

read -r -d '' PUNCH <<'EOF' || true
Designer punch list (Aug 26 resume):
  1. P0 this chat — Document two links on one line merge underlines (Not blocking for Live Library; owner wants it next)
     Repro: Shared To-Do → Document - User Menu. Shots in Tawala_Key_Documents/assets/Bug_-_Document-two-links-merged-underline-*.png
  2. Parked Document smoothness P0s — Face/Size multi-chip, invent-with-tables, chip drag, table reflow
  3. Parked polish (do not start unless I ask): native confirm(), Font Color picker, Skip/Process Cut/Copy/Paste/Undo stubs
  4. Jul 30 / Aug 11 polish — most CLOSED Aug 20–21; do not reopen unless a regression is reported
  5. .tawala convert batch C2/C3 — wait until Library vetting inventories examples
Do not pull into this chat:
  - Public Library listing look-and-feel (owner prefers My Tawala / Project Details; cosmetics before any public switch)
  - Going live but private / stranger gates (Test Drive #14, accounts, payments) — next Library session after Designer
EOF

hdr "Cursor — new chat steps"
printf '  1. New Chat in Cursor (Cmd+L → New Chat)\n'
printf '  2. Rename chat: %sDesigner thread%s\n' "$BLD" "$RST"
printf '  3. Paste the START SCRIPT below as your first message\n'
printf '  4. Optional @-attach: docs/START_DESIGNER_CHAT.md, Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md\n'

hdr "START SCRIPT (paste as first message)"
printf '%s\n' "$OPENER"

hdr "Designer punch list (reference)"
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
ok "Prep complete. Run with --start --copy to start services and copy the opener."
ok "For a full day in Designer, leave a Terminal open: cd designer-web && npm run keep"
