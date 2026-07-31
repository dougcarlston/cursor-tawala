#!/usr/bin/env bash
# Alias for the supervised server (./serve.sh already auto-restarts on crash).
# Kept so docs / muscle memory can use either name.
exec "$(cd "$(dirname "$0")" && pwd)/serve.sh" "$@"
