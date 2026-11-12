# Start the Library Cursor chat — owner guide

**Purpose:** Switch from the Designer track to the **Website / My Tawala** track with minimal friction.

**Read first (after 11 Nov 2026):** [`docs/HANDOFF_NOV11_2026.md`](HANDOFF_NOV11_2026.md) — public general-use site, not school-exam sales.

**One command:**

```bash
cd ~/Projects/Tawala
./scripts/start-library-chat.sh --start --copy
```

That checks (and starts) Tomcat `:8080`, website mock `:5500`, and Designer API `:3001`, fixes the common Docker network split, copies the chat opener to your clipboard, and prints review URLs.

---

## Part 1 — What the Library chat should read first

| Order | File | Purpose |
|-------|------|---------|
| 1 | `docs/HANDOFF_NOV11_2026.md` | Product pivot |
| 2 | `docs/START_LIBRARY_CHAT.md` | This guide |
| 3 | `docs/LIBRARY_WHERE_WE_ARE_SEP3.md` | Isolation close (Sep 14–15) — still true |
| 4 | `website-mock/README.md` | Task List, tenancy, Publish/Save/Deploy glossary |
| 5 | `.cursor/rules/tawala-work-scopes.mdc` | Stay on My Tawala / site unless Push wiring forces Designer |

**Do not read** the full Designer spec set unless a task explicitly needs Push/deploy URL wiring.

---

## Part 2 — Services (Library thread needs these)

| URL | Role | Start |
|-----|------|--------|
| http://localhost:5500 | Website mock (Library, My Tawala) | `cd website-mock && ./serve.sh` |
| http://localhost:8080 | Tomcat — Test Drive / live forms | `./scripts/docker-up.sh` or `./scripts/start-library-chat.sh --start` |
| http://localhost:3001 | Designer dev API — Purge, Records, Export | `designer-web/scripts/ensure-dev-api.sh` |

**Always use `http://localhost:5500`** — not `127.0.0.1:5500` (different browser localStorage).

### Docker network gotcha (Aug 24)

If `curl http://localhost:8080/home` returns **500** after `docker compose up -d --no-deps tawala`, Postgres may be on `ai-tawala_default` while Tomcat is on `tawala_default`. The handoff script auto-connects Tomcat to Postgres’s network. Manual fix:

```bash
docker network connect ai-tawala_default tawala-tomcat
docker restart tawala-tomcat
```

---

## Part 3 — Cursor steps

1. **Save** open files.
2. **New Chat** (`Cmd+L` → New Chat / `+`).
3. **Rename** the chat → `Library thread`.
4. Run `./scripts/start-library-chat.sh --copy` (or `--start --copy`).
5. **Paste** the START SCRIPT as your first message.
6. Optional `@` attach: `docs/START_LIBRARY_CHAT.md`, `website-mock/README.md`.

---

## Part 4 — Copy-paste START SCRIPT

Generated fresh by `./scripts/start-library-chat.sh`. Template:

```text
Project: Tawala (~/Projects/Tawala)
Track: Website mock — website-mock/ (Phase 3) — chat title: Library thread
Goal: Library-only ops — harden Delete/Purge, EXPORT/IMPORT, BACKUP/RESTORE; Test Drive stability; Publish/stub readiness. Do NOT open Designer canvas work unless a Test Drive URL truly requires deploy wiring.
Read first: docs/START_LIBRARY_CHAT.md, docs/CHAT_HANDOFF.md Chat 3, website-mock/README.md § Task List
Constraints: localhost:5500 (not 127.0.0.1); no commit unless I ask; defer parked Designer items
First task: P0 Task #26 private uniqueId on Copy to MyTawala; then confirm health and remaining Task List.
```

---

## Part 5 — Library punch list (Aug 24)

Execute in chunks; see `website-mock/README.md` § Task List for DONE/HOLD detail.

0. **P0 — Private uniqueId on Copy to MyTawala (#26)** — **WIRED Aug 24** (clone-on-acquire). Owner smoke: Copy Simple Survey → uniqueId ≠ Library Test Drive; Edit in Designer must not Redeploy public Simple Survey.
1. **EXPORT / IMPORT** — response-data spine (owner smoked Aug 24; mismatch on field names works)
2. **BACKUP / RESTORE** — confirm default package with owner before expanding UI
3. **Delete / Purge** — scoped confirms + Records refresh
4. **Test Drive (#14)** — honesty copy **DONE Aug 24** (wipe-on-start, shared uniqueId). Real leave/wipe + per-drive uniqueId waits for Library Live.
5. **Deploy / share (#10)** — start picker, embed, uniqueId hardening
6. **Usage stats (#13)** — Times used / Last used on My Tawala Use
7. **Stub cleanup (#18)** — Publish replacement → retire stub

**Parked:** Auth, Payments, end page promo, ratings, Designer Document P0s.

**Next Designer pass (later):** `.tawala` convert batch C2/C3, Document caret epic, post-website polish.

**11 Nov 2026:** School-exam GTM is parked. Public site = My Tawala + Designer for general use, not a teacher marketplace. Payments parked; accounts still required for persistence. Resume: `docs/HANDOFF_NOV11_2026.md`. Isolation Test Drive (`/t/`) already landed Sep 2026 — do not revive shared uniqueId TD.

---

## Part 6 — When Designer work is allowed in the Library chat

Only when necessary:

- Test Drive URL wrong in `demo-urls.js`
- Push/deploy to fix a live `:8080` form
- `:3001` probe or purge endpoint

Do **not** refactor `designer-web/src/components/*CanvasRow*` or Design canvas UX from the Library chat.

---

*Last updated: Aug 24, 2026 — handoff from Designer session (Tomcat network fix, FIB formatter toggle parked).*
