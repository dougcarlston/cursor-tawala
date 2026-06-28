#!/usr/bin/env bash
# Publish local main to github.com/dougcarlston/cursor-tawala
# Run from your Mac Terminal (not inside Cursor agent) so Git can prompt for auth.

set -euo pipefail
cd "$(dirname "$0")/.."

echo "Local commits on main:"
git log --oneline origin/main..main 2>/dev/null || git log --oneline -5

git fetch origin

if git merge-base --is-ancestor origin/main main 2>/dev/null; then
  echo "Fast-forward or direct push possible."
  git push -u origin main
  exit 0
fi

echo "Remote has unrelated history (GitHub 'Initial commit' README only)."
echo "Merging origin/main, then pushing project commits..."
git pull origin main --allow-unrelated-histories --no-rebase --no-edit
git push -u origin main

echo "Done: https://github.com/dougcarlston/cursor-tawala"
