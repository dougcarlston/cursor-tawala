# Tomcat / 8080 wiring — handoff for new agent chat

**Jul 28, 2026** — Clean start for the **8080 / Deploy / runtime / email / CSS** track.  
**Not** the Designer canvas agent — see `DESIGNER_FIELD_QUALIFICATION_HANDOFF.md` for palette/Document bugs.

---

## Paste into a new 8080 wiring chat (copy from here down)

```
Tomcat / 8080 wiring handoff (Jul 28, 2026).

TRACK: Deploy → local Java runtime on :8080, docker/Tomcat, email SMTP, theme CSS, server-side export when runtime breaks. Repo /Users/DougC1/Projects/Tawala, branch cursor/forms-canvas-wysiwyg.

DO NOT this chat: Design canvas (*CanvasRow), Fields palette drag/drop, Document caret UX, .tawala→JSON Library reconverts, Website/Library rebuild, SportsDashboards line-by-line compare.

Owner workspace: open /Users/DougC1/Projects/Tawala (NOT empty ~/Projects/AI-Tawala stub).

Stack:
- Designer UI :5173, API :3001 (designer-web/) — needed to Deploy only
- Tomcat :8080, Postgres — docker compose -p ai-tawala
- Deploy creds: dev/dev
- Email: Resend via .env.email.local (gitignored), From doug@carlston.net

DONE recently (Jul 27–28):
- 28 theme CSS in docker/tomcat/css/project/ (baked in image Jul 27)
- form-layout-core.css row spacing on Deploy (was missing after rebuild; fixed + baked)
- Process SET math export (jsonToXml.mjs) — restart :3001 after any server/* edit
- Email.java: SMTP From = server mailbox; Reply-To sanitized (buildSafeReplyTo) — Resend 550 fixed
- Sign-up Send delivers; blank body was Document using <<Email>> not <<Form 1:Email>> → Designer agent, not 8080

OPEN for 8080 track:
- Finish Sign-up Sheet w Email end-to-end smoke after owner fixes Document field refs (or manual <<Form 1:…>> in JSON)
- Push 2 local commits if owner asks (ec37d51 File Uploader defer, d863c54 email — verify origin)
- Rebuild WAR + recreate Tomcat after Java changes (tawala.war.dev may be dirty locally)
- Optional: template Deploy smokes still unmarked in DESIGNER_TEMPLATE_MATRIX
- Website/Library: LATER — mine Java in TawalaWebapp-build1700, not resurrect full site now

Ops habits:
- After designer-web/server/* change: restart port 3001 (ensure-dev-api.sh) — stale node process caused false “SET still text” once
- After docker/tomcat/css/* change: rebuild image OR docker cp to container css/project/
- Hard refresh :8080 after CSS deploy

Read full handoff: Tawala_Key_Documents/TAWALA_8080_WIRING_HANDOFF.md
Also: docs/EMAIL_DELIVERY_OPS.md, .cursor/rules/tawala-java-deploy-parity.mdc
```

---

## Chat demarcation (keep agents aligned)

| Agent | Owns | Does **not** own |
|-------|------|------------------|
| **8080 / wiring (this chat)** | Tomcat, docker, `TawalaWebapp-build1700/`, `.env.email.local`, Resend/SMTP, `docker/tomcat/css/`, Deploy failures on :8080, Java runtime field resolution behavior | Fields palette UI, Document editor chips, canvas rows |
| **Designer** | `designer-web/src/` canvas, palette, Process/Document/Send **editors** | SMTP config, theme files on disk |
| **Deploy export (shared)** | `designer-web/server/jsonToXml.mjs`, `documentHtmlToXml.mjs`, fib/MQL export — fix here when XML wrong at Deploy | Only if symptom is authoring (wrong token in JSON) → Designer |
| **Converter** | `tawalaXmlToJson.mjs`, Library `.tawala` reconverts | Routine 8080 smokes |
| **Website/Library (future)** | Java Library JSPs, API, Postgres | Current build gate |

**Rule:** Wrong on **:8080 after Deploy** → wiring/runtime. Wrong **before** Deploy (what got saved in project) → Designer.

---

## Current state (Jul 28 AM)

### Repo & branch

- **Path:** `/Users/DougC1/Projects/Tawala`
- **Branch:** `cursor/forms-canvas-wysiwyg` (**2 commits ahead** of origin as of handoff)
- **Recent commits:** themes (075ceec), SET math (4c701d4), File Uploader defer (ec37d51), email From/Reply-To (d863c54)
- **Uncommitted (do not assume pushed):** `DESIGNER_FIELD_QUALIFICATION_HANDOFF.md`, Send validation edits, `tawala.war.dev` rebuild artifact

### Services

| URL | Role | Notes |
|-----|------|-------|
| http://localhost:5173 | Browser Designer | `cd designer-web && npm run dev` |
| http://localhost:3001 | Deploy API | `designer-web/scripts/ensure-dev-api.sh` |
| http://localhost:8080 | Java runtime | `docker compose -p ai-tawala up -d` |
| Postgres | DB | Same compose stack |

### Docker

- Project name: **`ai-tawala`** (not bare `tawala`)
- Container: **`tawala-tomcat`**
- CSS baked via `docker/tomcat/Dockerfile` COPY — rebuild after CSS/Java changes:  
  `docker compose -p ai-tawala build tawala && docker compose -p ai-tawala up -d --force-recreate tawala`
- Quick CSS sync without rebuild:  
  `docker cp docker/tomcat/css/project/. tawala-tomcat:/usr/local/tomcat/webapps/ROOT/css/project/`

### Email (Resend)

- Config: **`.env.email.local`** at repo root (see `docker/tomcat/mail.env.example`)
- From: **Doug Carlston &lt;doug@carlston.net&gt;** via `smtp.resend.com:587`
- GoDaddy owns domain DNS; **Resend dashboard** must verify `carlston.net`
- Designer: **Project → Email Delivery…** → Send Test
- **Jul 27 fix:** `Email.java` — SMTP From always server mailbox; Reply-To sanitized (`buildSafeReplyTo`) so `<<Form 1:FirstName>>` in process alias does not break Resend
- **Preview never sends email** — only Deploy → :8080

### Themes

- **28** official themes under `docker/tomcat/css/project/{theme}/`
- Sign-up template uses **`baseball`**
- Layout spacing: **`form-layout-core.css`** (required; was 404 after bad rebuild Jul 27 evening)

### Deploy export (server — restart :3001 after edits)

- **SET math/text:** `designer-web/server/jsonToXml.mjs` — compiles `+ - * / ^` to Java `<add>/<sub>/…` (fixed Jul 27)
- Smoke: Horses and Penguins — `Set Score to <<Score>> + 1` must Deploy as math, not literal `"1+2"`

---

## Owner priorities for this build (8080-relevant)

1. **Wiring stable** — Deploy small test projects; `:8080` behaves; email optional but working
2. **Known holes closed** — themes done; File Uploader **deferred** (Jul 27)
3. **Not this build** — full SportsDashboards compare, Website/Library, File Uploader

### Sign-up Sheet w Email (in progress)

- Owner rebuilt project Jul 27 evening; **Send works** (Resend activity)
- Email **body** was blank when Document used bare `<<Email>>` etc. — runtime correctly treats those as **variables**, not FIB fields; needs `<<Form 1:Email>>` ( **Designer** fixes palette insert; owner can hand-edit meanwhile)
- **Form/Process/Document** template smoke: owner passed; Sign-up w Email pending full pass after Document fix

---

## Key paths

| What | Where |
|------|--------|
| Java webapp source | `TawalaWebapp-build1700/src/` |
| Email send path | `.../email/Email.java`, `EmailQueueWorker` |
| Docker / Tomcat | `docker/`, `docker-compose.yml` |
| Theme CSS | `docker/tomcat/css/project/` |
| Deploy JSON→XML | `designer-web/server/jsonToXml.mjs` |
| Registration FIB export | `designer-web/server/registrationFibToXml.mjs` |
| Email ops doc | `docs/EMAIL_DELIVERY_OPS.md` |
| Deploy parity traps | `.cursor/rules/tawala-java-deploy-parity.mdc` |
| Archives (Tony themes, etc.) | `/Users/DougC1/Projects/00-Tawala-ARCHIVES-do-not-use-for-Designer/` |
| Library JSON (outside git) | `/Users/DougC1/Projects/Tawala Projects/Being Reconverted/` |

---

## Smoke checklist (8080 agent)

1. `curl http://localhost:3001/api/health` → `ok:true`, `javaProxy:true`
2. Tomcat up; deploy dev/dev succeeds
3. Sign-up or Potluck on :8080 — row spacing, theme CSS load (`form-layout-core.css` 200)
4. Process SET math project Deploy + submit — numeric result not string
5. Email Delivery Send Test → inbox; Sign-up submit → Delivered in Resend (after Document field refs fixed)

---

## Related handoffs (other chats)

- **Designer field qualification:** `DESIGNER_FIELD_QUALIFICATION_HANDOFF.md`
- **SET variables (Deploy fixed):** `DESIGNER_VARIABLES_TYPING_HANDOFF.md`
- **Theme audit:** `AANew Files about CSS/THEME_CSS_AUDIT_2026-07-27.md`

---

## Tony / Website note (future, not 8080 sprint)

Legacy Java Library + theme source exists in repo and archives. Owner may rebuild Website/Library later with API + React — **do not** stand up full www.tawala.com in this wiring chat unless owner explicitly reopens.
