# Website mock

Static rough draft of the legacy Tawala site (home, Library, project detail, My Tawala), derived from JSP under `TawalaWebapp-build1700/web/WEB-INF/jsp/`.

**Site CSS (July 2026):** Legacy styles copied into `css/legacy/` from owner archives (`tawala-base.css`, `pages/homepage.css`, `pages/library.css`). Template images under `images/`. Mock-only chrome (banner, pending links, test-drive boxes) in `css/tawala-chrome.css`. Stub pages still use the older all-in-one `css/tawala-mock.css`.

## View locally

```bash
cd website-mock
python3 -m http.server 5500
```

Open http://localhost:5500/

**Prereq:** Java runtime on http://localhost:8080 with templates deployed. Test-drive links read from `js/demo-urls.js`.

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

1. **`mytawala.html`** — My Projects listing only (name, dates, responses, access; row **Purge** / **Delete**). Click project name → Project Details. Sub-menu: **My Projects · My Account · Change Password**.
2. **`mytawala-project.html?project=…`** — Project Details: **EXPORT…PUBLISH** action bar, left sidebar (REVISE / ONLINE-OFFLINE / Include / Invite), collapsible sections (Start points, Project Data, Versions, Backups…). Start-point test-drives stay on `:8080`.
3. **`project-ops-review.html`** — full recovered label catalog (listing vs detail bar vs sidebar vs sections vs Save/Clone wizards) for memory / archive review. Linked from My Tawala sidebar; not stacked on the working pages.

Labels live in `js/project-ops.js`. Most ops are stubs (**not wired**). **PURGE** shows the legacy confirm, then points at local DirtBowl Registration cleanup only:

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

Edit `js/demo-urls.js` with the new start-point URLs from the script output. Home, Library, detail, and My Tawala all read that file (via `TawalaDemo` helpers) — keep Phase 2 `:8080` URLs only.
