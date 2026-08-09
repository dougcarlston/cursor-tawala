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
- **Docs** (mock ops guide): http://localhost:5500/docs.html — also linked quietly from the yellow mock banner and the site footer on every chrome page; full README as plain text at http://localhost:5500/README.md
- **Legacy reference (static, non-operational):** http://localhost:5500/legacy-reference/ — Beta lookalikes of My Tawala / Project Details / Library from From Tony stills (Library = reconstructed). Side-by-side with live mock pages above. **Owner decisions:** § [Aug 9 decisions (lists 1+4)](#aug-9-decisions-lists-14) below (stills + lookalike shots under `legacy-reference/stills/`). Feature comparison canvas (outside repo): `~/.cursor/projects/Users-DougC1-Projects-Tawala/canvases/legacy-vs-mock-pages.canvas.tsx`.

**Stop:** `Ctrl+C` in the serve terminal, or from another shell: `cd website-mock && ./serve.sh stop`.

**Hardening:** `serve.sh` binds **127.0.0.1**, always uses `--directory` / `SERVE_ROOT` = this folder, prefers Node (`serve-static.mjs`, no extra deps) with Python `http.server` as fallback, kills a **stale** prior mock listener via `.serve.pid` (or a known `http.server` / `serve-static` on the port), and **restarts on crash** with backoff. Logs: `website-mock/.serve.log`. If an unknown process holds 5500: `SERVE_FORCE=1 ./serve.sh`.

**Prereq:** Java runtime on http://localhost:8080 with templates deployed. Test-drive links read from `js/demo-urls.js`.

## While you were away (Aug 1, 2026 — file cleanup / family day)

**Headline finding:** the designer-web dev API (`:3001`) was **completely broken** at the start of this session — `server/index.mjs` imported `exportProjectResponsesByUniqueId` / `importProjectResponsesByUniqueId` from `server/projectResponses.mjs`, but that file only exported differently-named functions with an incompatible shape. Node throws a hard `SyntaxError` on a missing named ES-module export, so the **whole server failed to start** — not just Export/Import, everything (Deploy, Preview, Purge, all of it). This is almost certainly why prior sessions on this track stalled. A stale Node process happened to still be listening on `:3001` from an earlier (and, on inspection, also-outdated) version of the code, which is why things may have *looked* fine if you didn't restart it — see "gotcha" below.

**What actually got fixed this session:**

1. **`designer-web/server/projectResponses.mjs`** — added the missing `exportProjectResponsesByUniqueId` / `importProjectResponsesByUniqueId` wrapper functions (grouped-by-form wire shape matching `js/data-ops.js` + `js/demo-urls.js`), with a Postgres → dev-session fallback mirroring how Purge already falls back for dev-only deploys. The dev API now starts and `/api/export-responses` + `/api/import-responses` work.
2. **`scripts/export-project-responses.sh` + `scripts/restore-project-responses.sh`** — a real second bug: the `psql` existence-check call (`psql -tAc "SELECT COUNT(*)…"`) ran *before* the real `psql -f -` script body, and both share one `docker compose exec -T` stdin pipe. Without an explicit `</dev/null` on the check, it silently drained the real SQL meant for `-f -`, so **Restore/Import silently no-op'd** — no error, `deleted` always came back `null`, and it looked like it "worked" only because re-importing the *same* data left the row count unchanged by coincidence. Fixed by redirecting every `psql` call that doesn't need the real stdin from `/dev/null`. Verified with a genuine differential test (reduce to 3 rows, confirm count drops, restore to 37, confirm it comes back) — see git history of this session or re-run the Smoke Export/Import steps below.
3. **Gotcha for next time:** `designer-web/scripts/ensure-dev-api.sh` only restarts the API when `/api/health` is **down** — a stale-but-responding process (like the one found this session) will not be replaced automatically. After pulling server-side changes, prefer a hard restart: `lsof -nP -tiTCP:3001 -sTCP:LISTEN | xargs kill; cd designer-web && ./scripts/ensure-dev-api.sh`.

**Newly wired this session (were grey stubs):**

- **Pull** (My Tawala listing icon + Project Details **PULL FROM LIBRARY**) — pick a Library project, confirm, refreshes description/category/reference start points only; never touches this row's own name, rating, or `:8080` deploy identity. See § Save/Deploy/Publish glossary and § "My Tawala layout" **PULL** below.

**Verified solid (already wired by an earlier session, now actually confirmed working end-to-end via curl round-trips against real Postgres data, not just read from code):**

- **EXPORT / IMPORT** and **BACKUP / RESTORE** (§ "My Tawala layout" below has the full contract + smoke steps). Purge re-verified unaffected.

**Still stubbed / out of scope this session:**

- **Online/Offline** (`TAKE PROJECT ONLINE` / `TAKE PROJECT OFFLINE` sidebar buttons) — deliberately **not** wired. There's no existing "active/inactive" flag on My Tawala projects, no UI badge for it, and no defined semantics for what Offline should actually gate (block test-drive? block new submissions? just cosmetic?) — wiring it "for real" needs an owner decision, and a cosmetic-only flip felt like exactly the kind of half-done stub the owner asked to avoid. Left as a clearly-scoped next stub, not attempted.
- Everything else already parked per `.cursor/rules/tawala-designer-parked-post-website.mdc` and the README **Parked / backlog** section below (My Tawala version piles, listing UX, folders, look-and-feel polish, etc.) — untouched.

**Not touched:** Desktop, `~/Tawala Projects/` piles, any commit/push (all work in this session is uncommitted, per instructions, for owner review).

**Smoke-test this first when you're back**, in this order: (1) confirm `:3001` is running the current code — `curl -s http://localhost:3001/api/health`, and if in doubt, hard-restart per the gotcha above; (2) **Smoke Export/Import** below (the API round-trip is fastest); (3) **Smoke Purge** (must-not-break, re-verified); (4) **Smoke Pull**; (5) Backup/Restore through the actual UI (only curl-tested this session, not click-tested — Cursor's browser tool was unavailable again this session, same blocker noted in `docs/CHAT_HANDOFF.md`).

## Library vs My Tawala (split piles)

### Tenancy (product truth — owner Jul 31, 2026)

There is **one public Library** (shared catalog). Each **account** has its **own separate, private My Tawala**; another account cannot see or access your My Tawala contents. The bridge out of private My Tawala into the public world is **Publish** (to Library) — optional and deliberate. Until Publish, projects stay private to that account’s My Tawala.

**Current priority (owner Jul 31, 2026):** **Libraries are the core of the Website** (Library + My Tawala / catalog ops). Harden **Delete / Purge** and data/backup lifecycle ops (**EXPORT / IMPORT**, **BACKUP / RESTORE**) over look-and-feel; solid basis for libraries already exists. Library → My Tawala **Save a copy** remains the acquire stub; **Use** (run start link) is on My Tawala. **Do not** chase more visual polish now — **Home** L&F is optional and non-critical. Version piles stay deferred until ops are trustworthy (see glossary **Sequencing / hold** below).

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

Main Menu public templates (Simple Survey, Sign-up, Potluck, Get Together, Multiple Question Survey) stay in the Library catalog with current Phase 2 `:8080` URLs. JSON for most of those is under `designer-web/public/samples/templates/`; Multiple Question Survey also has a WebLibrary backup under `projects/library/`. **Sign-up Sheet Template w Email** (`signup-sheet-email`) was **retired from the public Library** (owner Aug 1, 2026) — not drop-in ready yet; remains on **Designer → File → New Project** only (not seeded into default My Tawala). Re-Publish when a finished runtime-customizable version exists.

### Library Actions / Use framing (owner Aug 1, 2026)

**Product split (implemented in mock):** Library = discovery / acquire; My Tawala = operate. Library Actions are **Test drive** | **Save a copy** only — **no Use** on Library. **Use** lives on **My Tawala** (listing icon + Project Details **USE**): **single start point** → opens that `:8080` URL; **multiple start points** (e.g. Online Exam Setup + Exam) → navigates to **Project Details** so the user chooses an entry point (does **not** jump straight to Exam). Grey when no deploy URL. Does **not** purge-on-start (unlike Library Test drive). This is **not** legacy CloneAndCustomize / web customize — that path stays deferred (Sophisticated in-project customize vs retire).

**Background:** Most users are not comfortable with Designer. Some projects cannot really be used without going through Designer first (e.g. **Sign-up Sheet Template w Email** — customize in Designer); those belong on **Designer → File → New Project**, not the public Library (retired from catalog Aug 1, 2026). The alternative path is versions customizable entirely from within the Project (no Designer) — historically **"Sophisticated"**. Users who will not pull into Designer may just want a **copy in My Tawala to run**, then **Use** from My Tawala.

**Implications:**

- Library should prefer **runnable / admin-from-runtime** projects, or clearly label **Designer-required** starters.
- Designer-only starters stay on the New Project menu; `signup-sheet-email` is no longer in `TAWALA_LIBRARY`.
- Glossary: **Use** (My Tawala — single-start run link, or Project Details when multi-entry) ≠ **Save a copy** (Library → My Tawala) ≠ **open in Designer** ≠ legacy **USE IT** / CloneAndCustomize (not wired).
- Aligns with tenancy: **Publish** to Library; **operate** from My Tawala.
- **Owner evidence (Aug 1, 2026):** *“Every year the sports leagues asked how to move over rosters from previous years so they wouldn't have to re-enter all that data.”* **Refinement:** leagues **exported via Excel** at end of season and **archived** it; they only **reused player data**, stripping graduates and adding new kids. Season handoff ≈ **Export (Excel) archive + selective Import/reuse of a roster subset** — not full **Backup/Restore** of everything, not delete-and-re-pull Library. Aligns with the documented **EXPORT/IMPORT** data spine (Excel). Soft open: a future “roll season” could mean export-archive + import filtered roster. Reinforces **named long-lived instances + cross-season data move**; distinct from Designer Customize vs Copy to My Tawala. (Parked — no feature invented here; see Parked / backlog.)

### Library quality / community (owner Aug 1, 2026 — open questions; L&F / later product)

**Parked — framing only; do not build ratings, reputation, or community-moderation wiring now.** Same sequencing as other look-and-feel / later-product ideas: **Library / catalog ops first**; this is not initial Publish wiring.

**Owner thoughts (open):**

- There is a real difference between **professionally developed** projects and **amateur** / community-shared ideas.
- Value in people sharing ideas — but hard to mix that with **very complex** projects (e.g. SportsDashboards / DirtBowl class) that required professional design.
- Possible: **reviews and ratings** gradually promote the best products to the top.
- Caution: **reputation systems are easy to game** — uncertain how well that works in practice.
- Analogy: YouTube — much of what worked was making it **really easy to load content**; for Tawala, **Designer would need to get a lot easier** for community contribution to work well.

**Open questions (not decided):** How (or whether) to surface pro vs community projects in one Library; whether ratings/reviews are worth the gameability risk; how much Designer ease-of-use is a prerequisite for a healthy publish culture. Keep today’s mock stars / comments as stubs only — no reputation product work in this phase. See also Parked / backlog.

### Library stubs and their retirement path (owner Aug 1, 2026)

Most `projects/library/` WebLibrary backups were bulk-converted placeholders — never deployed, never test-driven, not owner-vetted. The **correct** way to clean these up is **Designer → Deploy → Publish**: open the equivalent working copy in Designer, Deploy it, verify, then Publish it into the Library. **Publish is wired** (see § Publish below) — until a stub has a real Deploy/Publish replacement, it **stays in the catalog as a visible reminder**; there is no shortcut that skips checking the Designer/Deploy equivalent first.

**How a stub is identified:** `stub: true` on a `TAWALA_LIBRARY` entry in `js/demo-urls.js` — set on placeholders / converted-but-unverified projects that are **not** `liveReady` and have no real `:8080` deploy. The entry's display `name` also carries a visible **`" (stub)"` suffix** (e.g. `"AlexTimon (stub)"`) so the Library listing itself shows what still needs replacing, without any extra rendering code (titles everywhere already flow through `TawalaDemo.displayName()`). `liveReady: true` entries (Aug 4, 2026: Simple Survey, Sign-up Sheet Template, Potluck, Get Together, Horses and Penguins Test, Multiple Question Survey Template, Online Exam Builder) are owner-vetted working demos and are **never** marked as stubs. Other real (non-`stub`) rows that are not yet `liveReady` are never stubs either — see "Unvetted non-stub entries" below.

**Current stub list (14 of 21 Library entries, Aug 4, 2026):** AlexTimon, Automated List Builder, ClientProfiler, CYO CheckDeposit Request1, CYO Exceptions App, DirtBowl, GenericListManager, League Age calculator, Lunch Order Menu, MVSC Communicator, MVSC Registration, SportsDashboards Template, St Patrick SportsDashboards, Tawala Invoicing. (**Online Exam Builder** promoted to Live Aug 4.) Get the live list any time (never goes stale) with:

```bash
cd website-mock && node scripts/list-library-stubs.mjs          # table
node scripts/list-library-stubs.mjs --ids                       # ids only
node scripts/list-library-stubs.mjs --json                      # id/name/category/jsonFile
```

**Retirement path — no public Library Delete button.** Owner already knows Library must not get a public Delete control (unlike My Tawala's private row Delete, see below). Two supported paths now:

- **Self-serve (Aug 1, 2026):** open `library-admin.html` (see § Library admin path) and click **Retire** on the row — works on this browser, writes the localStorage overlay, done immediately.
- **Agent-assisted, ships into the repo catalog:** owner asks an agent to retire it (e.g. *"retire stub dirtbowl"* or *"move all stubs to My Tawala"*), and the agent:
  1. Runs `list-library-stubs.mjs` to confirm current stub ids (read-only — never edits the catalog itself).
  2. Removes the entry from `TAWALA_LIBRARY` in `js/demo-urls.js` — and, if moving to My Tawala instead of deleting outright, adds an equivalent entry to `TAWALA_MYTAWALA` **keeping** the `" (stub)"` suffix and `stub: true` flag (owner Aug 1, 2026 — the safety-net copy stays visibly marked as a retired stub; never dropped).
  3. Moves the backing JSON from `projects/library/<file>` to `projects/mytawala/<file>` when moving to My Tawala (or deletes it when just retiring the placeholder outright with no replacement).
  4. If a **real** replacement was Deployed/Published instead, adds/updates the new catalog entry per the "Owner checklist" below (`deployed: true`, `testDriveUrl`, `liveReady: true` once vetted) rather than just deleting the stub.

Both paths keep stub cleanup deliberate and reversible (My Tawala safety copy, or `library-admin.html` "Restore to Library") — never a one-click public Delete.

**Unvetted non-stub entries (owner Aug 1, 2026 — "a lot of these").** Real (non-`stub`) Library rows catalogued without owner vetting stay unmarked as stubs. **Sign-up Sheet w Email** was an example; it was **removed from `TAWALA_LIBRARY`** (Aug 1, 2026) rather than seeded into everyone's My Tawala — still on Designer New Project; owner may keep a personal copy via admin Retire if they already did. For remaining unvetted non-stubs still in the catalog, Retire (via `library-admin.html` or `TawalaTransfer.retireLibraryEntry`) moves them into the My Tawala overlay so the owner can open/continue in Designer and re-Publish later — display **name left as-is**, no fake `" (stub)"` suffix (`retiredFromLibraryId` + `retiredWasStub: false` + note). The admin table flags any non-stub, non-`liveReady` row with a quiet **"not vetted"** badge as a cleanup hint.

## Navigation and stub pages

Shared chrome lives in `js/chrome.js` (header, footer, guest/logged-in status).

| Page | File |
|------|------|
| Home | `index.html` |
| Library | `library.html` |
| Project detail (Library) | `library-detail.html?project=…` |
| My Tawala — My Projects | `mytawala.html` |
| My Tawala — Project Details | `mytawala-project.html?project=…` |
| Library admin tools (maintainer, gated) | `library-admin.html` (see § Library admin path) |
| Ops archive review | `project-ops-review.html` |
| **Docs** (curated mock ops HTML) | `docs.html` — linked from mock banner + footer; also `README.md` via static server |
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
| **Library → My Tawala** | Listing / detail **Save a copy** / **Save to My Tawala** | Grey stub (acquire). **Use** is on My Tawala, not Library — see § Library Actions / Use framing. |
| **My Tawala → Library** | Listing **Publish**, detail **PUBLISH** | Wired (Aug 1, 2026) — opens the Publish dialog. Sidebar **Publish to Library** stays a grey stub (no bound project in that context; use the listing/detail action). |
| **Library ← My Tawala upgrade** | Listing **Pull**, detail **PULL FROM LIBRARY** | Wired (Aug 1, 2026) — opens a dialog to pick a Library project; `TawalaTransfer.pullFromLibrary` refreshes description/category/reference start points on the My Tawala row. Sidebar **Pull from Library** (in the symbiotic-flows archive table only, not a live control) stays informational. |
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
| **Save a copy** (Library) | Library → My Tawala | Copy a public Library project into *this account’s* private My Tawala (acquire). Stays private until Publish. Not Use; not open in Designer; not a PM submissions verb. Stub in mock. |
| **Use** | My Tawala listing + Details | Single-start: open that `:8080` URL (operate). Multi-start: open **Project Details** to pick Setup vs Exam (etc.). Wired Aug 1 / Aug 4, 2026 — active when a deploy URL exists; grey otherwise. Does **not** purge-on-start. ≠ Save a copy ≠ open in Designer ≠ legacy CloneAndCustomize. See § Library Actions / Use framing. |
| **Open in Designer** | Library / My Tawala → Designer | Author / customize the project definition in Designer. Required for some starters (e.g. Sign-up Sheet w Email); majority of users are not comfortable here — prefer runtime-admin or My Tawala copy-to-run paths when possible. |
| **EXPORT** | My Tawala / Project Manager | Outbound Excel **response data** only. Not definition versioning; not Backup. |
| **IMPORT** | My Tawala / Project Manager | Restore messed-up **data** into the current project. Field mismatch **fails** (correct — e.g. Export from an older schema into a project that later added an MCQ); does **not** roll back definition. Same data spine as Export. |
| **BACKUP** | My Tawala / Project Manager | Write a `.backup` ZIP — paired **definition + data** (plus properties / links). |
| **RESTORE** | My Tawala / Project Manager | Re-apply a **Backup** snapshot (legacy target: matching definition, then data). **Not** “switch Deploy to an earlier definition version” / make-version-current — that is **Deploy this version** (B7; wired Aug 7, 2026 for snapshot-backed rows). Distinct from Import. |
| **Deploy** | Designer → runtime / My Tawala | Mints a My Tawala **definition version** and (in shipped Java) auto-deploys it; optionally surface via **Show in My Tawala**. Deploy ≠ Backup ≠ Publish to Library. |
| **Publish** | My Tawala → Library | Deliberate bridge from private My Tawala into the **one** public Library catalog. Optional; until then, projects remain account-private. **Wired Aug 1, 2026** (mock — localStorage Library overlay): My Tawala listing **Publish** / detail **PUBLISH** open a dialog to rename on the way in and optionally replace an existing Library entry. See § Publish below. |
| **Pull** | Library → My Tawala (upgrade) | Refresh a My Tawala project's descriptive content from a public Library version. **Wired Aug 1, 2026** (mock — no real project-definition versioning yet, so this refreshes description/category/reference links only, not an actual newer Designer definition; name/rating/comments/deploy identity are preserved). See § "My Tawala layout" **PULL** below. |
| **Versioning** | My Tawala project | Immutable project-definition snapshots (deployed vs non-deployed, Library submit rules, test-drive, upload metadata) — see triage **B7 Project Versioning**. Historically present in PM **Versions** UI; separate from Export/Import and from Backup/Restore. **Mock first slice:** history lives on Project Details **Versions** only (flat listing unchanged). |
| **Designer Save** | Designer File → Save | Persist local definition on disk — authoring only. |

**Pillars (do not conflate):**

1. **Tenancy** — one public Library; per-account private My Tawala; Publish is the only intentional public bridge (see § Tenancy above).
2. **Data ops (Excel)** — **EXPORT / IMPORT** — response data only; Import does not change definition.
3. **Paired backup ops** — **BACKUP / RESTORE** — `.backup` ZIP with matching definition + data (plus properties / links).
4. **Versioning** — which project-definition revision is active / test-driven / submitted to Library (B7 / Deploy), independent of Excel data tools and of Backup packages.
5. **Deploy vs Publish** — Deploy mints/auto-deploys a definition version for an account’s project; Publish shares into the shared Library catalog.

**Sequencing / hold (owner Jul 31, 2026; first slice Aug 5, 2026; Deploy-switch Aug 7, 2026):** Do **not** fill the My Tawala **listing** with **version piles** — keep **one flat row per project**. The hold is **listing clutter**, not “versions never existed.” **Safe first slice (owner green light):** Deploy → Show in My Tawala mints a monotonic `versionNumber` + optional **version description** on the overlay/receipt; Project Details **Versions** shows that history (number, description, date, current/deployed) and may offer a minimal JSON download; **description of an existing version is editable** (legacy: versions immutable except description). **Deploy this version (Aug 7):** radios select a row; the button switches the live `:8080` definition to that row’s **saved snapshot** (same project name / uniqueId when Java matches by name) — does **not** mint a new Versions row. Still **not** wired: listing piles, **delete-version**, audit trail. Legacy intent (Deploy creates versions, one deployed, Library = published snapshot) remains the **target** model (B7).

**Owner smoke Aug 7, 2026 — Versions / Deploy-switch (updated after wire):**

| Expectation (owner) | What the mock does | Honest status |
|---------------------|--------------------|---------------|
| **Version radios** + **Deploy this version** switch the **deployed** definition (e.g. recover after a bad overwrite) | Radios select a row; **Deploy this version** is enabled only for a **non-current** row that has a **definition snapshot** (from Deploy → Show in My Tawala). Confirm warns that responses under a newer form may not match. Calls `:3001/api/deploy` with the saved project JSON, then marks that row current/deployed and refreshes start points. | **Wired** — still easy to get in trouble with data/schema (see below). |
| Older Versions rows (metadata-only, before snapshots) | Status shows **No snapshot**; Deploy stays grey; click path alerts: *“This version has no saved definition — only versions created after Deploy → Show in My Tawala started saving snapshots (Aug 7, 2026+) can be redeployed.”* | Expected — re-Deploy + Show in My Tawala to mint snapshot-backed rows going forward. |
| **Restore** = make an earlier **definition version** current | Mock **BACKUP / RESTORE** is **data-oriented** (re-apply properties + submissions from a `.backup.json`); it does **not** switch which Deployed version is current on `:8080` | Glossary: Restore ≠ Deploy-switch. Use **Deploy this version** for definition switch. |
| **Import** into a newer schema (project later gained an MCQ) | Field-mismatch check **fails** when Export data does not match the current project | **Correct behavior** / education — Import does not roll back definition. Switching Deploy back to an older definition does **not** rewrite submission rows. |
| **Version Download** unlocks switch / full definition recovery | Downloads **minimal metadata JSON** only — **not** the switch path (switch uses the server/overlay snapshot) | Do not treat Download as Deploy-switch. |

**Ways to get in trouble (Deploy this version — owner invited):** switching to an older form while newer responses exist; Import after a schema change; clearing `:3001` `.deployed/version-snapshots/` so overlay `snapshotId`s go dead; renaming the My Tawala project so Java mint/match-by-name diverges; Deploy without **Show in My Tawala** (no Versions row / no snapshot).

**Open questions (park — glossary only; do not implement):** Exact product meanings of Export / Import / Backup / Restore are still TBD with the owner (do **not** redesign those verbs yet). Possible later addition: some **functions** may export only the fields they incorporate — i.e. **standalone** function-scoped export vs project-level E/I/B/R. **Selective Purge by form** (owner Aug 7, 2026) — e.g. SportsDashboards: keep Player/Coach data, purge statistics for a new year. Owner will ask commissioners whether they needed this and whether legacy supported it. Park under **Purge / E/I/B/R**; do **not** build.

## My Tawala layout (lean) vs ops archive

Legacy Project Manager lives under `TawalaWebapp-build1700/web/WEB-INF/jsp/projectmanager/` (not the thin `mytawala/*.jsp` news pages). Mock mirrors the shallow structure:

1. **`mytawala.html`** — My Projects listing (name, created, updated + **Use** · Export…Publish · Pull icon columns). **Use**: single-start opens `:8080` (no purge); multi-start opens Project Details to choose an entry point. Click headings to sort; drag header edges to resize. Row **Delete** confirms and removes the **account-private** My Tawala row (clears Deploy overlay + inbox receipt; seed rows stay hidden via `localStorage` deleted set). **Purge** clears submissions only (separate). Click project name → Project Details. Sub-menu: **My Projects · My Account · Change Password**. **Column groups (Aug 5, 2026):** visual separators between Project info · Data transfer · Backup · Destructive (red Purge/Delete) · Library transfer — flat listing only (no version piles).
2. **`mytawala-project.html?project=…`** — Project Details: **USE · EXPORT…PULL** action bar (no Publish on Details — listing only), left sidebar (REVISE / ONLINE-OFFLINE / Include / Invite), collapsible sections (Start points, Project Data, Versions, Backups…). **USE** same as listing (single → run; multi → stay on Details / Start points). **DELETE** same as listing, then returns to My Projects. Start-point links stay on `:8080`. **Versions:** Deploy-minted history (`versionNumber`, description, date, current/deployed); description editable inline; **Download = minimal metadata JSON**; radios select a row; **Deploy this version** switches live `:8080` to that row’s saved definition snapshot (non-current + snapshot required; confirm warns about response/schema mismatch). Delete version stays grey. Rows without snapshots (pre–Aug 7 history) show **No snapshot**.
3. **`project-ops-review.html`** — full recovered label catalog split by surface (**Public Library** vs **My Tawala / Project Manager**) for memory / archive review. Linked from My Tawala sidebar; not stacked on the working pages.

Labels live in `js/project-ops.js`. Inactive Project Manager ops are **disabled** (grey only — no “not wired” text). Active ops (**PURGE**, **DELETE**) use Designer-accent blue styling from `css/tawala-chrome.css` (`--tw-accent` mirrors `designer-web/src/styles.css`).

**DELETE** (wired Jul 31, 2026 — was mock row-remove only): confirm → `TawalaTransfer.deleteMyTawalaProject` removes the private My Tawala entry for this browser account mock — clears `tawala.mock.myTawalaOverlay` + matching `tawala.mock.deployInbox` receipts, and records `tawala.mock.myTawalaDeleted` so catalog seed rows from `TAWALA_MYTAWALA` do not reappear on reload. Does **not** delete public Library / `liveReady` catalog entries, does **not** purge `:8080` submissions (use **PURGE**), and does **not** remove Tomcat project XML. Re-Deploy → **Show in My Tawala** clears the deleted mark and restores the row. Listing and Details share the same handler; Details redirects to `mytawala.html` after Delete.

**PURGE** (hardened Jul 31, 2026): confirm → resolve `:8080` **uniqueId** from My Tawala project (`uniqueId` field on Deploy overlay, else `testDriveUrl` / start-point URLs, else deploy-inbox receipt) → POST `http://localhost:3001/api/purge-responses` (`dev` / `dev`). Deletes **all** `submission` rows for that `user_project.unique_random_id` in Docker Postgres — same effect as Java Project Manager `purgeProjectResponses`. Listing and Details share the same handler; success/failure both surface via status line + alert (deleted row count on success). Does **not** remove the My Tawala row (use **DELETE**), does **not** touch public Library / `liveReady`, and does **not** remove Tomcat project XML. Undeployed seed rows (no uniqueId) get a clear “Deploy first” message. **Not** form-selective (owner Aug 7 park: Selective Purge by form — see Open questions / Parked backlog).

**PUBLISH** (wired Aug 1, 2026 — was grey stub): confirm → opens a dialog (name editable, default = current name with any `" (stub)"` suffix stripped, since a Publish target is never itself a stub) → `TawalaTransfer.publishToLibrary` writes a `tawala.mock.libraryOverlay` entry merged on top of the public Library (mock only — see § Publish below for the full contract, matching rules, and what still needs a human to “ship to Live”). **Purges `:8080` responses on Publish by default** (owner Aug 1, 2026 — one account must never see another's submissions): after a successful Publish, `TawalaDemo.purgeAfterPublish(sourceProjectId)` resolves the same uniqueId as My Tawala **PURGE** and calls the same `:3001/api/purge-responses` endpoint. A **"Purge responses when publishing"** checkbox in the dialog (default **checked**) is the escape hatch; leaving it checked (the norm) still lets Publish succeed even if there's no linked deploy or the purge call fails — the success alert just adds a clear ⚠️ line saying responses were **not** cleared instead of silently pretending they were.

**EXPORT / IMPORT / BACKUP / RESTORE** (fixed + hardened Aug 1, 2026 — see § "While you were away" below for what was actually broken): no confirm on Export/Backup (non-destructive downloads); Import/Restore confirm first (field-mismatch check runs before the confirm on Import). All four go through `js/data-ops.js` → `TawalaDemo.exportResponses` / `importResponses` (`js/demo-urls.js`) → `POST :3001/api/export-responses` / `/api/import-responses` → `designer-web/server/projectResponses.mjs`. **EXPORT** downloads `<slug>.export.json` (response data only, per-form field lists for the Import mismatch check). **IMPORT** requires an Export-shaped file, runs the field-mismatch check against the project's own current data (skipped per-form when there's no current data to compare against — documented mock gap, see `projectResponses.mjs` header), then confirms and replaces all submissions for that project via `mode:"replace"` (no merge mode implemented). **BACKUP** downloads `<slug>.backup.json` — My Tawala catalog properties + this same response data + (when `jsonFile` points inside `website-mock/projects/`) a fetched copy of the project definition JSON; real Java Backup is a `.backup` ZIP with the full Designer definition XML, not a mock JSON bundle. **RESTORE** requires a Backup-shaped file, confirms, re-applies the properties (`TawalaTransfer.upsertMyTawalaProperties`) and then the data — but does **not** push the definition JSON into a live Designer session (`:5173` and this mock `:5500` do not share storage; open it manually if you need the design itself back), and does **not** switch which definition version is deployed on `:8080` (that is **Deploy this version**, wired Aug 7 for snapshot-backed rows). Owner smoke Aug 7: do not read Restore as “make earlier definition version current.” **IMPORT** field-mismatch failure when data does not match the current (newer) schema is **expected** — see glossary table above.

Server-side, both **Postgres** (real `user_project` rows) and the Node **dev-session** store (`.deployed/sessions/<uniqueId>.json` — dev-only deploys with no Postgres row) are supported, the same fallback pattern PURGE already used — Export/Import try Postgres first, then dev-session; an unknown uniqueId (neither store has it) is a hard failure. Dev-session export includes whatever is in `session.records`, including non-submission seed rows (e.g. DirtBowl's `AdminSetup`/`Divisions`) — documented gap, not filtered out.

**PULL** (wired Aug 1, 2026 — was grey stub): opens a dialog to pick a public Library project (exact name/id matches are preselected, e.g. a My Tawala row still named "Simple Survey"); confirm → `TawalaTransfer.pullFromLibrary` overwrites **only** this My Tawala row's `category` / `shortDescription` / `longDescription` / `iconLabel` / `jsonFile` / `startPoints` / `testDriveUrl` from the picked Library entry, and records `pulledFromLibraryId` / `pulledFromLibraryName` / `pulledAt`. Deliberately does **not** touch this row's own `name`, `rating`, `comments`, `created`, `uniqueId`, or `deployed` — a Pull must never repoint this account's private Export/Import/Purge at the shared public Library demo's own `:8080` submission data. Pull only refreshes descriptive/reference content (and optional `jsonFile`); it does **not** mint or switch Deploy versions — use Designer Deploy / **Deploy this version** for definition lifecycle.

**Smoke Purge:** with `:3001` + Postgres up — `curl -s -X POST http://localhost:3001/api/purge-responses -H 'Content-Type: application/json' -d '{"uniqueId":"gy1zssbrwm4fgfm","credentials":{"user":"dev","password":"dev"}}'`. UI: Deploy a project → **Show in My Tawala** → Purge from listing or Details → confirm → alert shows deleted count; Test drive still purge-on-start. Undeployed seed → Purge → “no uniqueId / Deploy first” alert.

**Smoke Delete:** open http://127.0.0.1:5500/mytawala.html → Delete a seed row → reload → row still gone → Library still lists the public twin if any. Deploy overlay: use Designer **Show in My Tawala**, Delete from listing or Details → overlay + inbox entry gone; Library catalog unchanged.

**Smoke Deploy this version (B7 switch):** with Designer `:5173`, API `:3001`, Tomcat `:8080`, and website-mock `:5500` up:

1. Open **Simple Survey** (or similar) in Designer → Deploy → optional note “v1” → **Show in My Tawala** → Versions shows v1 Current · Deployed (not “No snapshot”).
2. In Designer, add an MCQ (or other field) → Deploy → note “v2 with MCQ” → **Show in My Tawala** → Versions shows v2 current; v1 still listed with a snapshot.
3. Submit a response on `:8080` under v2 (so live data includes the new field).
4. On Project Details → Versions: select **v1** radio → **Deploy this version** enables → confirm the schema warning → success → v1 is Current · Deployed; start-point URL still same uniqueId; form on `:8080` matches v1 (no extra MCQ).
5. Optional trouble: Export/Import after switching — Import may fail field-mismatch if data was collected under v2. Pre-snapshot history rows stay **No snapshot** and cannot redeploy.

**Smoke Publish:** open http://127.0.0.1:5500/mytawala.html → click **Publish** on any row (or open Project Details → **PUBLISH**) → dialog opens with the name pre-filled and **"Purge responses when publishing"** checked by default → type a new name, e.g. “DirtBowl League Manager”, pick the flagged “DirtBowl (stub) — retires to My Tawala, kept marked (stub)” target → **Publish** → confirm → alert confirms the new Library entry, the retired stub, **and** either a purged-row count or a clear ⚠️ note if purge was skipped/failed → reload `library.html`: new name appears (no `(stub)` suffix), old “DirtBowl (stub)” row is gone → reload `mytawala.html`: a new private row named “DirtBowl (stub)” appears (safety copy). Repeat picking **None** to just add a new entry with no retirement, and picking a **non-stub** target (e.g. an out-of-date Main Menu template) to see it replaced in place with no My Tawala copy. Uncheck the purge checkbox once to confirm Publish still succeeds and the alert says responses were **not** purged. Pick a project with a live `:8080` deploy and prior test-drive submissions, Publish it with the checkbox checked, then **Test drive** the newly published Library entry — it should open with a clean slate, not the previous account's answers.

**Smoke Export/Import (API round-trip — no UI needed for a quick check):** with `:3001` + Postgres up and a `uniqueId` that has submissions (`docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT up.unique_random_id, count(*) FROM submission s JOIN user_project up ON s.project_id=up.project_id GROUP BY 1 ORDER BY 2 DESC LIMIT 5;"`):

```bash
curl -s -X POST http://localhost:3001/api/export-responses -H 'Content-Type: application/json' \
  -d '{"uniqueId":"<uniqueId>","credentials":{"user":"dev","password":"dev"}}' > /tmp/export.json
python3 -c "import json;d=json.load(open('/tmp/export.json'));print('count',d['count'])"
# round-trip it straight back in (mode: replace) — count should be unchanged after:
python3 -c "
import json
d = json.load(open('/tmp/export.json'))
json.dump({'uniqueId': d['uniqueId'], 'forms': d['forms'], 'source': d['source'], 'mode': 'replace',
           'credentials': {'user': 'dev', 'password': 'dev'}}, open('/tmp/import.json', 'w'))
"
curl -s -X POST http://localhost:3001/api/import-responses -H 'Content-Type: application/json' --data-binary @/tmp/import.json
```

Expect `{"status":"success", ..., "deleted": N, "inserted": N}` with `deleted` equal to the count before the round-trip (if `deleted` comes back `null`/wrong on a fresh checkout, the `psql -f -` stdin race documented in `scripts/restore-project-responses.sh` has regressed — re-check the `</dev/null` on every `psql` call that doesn't need the real stdin body). **UI:** My Tawala listing/Details → **Export** icon/button downloads `<name>.export.json` (no confirm) → **Import** on the same row, pick that file back → confirm → alert reports rows imported. **Backup** downloads `<name>.backup.json` (no confirm) → **Restore**, pick that file → confirm → alert reports rows restored.

**Smoke Pull:** open http://127.0.0.1:5500/mytawala.html → **Pull** icon on a row (or Project Details → **PULL FROM LIBRARY**) → dialog opens; if the project's name matches a Library entry it's preselected under "Possible matches", otherwise pick any Library project from the list → **Pull** → confirm → alert confirms → reload: the row's description/category refreshed from the picked Library entry, but its **name**, rating, comments, and `:8080` deploy/uniqueId are unchanged (Export/Purge on that row should still hit the same submissions as before the Pull).

CLI equivalents:

```bash
./scripts/dev-data.sh purge-by-unique-id gy1zssbrwm4fgfm
# or DirtBowl Registration-only:
./scripts/dev-data.sh cleanup-registrations
```

Requires designer-web API (`:3001`) and `docker compose` postgres. SportsDashboards spelling (not SportsBoard).

### Test drive purge-on-start

Library / home **primary Test Drive** (listing icon + detail button) purges that project’s responses **before** opening `:8080`, so each fresh drive starts clean. That is appropriate for simple surveys with no setup state.

**Library Project Details start-point links do not purge on click** (same as My Tawala Details): after Setup adds questions, opening **Exam** from the start-point list must keep those rows. Only the primary **Test Drive** button (and listing Test drive icon) purge-on-start. For **Online Exam Builder**, Library Test Drive prefers **Administration** / **Setup** (not Exam) so the first open is where you configure questions.

**My Tawala Project Details start points do not purge on click** (same as single-start **Use**): they open the live Deploy URL (`/p/{uniqueId}/{formToken}.FormName`) and keep stored data. This is required for multi-start apps like **Online Exam Builder** — Admin/Setup writes **Question** and **SetupVariables** rows; purging before Exam wiped those rows, so Answer showed only `1)` with a blank after Name submit. Use Project Actions **PURGE** when you deliberately want a clean slate.

My Tawala **Use** for **multi-start** projects goes to **Project Details** (not Exam). Start-point list ordering may still prefer end-user forms (**Exam**, **Registration**, …) in the list. Library Test Drive for Exam+Admin apps prefers Setup/Admin instead. Single-start **Use** opens the one `:8080` URL directly.

**Limitation:** the form opens in a new tab; there is no reliable purge-on-tab-close in this static mock. Legacy Library test drive used an in-memory session world (no durable DB writes); local mock hits the real deployed project, so Library purge-on-start (primary button only) is the practical substitute.

## Publish (My Tawala → Library)

Wired Aug 1, 2026 (was a grey stub). Entry points: My Tawala listing **Publish** icon, Project Details **PUBLISH** button, and `library-admin.html` (§ Library admin path) for a maintainer running it without opening each project.

**What the dialog does:**

1. **Name** — editable text field, defaults to the current project's display name with any `" (stub)"` suffix stripped (a Publish target is never itself a stub). Owner can rename on the way in — this is the *temporary* rename hook called for until real project versioning exists.
2. **Replace an existing Library project (optional)** — a dropdown listing every current Library entry (stub and non-stub), each labeled with exactly what choosing it will do:
   - `"<name> — retires to My Tawala, kept marked (stub)"` for stub targets.
   - `"<name> — replaces in place — overlay only"` for non-stub targets (liveReady, Main Menu, or an earlier Publish).
   - Grouped under **Possible matches** (flagged `exact` or `possible`) then **Other Library projects** — see matching rules below. Nothing is ever preselected except an *exact* id/name match, so a name collision never silently replaces the wrong row.
3. **Purge responses when publishing** — checkbox, default **checked**. Owner Aug 1, 2026: *"can't think of a case where one user should see another's responses"* — Publish always purges by default; this is only an escape hatch for the rare case it shouldn't.
4. **Confirm** — a `window.confirm()` spelling out the consequence (new entry / stub retirement / in-place replace / whether responses will be purged) before anything is written.

**Purge-on-Publish (owner Aug 1, 2026):** once `publishToLibrary` returns success, and the checkbox is checked, the dialog calls `TawalaDemo.purgeAfterPublish(sourceProjectId)` — same uniqueId resolution as My Tawala **PURGE** (`resolvePurgeUniqueId`), same `POST :3001/api/purge-responses` call. Three outcomes, all surfaced in the same success alert (Publish itself never fails because of this step):

- **Purged** — `"Purged prior responses (deleted N submission row(s))"`.
- **Skipped** (no linked `:8080` deploy yet) — `"⚠️ Responses were NOT purged — … isn't linked to a live :8080 deploy yet."`
- **Failed** (API/Postgres down) — `"⚠️ Responses were NOT purged — purge failed: …"`.

Does **not** run on **Retire**, **Rename**, or a category move — only an actual Publish (new entry or overwrite) triggers it, since those other ops don't introduce a new "who can see this project's data" boundary.

**Matching rules (ignore the `" (stub)"` suffix on both sides):** `TawalaTransfer.findMatchingLibraryTargets(proposedName, sourceProjectId, sourceProjectName)` compares the proposed name and the source My Tawala id/name against every Library entry's id and stub-suffix-stripped name — both as an exact slug/compact-string match and a looser substring match (so a Deploy-overlay project literally named “Signup sheet” still surfaces the catalog “Sign-up Sheet Template” as a *possible* match, without ever auto-selecting it and risking the wrong overwrite).

**Two different outcomes, same “replace” picker:**

| Target picked | What happens | My Tawala copy? |
|---|---|---|
| *(none)* | New `tawala.mock.libraryOverlay` entry at a fresh slug from the name. | No. |
| Existing **stub** (`stub: true`) | Stub is retired: dropped from the Library listing (`tawala.mock.libraryRetired`), new content takes its id. | **Yes** — safety copy in My Tawala, keeps the `" (stub)"` name suffix + `stub: true` flag (owner Aug 1, 2026 — never dropped). |
| Existing **non-stub** (liveReady / Main Menu / prior Publish) | Content is overlaid **in place** at the same id — a true replace/update, e.g. fixing an out-of-date Sign-up Sheet Template. | No — it was never a placeholder stub, so no safety copy is made here (use admin **Retire** first if you want the *old* version preserved instead of overwritten). |

**Collision guard:** picking *(none)* always gets a **free slug** — `TawalaTransfer.publishToLibrary` checks the new name's slug against every currently-visible Library id and appends `-2`, `-3`, … if it collides, so e.g. publishing a project named "Signup sheet" with no target chosen adds a *new* row (`signup-sheet-2`) instead of silently shadowing the real `signup-sheet` catalog entry. Only an explicit target pick (via the dialog's dropdown) reuses an existing id.

**Storage (mock-only):** `tawala.mock.libraryOverlay` (`{ [libraryId]: catalog-shaped entry }`, merged on top of `TAWALA_LIBRARY` by `TawalaDemo.libraryEntries()` / `getLibrary()`) and `tawala.mock.libraryRetired` (`{ [libraryId]: true }`, hides the old catalog row for a retired stub). Both are single-browser `localStorage` — **shipping a Publish overlay into the repo catalog (`js/demo-urls.js`) and flipping `liveReady: true` remains a separate, deliberate maintainer step**, same as the existing Deploy → My Tawala overlay pattern. Nothing here edits `js/demo-urls.js` or moves files on disk.

## Library admin path

Owner Aug 1, 2026 — self-serve Library maintenance "for when you won't always be available", so rename/overwrite/retire don't require an agent.

**Enable it** (mock-only client gate, no real auth — anyone with the URL can flip it, same trust level as everything else in this static mock):

- Visit `library-admin.html?admin=1` once (sets `localStorage["tawala.mock.libraryAdmin"] = "1"` and cleans the URL), **or**
- Open `library-admin.html` and click **Enable Library admin mode on this browser**.
- Turn it off from the same page (**Turn off** button) or `library-admin.html?admin=0`.

Linked quietly from the My Tawala sidebar (**Maintainer tools**) and the Library sidebar — the link is always visible, the gate still applies.

**Once enabled, the page offers:**

1. **Publish / overwrite from a My Tawala project** — the same `TawalaTransfer.publishToLibrary` call as the My Tawala Publish dialog (source project + name + optional overwrite target), run from one place for every project instead of opening each one's Details page. Same **"Purge responses when publishing"** checkbox (default checked) and same `TawalaDemo.purgeAfterPublish` call afterward — see § Publish above.
2. **Rename** — edit any current Library entry's display name in place (`TawalaTransfer.renameLibraryEntry`) without Publishing anything new. Useful for typo fixes or reworking a stub's title before deciding to retire it.
3. **Retire** — remove *any* Library id from the public listing with **no replacement**, always keeping a My Tawala safety copy (`TawalaTransfer.retireLibraryEntry`; never hard-deleted):
   - **Stub** rows keep their `" (stub)"` name suffix + `stub: true` flag in the safety copy.
   - **Non-stub, unvetted** rows (owner Aug 1, 2026 — *"a lot of these"*: real entries catalogued without ever being deployed/verified) land in My Tawala with their **name left as-is** — no invented `" (stub)"` suffix — but are marked via `retiredFromLibraryId` / `retiredWasStub: false` plus a description note, so it's still traceable that the row came from a Library retirement. The table flags these rows with a quiet **"not vetted"** badge (any entry that is neither `stub` nor `liveReady`) as a cleanup hint. This is the recommended path for unvetted real entries still in the catalog: retire first (into My Tawala, safe), then open in Designer and re-Publish once it's actually checked. (**Sign-up Sheet w Email** was removed from the repo catalog instead of seeded into default My Tawala.)
   - **Restore to Library** (in the "Retired from Library" section) undoes a retirement — brings the original catalog row back — as long as nothing has since Published/overwritten that same id.
4. **Use as overwrite target ↑** on any row is a shortcut that scrolls to the Publish/overwrite form above with that row preselected as the target — it doesn't do anything by itself.

There is still **no public Library Delete button** for normal users — `library-admin.html` is the maintainer-only escape hatch, and even there, Retire always preserves a My Tawala copy rather than deleting.

**Smoke:** `library-admin.html` with admin mode off → gate screen only, no table. Enable → table lists every current Library entry (stub + non-stub, with “stub” / “live” / “not vetted” / “published overlay” badges) plus a Publish/overwrite mini-form and a Retired section (empty until something is retired). Publish/overwrite a project with the purge checkbox checked (default) → status line shows the purge outcome (deleted count, or a ⚠️ skipped/failed note) alongside the publish confirmation — same purge-on-Publish behavior as the My Tawala dialog. Rename a row → name updates on `library.html` after reload, **no purge call** (Rename never purges). Retire a stub → gone from `library.html`, appears in `mytawala.html` still named `"… (stub)"`, **no purge call** (Retire never purges). Retire a non-stub (e.g. a Main Menu template) → gone from `library.html`, appears in `mytawala.html` under its original name. Restore either → back in `library.html`.

## Aug 9 decisions (lists 1+4)

On Aug 7–8 we compared From Tony stills and the `legacy-reference/` lookalikes to the live mock and sorted features into four lists. On **Aug 9, 2026** the owner walked through lists **1** (Ask to evaluate) and **4** (Unsure — talk further) and settled the product meanings below. This section is the **canonical** write-up of those agreements, in plain English so a future reader (or a compacted chat) does not need the session shorthand.

Short pointers: `docs/CHAT_HANDOFF.md` Chat 3; `Tawala_Key_Documents/LEGACY_MEMO_TRIAGE.md` (B7). Outside-repo stills notes: `…/From Tony/FROM_TONY_IMAGES_NOTES.md`. Comparison canvas (not in git): `~/.cursor/projects/Users-DougC1-Projects-Tawala/canvases/legacy-vs-mock-pages.canvas.tsx`.

**What to do next:** execute the [Task List](#task-list-aug-9) (list-2 work first, in chunks). Do not re-debate the decisions below unless new evidence appears.

### Reference stills (committed under `legacy-reference/stills/`)

Tony Beta stills (source: From Tony `images/`):

![My Tawala Beta still](legacy-reference/stills/MyTawala-600px.png)

*My Tawala listing — `MyTawala-600px.png`*

![Project Details Beta still](legacy-reference/stills/ProjectDetails-600px.png)

*Project Details (Potluck) — `ProjectDetails-600px.png`*

**Library:** no full-page still. Legacy CTAs from asset GIFs:

![SEE DEMO](legacy-reference/stills/seedemo-button.gif) ![Test Drive Latest](legacy-reference/stills/testdrivelatest-button.gif) ![Download Latest](legacy-reference/stills/downloadlatest-button.gif) ![Customize](legacy-reference/stills/customize-button.gif)

Static lookalikes (served at `/legacy-reference/`; screenshots Aug 7):

![Lookalike My Tawala](legacy-reference/stills/lookalike-my-tawala.png)

*Lookalike — `legacy-my-tawala.html`*

![Lookalike Project Details](legacy-reference/stills/lookalike-project-details.png)

*Lookalike — `legacy-project-details.html`*

![Lookalike Library (reconstructed)](legacy-reference/stills/lookalike-library.png)

*Lookalike — `legacy-library.html` (reconstructed / uncertain)*

### Publish and the Library catalog

**Publish** means: take a project from private My Tawala and create a **new** entry in the one public Library.

**Update an existing Library app** means: put a **new version** onto a Library entry that already exists (the same catalog row keeps its identity; the published definition advances). That is the normal way to refresh something already in the Library — not a separate mystery product verb.

Drop the third Publish-dialog choice that asked something like “Update library with this version?” unless clear evidence of that third path resurfaces in legacy or real use. Two outcomes are enough: **new Library entry**, or **new version on an existing Library entry**.

On the **Library** surface, the actions people should see are:

- **Test Drive** — try the live app (see Test Drive below).
- **Save a copy** — acquire into the user’s private My Tawala (rename on the way in).

Park **SEE DEMO** until there are real demo videos to show. **Download latest** is deferred for a later conversation; the preferred direction is **Pull into Designer** (bring the definition into authoring) rather than “push a download from the website.” Do **not** add a Library **Customize** control that dumps the user into Designer from the catalog.

**Stars** after Library names may stay as a visual placeholder. **Comments** are a placeholder on the **Library detail** page only (not a My Tawala list column). A real reputation / ratings product is a **separate future project** — do not build gaming-prone ratings as part of this Website ops pass.

### Active and De-activate

**De-activate** (and the reverse, take Active again) is a useful **temporary take-down** while the author investigates a problem. It is not Purge and not Delete.

When a project is **Inactive**:

- It is **hidden from the public Library** — visitors cannot Test Drive it or Make / Save a copy from the catalog.
- It **remains on the author’s My Tawala**, with a clear Offline / Inactive marker so the author can reactivate it.

Who can flip the flag: the **author** for now (admin tooling later). The control lives on **Project Details**, not as a confusing listing-only mystery. Keep the language and UI distinct from **Purge** (wipe response data) and **Delete** (remove this My Tawala copy).

### Shared Data — parked

**Shared Data** is a Fleischauer-era idea with no usable spec in hand. Park it. Do not invent behavior for it in this phase.

### Invite collaborators — no ACL for now

We are **not** building real access-control / collaborator ACLs yet.

Instead, use **Project Details → Start points** as the place to help authors distribute the right URLs:

- Short **help text** explaining what each start point is for.
- A **Copy link** control so teachers / coaches can paste the URL to students or players.
- Optional **owner-chosen labels** on those start points (for example “participant” vs “admin”) — labels for humans, not a full permissions system.
- Putting a stable **uniqueId in the URL** is the coded stretch when we wire this for real.

**Include in Web Page** belongs in the same Start-points area: copy an **iframe / embed** snippet for the chosen start URL. A concrete use case is a Foundation teacher nomination form embedded on an external site.

### Access column — parked

The listing **Access** column (last-used style) is parked. **Updated / last modified** on the listing is less critical if **Versions** on Project Details already carry dated history.

### Records, clone count, and times used

**Records** (also called **Responses**) means the **project-wide count of submissions**. Show that number on the **My Tawala listing** and again on **Project Details**. Break it down **per form** on the **Project Data** section (not as a substitute for the project-wide total).

**Clone count** means how many times people used **Save a copy** (or equivalent) to pull this Library project into their own My Tawala — **copies of the app**, not rows of response data. Only meaningful once Save a copy exists for real.

**Times used** (proposed definition for when we wire it): count a **new respondent session that begins from a start URL**. Do **not** count Library Test Drive sessions. Do **not** count every click inside an already-open session. When wired, show **Times used** on Project Details; **Last used** is the timestamp of the most recent such session.

### Project Data and Purge

On **Project Details**, add a **Project Data** section with a **collapsible form list**. Collapse modes the owner wants available: closed; **starting points only**; **all forms**.

From that section the author **selects** either one form or the **entire project**, then uses a shared toolbar: **Export / Import / Purge**. We do not require mysterious per-row icons as the only way to act — selection plus toolbar is the model.

Build **whole-project Purge** first. **Per-form Purge** uses the same UI (Selective Purge). Sports commissioners may still need a “stats only” nuance later; that can refine Selective Purge without inventing a second product.

**Purge** and **Delete** are not the long-term focus of the My Tawala **listing** chrome. **Delete** means: remove this copy from My Tawala. **Purge** means: clear response data (for the selection). Keep those meanings separate from De-activate and from Backup/Restore.

### Designer entry points

Put **“Edit project in Designer”** on **Project Details only**, with that honest name. Do not use a vague **“Revise”** label that hides the fact that Designer is involved.

Put **nothing about Designer** on the My Tawala **listing**. A Home-sidebar link toward Designer can come later, and should sit **away from** the Library chrome so catalog browsing does not feel like an authoring trap.

### Make a Copy

On **Project Details**, **Make a Copy** forks the author’s **own** project without opening Designer: mint a **new uniqueId**, default to **empty response data**, and let the author **rename** (for example “Biology Mid-term 2026”). Renaming alone is **not** a new instance — only Make a Copy (or Library Save a copy) creates a new project identity.

Optional **copy-with-data** (with a clear confirm) can come later.

On the Library side, **Save a copy** acquires into My Tawala and should support **rename on acquire** the same way.

### Backup

Do **not** build Backup further until the package contents are decided. Default proposal on the table: Backup = **currently deployed definition + current response data**. Remember: **Restore** is not the same as **Deploy this version** (Restore re-applies a backup package; Deploy this version switches which definition revision is live on `:8080`).

### Test Drive

**Test Drive** belongs on the **Library**. When the visitor leaves the Test Drive, **purge** the demo responses. While they are in the drive, they may use **all start points** before that wipe (so multi-step apps like Exam + Admin Setup still work during the session).

---

### Earlier triage lists (Aug 7–8 — historical)

These four lists framed the Aug 9 walkthrough. Lists **1** and **4** are now decided above; list **2** is the execution backlog in the Task List; list **3** stays deprioritized.

1. **Ask to evaluate** *(discussed Aug 9 — decisions above)* — Publish vs update Library; Active / De-activate; Shared Data; Invite / Include in Web Page; SEE DEMO vs Test Drive vs Download; Access column; Times used; Customize; Comments; Selective Purge by form.
2. **Prioritize and execute** — Lean My Tawala list; decision ops on Project Details; Records/Responses; Theme / Appearance; per-form Project Data; Published Yes + link; author / version / description rail; Versions (keep Deploy this version); Library blurb + clearer acquire; Make a Copy when ready.
3. **Deprioritize / skip for now** — Full Beta nav peers (Forum / News / Admin …); Library version history on the public site; project icon tiles; dense ops icons forever on the listing; Flash SEE DEMO until videos exist; full ratings as a forever display-only product (reputation = separate future project per Aug 9).
4. **Unsure — talk further** *(discussed Aug 9 — decisions above)* — Revise vs Edit in Designer; Comments UX; E/I/B/R / Backup meanings; Active/Offline vs Purge; Use vs Library Download; Project Data chrome; Access column; ratings solicit design.

### Synopsis — Aug 7 (implementation day) and Aug 9 (decision day)

- **Aug 7:** **Deploy this version** wired (definition snapshots on Deploy → Show in My Tawala; radios + button switch live `:8080`). Tony stills / `FROM_TONY_IMAGES_NOTES.md`; `legacy-reference/` HTML; comparison canvas; stills under `legacy-reference/stills/`. Initial feature triage into the four lists; **Responses** already clarified as project-wide submission count.
- **Aug 9:** Walked lists 1+4 to agreement. Publish = new Library entry; update existing = new version on that entry; drop the third “update library with this version?” prompt unless evidence returns. Library CTAs = Test Drive + Save a copy. Active/De-activate = temporary take-down (not Purge/Delete). Invite = Start-point help + Copy link + optional labels + Include-in-Web-Page embed (no ACL). Records on list + Details; clones = Library copies; Times used = new respondent sessions from start URLs. Project Data selection + Export/Import/Purge toolbar; whole-project Purge first. Designer only from Details as “Edit project in Designer.” Make a Copy = own fork with new uniqueId. Backup parked pending package decision. Test Drive = Library, purge on leave, all start points allowed first.

### Task List (Aug 9)

Ordered work for the next Website sessions. **Do list-2-shaped chunks first.** Items marked **HOLD** wait on a decision or asset; do not half-build them.

1. **Lean the My Tawala listing** — Prefer a calmer list; move heavy decision ops (Publish lifecycle, Purge / Delete emphasis, Export / Import / Backup / Restore placement) onto **Project Details** so the listing is not a dense icon strip forever.
2. **Project Details ops rail** — Group Publish / update-Library, Active/De-activate, Purge, Delete, and data tools where the author actually decides. Keep Purge ≠ Delete ≠ De-activate visually and in copy.
3. **Records (Responses) count** — Show project-wide submission count on the My Tawala list and on Project Details. Add per-form counts inside Project Data when that section lands.
4. **Project Data section** — Collapsible form list (closed / starting points only / all forms). Select form or entire project → shared **Export / Import / Purge** toolbar. Ship **whole-project Purge** in this UI first; then Selective (per-form) Purge in the same pattern. **HOLD nuance:** sports “stats-only” Selective Purge after commissioner feedback if needed.
5. **Theme / Appearance** — Surface when it fits the Details rail; do not block Records or Project Data on pixel-perfect theme chrome.
6. **Published indicator** — On Details: clear Yes (or equivalent) plus a link to the Library entry when published.
7. **Author / version / description rail** — Show who owns it, current version number, and description beside Versions (keep **Deploy this version**; richer version download can wait).
8. **Library listing / detail acquire clarity** — Blurb under the name; CTAs = **Test Drive** + **Save a copy** only. Rename on Save a copy. No Customize→Designer dump. No SEE DEMO until videos exist.
9. **Make a Copy (own project)** — On Project Details: fork with new uniqueId, empty data by default, rename in the flow. Wire **Library Save a copy** rename-on-acquire alongside. **HOLD:** optional copy-with-data (confirm) until the empty-data path is solid.
10. **Start points: distribute + embed** — Help text, Copy link, optional participant/admin-style labels, and **Include in Web Page** (iframe/embed for the chosen start URL). **HOLD coded stretch:** uniqueId-in-URL hardening as part of real wiring.
11. **Edit project in Designer** — Details-only control with that exact honest label. Remove or avoid “Revise” that conceals Designer. No Designer column or icon on the My Tawala listing. **HOLD:** Home sidebar Designer link later, placed away from Library.
12. **Active / De-activate** — Author control on Details; Inactive hides from public Library (no Test Drive / Save a copy) but stays on My Tawala with Offline/Inactive marker.
13. **Times used / Last used / Clone count** — Wire when Save a copy and session accounting exist; use the Aug 9 definitions (clones ≠ data rows; times used ≠ Test Drive).
14. **Test Drive contract** — Keep Library-owned; allow all start points during the drive; purge when the visitor leaves (document and implement the leave/wipe path honestly in the mock’s limits).
15. **HOLD — Backup package** — Do not expand Backup/Restore until the owner confirms the default proposal (deployed definition + current responses) and how Restore differs from Deploy this version in the UI copy.
16. **HOLD — Download latest** — Talk later; prefer Pull into Designer over site Push/Download.
17. **HOLD / park — Shared Data, Access column, SEE DEMO videos, reputation/ratings product, ACL collaborators** — No build in this phase.
18. **Ongoing — Library stub cleanup** — Separate catalog work: Designer → Deploy → Publish a real replacement, then retire the stub (see § Library stubs).

## Parked / backlog (owner Aug 1, 2026) — look & feel / organization phase

Worth doing, but **not initial wiring**. These are **look & feel / organization** ideas for a later phase — same Website priority as above: **library / catalog ops first**; L&F parked. Flat My Tawala **listing** (one row per project) stays current. Details-only Versions history (first slice Aug 5, 2026) does **not** lift the listing hold — see § Save/Deploy/Publish **Sequencing / hold**. Folder UX / listing piles come with the later L&F/organization pass (delete-version, audit; Deploy-switch wired Aug 7). **Owner decisions (Aug 9):** see § [Aug 9 decisions (lists 1+4)](#aug-9-decisions-lists-14) above (canonical). Task execution order is in that section’s Task List.

1. **My Tawala listing column groups — DONE (Aug 5, 2026)** — Implemented on the flat listing (one row per project — no version piles). Visual groups via thick left rules + padding: **Project info** (Name, Created, Updated, Use) · **Data transfer** (Export, Import) · **Backup** (Backup, Restore) · **Destructive** (Purge, Delete — solid red icon buttons, not a thin outline) · **Library transfer** (Publish, Pull). Code: `js/project-ops.js` (`group` / `groupStart` / `destructive` on `LISTING_ACTIONS`), `css/tawala-chrome.css`, `mytawala.html`. Still optional later polish from earlier L&F notes: sticky/frozen icon-column headings; more distinct glyphs within a group. Not needed for ops wiring. **Aug 9:** prefer **leaner** listing long-term (ops on Details) — see Task List items 1–2.
2. **User-controlled folders** — group (and hide) groups or versions of projects; drag-and-drop so the user can keep projects they’re working on at the top. Organization UX — after ops are solid.
3. **Complete-private list** — a separate list for a user’s projects that are **complete but not intended for the public Library** (private archive / complete-private vs My Tawala working set vs Library Publish). Organization UX — after ops are solid.
4. **Season / roster carry-forward** — sports leagues annually asked how to move prior-year rosters (owner Aug 1, 2026 — quote under § Library Actions / Use framing). Historical practice: **Excel Export → archive**, then **reuse player subset only** (drop graduates, add new kids) via selective Import — not Backup/Restore-everything, not Library re-pull. Soft open: “roll season” ≈ export archive + import filtered roster on the EXPORT/IMPORT spine. Backlog signal for SDT / season handoff; not Customize vs Copy. No feature designed here.
5. **Library quality / community** — pro vs amateur/community Publish mix; reviews/ratings as promotion vs gameable reputation; Designer ease as prerequisite for contribution (YouTube “easy to load” analogy). Open questions only — see § Library quality / community above. **Aug 9:** stars/comments stay placeholders; reputation/ratings = separate future project — do not build here.
6. **Versions / Deploy this version (wired Aug 7, 2026)** — radios + **Deploy this version** switch snapshot-backed rows; delete-version still grey; Download still metadata-only; Restore ≠ make-definition-version-current. Documented under glossary **Owner smoke Aug 7**. Still easy to get in trouble with response/schema mismatch — see smoke steps.
7. **Selective Purge by form** — Aug 7 park asked commissioners; **Aug 9:** same UI as whole-project Purge (select form → Purge toolbar). Build whole-project first, then per-form in that UI; sports stats-only nuance may refine later. No longer “never build” — it is Task List item 4 after whole-project Purge.

## Owner checklist — product ready for Library (Deploy → Live)

This is also the checklist for **replacing a stub** (§ Library stubs above) — run it for the stub's equivalent working copy, then ask an agent to retire the old `" (stub)"` entry once the new one is live.

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
