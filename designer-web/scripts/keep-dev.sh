#!/usr/bin/env bash
# Keep Designer (Vite :5173 + API :3001) alive in a Terminal you leave open.
#
# Why: agent/IDE tool shells get reaped; when the parent dies, concurrently and
# both servers die with it → Deploy "Failed to fetch". This script is meant to
# run in Terminal.app (or iTerm) that *you* keep open. Each child restarts on
# crash without killing the other.
#
# Usage:
#   cd ~/Projects/AI-Tawala/designer-web
#   npm run keep          # Deploy → Tomcat :8080 when available
#   npm run keep:local    # Node-only Deploy (no Tomcat)
#
# Stop: Ctrl+C in that Terminal.
#
# Or double-click: scripts/Keep-Designer-Dev.command
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

echo "════════════════════════════════════════════════════════"
echo "  Tawala Designer — keep-alive stack"
echo "  UI:  http://localhost:5173"
echo "  API: http://localhost:3001/api/health"
echo "  Leave this Terminal open. Ctrl+C to stop."
echo "════════════════════════════════════════════════════════"

# Clear stale listeners once so a prior dead shell does not block ports.
for port in 3001 5173; do
  pids=$(lsof -ti:"$port" 2>/dev/null || true)
  if [ -n "$pids" ]; then
    echo "Stopping leftover process on port $port..."
    # shellcheck disable=SC2086
    kill $pids 2>/dev/null || true
    sleep 1
  fi
done

if [ "${TAWALA_DEV_ONLY:-}" = "1" ]; then
  echo "Mode: local-only (Deploy → :5173 runtime)"
  unset TAWALA_JAVA_URL || true
  export TAWALA_DEV_HOST="${TAWALA_DEV_HOST:-http://localhost:5173}"
  case "${TAWALA_DEV_HOST}" in
    *:8080|*:8080/*) export TAWALA_DEV_HOST=http://localhost:5173 ;;
  esac
else
  export TAWALA_JAVA_URL="${TAWALA_JAVA_URL:-http://localhost:8080}"
  export TAWALA_DEV_HOST="${TAWALA_DEV_HOST:-http://localhost:5173}"
  echo "Mode: Java Deploy when Tomcat is up ($TAWALA_JAVA_URL)"
fi

# Restart forever independently: API crash must not take Vite down (and vice versa).
# Negative --restart-tries = forever. Delay 1s to avoid tight crash loops.
exec npx concurrently \
  -n web,api \
  -c blue,green \
  --restart-tries -1 \
  --restart-after 1000 \
  "vite" \
  "node server/index.mjs"
