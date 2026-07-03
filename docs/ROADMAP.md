# AI-Tawala — Roadmap

*Living plan — updated June 2026. Owner reviews checkboxes and demo URLs.*

Three parallel tracks (see `.cursor/rules/tawala-work-scopes.mdc`):

| Track | Folder / output | Owner role |
|-------|-----------------|------------|
| **1. Legacy Designer spec** | `Tawala_Key_Documents/DESIGNER_*.md` | Screenshots, hover text, click-tests |
| **2. Template deploy → 8080** | `scripts/deploy-tawala-template.mjs` | Open URLs, report breakage |
| **3. Website mock** | `website-mock/` | Visual approval |

---

## Phase 1 — Spec closure

**Goal:** Designer shell documented well enough to implement without guessing.

| Task | Owner | Agent | Status |
|------|-------|-------|--------|
| Heading form item | Owner screenshots | `DESIGNER_FORM_ITEMS_HEADING.md` | Done |
| Form vs Document format toolbar | Owner Styles screenshots | `DESIGNER_FORM_FORMAT_TOOLBAR.md` | Done |
| Template matrix | — | `DESIGNER_TEMPLATE_MATRIX.md` | Done |
| Preview tab walkthrough | Owner confirmed error only | `DESIGNER_STARTUP_AND_FORM_CANVAS.md` | **Done** |
| Page Header dialog | Owner screenshot | `DESIGNER_PAGE_HEADER.md` | Done |
| Form Format → Styles dialogs | Screenshots | `DESIGNER_FORM_FORMAT_TOOLBAR.md` | **Done** (owner June 2026) |
| Trim `DESIGNER_UI_REFERENCE.md` open items | — | Mark historical | Pending |
| Per-function Configure dialogs | When implementing runtime | Deferred | — |

---

## Phase 2 — Template deploy (8080)

**Goal:** Legacy `.tawala` XML → Tomcat `/client` → working start URLs.

**Prereq:** `docker compose up` or Tomcat on `http://localhost:8080`, user `dev` / `dev`.

```bash
# List templates
node scripts/deploy-tawala-template.mjs --list

# Deploy one
node scripts/deploy-tawala-template.mjs "Simple Survey Template"
```

| Template | Deploy | Start URL | Smoke test | Status |
|----------|--------|-----------|------------|--------|
| Simple Survey Template | OK | See matrix | Submit MCQ → Report tallies | **Passed** (owner, June 2026) |
| Signup Sheet Template | OK | See matrix | FIB submit → Thank You → back → table grows | **Passed** (owner, July 2026) |
| Form with Process | OK | See matrix | Blank form (Designer demo — not a sample app) | **Passed** (owner, July 2026) |
| Form with process connecting a document | OK | See matrix | Submit → empty **Document 1** shell | **Passed** (owner, July 2026) — empty doc by design; spinner gif patched |
| Signup Sheet Template w Email | OK | See matrix | FIB + table OK; **Send** needs SMTP | **Blocked** — outbound mail deferred (see Backlog) |
| Get Together Template | OK | See matrix | Survey → Report correlation table | **Passed w/ caveats** (owner, July 2026) |
| Multiple Question Survey Template | OK | See matrix | Multi MCQ → Report tallies + table | **Passed** (owner, July 2026) |
| Potluck Template | OK | See matrix | Organizer → Details + thank-you doc | **Passed w/ caveats** (owner, July 2026) |
| Empty Project | — | — | N/A | — |

Matrix detail: `Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md`.

---

## Phase 3 — Website mock

**Goal:** Browseable rough draft of tawala.com / MyTawala / Library from legacy JSP/CSS.

**Sources:** `TawalaWebapp-build1700/web/WEB-INF/jsp/` (mytawala, library, community).

| Page | Status |
|------|--------|
| Home / landing | **Draft** — `website-mock/index.html` |
| Project Library search | **Draft** — `website-mock/library.html` |
| Project detail | **Draft** — `website-mock/library-detail.html` |
| MyTawala dashboard | **Draft** — `website-mock/mytawala.html` |
| Link “Test drive” → Phase 2 URLs | **Wired** — `website-mock/js/demo-urls.js` |
| Stub pages (About, FAQ, Login, …) | **Draft** — `about.html`, `faq.html`, `login.html`, `signup.html`, `terms.html`, `privacy.html`, `designer.html`, `logout.html` |
| Pending links greyed out | **Wired** — `js/chrome.js` (`link-pending`) |
| Legacy site CSS | **Imported** — `css/legacy/tawala-base.css`, `pages/homepage.css`, `pages/library.css` |
| Template images | **Copied** — `images/template/` from build1700 |

Serve: `cd website-mock && python3 -m http.server 5500` → http://localhost:5500/

**Owner (July 2026):** All mock pages load; test-drive / library / My Tawala links OK. Stub pages (About, FAQ, Login, …) are placeholders. Home/Library now use legacy CSS + white logo in header. **Jobs** removed from footer (no longer relevant).

---

## Phase 4 — Browser Designer (`designer-web`)

**Active (July 2026).** Shell, deploy, and DirtBowl sample JSON exist; **New Project** starters aligned to home-page featured apps.

| Milestone | Status |
|-----------|--------|
| File → New Project… template picker | **Done** — featured 4 + Basic templates |
| JSON starters in `public/samples/templates/` | **Done** |
| Deploy dialog — project-scoped URLs only | **Done** — `server/deployParse.mjs` filters Java response; **restart `npm run dev`** after pull |
| Deploy featured templates to 8080 | **Simple Survey passed** (owner, July 2026) — **Sign-up Sheet** next |
| DirtBowl off File menu | **Done** — open via **Open Project…** or `deploy-dirtbowl-java.mjs` |
| DirtBowl → website Library | Backlog — then no “master menu” of all dev-user start points |
| Full Potluck / email templates | Deferred |
| `.tawala` → JSON import | Backlog |
| FIB canvas layout (mix text + blanks freely) | Backlog — owner July 2026; current block layout is MVP |
| Fields palette drag into text/docs/process | Partial — drag source wired; drop targets backlog |

**Prerequisites before Process editing (owner July 2026)** — not needed for Sign-up Sheet yet, but **required before** serious Process work:

| Item | Priority | Notes |
|------|----------|--------|
| **Insertion-point arrow** (Form / Process / Document) | **Blocker for Processes** | Legacy blue arrow marks where the next insert lands; replicate before Process editor work. Defer during Sign-up Sheet pass only. |
| **Move Up / Move Down** (form items, process statements, document blocks) | **Blocker for Processes** | Essential once a script or form has more than a few lines; legacy Designer had this throughout. Defer during Sign-up Sheet pass only. |

Run: `cd designer-web && npm run dev` → http://localhost:5173 — **File → New Project…**

For Java deploy URLs: `TAWALA_JAVA_URL=http://localhost:8080 npm run dev` (see `designer-web/README.md`).

See `designer-web/README.md` for MVP scope and runtime gaps.

---

## Backlog — deferred (do not lose)

| Item | Context | Notes |
|------|---------|--------|
| **Outbound email (8080)** | Sign-up Sheet w Email template; any `Send` process | Legacy stack already uses Spring **`JavaMailSender`** (`WEB-INF/notification-config.xml`, `mail.properties` → `mail.host`). Docker has no SMTP today. **Owner (July 2026):** prefer extending **JavaMailSender** (real SMTP host / relay) over a Resend-specific adapter — but **decision deferred**; revisit in a dedicated session. Owner also has a Resend account if we compare options later. Unblocks: Sign-up Sheet w Email end-to-end smoke test. |

---

## How to oversee

1. **One active phase per session** — say which track in your first message.
2. **Check this file** for status and **demo URLs** after each milestone.
3. **Gate reviews:** spec sign-off → first template URL → website wireframe.
4. Agent reports: *what changed · URL to try · what I need from you*.

**Chat workflow:** Paste openers and track names from [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md). Cursor UI, context ring, rename/switch chats, billing: [`CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md).

---

*Last updated: July 2026.*
