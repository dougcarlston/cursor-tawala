# Website mock

Static rough draft of the legacy Tawala site (home, Library, project detail, My Tawala), derived from JSP under `TawalaWebapp-build1700/web/WEB-INF/jsp/`.

**Site CSS (July 2026):** Legacy styles copied into `css/legacy/` from owner archives (`tawala-base.css`, `pages/homepage.css`, `pages/library.css`). Template images under `images/`. Mock-only chrome (banner, pending links, test-drive boxes) in `css/tawala-chrome.css`. Stub pages still use the older all-in-one `css/tawala-mock.css`.

## View locally

Serve **from `website-mock/`** (not the repo root). If the server’s cwd is the repo, `http://localhost:5500/library.html` returns **404** (only `/website-mock/library.html` would work).

```bash
cd website-mock
./serve.sh
# or: python3 -m http.server 5500 --directory .
```

Open:
- Home: http://localhost:5500/
- **Library:** http://localhost:5500/library.html
- My Tawala: http://localhost:5500/mytawala.html

**Prereq:** Java runtime on http://localhost:8080 with templates deployed. Test-drive links read from `js/demo-urls.js`.

## Library vs My Tawala (split piles)

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
| Designer **Project → Project Manager…** or **Help → Website mock (My Tawala)…** | `http://localhost:5500/mytawala.html` |
| Designer **Help → Website mock (Library)…** | `http://localhost:5500/library.html` |
| Designer **Deploy** dialog → **Show in My Tawala** | `mytawala.html?deployReceipt=…` (inbox on :5500; not the pile) |

Test-drive start points stay on `:8080` via `js/demo-urls.js`.

### Transfer flows (scaffold)

Shared helpers: `js/transfer.js` (localStorage on `:5500` only — Designer `:5173` does not share storage).

| Hop | UI | Status |
|-----|-----|--------|
| **Web Designer → My Tawala** | Deploy dialog **Show in My Tawala**; My Tawala **From Web Designer** inbox | Wired (receipt inbox + :8080 links). Does **not** auto-add to `TAWALA_MYTAWALA` pile yet. |
| **Library → My Tawala** | Listing / detail **Save this project under My Tawala**, **USE IT** (legacy customize entry; was mislabeled “Clone”) | Grey stubs. |
| **My Tawala → Library** | Listing **Publish**, detail **PUBLISH**, sidebar **Publish to Library** | Grey stubs. |
| **Library ← My Tawala upgrade** | Listing **Pull**, detail **PULL FROM LIBRARY**, sidebar **Pull from Library** | Grey stubs. |
| **Library category assignment** | **Edit Categories** (submenu + admin bar); detail sidebar **Group** select | Wired locally — overrides in `localStorage` until copied into `demo-urls.js`. |

Grey controls use Designer accent palette (`--tw-accent`); disabled = opacity only.
## My Tawala layout (lean) vs ops archive

Legacy Project Manager lives under `TawalaWebapp-build1700/web/WEB-INF/jsp/projectmanager/` (not the thin `mytawala/*.jsp` news pages). Mock mirrors the shallow structure:

1. **`mytawala.html`** — My Projects listing (name, created, updated + Export…Publish icon columns). Click headings to sort; drag header edges to resize. Row **Delete** confirms; **Purge** uses local cleanup hint. Click project name → Project Details. Sub-menu: **My Projects · My Account · Change Password**.
2. **`mytawala-project.html?project=…`** — Project Details: **EXPORT…PUBLISH** action bar, left sidebar (REVISE / ONLINE-OFFLINE / Include / Invite), collapsible sections (Start points, Project Data, Versions, Backups…). Start-point test-drives stay on `:8080`.
3. **`project-ops-review.html`** — full recovered label catalog split by surface (**Public Library** vs **My Tawala / Project Manager**) for memory / archive review. Linked from My Tawala sidebar; not stacked on the working pages.

Labels live in `js/project-ops.js`. Inactive Project Manager ops are **disabled** (grey only — no “not wired” text). Active ops (**PURGE**, **DELETE**) use Designer-accent blue styling from `css/tawala-chrome.css` (`--tw-accent` mirrors `designer-web/src/styles.css`). **PURGE** confirms, then POSTs `http://localhost:3001/api/purge-responses` with the project’s `:8080` **uniqueId** (from `testDriveUrl` / start points). That deletes all `submission` rows for that `user_project.unique_random_id` in Docker Postgres — same effect as Java Project Manager `purgeProjectResponses`. Credentials: **`dev` / `dev`**.

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

Keep Phase 2 `:8080` URLs only (no www.tawala.com).

- **Home / Library / library-detail** → `TawalaDemo.libraryEntries()` / `getLibrary()`
- **My Tawala / mytawala-project** → `TawalaDemo.myTawalaEntries()` / `getMyTawala()`
