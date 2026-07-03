# Chat handoff — three parallel tracks

Use this file when starting or resuming a Cursor chat. Copy the **5-line opener** for the track you are working on, paste it as your first message, and rename the chat to match the suggested title.

Status dashboard: [`docs/ROADMAP.md`](ROADMAP.md). Cursor usage tips: [`docs/CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md).

---

## Chat 1 — Browser Designer (`designer-web/`)

**Suggested title:** `Designer — Sign-up Sheet & Phase 4`

### 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — designer-web/ (Phase 4)
Goal: Continue Sign-up Sheet template in browser Designer; deploy smoke test on 8080
Read first: docs/ROADMAP.md Phase 4, designer-web/README.md, Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md
Constraints: Do not mix 8080 CSS/docker or website-mock work in this chat; preview/deploy local only (5173/3001/8080, not www.tawala.com)
```

### Work to date

- Browser Designer shell running at http://localhost:5173 (`cd designer-web && npm run dev`).
- **File → New Project…** template picker with featured starters in `public/samples/templates/`.
- **Simple Survey** deploy smoke test **passed** (owner, July 2026).
- **Sign-up Sheet** in progress — next featured template to exercise FIB + itemization table.
- Deploy dialog filters Java response to project-scoped URLs (`server/deployParse.mjs`); restart dev server after pull.
- Preview/runtime fixes landed in `server/` and `src/api/`.
- **Not built yet (blockers for Process work, deferred during Sign-up Sheet pass):**
  - Insertion-point arrow (Form / Process / Document)
  - Move Up / Move Down for form items, process statements, document blocks
  - Full Fields palette drop targets; `.tawala` import; Potluck / email templates
- Repo checkpoint pushed: `897ac8a` — *Checkpoint browser Designer, deploy pipeline, and legacy reference assets.*

### Key files

| Area | Path |
|------|------|
| UI shell | `designer-web/src/App.tsx`, `MenuBar.tsx`, `ToolBar.tsx`, `ProjectExplorer.tsx` |
| Form editing | `designer-web/src/components/FormEditor.tsx`, `FormInsertMenu.tsx`, `FormItemProperties.tsx` |
| State | `designer-web/src/store/projectStore.ts`, `types/tawala.ts` |
| Preview / API | `designer-web/src/api/preview.ts`, `server/runtime.mjs`, `server/fibToXml.mjs` |
| Deploy | `designer-web/src/components/DeployDialog.tsx`, `server/deployParse.mjs`, `src/api/deploy.ts` |
| Templates | `designer-web/src/templates/catalog.ts`, `public/samples/templates/*.json` |
| Specs | `Tawala_Key_Documents/DESIGNER_*.md`, `DESIGNER_TEMPLATE_MATRIX.md` |

### Immediate phases ahead

1. **Sign-up Sheet** — open template in Designer, deploy to 8080, owner smoke test (FIB submit → Thank You → back → table grows).
2. **UX feedback** — canvas layout, inspector, deploy dialog after owner tries Sign-up Sheet.
3. **Get Together** — next featured template after Sign-up Sheet passes.
4. **Insertion-point + Move Up/Down** — required before serious Process editing (see ROADMAP Phase 4 prerequisites).
5. **Backlog** — DirtBowl → website Library link, FIB free-mix layout, `.tawala` import, outbound email (separate session).

---

## Chat 2 — Template deploy & `:8080`

**Suggested title:** `8080 — templates, Docker, Tomcat CSS`

### 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Template deploy → Tomcat 8080 (Phase 2)
Goal: Deploy/smoke-test templates; fix runtime CSS or deploy scripts when URLs break
Read first: docs/ROADMAP.md Phase 2, Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md, docker/tomcat/README.md
Constraints: Do not refactor designer-web UI or website-mock in this chat unless deploy URL wiring requires it
```

### Work to date

- Deploy script: `scripts/deploy-tawala-template.mjs` (`--list`, deploy by template name).
- Template smoke tests documented in ROADMAP — Simple Survey, Sign-up Sheet, Form with Process, Get Together, Potluck, Multiple Question Survey passed with noted caveats.
- **Sign-up Sheet w Email** blocked on outbound SMTP (backlog).
- Tomcat CSS under `docker/tomcat/css/project/` — themes per template (e.g. `baseball/`, `greentea/`, `dirtbowl2/`).
- **Docker rebuild note:** after CSS or `Dockerfile` changes, rebuild the Tomcat image (`docker compose build` / see `docker/tomcat/README.md`) before re-testing 8080 URLs.
- DirtBowl deploy helper: `scripts/deploy-dirtbowl-java.mjs`.
- Prereq: Tomcat on http://localhost:8080, user `dev` / `dev`.

### Key files

| Area | Path |
|------|------|
| Deploy script | `scripts/deploy-tawala-template.mjs` |
| DirtBowl deploy | `scripts/deploy-dirtbowl-java.mjs` |
| Registration test | `scripts/test-registration-flow.mjs` |
| Docker / Tomcat | `docker/tomcat/Dockerfile`, `docker/tomcat/README.md`, `entrypoint.sh` |
| Project CSS | `docker/tomcat/css/project/**` |
| Template matrix | `Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md` |
| Legacy XML sources | `TawalaDesigner/` sample `.tawala` files (referenced in matrix) |
| Registration XML helpers | `designer-web/server/registration*.mjs` (8080 parity only) |

### Immediate phases ahead

1. Re-run smoke tests when deploy pipeline or CSS changes.
2. **Sign-up Sheet w Email** — dedicated session when SMTP decision is made (JavaMailSender vs relay).
3. Theme/CSS gaps — owner reports visual breakage on specific template URLs.
4. Docker image refresh after CSS edits.
5. Registration / DirtBowl runtime parity (paused unless owner reopens Page 2+, Review headers, RegStep2).

---

## Chat 3 — Website mock

**Suggested title:** `Website — library mock & test-drive links`

### 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Website mock — website-mock/ (Phase 3)
Goal: Polish library/home pages; keep test-drive links pointed at live 8080 template URLs
Read first: docs/ROADMAP.md Phase 3, website-mock/README.md, website-mock/js/demo-urls.js
Constraints: Do not change designer-web or Tomcat deploy logic here; link to Phase 2 URLs only
```

### Work to date

- Static mock at http://localhost:5500 (`cd website-mock && python3 -m http.server 5500`).
- Draft pages: home, library, library detail, MyTawala, About/FAQ/Login/signup/terms/privacy/designer stubs.
- **Test drive** links wired via `website-mock/js/demo-urls.js` → Phase 2 deploy URLs.
- Legacy CSS imported: `css/legacy/tawala-base.css`, `pages/homepage.css`, `pages/library.css`.
- Chrome helpers: `js/chrome.js` (pending links greyed with `link-pending`).
- Template images copied from build1700; Jobs removed from footer (owner, July 2026).
- Owner confirmed all mock pages load; test-drive / library / My Tawala links OK.

### Key files

| Area | Path |
|------|------|
| Entry / home | `website-mock/index.html` |
| Library | `website-mock/library.html`, `library-detail.html` |
| Dashboard | `website-mock/mytawala.html` |
| Test-drive URLs | `website-mock/js/demo-urls.js` |
| Shared chrome | `website-mock/js/chrome.js`, `css/tawala-chrome.css` |
| Legacy styles | `website-mock/css/legacy/**` |
| Stub pages | `website-mock/about.html`, `faq.html`, `login.html`, … |
| README | `website-mock/README.md` |

### Immediate phases ahead

1. Owner visual review — home vs library layout, logo, typography.
2. Keep `demo-urls.js` in sync when template deploy names or paths change on 8080.
3. Flesh out stub pages (About, FAQ, Login) when copy is ready.
4. Wire **Designer** marketing page when browser Designer is demo-ready.
5. DirtBowl → Library link (backlog — depends on Designer/deploy track).

---

## What to do with the three chats

| Action | Which chat |
|--------|------------|
| **Continue this chat** | Designer (Chat 1) — active Phase 4 work |
| **Focus first** | Designer — Sign-up Sheet deploy + UX |
| **Park** | 8080 (Chat 2) and Website (Chat 3) — resume when deploy breaks or mock needs polish |

### Practical split

1. **One active chat at a time** — say which track in your first message (use the opener above).
2. **Rename each chat** to the suggested title so the Agents sidebar stays scannable.
3. **Parked chats** — leave them idle; do not paste unrelated work into the Designer chat.
4. When you **park** a track, add a one-line note under the relevant ROADMAP phase (*"Paused — resume in 8080 chat when …"*) so status stays honest.
5. Need context from an old chat? Use **@Past Chats** in the new message (see [`CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md)).

---

## Multitasking strategy

- **One active chat** — context ring and agent memory are per conversation; mixing tracks in one chat causes wrong-file edits and wasted tokens.
- **Rename chats** — e.g. `Designer — Sign-up Sheet & Phase 4`, `8080 — templates, Docker, Tomcat CSS`, `Website — library mock & test-drive links`.
- **ROADMAP as dashboard** — check [`docs/ROADMAP.md`](ROADMAP.md) at session start and after each milestone; update checkboxes when you park or complete work.
- **Rules follow you** — `.cursor/rules/tawala-work-scopes.mdc` applies in every chat in this repo; the opener still tells the agent which track to prioritize.
