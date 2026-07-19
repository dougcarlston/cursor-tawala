#!/bin/sh
# Empty Lucene dirs must not exist — IndexerBase only creates a new index when the
# directory is missing. Pre-created empty folders cause startup failure.
set -e
for dir in /var/tawala/lucene/projects /var/tawala/lucene/users; do
  if [ -d "$dir" ] && [ ! -f "$dir/segments" ]; then
    rm -rf "$dir"
  fi
done

# --- Outbound email (server-owned SMTP). Secrets from env / file; never logged. ---
MAIL_ENABLED="${TAWALA_MAIL_ENABLED:-true}"
MAIL_HOST="${TAWALA_MAIL_HOST:-mailpit}"
MAIL_PORT="${TAWALA_MAIL_PORT:-1025}"
MAIL_USER="${TAWALA_MAIL_USERNAME:-}"
MAIL_PASS_FILE="${TAWALA_MAIL_PASSWORD_FILE:-}"
MAIL_PASS="${TAWALA_MAIL_PASSWORD:-}"
if [ -n "$MAIL_PASS_FILE" ] && [ -f "$MAIL_PASS_FILE" ]; then
  MAIL_PASS=$(cat "$MAIL_PASS_FILE")
fi
MAIL_AUTH="${TAWALA_MAIL_SMTP_AUTH:-false}"
MAIL_STARTTLS="${TAWALA_MAIL_SMTP_STARTTLS:-false}"
# When STARTTLS is on, require it so we don't silently fall back to plain SMTP.
if [ "$MAIL_STARTTLS" = "true" ]; then
  MAIL_STARTTLS_REQUIRED="${TAWALA_MAIL_SMTP_STARTTLS_REQUIRED:-true}"
else
  MAIL_STARTTLS_REQUIRED="${TAWALA_MAIL_SMTP_STARTTLS_REQUIRED:-false}"
fi
# JDK 8 + Resend/Postmark/Brevo need an explicit TLS 1.2 protocol list.
MAIL_SSL_PROTOCOLS="${TAWALA_MAIL_SMTP_SSL_PROTOCOLS:-TLSv1.2}"
MAIL_FROM_ADDR="${TAWALA_MAIL_FROM_ADDRESS:-noreply@tawala.local}"
MAIL_FROM_NAME="${TAWALA_MAIL_FROM_NAME:-Tawala Local}"
MAIL_WORKER="${TAWALA_MAIL_WORKER_ENABLED:-true}"

MAIL_PROPS=/tmp/tawala-mail.properties
cat > "$MAIL_PROPS" <<EOF
mail.enabled=${MAIL_ENABLED}
mail.host=${MAIL_HOST}
mail.port=${MAIL_PORT}
mail.username=${MAIL_USER}
mail.password=${MAIL_PASS}
mail.smtp.auth=${MAIL_AUTH}
mail.smtp.starttls=${MAIL_STARTTLS}
mail.smtp.starttls.required=${MAIL_STARTTLS_REQUIRED}
mail.smtp.ssl.protocols=${MAIL_SSL_PROTOCOLS}
mail.smtp.timeout=10000
mail.smtp.connectiontimeout=10000
mail.from.address=${MAIL_FROM_ADDR}
mail.from.name=${MAIL_FROM_NAME}
mail.worker.enabled=${MAIL_WORKER}
mail.worker.intervalSeconds=10
mail.worker.batchSize=20
mail.worker.maxSecondsPerRun=30
mail.worker.staleSendingMinutes=15
EOF

# Inject into ROOT.war so Spring classpath:mail.properties picks it up (overrides WEB-INF defaults).
PATCH_DIR=/tmp/mail-war-patch
rm -rf "$PATCH_DIR"
mkdir -p "$PATCH_DIR/WEB-INF/classes"
cp "$MAIL_PROPS" "$PATCH_DIR/WEB-INF/classes/mail.properties"
( cd "$PATCH_DIR" && jar uf /usr/local/tomcat/webapps/ROOT.war WEB-INF/classes/mail.properties )
rm -rf "$PATCH_DIR" "$MAIL_PROPS"
echo "Email config applied: enabled=${MAIL_ENABLED} host=${MAIL_HOST} port=${MAIL_PORT} from=${MAIL_FROM_ADDR} worker=${MAIL_WORKER}"

exec catalina.sh run
