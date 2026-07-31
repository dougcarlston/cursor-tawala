#!/usr/bin/env bash
# Supervised static server for website-mock on :5500 (binds 127.0.0.1).
#
# Always run from this directory (script cd's itself):
#   cd website-mock && ./serve.sh
#
# Commands:
#   ./serve.sh              # start supervised server (default port 5500)
#   ./serve.sh 5501         # alternate port
#   ./serve.sh stop         # stop via pid file / known listener
#   ./serve.sh status       # print listener + pid file
#   ./serve.sh once         # one shot (no restart loop) — for debugging
#
# Env:
#   SERVE_FORCE=1           # kill whatever holds the port (even if not ours)
#   SERVE_BACKEND=node|python  # force backend (default: node if available)
#
# Logs: website-mock/.serve.log   Pid: website-mock/.serve.pid
set -uo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

PORT="${1:-5500}"
CMD="start"
if [[ "${1:-}" == "stop" || "${1:-}" == "status" || "${1:-}" == "once" ]]; then
  CMD="$1"
  PORT="${2:-5500}"
elif [[ "${1:-}" =~ ^[0-9]+$ ]]; then
  PORT="$1"
  CMD="start"
fi

HOST="127.0.0.1"
PIDFILE="$ROOT/.serve.pid"
LOGFILE="$ROOT/.serve.log"
FORCE="${SERVE_FORCE:-0}"

log() {
  local line="[$(date '+%Y-%m-%d %H:%M:%S')] $*"
  printf '%s\n' "$line" | tee -a "$LOGFILE" >&2
}

listener_pid() {
  # Prefer lsof; fall back to empty
  lsof -nP -iTCP:"$PORT" -sTCP:LISTEN 2>/dev/null | awk 'NR>1 {print $2; exit}'
}

cmd_for_pid() {
  local pid="$1"
  ps -p "$pid" -o command= 2>/dev/null || true
}

is_our_server_cmd() {
  local c="$1"
  [[ -z "$c" ]] && return 1
  # Our Node static server
  if [[ "$c" == *"serve-static.mjs"* ]]; then
    return 0
  fi
  # Python http.server aimed at this tree (legacy / fallback)
  if [[ "$c" == *"http.server"* && ( "$c" == *"$ROOT"* || "$c" == *"website-mock"* ) ]]; then
    return 0
  fi
  if [[ "$c" == *"http.server"* && "$c" == *"$PORT"* ]]; then
    # Likely a prior mock server started from this dir without absolute --directory
    return 0
  fi
  return 1
}

stop_pid() {
  local pid="$1"
  local reason="${2:-stop}"
  if [[ -z "$pid" ]] || ! kill -0 "$pid" 2>/dev/null; then
    return 0
  fi
  log "Stopping pid $pid ($reason)"
  kill "$pid" 2>/dev/null || true
  local i=0
  while kill -0 "$pid" 2>/dev/null && (( i < 20 )); do
    sleep 0.1
    i=$((i + 1))
  done
  if kill -0 "$pid" 2>/dev/null; then
    log "Force-killing pid $pid"
    kill -9 "$pid" 2>/dev/null || true
  fi
}

free_port() {
  local pid
  if [[ -f "$PIDFILE" ]]; then
    pid="$(tr -d '[:space:]' < "$PIDFILE" || true)"
    if [[ -n "$pid" ]] && kill -0 "$pid" 2>/dev/null; then
      local c
      c="$(cmd_for_pid "$pid")"
      if is_our_server_cmd "$c" || [[ "$FORCE" == "1" ]]; then
        stop_pid "$pid" "pidfile"
      else
        log "Pidfile $pid is not our server: $c"
        log "Refusing to kill. Set SERVE_FORCE=1 to override, or ./serve.sh stop after checking."
        return 1
      fi
    fi
    rm -f "$PIDFILE"
  fi

  pid="$(listener_pid || true)"
  if [[ -n "$pid" ]]; then
    local c
    c="$(cmd_for_pid "$pid")"
    if is_our_server_cmd "$c" || [[ "$FORCE" == "1" ]]; then
      stop_pid "$pid" "port $PORT listener"
    else
      log "Port $PORT held by pid $pid — not our server:"
      log "  $c"
      log "Refusing to kill. SERVE_FORCE=1 to override, or free the port manually."
      return 1
    fi
  fi
  return 0
}

pick_backend() {
  if [[ "${SERVE_BACKEND:-}" == "python" ]]; then
    echo "python"
    return
  fi
  if [[ "${SERVE_BACKEND:-}" == "node" ]]; then
    echo "node"
    return
  fi
  if command -v node >/dev/null 2>&1; then
    echo "node"
  elif command -v python3 >/dev/null 2>&1; then
    echo "python"
  else
    echo ""
  fi
}

# Must be invoked as a background job so `exec` replaces that child with the server
# (pidfile then holds the real Node/Python pid, not a wrapper subshell).
run_server_exec() {
  local backend
  backend="$(pick_backend)"
  if [[ -z "$backend" ]]; then
    log "ERROR: need node or python3 to serve website-mock"
    exit 127
  fi

  export SERVE_ROOT="$ROOT"
  export SERVE_PORT="$PORT"
  export SERVE_HOST="$HOST"

  if [[ "$backend" == "node" ]]; then
    echo "[serve] Node serve-static.mjs on ${HOST}:${PORT} (root=$ROOT)"
    exec node "$ROOT/serve-static.mjs" "$PORT"
  fi

  echo "[serve] python3 http.server on ${HOST}:${PORT} (root=$ROOT)"
  exec python3 -m http.server "$PORT" --bind "$HOST" --directory "$ROOT"
}

write_pid() {
  echo "$1" > "$PIDFILE"
}

do_status() {
  echo "Root:     $ROOT"
  echo "Port:     $PORT"
  echo "Host:     $HOST"
  echo "Pidfile:  $PIDFILE"
  echo "Log:      $LOGFILE"
  if [[ -f "$PIDFILE" ]]; then
    local pid
    pid="$(tr -d '[:space:]' < "$PIDFILE")"
    echo "Pidfile pid: $pid ($(cmd_for_pid "$pid" || echo dead))"
  else
    echo "Pidfile pid: (none)"
  fi
  local lp
  lp="$(listener_pid || true)"
  if [[ -n "$lp" ]]; then
    echo "Listener: $lp — $(cmd_for_pid "$lp")"
  else
    echo "Listener: (nothing on $PORT)"
  fi
}

do_stop() {
  free_port || true
  rm -f "$PIDFILE"
  log "Stopped (port $PORT)"
}

supervise() {
  local once="${1:-0}"
  free_port || exit 1

  local backoff=1
  local max_backoff=15
  local child=""

  # Survive parent-shell hangup when launched with & / nohup (Ctrl+C still works via INT).
  trap '' HUP

  cleanup() {
    local code=$?
    trap - INT TERM EXIT
    if [[ -n "${child:-}" ]] && kill -0 "$child" 2>/dev/null; then
      stop_pid "$child" "supervisor exit"
    fi
    rm -f "$PIDFILE"
    exit "$code"
  }
  trap cleanup INT TERM EXIT

  while true; do
    # Background + exec inside → $! is the real server pid
    run_server_exec >>"$LOGFILE" 2>&1 &
    child=$!
    write_pid "$child"
    log "Child pid $child (backoff=${backoff}s). Tail: tail -f $LOGFILE"
    echo "Serving $ROOT at http://${HOST}:${PORT}/"
    echo "Library: http://${HOST}:${PORT}/library.html"
    echo "Stop:    cd website-mock && ./serve.sh stop   (or Ctrl+C)"
    echo "Log:     $LOGFILE"

    wait "$child"
    local code=$?
    child=""
    rm -f "$PIDFILE"
    log "Server exited with code $code"

    if [[ "$once" == "1" ]]; then
      exit "$code"
    fi

    # 130/143 ≈ interrupted — don't restart
    if [[ "$code" -eq 130 || "$code" -eq 143 ]]; then
      exit 0
    fi

    log "Restarting in ${backoff}s…"
    sleep "$backoff"
    backoff=$((backoff * 2))
    if (( backoff > max_backoff )); then
      backoff=$max_backoff
    fi
    # Clear anything that may still hold the port after a crash
    free_port || true
  done
}

case "$CMD" in
  stop)
    do_stop
    ;;
  status)
    do_status
    ;;
  once)
    supervise 1
    ;;
  start|*)
    supervise 0
    ;;
esac
