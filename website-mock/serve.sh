#!/usr/bin/env bash
# Serve website-mock on :5500 from this directory (not the repo root).
# Wrong cwd → http://localhost:5500/library.html returns 404.
set -euo pipefail
cd "$(dirname "$0")"
PORT="${1:-5500}"
echo "Serving $(pwd) at http://127.0.0.1:${PORT}/"
echo "Library: http://127.0.0.1:${PORT}/library.html"
exec python3 -m http.server "$PORT" --bind 127.0.0.1 --directory "$(pwd)"
