# Outbound email delivery — operations

Server-owned, provider-neutral SMTP for the Java/Tomcat runtime (`:8080`).
Credentials never live in project JSON, Designer downloads, or Deploy XML.

## Architecture

1. Process **Send** creates a `UserProjectEmail` row (`READY`).
2. In-process `EmailQueueWorker` drains `READY` → SMTP via Spring `JavaMailSender`.
3. Verified server From is the SMTP/visible sender; process From is **Reply-To**.
4. **PROJECT EMAIL COUNT** counts all project email rows (any state).

**Recommended provider for experimental / demo use: Resend** (SMTP `smtp.resend.com`, username always `resend`, password = API key).

Owner local credentials live in gitignored **`.env.email.local`** at the repo root (template: `docker/tomcat/mail.env.example`). That one file applies to **all** deployed demo projects because SMTP is server-owned.

Optional **Mailpit** profile for capture-only testing: `docker compose --profile mailpit up -d`.

## Environment variables (Tomcat entrypoint)

| Variable | Resend (`.env.email.local`) | Mailpit fallback | Purpose |
|----------|-----------------------------|------------------|---------|
| `TAWALA_MAIL_ENABLED` | `true` | `true` | Master switch |
| `TAWALA_MAIL_HOST` | `smtp.resend.com` | `mailpit` | SMTP host |
| `TAWALA_MAIL_PORT` | `587` | `1025` | SMTP port |
| `TAWALA_MAIL_USERNAME` | `resend` | _(empty)_ | SMTP user (literal `resend` for Resend) |
| `TAWALA_MAIL_PASSWORD` | API key | _(empty)_ | SMTP password / API key |
| `TAWALA_MAIL_PASSWORD_FILE` | _(optional)_ | _(unset)_ | Path to password secret file |
| `TAWALA_MAIL_SMTP_AUTH` | `true` | `false` | `mail.smtp.auth` |
| `TAWALA_MAIL_SMTP_STARTTLS` | `true` | `false` | STARTTLS |
| `TAWALA_MAIL_SMTP_SSL_PROTOCOLS` | `TLSv1.2` | `TLSv1.2` | Required on JDK 8 for Resend/modern relays |
| `TAWALA_MAIL_FROM_ADDRESS` | `doug@carlston.net` | `noreply@tawala.local` | Verified From address |
| `TAWALA_MAIL_FROM_NAME` | `Doug Carlston` | `Tawala Local` | Default From display name |
| `TAWALA_MAIL_WORKER_ENABLED` | `true` | `true` | Queue consumer |

Entrypoint regenerates `classpath:mail.properties` inside `ROOT.war` at start and logs only non-secret fields.

## Local smoke (Resend)

1. Copy template if needed: `cp docker/tomcat/mail.env.example .env.email.local`
2. Paste Resend API key into `TAWALA_MAIL_PASSWORD=` (From is already `doug@carlston.net`).
3. In Resend: verify the domain for `carlston.net` (or confirm `doug@carlston.net` is an allowed sender).
4. Rebuild WAR if Java sources changed, then `docker compose up -d --build`.
5. Designer with `TAWALA_JAVA_URL=http://localhost:8080` → **Project → Email Delivery…** → Send Test to yourself.
6. Deploy any demo with **Send** — all projects share the same Resend credentials.

## Local smoke (Mailpit only)

1. Remove or rename `.env.email.local` so Compose falls back to Mailpit defaults.
2. `docker compose --profile mailpit up -d`
3. Send Test / form Submit → inspect http://localhost:8025

## Production / provider notes

Resend SMTP:

```bash
TAWALA_MAIL_HOST=smtp.resend.com
TAWALA_MAIL_PORT=587
TAWALA_MAIL_USERNAME=resend
TAWALA_MAIL_PASSWORD=<api-key>
TAWALA_MAIL_SMTP_AUTH=true
TAWALA_MAIL_SMTP_STARTTLS=true
TAWALA_MAIL_FROM_ADDRESS=doug@carlston.net
```

Postmark / Brevo remain supported via the same env keys (different host/username). Prefer `TAWALA_MAIL_PASSWORD_FILE` for production secrets.

## TLS / JavaMail note

Tomcat image uses **OpenJDK 8u292+**, which disables TLS 1.0/1.1. Legacy `mailapi.jar` / `smtp.jar` (JavaMail **1.3.2**) hard-fail STARTTLS to Resend with:

`SSLHandshakeException: No appropriate protocol`

The webapp ships **`javax.mail-1.6.2.jar`** instead (old jars kept under `TawalaWebapp-build1700/lib-legacy-mail/` for reference only). Entrypoint also sets `mail.smtp.ssl.protocols=TLSv1.2`.

## Disable / rollback

- `TAWALA_MAIL_ENABLED=false` — queue stays `READY` / test send refuses; no SMTP.
- Point host back to Mailpit for safe local capture.
- Worker off: `TAWALA_MAIL_WORKER_ENABLED=false` (mail accumulates as `READY`).

## Credential rotation

1. Create a new SMTP token at the provider.
2. Update `TAWALA_MAIL_PASSWORD_FILE` (or env) and restart Tomcat.
3. Send a test; revoke the old token after success.

## Queue inspection

- Designer → Email Delivery… shows ready/sending/sent/error counts and last sanitized error.
- Client API (authenticated): `queryEmailStatus`, `sendTestEmail`.
- Postgres: `email` table (`state`, `from_address`, `to_address`, `subject`, `create_dt`, `sent_dt`).

## Deferred

Provider delivery/bounce webhooks, per-project credentials, Preview sending, broadcast streams.

Until webhooks exist, `SENT` means the SMTP relay accepted the message — not mailbox delivery.
