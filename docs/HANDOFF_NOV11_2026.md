# Handoff — 11 November 2026

**Use this file as the first read** after the Cursor update. Paste the opener at the bottom as the first message in a **new** chat.

**Repo:** `~/Projects/Tawala`  
**Branch:** `cursor/forms-canvas-wysiwyg`  
**Last pushed commit:** `09ad53e` (22 Sep 2026) — *Capture OEB teacher-packet plan and park SMS Send.*  
**Working tree:** has **uncommitted** Java / Docker / jsonToXml changes from the October teacher-packet host. Do **not** discard them. Do **not** commit unless the owner asks.

---

## Why we paused

Owner’s 102-year-old father died; owner is executor. Coding stopped after the September Library isolation pass and an October teacher-exam packet experiment (`teacherexams.org`).

---

## Product pivot (owner, 11 Nov 2026) — locked

Teachers said they cannot rely on written or online exams because students have AI. None wanted to explore Tawala as an exam product.

**New north star:**

1. **Finish Tawala** anyway.
2. **Designer** and **My Tawala** are the product to make solid.
3. Put a **persistent publicly accessible site** on the internet for **general use** (surveys, sign-up, Get Together, Shared To-Do, and whatever else people build).
4. **Not monetizing** for now. Payments, email billing/metering, and ratings stay parked.
5. **Open for developer use** if alternative apps get traction. One person wanted a Match.com-style dating service **using** Tawala — we do **not** build that dating product; Tawala stays the form / Process / document runtime they would author in.
6. **School / exam GTM is parked.** Teacher packets, `teacherexams.org` as a sales host, OEB “Publish as an exam” Version 2, and Library-as-teacher-marketplace wait. Keepers and isolation rules for live copies still stand; do not delete public Library listings just because exams are no longer the pitch.

Library listing cosmetics (owner Aug 26: weaker than My Tawala / Project Details) still wait until we care about a public catalog. A public **site** can be My Tawala + Designer first, with Library as samples.

---

## What this does **not** change

- Design canvas ≫ Push ≫ Preview. Do not mix Designer canvas work and website-mock chrome in one change unless the owner asks.
- Live `/p/{uniqueId}` is one World. Test Drive `/t/` is per HTTP session. Do not “fix” isolation by copying live `/p/` into a Test Drive.
- Identity, mail, and occupancy stay on the existing Java / Tomcat / Hibernate stack. Do not invent a second auth or an unauthenticated API that returns another user’s rows.
- **Accounts (#21)** are still required for a public My Tawala that survives a new browser. That is **not** monetization. Magic-link + personal account was the last owner shape (Aug 25 / Sep 14). Vendor not chosen. Local mock stays “Log in as `dev`.”
- Test Drive isolation **landed September 2026** (`/t/` session World, empty copy, leave-wipe ~60s). Do not revive shared-catalog-uniqueId Test Drive.

---

## Track order (owner)

**This stretch:** Designer until authoring is something you’d hand a developer.  
**Then:** My Tawala on a host that stays up (accounts so projects persist; Push from Designer; Use on `:8080` / public hostname).

Do not start payments, Clerk/Stripe greenfield, or a teacher-marketplace redesign.

---

## Designer — where it actually is

Two-link Document underline merge (Aug 26 first task) is **fixed** (commit `12d8c94`, 2 Sep 2026). Do not reopen unless it regresses.

**Reasonable first Designer work** (owner pick; do not start all at once):

| Item | Notes |
|------|--------|
| Document smoothness P0s (**B1**) | Face/Size multi-chip, invent-with-tables, chip drag, table reflow. Spec: `DESIGNER_OPEN_TODOS.md` B1; parked rule. |
| **F1** 3-browser smoke | Chrome + Safari + Edge when Designer is “basically finished.” Re-surfaced Sep 3. |
| Native `confirm()` polish | Before public use. |
| Process opening-quote wrap | Sep 4 polish note in `DESIGNER_OPEN_TODOS.md`. |
| **C14** FIB Above + `*` | Implemented 15 Sep — do not rebuild; re-smoke if touching FIB Styles. |
| **C15** Pre-populate / Block Back | Pre-populate smoked 15 Sep; Block Back traps browser Back on `:8080`. |

Start services: `./scripts/start-designer-chat.sh --start --copy`  
UI: http://localhost:5173 — leave a Terminal open with `cd designer-web && npm run keep`.

---

## My Tawala / public site — where it actually is

Website mock: http://localhost:5500 (not `127.0.0.1`).  
Live forms: http://localhost:8080.

**Done enough to remember:** Copy to My Tawala gets a private uniqueId; Publish clones an empty Library copy (does not wipe the author’s My Tawala); Library Test Drive is `/t/` isolation; Get Together + Potluck are Live Library; Automated List Builder stays **My Tawala only** until invitation Confirm links use a public hostname.

**Still required for a public general-use site (no payments):**

- A host that stays up (not only this laptop).
- Accounts so My Tawala is not `localStorage` on `:5500`.
- Same-host redirects after Submit (see uncommitted Java below — written for `teacherexams.org`, still the right idea for any public hostname).
- Mail worker for apps that send mail (List Builder, invites) — not a billing counter.

Resume Library/site details: `docs/LIBRARY_WHERE_WE_ARE_SEP3.md` (filename is older than the Sep 14–15 close). Task list: `website-mock/README.md`.

---

## Parked on purpose (do not start)

- Payments (#22), email metering (#20), ratings (#17)
- OEB teacher packets / `docs/OEB_TEACHER_PACKETS_OCT2026.json` / `scripts/_mint-teacher-oeb.mjs` as a GTM
- Builder “Publish as an exam” Version 2 (`LIBRARY_WHERE_WE_ARE_SEP3.md` § D)
- Font Color picker (Version 2 or never)
- Undo last deleted item (**B8**, Version 2)

---

## Uncommitted work — keep

Do **not** `git restore` these. They are from the October public-hostname / teacher Mini pass:

| Path | What it is |
|------|------------|
| `TawalaWebapp-build1700/src/com/tawala/web/project/DataCollectingProjectController.java` | After last Submit: “You can close this window.” Redirect stays on the **same host** (so `teacherexams.org` does not bounce to `localhost:8080`). |
| `docker/tomcat/entrypoint.sh` + `docker-compose.yml` | `TAWALA_PUBLIC_HOSTNAME`; Postgres bound to `127.0.0.1:5432`. |
| `designer-web/server/jsonToXml.mjs` + test | Form Text can Deploy as raw HTML via `<!--tawala-raw-html-->` (in-form submit links). |
| `.cursor/rules/tawala-concurrent-users.mdc` | Always-on: occupancy, races, Test Drive session scope. |
| `.cursor/rules/tawala-durable-state.mdc` | Always-on: do not panic-patch 500s; keep Java identity/mail. |

`teacherexams.org` as a **teacher product host** is parked. The same-host redirect and `TAWALA_PUBLIC_HOSTNAME` are still useful for a general public site. Review with the owner before committing.

Also untracked (older, not this pivot): `_diag-*`, tray-reorg staging, `tawala.war.dev` — leave them.

---

## How to open the next chat

1. Finish the Cursor update.
2. New Chat. Rename: **Designer thread** (or **Public My Tawala** if the owner wants site/host first).
3. Optional: `./scripts/start-designer-chat.sh --start --copy` if working Designer.
4. Paste the opener below. `@` attach this file.

---

## New-chat opener (paste as first message)

```text
Project: Tawala (~/Projects/Tawala)
Branch: cursor/forms-canvas-wysiwyg @ 09ad53e (working tree has uncommitted public-hostname / jsonToXml / Java thank-you changes — do not discard)
Track: Designer + My Tawala toward a persistent public general-use site. Chat title: Designer thread
Read first: docs/HANDOFF_NOV11_2026.md
Pivot (owner 11 Nov 2026): teachers will not adopt exam software because of AI. Finish Tawala anyway. Designer and My Tawala are the product. Public site for general use, not school sales, not monetization. Developer use welcome; do not build a dating app. Park payments, ratings, teacher packets, OEB Publish-as-exam V2.
Constraints: Design canvas ≫ Push ≫ Preview; do not mix website-mock cosmetics into a Designer change; no commit unless I ask; never www.tawala.com as the default deploy target.
Do not restore uncommitted files. Uncommitted same-host redirect is for any public hostname, not only teacherexams.org.
First: confirm you read HANDOFF_NOV11_2026.md, then I will pick Designer (Document smoothness P0s / F1) or public My Tawala host/accounts.
```
