#!/usr/bin/env bash
# Ensure Designer API is listening on :3001 (Vite on :5173 often survives while API dies).
# Safe to run anytime Preview/Deploy fails with "failed to fetch".
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

if curl -sf "http://localhost:3001/api/health" >/dev/null 2>&1; then
  echo "API already healthy on http://localhost:3001"
  curl -s "http://localhost:3001/api/health"
  echo
  exit 0
fi

echo "API down — restarting detached on :3001…"
for p in $(lsof -nP -tiTCP:3001 -sTCP:LISTEN 2>/dev/null || true); do
  kill "$p" 2>/dev/null || true
done
sleep 0.3

export TAWALA_DEV_ONLY="${TAWALA_DEV_ONLY:-1}"
export TAWALA_DEV_HOST="${TAWALA_DEV_HOST:-http://localhost:5173}"
export TAWALA_DEV_PORT=3001
LOG="$ROOT/.dev-server-api.log"
PIDFILE="$ROOT/.dev-server-api.pid"

# Double-fork so the API outlives this shell (agent/IDE shell exit otherwise SIGTERM it).
python3 - "$ROOT" <<'PY'
import os, sys, time, shutil
from pathlib import Path
root = Path(sys.argv[1])
log = root / ".dev-server-api.log"
node = "/usr/local/bin/node"
if not os.path.exists(node):
    node = shutil.which("node") or "node"
if os.fork() == 0:
    os.setsid()
    if os.fork() == 0:
        os.chdir(root)
        env = os.environ.copy()
        logf = open(log, "a")
        logf.write("\n--- ensure-dev-api ---\n")
        logf.flush()
        os.dup2(logf.fileno(), 1)
        os.dup2(logf.fileno(), 2)
        os.execve(node, [node, "server/index.mjs"], env)
    else:
        os._exit(0)
else:
    time.sleep(1.2)
PY

for i in 1 2 3 4 5; do
  if curl -sf "http://localhost:3001/api/health" >/dev/null 2>&1; then
    pid=$(lsof -nP -tiTCP:3001 -sTCP:LISTEN 2>/dev/null | head -1 || true)
    echo "$pid" > "$PIDFILE"
    echo "API up (pid $pid). Log: $LOG"
    curl -s "http://localhost:3001/api/health"
    echo
    exit 0
  fi
  sleep 0.5
done
echo "API failed to start — see $LOG" >&2
tail -30 "$LOG" >&2 || true
exit 1
