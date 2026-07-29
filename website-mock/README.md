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
| Project detail | `library-detail.html?project=…` |
| My Tawala | `mytawala.html` |
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

## My Tawala project operations (archive labels)

Legacy Project Manager lives under `TawalaWebapp-build1700/web/WEB-INF/jsp/projectmanager/` (not the thin `mytawala/*.jsp` news pages).

**Project Actions** bar (`detail.jsp`): **EXPORT · IMPORT · BACKUP · RESTORE · PURGE · DELETE · PUBLISH**

Listing icons (`view.jsp`): **Purge** / **Delete**. Sub-menu (`submenu-mytawala.jsp`): **My Projects · My Account · Change Password**.

Also recovered (sidebar / data / versions / backups): REVISE PROJECT, TAKE PROJECT ONLINE/OFFLINE, Include Project in Web Page, Invite…, form-level Export/Import/Purge, Deploy/Delete version, backup schedule ops, DELETE ALL PROJECT EMAILS, publish-to-library dialogs. Save/Clone labels are on customization & Library flows (`Save this project under My Tawala`, `DEPLOY TO MY TAWALA`, Clone and Customize) — not on the Project Actions bar. No exact **Rename** label found there.

Mock UI: `mytawala.html` + `js/project-ops.js` surfaces the full label lists (stubs tagged **not wired**). **PURGE** shows the legacy confirm, then points at local DirtBowl Registration cleanup only:

```bash
./scripts/dev-data.sh cleanup-registrations
```

Full `purgeProjectResponses` is Java Project Manager / DB — not on designer-web `:3001` or this static mock.

## Update deploy URLs

After redeploying a template:

```bash
node scripts/deploy-tawala-template.mjs "Simple Survey Template"
node scripts/deploy-tawala-template.mjs "Sign-up Sheet"
```

Edit `js/demo-urls.js` with the new start-point URLs from the script output. Home, Library, detail, and My Tawala all read that file (via `TawalaDemo` helpers) — keep Phase 2 `:8080` URLs only.
