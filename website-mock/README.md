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

## Update deploy URLs

After redeploying a template:

```bash
node scripts/deploy-tawala-template.mjs "Simple Survey Template"
node scripts/deploy-tawala-template.mjs "Sign-up Sheet"
```

Edit `js/demo-urls.js` with the new start-point URLs from the script output.
