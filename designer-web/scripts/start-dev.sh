#!/usr/bin/env bash
# Start Designer (Vite + API). Deploy targets Tomcat on :8080 unless TAWALA_DEV_ONLY=1.
#
# Prefer `npm run keep` (scripts/keep-dev.sh) in a Terminal you leave open —
# that path restarts crashed children independently. This script is the short
# `npm run dev` entry and now also auto-restarts children.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

# Kill stale dev servers so code changes and JAVA_URL always apply.
for port in 3001 5173; do
  pids=$(lsof -ti:"$port" 2>/dev/null || true)
  if [ -n "$pids" ]; then
    echo "Stopping process on port $port..."
    # shellcheck disable=SC2086
    kill $pids 2>/dev/null || true
    sleep 1
  fi
done

if [ "${TAWALA_DEV_ONLY:-}" = "1" ]; then
  echo "TAWALA_DEV_ONLY=1 — deploy uses dev runtime on :5173"
  unset TAWALA_JAVA_URL
  export TAWALA_DEV_HOST="${TAWALA_DEV_HOST:-http://localhost:5173}"
  # Never keep a stale Tomcat base for form actions in local-only mode.
  case "${TAWALA_DEV_HOST}" in
    *:8080|*:8080/*) export TAWALA_DEV_HOST=http://localhost:5173 ;;
  esac
else
  export TAWALA_JAVA_URL="${TAWALA_JAVA_URL:-http://localhost:8080}"
  export TAWALA_DEV_HOST="${TAWALA_DEV_HOST:-http://localhost:5173}"
  echo "Deploy target: $TAWALA_JAVA_URL"
fi

# Independent forever-restart so one crash does not strand the other as "Failed to fetch".
exec npx concurrently \
  -n web,api \
  -c blue,green \
  --restart-tries -1 \
  --restart-after 1000 \
  "vite" \
  "node server/index.mjs"
