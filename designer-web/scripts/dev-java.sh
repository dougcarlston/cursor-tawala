#!/usr/bin/env bash
# Start Designer with deploy explicitly forwarded to Tomcat on :8080.
set -euo pipefail
cd "$(dirname "$0")/.."
export TAWALA_JAVA_URL=http://localhost:8080
exec npm run dev -- "$@"
