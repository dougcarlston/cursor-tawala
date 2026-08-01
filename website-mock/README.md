# Website mock

Static rough draft of the legacy Tawala site (home, Library, project detail, My Tawala), derived from JSP under `TawalaWebapp-build1700/web/WEB-INF/jsp/`.

**Site CSS (July 2026):** Legacy styles copied into `css/legacy/` from owner archives (`tawala-base.css`, `pages/homepage.css`, `pages/library.css`). Template images under `images/`. Mock-only chrome (banner, pending links, test-drive boxes) in `css/tawala-chrome.css`. Stub pages still use the older all-in-one `css/tawala-mock.css`.

## View locally

**Always** serve from `website-mock/` (not the repo root). Wrong cwd → `http://localhost:5500/library.html` returns **404**.

```bash
cd website-mock
./serve.sh                 # supervised Node static server on 127.0.0.1:5500 (auto-restarts)
# optional aliases:
./serve-watch.sh           # same as ./serve.sh
./serve.sh stop            # free the port / kill our pid
./serve.sh status          # listener + pid file
./serve.sh once            # no restart loop (debug)
```

Open:
- Home: http://localhost:5500/
- **Library:** http://localhost:5500/library.html
- My Tawala: http://localhost:5500/mytawala.html

**Stop:** `Ctrl+C` in the serve terminal, or from another shell: `cd website-mock && ./serve.sh stop`.

**Hardening:** `serve.sh` binds **127.0.0.1**, always uses `--directory` / `SERVE_ROOT` = this folder, prefers Node (`serve-static.mjs`, no extra deps) with Python `http.server` as fallback, kills a **stale** prior mock listener via `.serve.pid` (or a known `http.server` / `serve-static` on the port), and **restarts on crash** with backoff. Logs: `website-mock/.serve.log`. If an unknown process holds 5500: `SERVE_FORCE=1 ./serve.sh`.

**Prereq:** Java runtime on http://localhost:8080 with templates deployed. Test-drive links read from `js/demo-urls.js`.

## Library vs My Tawala (split piles)

### Tenancy (product truth — owner Jul 31, 2026)

There is **one public Library** (shared catalog). Each **account** has its **own separate, private My Tawala**; another account cannot see or access your My Tawala contents. The bridge out of private My Tawala into the public world is **Publish** (to Library) — optional and deliberate. Until Publish, projects stay private to that account’s My Tawala.

**Current priority (owner Jul 31, 2026):** **Libraries are the core of the Website** (Library + My Tawala / catalog ops). Harden **Delete / Purge** and data/backup lifecycle ops (**EXPORT / IMPORT**, **BACKUP / RESTORE**) over look-and-feel; solid basis for libraries already exists. Library → My Tawala **Save this project** remains the customize-entry stub. **Do not** chase more visual polish now — **Home** L&F is optional and non-critical. Version piles stay deferred until ops are trustworthy (see glossary **Sequencing / hold** below).

**Mock note:** today’s mock is single-browser `localStorage` (no real multi-account auth). The **product model** is still multi-account private My Tawala; do not design as if all My Tawala piles were shared.

Owner working copies live outside the repo under `~/Projects/Tawala Projects/`. Repo backups:

| Role | Owner folder | Repo backup | Catalog in `js/demo-urls.js` |
|------|--------------|-------------|------------------------------|
| Public Library / try-outs | `WebLibrary/` | `projects/library/` | `TAWALA_LIBRARY` |
| Personal My Tawala | `MyTawala/` | `projects/mytawala/` | `TAWALA_MYTAWALA` |

See `projects/README.md` for refresh commands and MANIFEST files.

**Display names:** Listing/detail titles never show `.json` / `.JSON` (or legacy `.tawala`). On disk, Designer project files are JSON today; legacy `.tawala` was the C# XML project format (still openable/convertible in Designer). Mock UI uses the human project name only — `jsonFile` paths are maintainer metadata in `demo-urls.js`, not shown to end users.

**Product rules (mock):**

- **My Tawala** lists only the MyTawala pile — never the Library / starter catalog as “my” projects.
- **Library** does **not** list Designer-only New Project basics (Empty/Blank, Form with Process, Form with Process & Document). Those stay File → New Project in the Designer.
- Library **listing** is dense like My Tawala (name + stars inline / times used / updated / row actions); descriptions and **comments** live on the detail page (comments stub on Project Details — not a list column).
- Categories are **collapsible groups** mirroring Designer **File → New Project** (Activities, Meetings and Gatherings, Polls and Surveys) plus WebLibrary extras (Sports, Business, Entertainment, Advanced). Designer-only **Basic** templates are never listed.
- **`liveReady: true`** in `demo-urls.js` = owner-vetted product with a working `:8080` test-drive. Library list shows a quiet green **Live** chip (placeholders stay unmarked; Test drive stays disabled until `deployed` + URL).
- List rows are **name-only** (no letter-tile icon fallbacks — those were CSS stand-ins for missing project icon bitmaps). Missing `:8080` deploys are **not** bannered on list chrome — Test drive stays disabled with a title tip; quiet status may appear on detail / start-point stubs only. Listing titles never show a `.json` extension — on-disk backups may still be JSON; display uses the project name only.

Main Menu public templates (Simple Survey, Sign-up, Potluck, Get Together, Sign-up w Email, Multiple Question Survey) stay in the Library catalog with current Phase 2 `:8080` URLs. JSON for most of those is under `designer-web/public/samples/templates/`; Multiple Question Survey also has a WebLibrary backup under `projects/library/`.

## Navigation and stub pages

Shared chrome lives in `js/chrome.js` (header, footer, guest/logged-in status).

| Page | File |
|------|------|
| Home | `index.html` |
| Library | `library.html` |
| Project detail (Library) | `library-detail.html?project=…` |
| My Tawala — My Projects | `mytawala.html` |
| My Tawala — Project Details | `mytawala-project.html?project=…` |
| Ops archive review | `project-ops-review.html` |
| About / Company Info | `about.html` |
| FAQ | `faq.html` |
| Login | `login.html` |
| Sign up | `signup.html` |
| Logout | `logout.html` |
| Designer | `designer.html` |
| Terms | `terms.html` |
| Privacy | `privacy.html` |

Links **without** a stub yet are greyed out via class `link-pending` in `chrome.js` (`ready: false`). Add a stub HTML file and set `ready: true` to enable the link.

## Symbiotic hops (Designer ↔ mock)

| From | To |
|------|-----|
| Mock chrome banner / My Tawala sidebar | Web Designer `http://localhost:5173` |
| Designer **Project → Project Manager…** (or toolbar) | `mytawala-project.html?project=…` for the open project name (slug id) |
| Designer **Help → Website mock (My Tawala)…** | `http://localhost:5500/mytawala.html` |
| Designer **Help → Website mock (Library)…** | `http://localhost:5500/library.html` |
| Designer **Deploy** dialog → **Show in My Tawala** | `mytawala-project.html?project=…&deployReceipt=…` — upserts My Tawala pile overlay + opens Details |

Test-drive start points stay on `:8080` via `js/demo-urls.js`.

### Transfer flows (scaffold)

Shared helpers: `js/transfer.js` (localStorage on `:5500` only — Designer `:5173` does not share storage).

| Hop | UI | Status |
|-----|-----|--------|
| **Web Designer → My Tawala** | Deploy dialog **Show in My Tawala**; My Tawala **From Web Designer** inbox | Wired — receipt upserts `localStorage` overlay into My Projects pile + opens Project Details. Overlay survives reload until **Delete** (or clear). Not written into `demo-urls.js`. |
| **Library → My Tawala** | Listing / detail **Save this project under My Tawala**, **USE IT** (legacy customize entry; was mislabeled “Clone”) | Grey stubs. |
| **My Tawala → Library** | Listing **Publish**, detail **PUBLISH**, sidebar **Publish to Library** | Grey stubs. |
| **Library ← My Tawala upgrade** | Listing **Pull**, detail **PULL FROM LIBRARY**, sidebar **Pull from Library** | Grey stubs. |
| **Library category assignment** | **Edit Categories** (submenu + admin bar); detail sidebar **Group** select | Wired locally — overrides in `localStorage` until copied into `demo-urls.js`. |

Grey controls use Designer accent palette (`--tw-accent`); disabled = opacity only.

## Save / Deploy / Publish (product glossary)

Short product meanings for Website + Designer hops. Sits next to Project Versioning (legacy memo B7) and the transfer stubs above. Not implemented as multi-account auth in this mock.

**Ops verb split (evidence-backed — Java build1700 + memos; owner Jul 31, 2026 reconciled):** Keep **EXPORT / IMPORT** and **BACKUP / RESTORE** separate. Do **not** conflate either with **Deploy** (definition versions) or Designer **File → Save** (local authoring). Shipped Project Manager UI had **no “Save” for submissions** — the ops verbs were **EXPORT / IMPORT / BACKUP / RESTORE** (plus Delete / Purge / Publish, etc.). Owner colloquial “Save” for protecting a live project may have meant **Backup**; treat that gently when reading older notes.

| Spine | Verbs | What it carries | Contract |
|-------|--------|-----------------|----------|
| **Data only** | **EXPORT** / **IMPORT** | Excel **response data** (submissions) only | **Import** = restore messed-up **data** into the **current** project. Field mismatch **fails**; Import does **not** roll back the project definition. Not “export / import a project version” and not a general “move projects around.” |
| **Paired snapshot** | **BACKUP** / **RESTORE** | `.backup` ZIP = **paired project definition + data** (plus properties / links) | **Restore** re-applies the matching definition, then data — which is why restore “worked pretty well” across later field changes. This **already was** the paired snapshot path; it is **not** the same as Export/Import. |
| **Definition versions** | **Deploy** | My Tawala **definition versions** | Shipped Java **auto-deploys** the new version. Separate from Backup. See B7. |
| **Local authoring** | Designer **File → Save** | Local project definition only | Not My Tawala data, not Backup, not Deploy versioning. |

**Reconciled (replaces earlier “evolving perhaps Save must also preserve project” note):** Schema drift makes **data-only Import** fail or lose fidelity when fields no longer match — that is expected for Export/Import. The paired **definition + data** package was **Backup / Restore**, not Export/Import and not a PM “Save.” Do not invent a new Save-as-paired-bundle verb on top of this split.

| Verb | Scope | Meaning |
|------|--------|---------|
| **Save** (Library) | Library → My Tawala | Copy a public Library project into *this account’s* private My Tawala (customize entry / “under My Tawala”). Stays private until Publish. Not a PM submissions verb. |
| **EXPORT** | My Tawala / Project Manager | Outbound Excel **response data** only. Not definition versioning; not Backup. |
| **IMPORT** | My Tawala / Project Manager | Restore messed-up **data** into the current project. Field mismatch fails; does **not** roll back definition. Same data spine as Export. |
| **BACKUP** | My Tawala / Project Manager | Write a `.backup` ZIP — paired **definition + data** (plus properties / links). |
| **RESTORE** | My Tawala / Project Manager | Re-apply that paired snapshot (matching definition, then data). Distinct from Import. |
| **Deploy** | Designer → runtime / My Tawala | Mints a My Tawala **definition version** and (in shipped Java) auto-deploys it; optionally surface via **Show in My Tawala**. Deploy ≠ Backup ≠ Publish to Library. |
| **Publish** | My Tawala → Library | Deliberate bridge from private My Tawala into the **one** public Library catalog. Optional; until then, projects remain account-private. |
| **Pull** | Library → My Tawala (upgrade) | Replace a My Tawala project with a newer public Library version (stub). |
| **Versioning** | My Tawala project | Immutable project-definition snapshots (deployed vs non-deployed, Library submit rules, test-drive, upload metadata) — see triage **B7 Project Versioning**. Historically present in PM **Versions** UI; separate from Export/Import and from Backup/Restore. |
| **Designer Save** | Designer File → Save | Persist local definition on disk — authoring only. |

**Pillars (do not conflate):**

1. **Tenancy** — one public Library; per-account private My Tawala; Publish is the only intentional public bridge (see § Tenancy above).
2. **Data ops (Excel)** — **EXPORT / IMPORT** — response data only; Import does not change definition.
3. **Paired backup ops** — **BACKUP / RESTORE** — `.backup` ZIP with matching definition + data (plus properties / links).
4. **Versioning** — which project-definition revision is active / test-driven / submitted to Library (B7 / Deploy), independent of Excel data tools and of Backup packages.
5. **Deploy vs Publish** — Deploy mints/auto-deploys a definition version for an account’s project; Publish shares into the shared Library catalog.

**Sequencing / hold (owner Jul 31, 2026):** Do **not** fill the My Tawala **listing** with **version piles** before Delete / Purge and the ops above (**EXPORT / IMPORT**, **BACKUP / RESTORE**) are trustworthy — the catalog would get messy very fast. Versions **existed** historically in the PM **Versions** UI; the hold is on **listing clutter**, not on denying that versions existed. **Audit trail** belongs on that same trajectory (with versioning + lifecycle controls), not as a later orphan. Keep today’s flat mock: one My Tawala row per project (Deploy receipt / overlay), **not** a version pile per project yet. Multi-version list / “Deploy creates version N” exposure in the listing stays deferred until those ops are ready enough to manage the mess. When versioning lands in the listing, ship it with (or immediately after) operational data/backup handles **and** audit-trail visibility — one lifecycle package, not “versions first.” Legacy intent (Deploy creates versions, one deployed, Library = published snapshot) remains the **target** model (B7); gate **My Tawala listing exposure** of versions on ops readiness.

## My Tawala layout (lean) vs ops archive

Legacy Project Manager lives under `TawalaWebapp-build1700/web/WEB-INF/jsp/projectmanager/` (not the thin `mytawala/*.jsp` news pages). Mock mirrors the shallow structure:

1. **`mytawala.html`** — My Projects listing (name, created, updated + Export…Publish icon columns). Click headings to sort; drag header edges to resize. Row **Delete** confirms and removes the **account-private** My Tawala row (clears Deploy overlay + inbox receipt; seed rows stay hidden via `localStorage` deleted set). **Purge** clears submissions only (separate). Click project name → Project Details. Sub-menu: **My Projects · My Account · Change Password**.
2. **`mytawala-project.html?project=…`** — Project Details: **EXPORT…PUBLISH** action bar, left sidebar (REVISE / ONLINE-OFFLINE / Include / Invite), collapsible sections (Start points, Project Data, Versions, Backups…). **DELETE** same as listing, then returns to My Projects. Start-point test-drives stay on `:8080`.
3. **`project-ops-review.html`** — full recovered label catalog split by surface (**Public Library** vs **My Tawala / Project Manager**) for memory / archive review. Linked from My Tawala sidebar; not stacked on the working pages.

Labels live in `js/project-ops.js`. Inactive Project Manager ops are **disabled** (grey only — no “not wired” text). Active ops (**PURGE**, **DELETE**) use Designer-accent blue styling from `css/tawala-chrome.css` (`--tw-accent` mirrors `designer-web/src/styles.css`).

**DELETE** (wired Jul 31, 2026 — was mock row-remove only): confirm → `TawalaTransfer.deleteMyTawalaProject` removes the private My Tawala entry for this browser account mock — clears `tawala.mock.myTawalaOverlay` + matching `tawala.mock.deployInbox` receipts, and records `tawala.mock.myTawalaDeleted` so catalog seed rows from `TAWALA_MYTAWALA` do not reappear on reload. Does **not** delete public Library / `liveReady` catalog entries, does **not** purge `:8080` submissions (use **PURGE**), and does **not** remove Tomcat project XML. Re-Deploy → **Show in My Tawala** clears the deleted mark and restores the row. Listing and Details share the same handler; Details redirects to `mytawala.html` after Delete.

**PURGE** (hardened Jul 31, 2026): confirm → resolve `:8080` **uniqueId** from My Tawala project (`uniqueId` field on Deploy overlay, else `testDriveUrl` / start-point URLs, else deploy-inbox receipt) → POST `http://localhost:3001/api/purge-responses` (`dev` / `dev`). Deletes all `submission` rows for that `user_project.unique_random_id` in Docker Postgres — same effect as Java Project Manager `purgeProjectResponses`. Listing and Details share the same handler; success/failure both surface via status line + alert (deleted row count on success). Does **not** remove the My Tawala row (use **DELETE**), does **not** touch public Library / `liveReady`, and does **not** remove Tomcat project XML. Undeployed seed rows (no uniqueId) get a clear “Deploy first” message.

**Smoke Purge:** with `:3001` + Postgres up — `curl -s -X POST http://localhost:3001/api/purge-responses -H 'Content-Type: application/json' -d '{"uniqueId":"gy1zssbrwm4fgfm","credentials":{"user":"dev","password":"dev"}}'`. UI: Deploy a project → **Show in My Tawala** → Purge from listing or Details → confirm → alert shows deleted count; Test drive still purge-on-start. Undeployed seed → Purge → “no uniqueId / Deploy first” alert.

**Smoke Delete:** open http://127.0.0.1:5500/mytawala.html → Delete a seed row → reload → row still gone → Library still lists the public twin if any. Deploy overlay: use Designer **Show in My Tawala**, Delete from listing or Details → overlay + inbox entry gone; Library catalog unchanged.

CLI equivalents:

```bash
./scripts/dev-data.sh purge-by-unique-id gy1zssbrwm4fgfm
# or DirtBowl Registration-only:
./scripts/dev-data.sh cleanup-registrations
```

Requires designer-web API (`:3001`) and `docker compose` postgres. SportsDashboards spelling (not SportsBoard).

### Test drive purge-on-start

Library / home / detail **Test drive** (and start-point links) purge that project’s responses **before** opening `:8080`, so each drive starts clean. My Tawala **PURGE** uses the same API.

**Limitation:** the form opens in a new tab; there is no reliable purge-on-tab-close in this static mock. Legacy Library test drive used an in-memory session world (no durable DB writes); local mock hits the real deployed project, so purge-on-start is the practical substitute.

## Owner checklist — product ready for Library (Deploy → Live)

When a project is thoroughly vetted and should appear as **Live** in the mock Library:

1. **Designer :5173** — open the project, confirm Starting Point on the form(s) you want test-driven, save JSON.
2. **API :3001** — health check; restart if Deploy/Preview fails while Vite still loads:
   ```bash
   curl -s http://localhost:3001/api/health
   # if down or stale server code:
   cd designer-web && ./scripts/ensure-dev-api.sh
   ```
3. **Credentials** — local Java Deploy uses **`dev` / `dev`** (override with `TAWALA_DEV_USER` / `TAWALA_DEV_PASSWORD`).
4. **Tomcat :8080** — must be up (`docker compose up -d` if needed). Confirm `http://localhost:8080/client` responds.
5. **Deploy** — File → Deploy in Designer, **or** POST the JSON to `http://localhost:3001/api/deploy` (same credentials). Note the returned **start point URL(s)** (`http://localhost:8080/p/<uniqueId>/…`).
   - Legacy `.tawala` templates can also use: `node scripts/deploy-tawala-template.mjs "<Template Name>"`
6. **Copy JSON into the Library pile**
   - Working copy: `~/Projects/Tawala Projects/WebLibrary/` (and/or `Live Library Projects/`)
   - Repo backup: `website-mock/projects/library/`
7. **Update `website-mock/js/demo-urls.js`**
   - Correct **category** (add a group in `TAWALA_LIBRARY_CATEGORIES` if missing)
   - `deployed: true`, `testDriveUrl` + `startPoints[].url` from step 5
   - `liveReady: true` only when thoroughly vetted (shows quiet **Live** cue)
   - Leave `deployed: false` / no URL for placeholders (`jsonFile` is maintainer-only metadata)
8. **Browse the mock** — `cd website-mock && ./serve.sh` → http://localhost:5500/library.html
   - Stop with Ctrl+C or `./serve.sh stop`. Logs in `.serve.log`.

Keep Phase 2 `:8080` URLs only (no www.tawala.com).

- **Home / Library / library-detail** → `TawalaDemo.libraryEntries()` / `getLibrary()`
- **My Tawala / mytawala-project** → `TawalaDemo.myTawalaEntries()` / `getMyTawala()`
