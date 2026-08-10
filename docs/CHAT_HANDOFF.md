# Chat handoff — three parallel tracks

**New to Cursor?** Start here: [`docs/START_NEW_CHAT.md`](START_NEW_CHAT.md) — step-by-step new chat + copy-paste Start Script.

Use this file when starting or resuming a Cursor chat. Copy the **5-line opener** for the track you are working on, paste it as your first message, and rename the chat to match the suggested title.

**Skinny track handoffs (July 2026):** For focused Designer sessions with less context, use one of these instead of reading the full file below:

| Handoff | Use when |
|---------|----------|
| [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md) | Forms canvas WYSIWYG, FIB/MCQ, remaining form items, palette table/fx |
| [`CHAT_HANDOFF_PROCESSES.md`](CHAT_HANDOFF_PROCESSES.md) | Process editor, statements palette, Form↔Process links, insertion arrow |
| [`CHAT_HANDOFF_INTEGRATED.md`](CHAT_HANDOFF_INTEGRATED.md) | Documents editor, shared Formatting Palette, cross-Form/Process/Document shell |

Status dashboard: [`docs/ROADMAP.md`](ROADMAP.md). Cursor usage tips: [`docs/CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md).

---

## ⏱️ Session checkpoint — July 19, 2026 (evening)

**Track:** Browser Designer (`designer-web/`). Branch `cursor/forms-canvas-wysiwyg`.

### Accomplished this session

- **WHERE re-smoke complete** for condition-bearing functions (FRC, MQL, QCT, RMRL, RANKED RESPONSE COUNTS/NAME, SINGLE QUESTION LIST, SUM; Bar Graph / Totals FIB Where OK).
- **TODO #11 — MCQ-aware Function Where** implemented: `mcConditionOperators.ts`, `lookupFormFieldMcItem`, `FunctionConditionsEditor` switches to `mc*` ops by `onlyone`; export tests for `mcContains`/`mcEquals` + choice letter.
- **TODO #12 — RESPONSE TOTALS multi-select** investigated: no Totals-specific undercount (same tally as Bar Graph); Preview regression tests for array multi-select.
- Earlier same day (prior turns): MDI title-bar clamp under Fields; Process/QCT timing guidance; RMRL output semantics; docs for WHERE ladder.

### Tomorrow morning — do first

1. Owner smoke **#11** MCQ Where (letter `d` / `mcContains` on multi).
2. Owner smoke **#12** Totals vs Bar Graph counts on same multi MCQ.
3. Then held items as desired: Document drag-select with Field/function tokens; drag Function onto text line; Form rename → Process Show/Get/Delete cascade (optional).

### Key files touched (this evening)

- `designer-web/src/lib/mcConditionOperators.ts` (+ test)
- `designer-web/src/lib/projectModel.ts` (`lookupFormFieldMcItem`)
- `designer-web/src/components/FunctionConditionsEditor.tsx`
- `designer-web/src/lib/functionConditions.ts`
- `designer-web/server/documentHtmlToXml.test.mjs`, `responseTotalsPreview.test.mjs`, `choiceTallyPreview.test.mjs`
- Specs: `DESIGNER_OPEN_TODOS.md`, `DESIGNER_OPEN_BUGS.md`, `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`

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

**When this track resumes (after Website):** read `.cursor/rules/tawala-designer-parked-post-website.mdc` first — especially **MUST DO: conflate Invitation + Hyperlink** (Form link primary).

**Aug 2 morning — conversion inventory (no code fixes):** Owner is listing **`.tawala` → JSON** bugs before another large import trove; **batch-fix with tests later**. Queue: sequential FIB blanks elided (C1; multi-alt destroyed), MCQ alternate labels → Q1/Q2 (C2), function tables fail convert (C3, examples pending), Heading `<<field>>` Deploy fixed / convert residual watch (C4). Full table: `Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md` § **Legacy .tawala → JSON conversion (batch fix queue)**. Repros: Online Exam Builder, Priority Library Projects.

### 5-line paste opener

```
Project: Tawala (~/Projects/Tawala)
Track: Browser Designer — designer-web/ (Phase 4 return)
Goal: Designer pass after website — FIRST: unify Insert Invitation+Hyperlink (Form link primary); then parked polish + backlog
Read first: .cursor/rules/tawala-designer-parked-post-website.mdc (MUST DO Link conflation), Tawala_Key_Documents/DESIGNER_INSERT_MENU_AND_FUNCTIONS.md § Link/Invitation unified, docs/ROADMAP.md Phase 4
Constraints: Do not mix 8080 CSS/docker or website-mock in this chat; preview/deploy local only (5173/3001/8080); Form-in-project links are primary — Hyperlink is external-URL secondary mode only
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

0. **MUST (Jul 31 framing):** Unify Insert → Invitation + Hyperlink into one Link dialog — Form-in-project **primary**, external URL secondary, private InviteeID tertiary. Spec: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § unified Link. Do not treat Hyperlink as a peer product.
1. **Rename Deploy → Push (parked Aug 10 — UI/copy only):** Designer **Deploy…** / **Deploy this version** → **Push to My Tawala** / *Push Project to your MyTawala library*; keep `/api/deploy` code ids. Checklist: `website-mock/README.md` § Designer Push rename; also `.cursor/rules/tawala-designer-parked-post-website.mdc`. Do **not** rename website My Tawala Details **Deploy** (share/embed).
2. **Theme applies on Push (parked Aug 10 — Designer Push / Redeploy):** My Tawala Details Theme dropdown persists overlay `themePath` only (PARTIAL). Changing Theme does **not** restyle live `:8080` yet — Push/Redeploy must write CSS/theme into the deployed project from `themePath`. Cross-ref: `website-mock/README.md` Task #5 HOLD; `.cursor/rules/tawala-designer-parked-post-website.mdc`. Do **not** implement in website chat.
3. **Designer architecture backlog** — MDI, explorer collapse, form–process links, properties popups, menu bars ([`DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md)).
4. **Owner verify** — DirtBowl Registration page 1 Q4 email-note alignment on `:8080` vs `:5173`; then commit `project.css` + doc updates.
5. **Insertion-point + Move Up/Down** — required before serious Process editing (see ROADMAP Phase 4 prerequisites).
6. **UX feedback** — canvas layout, inspector after architecture items land.
7. **Backlog** — DirtBowl → website Library link, FIB free-mix layout, `.tawala` import, outbound email (separate session).
8. **Parked polish** from `.cursor/rules/tawala-designer-parked-post-website.mdc` (Document P0s, confirm, Font Color, Skip stubs, Jul 30 FIB/Text Deploy bugs).

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

**Suggested title:** `Library thread`

**Status (Aug 10, 2026):** Task List #9 (**Make a Copy**) wired in working tree (**uncommitted**). Review on **`http://localhost:5500`** — owner localStorage lives there; `127.0.0.1:5500` is a separate origin (banner redirects). Rename ≠ Make a Copy ≠ Library Save a copy.

### Clean start tomorrow (plain English)

Do these in order. Wait for each step to finish before the next.

1. **Open Cursor** and this repo: `~/Projects/Tawala`.
2. **Start Docker Desktop** — wait until the whale icon is steady (not animating).
3. **Start Postgres + Tomcat** (pick one; both are fine if Docker is ready):
   - Preferred: `./scripts/docker-up.sh` from the repo root, **or**
   - `docker compose -p ai-tawala up -d` from the repo root  
   Wait until http://localhost:8080/login loads in a browser.
4. **If Library Test Drive shows “We are very sorry”** (Tomcat is up but forms fail): in Terminal run  
   `docker restart tawala-tomcat`  
   Wait ~15 seconds, try the form URL again.
5. **Start the website mock** (static pages on port 5500):  
   `cd ~/Projects/Tawala/website-mock && ./serve.sh`  
   (or `python3 -m http.server 5500 --bind 127.0.0.1` — either host works; **open as localhost**)
6. **Optional — Designer API** (needed for Purge / Records / Test Drive fail-page probe):  
   `cd ~/Projects/Tawala/designer-web && npm run dev`  
   (or `./scripts/ensure-dev-api.sh` if Vite is already up and only `:3001` died).
7. **Open these review URLs** (hard-refresh / use the `?v=` so you get tonight’s JS) — **always `localhost`, not `127.0.0.1`** (separate localStorage):
   - My Tawala: http://localhost:5500/mytawala.html?v=20260810-makecopy1  
   - Library: http://localhost:5500/library.html?v=20260810-makecopy1  
   - Direct form smoke (should show a real form, not “We are very sorry”):  
     http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey  
     http://localhost:8080/p/u3hkqgwtrepjlur/ef6sx16.Administration
8. **Rename the chat** to **Library thread**. Paste: `Clean start Library thread`  
   or: “Continue Make a Copy (#9) review — read docs/CHAT_HANDOFF.md Chat 3.”

#### localhost vs 127.0.0.1 (owner Aug 10)

Same `serve.sh` / `:5500` server. Different browser origins → **different localStorage**. Owner has been adding projects on **localhost**; agent review links that used **127.0.0.1** looked empty. **Standardize on `http://localhost:5500`.** If you open `127.0.0.1`, the yellow mock banner shows a tip with a localhost link.

#### DONE vs OPEN (for tomorrow)

| Item | Status |
|------|--------|
| Purge offline demo Records | **DONE / committed** (`2dab7ec`) |
| Aug 9 Task List #8 Save a copy | Wired in tree (Use-ready); visual review ongoing |
| Aug 9 Task List #9 Make a Copy | **WIP in files, not committed** — Details + listing bar; fork ≠ Rename. **Owner should click-test on localhost.** |
| localhost vs 127.0.0.1 localStorage | **Documented + banner on 127.0.0.1**; Designer Show-in-My-Tawala → localhost |
| Commit of Make a Copy / host standardization | **OPEN** — do after owner confirms |

### Session checkpoint — Aug 9 evening (Library thread)

**Save a Copy (root cause):** Library Actions cell used `onclick="event.stopPropagation()"`, so the document-level click handler never saw **Save a copy** (button → silence). **Fix:** remove that stopPropagation; row navigation ignores clicks on the Actions cell; op handlers use capture phase. Cache-bust `?v=20260809-libfix1`.

**Test Drive (root cause):** Tomcat returned HTTP 200 with the legacy “We are very sorry” page because **World was not initialized** (often after Docker/Postgres race). Catalog `demo-urls.js` paths were OK. Restart Tomcat after Postgres is healthy. Optional `:3001/api/probe-java-url` helps the mock detect fail pages before opening a tab (needs API restart to load).

**Uncommitted (leave for morning commit if review OK):** `website-mock/library.html`, `js/project-ops.js`, `js/demo-urls.js`, library/mytawala HTML cache-busts, `designer-web/server/index.mjs` (probe), `docker/tomcat/Dockerfile` (healthcheck `/home`).

### Session checkpoint — Aug 9, 2026 (morning decisions)

**Done that morning:** Documented Aug 9 walkthrough agreements in plain English (Publish = new Library entry; update existing = new version on that entry; Active/De-activate; Start-point distribute/embed instead of ACL; Records/clones/times-used; Project Data + Purge toolbar; Edit in Designer on Details only; Make a Copy; Backup hold; Test Drive contract) plus ordered Task List.

**Next:** #8 owner visual review → commit if OK; then remaining Task List holds (Backup package, Download latest, SEE DEMO videos, ratings product, Shared Data, Access column).

### 5-line paste opener

```
Project: Tawala (~/Projects/Tawala)
Track: Website mock — website-mock/ (Phase 3) — chat title: Library thread
Goal: Clean start Library thread — confirm Save a Copy + Test Drive; then continue Aug 9 Task List #8 review. Read docs/CHAT_HANDOFF.md Chat 3 “Clean start tomorrow”.
Read first: docs/CHAT_HANDOFF.md (Chat 3 clean-start), website-mock/README.md § Aug 9 decisions (lists 1+4)
Constraints: Keep website-mock/ + :5500; no commit unless owner asks; if Test Drive shows “We are very sorry”, docker restart tawala-tomcat; defer parked Designer items
```

### Work to date

- Static mock at http://localhost:5500 (`cd website-mock && python3 -m http.server 5500`).
- Draft pages: home, library, library detail, MyTawala, About/FAQ/Login/signup/terms/privacy/designer stubs.
- **Test drive** links wired via `website-mock/js/demo-urls.js` → Phase 2 deploy URLs.
- Legacy CSS imported: `css/legacy/tawala-base.css`, `pages/homepage.css`, `pages/library.css`.
- Chrome helpers: `js/chrome.js` (pending links greyed with `link-pending`) — keep these; do not reinvent chrome.
- Template images copied from build1700; Jobs removed from footer (owner, July 2026).
- Owner confirmed all mock pages load; test-drive / library / My Tawala links OK.
- Prereq for fidelity work: stable `:8080` template URLs (Tomcat up; featured templates deployable).
- **Tenancy (owner Jul 31, 2026):** one public Library; each account’s My Tawala is private; **Publish** is the only deliberate bridge into Library. Documented in `website-mock/README.md` (§ Tenancy + Save/Deploy/Publish glossary). Mock stays single-browser `localStorage` — do not implement multi-account auth here.
- **Ops verb split (Java build1700 + memos; owner Jul 31, 2026 reconciled):** **EXPORT / IMPORT** = Excel **response data** only (Import = restore messed-up data into current project; field mismatch fails; no definition rollback). **BACKUP / RESTORE** = `.backup` ZIP = **paired definition + data** (plus properties / links) — the path that held across later field changes. **Deploy** mints definition versions (Java auto-deploys); Designer File→Save = local definition only. Shipped PM had **no submissions “Save”** — those four verbs; colloquial “Save” may have meant Backup. Earlier “perhaps Save must also preserve project” note → **Backup/Restore already was** that paired snapshot. Ops framing: glossary in `website-mock/README.md`. Sequencing hold on My Tawala **listing** version piles stands (Versions UI existed historically).
- **Library stub cleanup (owner Aug 1, 2026):** correct route to clean up Library placeholders is **Designer → Deploy → Publish**, not a shortcut. Until each stub has a real vetted replacement, 15 of 21 Library entries carry `stub: true` + a visible `" (stub)"` name suffix (`js/demo-urls.js`) so the listing shows what still needs replacing. `liveReady` (Aug 1: Simple Survey, Sign-up Sheet Template, Potluck, Get Together, plus Horses and Penguins Test, Multiple Question Survey Template) are never stubs. Retirement stays **agent-run, no public Library Delete button** — `website-mock/scripts/list-library-stubs.mjs` lists current stub ids (read-only); see README.md § "Library stubs and their retirement path" for the agent procedure.
- **Sign-up Sheet w Email retired from public Library (owner Aug 1, 2026):** removed `signup-sheet-email` from `TAWALA_LIBRARY` (not drop-in ready). Still on **Designer → File → New Project** (`signup-sheet-w-email.json`); **not** seeded into default My Tawala. Re-Publish when a finished runtime-customizable version exists. See `website-mock/README.md`.
- **USE IT / Library Actions framing (owner Aug 1, 2026):** "Use"/USE IT is unclear; most users avoid Designer; Designer-only starters stay on New Project (Sign-up Sheet w Email retired from Library catalog); prefer Sophisticated in-project customize or a clear My Tawala copy-to-run path; Library = discover/acquire, My Tawala = operate. USE IT wiring still deferred — see `website-mock/README.md` § Library Actions / USE IT framing.
- **Roster carry-forward (owner Aug 1, 2026):** sports leagues asked yearly how to move rosters from prior years — historically Excel Export→archive + selective player Import (strip graduates / add kids), not Backup/Restore-all or Library re-pull; soft open “roll season” on EXPORT/IMPORT spine. See `website-mock/README.md`.
- **Library quality / community (owner Aug 1, 2026 — L&F / later product, not initial wiring):** pro vs amateur/community ideas hard to mix with complex pro projects (SportsDashboards / DirtBowl class); reviews/ratings might surface best work but reputation is gameable; YouTube lesson ≈ easy upload → for Tawala, Designer must get much easier for community Publish to work. Framing/open questions only — **do not build ratings now**. See `website-mock/README.md` § Library quality / community.

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
| Parked Designer scope | `.cursor/rules/tawala-designer-parked-post-website.mdc` |

### Immediate phases ahead

1. **Library / My Tawala ops (owner Jul 31, 2026 — next focus):** Harden **Delete / Purge**, **EXPORT / IMPORT**, **BACKUP / RESTORE** (and related lifecycle ops); advance **Publish** stub readiness; keep **flat** My Tawala (one row per project). Library → My Tawala **Save this project** remains the customize-entry stub. Libraries are the core of the Website — not homepage chrome.
   - **Library stub suffixes landed (Aug 1, 2026):** 15 placeholder Library entries now read `" (stub)"` in the listing; retirement is agent-run per stub once its Designer → Deploy → Publish equivalent is checked (see README.md § Library stubs). Next real step: pick one stub (e.g. DirtBowl, since Registration parity already exists on `:8080` separately), verify/Deploy it properly, Publish, then ask an agent to retire the old stub entry.
2. Keep `demo-urls.js` in sync when template deploy names or paths change on 8080; verify Test drive links stay on the right live `:8080` URLs.
3. Flesh out stub pages (About, FAQ, Login) when copy is ready — non-blocking vs ops.
4. Wire **Designer** marketing page when browser Designer is demo-ready.
5. DirtBowl → Library link (backlog — depends on Designer/deploy track).

### Defer / out of scope (this chat)

- **Look-and-feel polish (owner Jul 31, 2026):** Do **not** bother with more visual polish now. **Home page** look-and-feel is optional only and **not critical wiring**. Libraries / catalog ops come first.
- **My Tawala version piles (owner Jul 31, 2026):** Do not expose multi-version / “Deploy creates version N” in the **listing** until Delete / Purge and **EXPORT / IMPORT** + **BACKUP / RESTORE** are solid enough; audit trail ships with that same lifecycle package. Versions existed in PM Versions UI historically — hold is listing clutter. Keep flat one-row-per-project mock for now. Target model still B7 — see `website-mock/README.md` § Save/Deploy/Publish **Sequencing / hold**.
- **My Tawala listing / folders / complete-private (owner Aug 1, 2026 — L&F / organization phase, not initial wiring):** ideas are worth doing, but parked with look-and-feel — sticky icon-column headings + icon-group rules + Delete/Purge color cues + clearer icons; user-controlled folders (group/hide, drag working projects to top); separate list for complete-but-not-for-Library (private archive vs working set vs Publish). Library ops first; flat list still current; folders/versions UI after ops+audit readiness — see `website-mock/README.md` § Parked / backlog.
- **Library quality / community (owner Aug 1, 2026 — L&F / later product):** pro vs community Publish mix, ratings/reputation gameability, Designer ease as contribution prerequisite — open questions only; no ratings build. See `website-mock/README.md` § Library quality / community.
- **Designer MainMenu** Project Manager + Email Delivery — wait until the site exists.
- Everything in `.cursor/rules/tawala-designer-parked-post-website.mdc`: Document P0s, native `confirm()`, Font Color picker, Skip/Process stubs; **plus Jul 30 parked (Not blocking):** FIB Styles squashed “Align right side” radio; Form Text blank-line loss on Deploy + image breaks highlight — see `DESIGNER_OPEN_BUGS.md` § Parked Jul 30.
- **Also for next Designer chat (not here):** unify Invitation + Hyperlink (Form link primary) — called out in that rule’s **MUST DO** and Chat 1 opener; **plus** Designer Deploy → Push rename (UI/copy; keep `/api/deploy`) — same rule + Chat 1 phase 1 + `website-mock/README.md` checklist.
- **Page Header / banner graphics** — until Deploy image pipeline is understood.
- **Owner offline sample JSON review** — not blocking; do not replace samples here.
- Do **not** thrash `designer-web/` or Tomcat/Docker unless a test-drive link truly requires URL wiring.
- **Library Live policy:** owner will not ship Live Library projects that depend on fixing **blocking** Designer bugs first; Jul 30 items above are **Not blocking** for Live.

---

## What to do with the three chats

| Action | Which chat |
|--------|------------|
| **Continue this chat** | Designer (Chat 1) — architecture backlog |
| **Focus first** | Website (Chat 3) — Library / My Tawala ops (Delete/Purge, EXPORT/IMPORT, BACKUP/RESTORE); **or** Designer when site ops pause |
| **Park** | 8080 (Chat 2) until deploy breaks; Website visual polish (except optional Home) until ops are solid |

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
