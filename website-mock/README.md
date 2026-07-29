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
- Library **listing** is dense (name + stars inline / comments / updated / row actions); descriptions live on the detail page.
- Categories are **collapsible groups** mirroring Designer **File → New Project** (Activities, Meetings and Gatherings, Polls and Surveys) plus WebLibrary extras (Sports, Business, Advanced). Designer-only **Basic** templates are never listed.
- Projects without a live `:8080` deploy show **Not deployed** (catalog accuracy first). Listing titles never show a `.json` extension — on-disk backups may still be JSON; display uses the project name only.

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

Test-drive start points stay on `:8080` via `js/demo-urls.js`.

## My Tawala layout (lean) vs ops archive

Legacy Project Manager lives under `TawalaWebapp-build1700/web/WEB-INF/jsp/projectmanager/` (not the thin `mytawala/*.jsp` news pages). Mock mirrors the shallow structure:

1. **`mytawala.html`** — My Projects listing only (name, created/updated, responses, **Access** = last-accessed date — not Active/Inactive; row **Purge** / **Delete**). Click project name → Project Details. Sub-menu: **My Projects · My Account · Change Password**.
2. **`mytawala-project.html?project=…`** — Project Details: **EXPORT…PUBLISH** action bar, left sidebar (REVISE / ONLINE-OFFLINE / Include / Invite), collapsible sections (Start points, Project Data, Versions, Backups…). Start-point test-drives stay on `:8080`.
3. **`project-ops-review.html`** — full recovered label catalog split by surface (**Public Library** vs **My Tawala / Project Manager**) for memory / archive review. Linked from My Tawala sidebar; not stacked on the working pages.

Labels live in `js/project-ops.js`. Inactive Project Manager ops are **disabled** (grey only — no “not wired” text). Active ops (e.g. **PURGE**) use Designer-accent blue button styling from `css/tawala-chrome.css` (`--tw-accent` mirrors `designer-web/src/styles.css`). **PURGE** shows the legacy confirm, then points at local DirtBowl Registration cleanup only:

```bash
./scripts/dev-data.sh cleanup-registrations
```

Full `purgeProjectResponses` is Java Project Manager / DB — not on designer-web `:3001` or this static mock. SportsDashboards spelling (not SportsBoard).

## Update deploy URLs

After redeploying a template:

```bash
node scripts/deploy-tawala-template.mjs "Simple Survey Template"
node scripts/deploy-tawala-template.mjs "Sign-up Sheet"
```

Edit `js/demo-urls.js` with the new start-point URLs from the script output. Set `deployed: true` and `testDriveUrl` / `startPoints[].url` for live deploys; leave `deployed: false` when there is no local test-drive yet (`jsonFile` stays as an internal path for maintainers — not shown in Library/My Tawala UI).

- **Home / Library / library-detail** → `TawalaDemo.libraryEntries()` / `getLibrary()`
- **My Tawala / mytawala-project** → `TawalaDemo.myTawalaEntries()` / `getMyTawala()`

Keep Phase 2 `:8080` URLs only (no www.tawala.com).
