#!/usr/bin/env bash
# Build ROOT.war from TawalaWebapp-build1700 sources (no unit tests).
# Requires: JDK 8 (or 11 with --release 8), Apache Ant.
# Postgres must be running: docker compose up -d (from repo root).

set -euo pipefail
cd "$(dirname "$0")/.."

if ! command -v java >/dev/null 2>&1; then
  echo "Java not found. Install JDK 8, e.g.: brew install --cask temurin@8" >&2
  exit 1
fi
if ! command -v ant >/dev/null 2>&1; then
  SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
  if [[ -x "$SCRIPT_DIR/../tools/apache-ant-1.10.14/bin/ant" ]]; then
    ANT="$SCRIPT_DIR/../tools/apache-ant-1.10.14/bin/ant"
  else
    echo "Apache Ant not found. Install with: brew install ant" >&2
    exit 1
  fi
else
  ANT=ant
fi

if [[ -z "${JAVA_HOME:-}" ]] && [[ -d /Library/Java/JavaVirtualMachines/jdk1.8.0_102.jdk/Contents/Home ]]; then
  export JAVA_HOME=/Library/Java/JavaVirtualMachines/jdk1.8.0_102.jdk/Contents/Home
fi

echo "Compiling and packaging tawala.war.dev ..."
"$ANT" -f build.xml war-without-videos -Dlabel="local-$(date +%Y%m%d)"

OUT="tawala.war.dev"
if [[ ! -f "$OUT" ]]; then
  echo "Build failed: $OUT not created" >&2
  exit 1
fi

cp -f "$OUT" ../ROOT.war
echo ""
echo "Wrote $(pwd)/$OUT"
echo "Copied to $(pwd)/../ROOT.war"
echo ""
echo "Deploy to Tomcat 7 as webapps/ROOT.war, or:"
echo "  export CATALINA_HOME=... && cp ../ROOT.war \$CATALINA_HOME/webapps/"
