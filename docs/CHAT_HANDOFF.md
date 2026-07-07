# Chat handoff — three parallel tracks

**New to Cursor?** Start here: [`docs/START_NEW_CHAT.md`](START_NEW_CHAT.md) — step-by-step new chat + copy-paste Start Script.

Use this file when starting or resuming a Cursor chat. Copy the **5-line opener** for the track you are working on, paste it as your first message, and rename the chat to match the suggested title.

Status dashboard: [`docs/ROADMAP.md`](ROADMAP.md). Cursor usage tips: [`docs/CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md).

---

## ⏱️ Session checkpoint — July 2026

**Track:** Browser Designer (`designer-web/`). **Commit + push** requested by owner (this checkpoint).

### Implemented this session

- **Docked Items palette** beside Project Explorer; removed from inside Form windows.
- **Process Statements palette** replaces Items when a Process window is active (context swap).
- **No Items column** when a Document window is active (Document uses format toolbar path, not form insert).
- **Heading canvas WYSIWYG** (`HeadingCanvasRow`) — edit/collapse, click-to-activate, legacy placeholder on insert.
- **Selection sync** on MDI window close/minimize so docked palettes target the correct active window.
- **Per-run Main/Sub** heading sizing on canvas; **badge label** editing (H1/H2-style badge).

### Documented only (not coded)

- **Text WYSIWYG** remains blocked on a shared **Formatting Palette** shell (see `DESIGNER_FORM_FORMAT_TOOLBAR.md`, Text section in `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`).
- **Per-item Properties popups** — incremental migration deferred; permanent Properties panel stays for non-Heading items.

### Next

1. **Formatting Palette** shell (row 2, app-wide enable/disable rules).
2. **Text canvas row** (`TextCanvasRow`) once palette focus context exists.

### Key files

`App.tsx`, `FormEditor.tsx`, `FormItemsPalette.tsx`, `ProcessStatementsPalette.tsx`, `HeadingCanvasRow.tsx`, `processStatements.ts`, `projectStore.ts`, `styles.css`, `types/tawala.ts`, `jsonToXml.mjs`, `runtime.mjs`, specs under `Tawala_Key_Documents/DESIGNER_FORM_*`, `docs/DESIGNER_BACKLOG_ARCHITECTURE.md`.

---

## ⏱️ When you return — session catch-up (July 2026, unattended session)

Earlier catch-up notes (MDI Pass 1, Fields Phase 2, D1–D3). Those commits are on **`origin/main`**; use this section for verify checklists, not push status.

### 1. Commits (on `origin/main` — historical)

| Hash | What |
|------|------|
| `9b5264b` | **Fields Phase 2** — drag & double-click field tokens into editors (with drop guardrails) |
| `e88d3ba` | **MDI Pass 1** — canvas window shell for forms/processes/documents |

Already pushed; see `git log origin/main` for current tip.

### 2. What works now — how to test

```
cd designer-web && npm run dev      # UI on http://localhost:5173 (or 5174 if busy)
```

- **MDI:** The center canvas is now a **window manager**. **On load the canvas is EMPTY** (owner decision D2 — no form auto-opens). In **Project Explorer**, **single-click** any **form / process / document** → it opens (or focuses) an overlapping window.
  - Each new window **cascades down-and-right** from the previous one (D1); the cascade resets when the canvas is emptied.
  - **Forms open in Design mode** (D3); Process/Document windows have no Design/Preview tab.
  - **Drag** a window by its title bar; **resize** from any edge/corner; **click** a background window to bring it to front.
  - **Minimize** (`_`) sends it to the bottom taskbar; click the taskbar chip to restore. **Close** (`×`) removes it.
  - Open several at once (DirtBowl test: Registration form + a Pre/Post process + a document).
  - Titles read `Form - ParentCoaches`, `Process - Pre-ParentCoaches`, etc.
- **Fields Phase 2:** From the right-hand **Fields** tree, **drag** a field leaf onto a Text/Heading/FIB-question/MCQ editor or the rich-text surface → inserts `<<name>>`; or **double-click** a leaf to insert into the last-focused editor. Name fields (form/process/document rename, FIB capture-box labels, FIB stored names) **reject** drops.

### 3. Placeholder vs real editor in windows

- **Real editors** are embedded (not placeholders): `FormEditor`, `ProcessEditor`, `DocumentEditor` render inside each window frame.
- The only empty-state placeholder is when **no** windows are open (all closed): a "Select a form… to open a window" hint.

### 4. Open questions / decisions — RESOLVED (owner, July 2026)

Three MDI window-open decisions were confirmed and implemented on top of Pass 1 (`e88d3ba`) — shipped in subsequent commits on `main`.

1. **Open trigger → SINGLE-CLICK (D1).** Single-click an Explorer leaf opens (or focuses) its window on top; each new window **cascades down-and-right** from the previous one and resets when the canvas empties. Re-opening an already-open entity focuses it (no duplicate). *(Was: single vs double vs right-click "Open".)*
2. **Per-window editor state → still Pass 2.** Design/Preview tab + selected item remain **global**. **D3 partial fix:** opening a form always defaults the shared tab to **Design**, so a freshly opened form is never left on another form's Preview. Full per-window tab/item state is still the Pass 2 refactor.
3. **Windows menu →** still Pass 2 (unchanged).
4. **Auto-open first form → NO; START EMPTY (D2).** The canvas opens with no windows; the placeholder hint shows until the first click.

**Also (D3):** window title format confirmed `{Type} - {name}` (already matched); **Forms open in Design**, and Process/Document windows have **no Design/Preview** tab. Removed the redundant inner `Form — Name` heading (the window title bar is the single heading now). See `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` §2 (D1–D3 table) for the full write-up.

**Conflict note (owner checklist review):** §6 checklist item _"click a form/process/document → window opens"_ still passes (now single-click, empty start). The old checklist line _"On load, `Form 1` opens as a window"_ is **superseded by D2 (start empty)** — verify the empty-canvas placeholder instead.

### 5. Recommended next steps (Pass 2)

- Per-window editor state (tab + selected item) instead of global store fields.
- **Windows** menu (list open children, cascade/tile), maximize button, layout persistence to the project file.
- Process-window **yellow connection banner** (§3/§6) once Form↔Process Connect UI exists.
- Double-click-to-open + right-click context menu if you prefer legacy click semantics.

### 5b. Form item canvas WYSIWYG — Heading first (July 2026, **implemented**)

Owner references:

| State | Screenshot |
|-------|------------|
| **Insert / editing** | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Drag_Heading_to_Canvas-e9347d50-26e5-4d29-b855-df7089e8b8d1.png` |
| **Finished / blur (collapsed)** | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Finished_Heading-be3d8494-10d0-4818-8857-bc062ba4c222.png` |

**Spec:** `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_HEADING.md` (state machine, property table, gap table).

**Target (legacy):** Click **Heading** in docked Items bar → row on **Form - …** Design canvas:

1. **Editing:** orange **H1** badge, inline placeholder `[Replace this with heading of your own.]` **selected**, **Heading Type: Main** dropdown on canvas.
2. **Blur / finished:** collapse to **H1** badge + heading text only (e.g. `Welcome Campers!`) — **no** Heading Type row, no editing chrome.
3. **Activation:** same as Text — **click the heading box** when focus was elsewhere to enter edit mode (restore editing chrome from collapsed state).

Heading is a **canvas-inline exception**: label, text, and type live on the canvas in Design mode; no per-item Properties popup needed. Permanent Properties panel stays for other items.

**Items dock — DONE:** `FormItemsPalette` beside Explorer; `insertFormItem` → active form.

**Heading WYSIWYG — DONE:** `FormEditor.tsx` uses `HeadingCanvasRow`; legacy placeholder on insert; edit/collapse; Main/Sub on canvas; badge label edit; Heading Type removed from permanent Properties for Heading selection.

**Still deferred:** Properties popup per item for non-Heading items (D-Form-items); rich-text toolbar / RTF via shared Formatting Palette.

### 5c. Text item + Formatting Palette dependency (July 2026, document only)

Owner references:

| State | Screenshot |
|-------|------------|
| **Text selected on Form canvas** | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Item_in_Forms-e712e814-aeeb-4eae-8601-29da8826b108.png` |
| **Formatting Palette detail (Document)** | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Tools_Palette-3a4f7923-cab1-46fb-b320-3555eaacc3be.png` |

**Specs:** `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` (Text section), `DESIGNER_FORM_FORMAT_TOOLBAR.md`, `DESIGNER_DOCUMENT_EDITOR.md` § Document format toolbar.

**Target (legacy):**

1. **Text** is **canvas-inline WYSIWYG** like Heading — orange **T1** badge, placeholder **`[Replace this with text of your own.]`** selected on insert, **no Properties popup**.
2. **Formatting Palette** (row 2, app shell) is **shared** across Form Text items and Documents — **not** inside the form window.
3. **Palette rules:** Heading focus → **entire palette greyed**; Text focus → **palette live**; table delete/row-column tools (#12–13) greyed until cursor is in a table.
4. Full Text WYSIWYG is **blocked on** shared Formatting Palette implementation (palette also critical for Documents).

**Repo assets located:**

| Asset | Path |
|-------|------|
| Form format toolbar spec | `Tawala_Key_Documents/DESIGNER_FORM_FORMAT_TOOLBAR.md` |
| Document format toolbar spec | `Tawala_Key_Documents/DESIGNER_DOCUMENT_EDITOR.md` |
| Legacy C# enable/disable | `TawalaDesigner/Code/TAWALA/Forms/MDIFormView.cs` (`Application_Idle`), `Documents/MDIDocumentView.cs` |
| Browser toolbar (row 1 only) | `designer-web/src/components/ToolBar.tsx` |
| Embedded mini rich toolbar | `designer-web/src/components/RichTextEditor.tsx` (B/I/U + size — Properties/Document only) |
| Text canvas (static preview) | `designer-web/src/components/FormEditor.tsx` `CanvasItem` `"text"` branch |

**Gap:** No shell-level Formatting Palette; Text edits in permanent Properties panel; canvas shows static HTML. `RichTextEditor` mini-toolbar is not the legacy shared palette.

**Resolved (owner, July 2026):** Heading and Text share **click-to-activate** — **click the box** when focus was elsewhere to make inline editing live. **Blur contrast only:** Heading **collapses** to badge + text (Heading Type hidden); Text has **almost no visual difference** between edit and non-edit. See `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` § Design-mode edit behavior.

**Recommended implementation order:**

1. **Shared `FormattingPalette`** component — row 2 below menu bar; 14 controls per spec; disabled by default.
2. **Rich-text focus context** — track active editor + parent kind (Heading / Text / Document / FIB / MCQ); wire palette enable/disable rules.
3. **Heading WYSIWYG** (in progress) — validates palette greyed when Heading focused.
4. **`TextCanvasRow`** — canvas-inline badge + contentEditable body; remove Text from Properties panel.
5. **Migrate `RichTextEditor`** — drop embedded toolbar; surface becomes palette target.
6. **Document editor** — reuse same palette when Document MDI active.
7. **Table tools** — `CursorInTable` detection; conditional enable for #12–13.
8. **fx / Insert menu** — function picker wired to palette **fx** button.

**Not in scope this pass:** Full Text WYSIWYG or palette implementation unless trivial doc-only.

### 6. How to verify (checklist)

- [ ] `cd designer-web && npm run build` → clean `tsc -b && vite build` (`tsc -b` re-verified after D1–D3).
- [ ] `npm run dev`, **new project / load a template → canvas is EMPTY** (placeholder hint, no auto-opened form) — **D2**.
- [ ] **Single-click** a form/process/document → window opens on top — **D1**.
- [ ] Open a second, then a third node → each new window **cascades down-right**, revealing the prior title bar — **D1**.
- [ ] Single-click **Registration** → window opens showing **Design** (not Preview) — **D3**.
- [ ] Open a **process** and a **document** → their windows have **no Design/Preview tab** (just the editor); titles read `Process - …` / `Document - …` — **D3**.
- [ ] Close all windows, open a new one → cascade **restarts at the origin** — **D1**.
- [ ] Drag / resize / minimize+restore / close a window.
- [ ] Open 3 windows, click between them → z-order front works.
- [ ] Rename a form in Explorer while its window is open → title updates, no "not found".
- [ ] Fields drag + double-click still insert `<<name>>`; name fields still reject.

### 7. Blockers hit

- **Live browser walkthrough not run** — the Cursor IDE browser tab was unavailable in this unattended session (new tabs vanished). Verified instead via clean `tsc -b && vite build` + dev-server HTTP 200 smoke test. Please run the manual checklist above.
- Stray empty `package-lock.json` at the **repo root** (from an accidental root `npm`) left **untracked** — safe to delete; not part of either commit.

---

## Chat 1 — Browser Designer (`designer-web/`)

**Suggested title:** `Designer — architecture backlog & Phase 4`

### 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — designer-web/ (Phase 4)
Goal: Implement Designer architecture backlog (MDI, explorer, properties UX); not Registration CSS parity
Read first: docs/DESIGNER_BACKLOG_ARCHITECTURE.md, docs/ROADMAP.md Phase 4, Tawala_Key_Documents/DESIGNER_MENU_SPEC.md
Constraints: Do not mix 8080 CSS/docker or website-mock work in this chat; preview/deploy local only (5173/3001/8080, not www.tawala.com)
```

### Work to date

- Browser Designer shell running at http://localhost:5173 (`cd designer-web && npm run dev`).
- **File → New Project…** template picker with featured starters in `public/samples/templates/`.
- **Simple Survey**, **Sign-up Sheet**, and **Get Together** deploy smoke tests **passed** (owner, July 2026).
- **DirtBowl Registration page 1** Preview/Deploy parity **substantially complete** — pending owner verify of final Q4 email-note CSS fix (uncommitted `docker/tomcat/css/project/dirtbowl2/project.css`).
- DirtBowl stress test documented five **architecture backlog** items: [`docs/DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md).
- Deploy dialog filters Java response to project-scoped URLs (`server/deployParse.mjs`); restart dev server after pull.
- Preview/runtime fixes landed in `server/` and `src/api/`.
- **Landed (July 2026):**
  - **Multi-window / MDI shell — Pass 1** (`src/components/mdi/`): open/close/drag/resize/minimize/z-order windows from Explorer; embedded real editors. See backlog §2 for Pass 2 items.
  - **Fields palette drop targets — Phase 2** (`9b5264b`): drag + double-click `<<name>>` insert with name-field guardrails.
  - Explorer Phase 1 collapse + linked Pre/Post processes; inline rename.
- **Not built yet (blockers for Process work and large-project authoring):**
  - MDI Pass 2: Windows menu, per-window editor tab/item state, connection banner, layout persistence, tile/cascade/maximize
  - Forms ↔ Processes **editing** (Connect Pre/Post UI, auto-name on attach)
  - Properties popups vs permanent panel; multiple merged menu bars
  - Insertion-point arrow (Form / Process / Document)
  - Move Up / Move Down for form items, process statements, document blocks
  - Fine-grained FIB drop map (blocked on WYSIWYG); `.tawala` import; Potluck / email templates
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

1. **Designer architecture backlog** — MDI, explorer collapse, form–process links, properties popups, menu bars ([`DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md)).
2. **Owner verify** — DirtBowl Registration page 1 Q4 email-note alignment on `:8080` vs `:5173`; then commit `project.css` + doc updates.
3. **Insertion-point + Move Up/Down** — required before serious Process editing (see ROADMAP Phase 4 prerequisites).
4. **UX feedback** — canvas layout, inspector after architecture items land.
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
5. Registration / DirtBowl runtime parity — **page 1 substantially complete** (July 2026); paused unless owner reopens Page 2+, Review headers, RegStep2. Pending: owner verify Q4 email-note CSS fix before commit.

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
| **Continue this chat** | Designer (Chat 1) — architecture backlog |
| **Focus first** | Designer — architecture backlog (MDI, explorer, properties) |
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
