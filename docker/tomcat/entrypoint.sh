#!/bin/sh
# Empty Lucene dirs must not exist — IndexerBase only creates a new index when the
# directory is missing. Pre-created empty folders cause startup failure.
set -e
for dir in /var/tawala/lucene/projects /var/tawala/lucene/users; do
  if [ -d "$dir" ] && [ ! -f "$dir/segments" ]; then
    rm -rf "$dir"
  fi
done
exec catalina.sh run
