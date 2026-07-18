#!/usr/bin/env bash
# Double-click in Finder to open Terminal and keep the Designer stack running.
# Same as: cd designer-web && npm run keep
cd "$(dirname "$0")/.." || exit 1
echo "Starting keep-alive Designer stack (Ctrl+C to stop)…"
npm run keep
echo
echo "Stack stopped. Press Enter to close this window."
read -r
