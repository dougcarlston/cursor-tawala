# Designer Backlog — Architecture (browser vs legacy C#)

*Owner findings from stress-testing with **DirtBowl Registration** (July 2026). These are **general Designer architecture gaps**, not template-specific blockers. DirtBowl served its purpose: surfacing structural failures that did not appear in smaller templates (Simple Survey, Sign-up Sheet, Get Together all passed Preview + Deploy earlier).*

**Related specs:** `Tawala_Key_Documents/DESIGNER_*.md` (especially `DESIGNER_MENU_SPEC.md`, `DESIGNER_STARTUP_AND_FORM_CANVAS.md`, `DESIGNER_UI_REFERENCE.md`, `DESIGNER_DOCUMENT_EDITOR.md`).

**Related runtime docs:** [`ROADMAP.md`](ROADMAP.md) Phase 4, [`COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md).

---

## 1. Multi-window / MDI architecture

**Legacy:** Designers can open Forms, Processes, and Documents in **separate MDI child windows** with multiple windows open at once. The **Windows** menu lists open children; the center pane hosts `Form - …`, `Process - …`, `Document - …` windows simultaneously.

**Browser Designer (`designer-web`):** Server-side layout uses a **single editor pane** — no MDI, no Windows menu, no parallel Form + Process + Document windows.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — shell layout (Project Explorer | MDI windows | Fields), Windows menu, MDI gap table
- `DESIGNER_STARTUP_AND_FORM_CANVAS.md` — Form canvas as center MDI child on Design tab
- `DESIGNER_DOCUMENT_EDITOR.md` — Document MDI child window
- `DESIGNER_PROCESS_STATEMENTS_IF.md` — Process MDI window

**Impact:** Blocks meaningful **large-project authoring** (e.g. full DirtBowl edit pass). Does **not** block using DirtBowl as a Preview / Deploy parity sample.

---

## 2. Forms ↔ Processes connection transparency

**Legacy:** Form ↔ Process links are explicit in **Form Properties** (PreProcess / PostProcess) and visible in the project tree. Processes are named and tied to forms in a way designers can inspect and navigate.

**Browser Designer:** The method for connecting forms and processes server-side is **not transparent** — unclear whether equivalent linking exists in `designer-web` state, deploy pipeline, or explorer presentation.

**Spec cross-refs:**
- `DESIGNER_UI_REFERENCE.md` — Form Properties (PreProcess / PostProcess), PostProcess terminology
- `DESIGNER_MENU_SPEC.md` — explorer selection drives Items vs Statements columns

**Impact:** Designers cannot see or verify form–process wiring while editing; stress-testing DirtBowl exposed this as a structural gap separate from Registration CSS parity.

---

## 3. Collapsible Explorer menus

**Legacy:** Explorer sections **collapse** — vital for **Fields** (all forms + Variables) and for **Forms** (with linked Processes shown as **submenus under forms**).

**Browser Designer:** Explorer lacks collapsible sections and does not show linked Processes as form submenus.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — Project Explorer columns (Forms / Processes / Documents / Images), Fields palette, explorer toolbar
- `DESIGNER_UI_REFERENCE.md` — project tree / explorer panel structure

**Impact:** Large projects become unwieldy in a flat tree; Fields and form–process hierarchy are hard to scan.

---

## 4. Properties panel vs popup dialogs

**Legacy:** **Item Properties** and **Form Properties** open as **popup dialogs** when an item or form is selected — they do not permanently consume layout space.

**Browser Designer:** A **permanent Properties panel** wastes too much horizontal space compared to legacy popup-on-select behavior.

**Spec cross-refs:**
- `DESIGNER_UI_REFERENCE.md` — Form Properties, Item Properties dialogs (confirmed fields)
- `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` — validation popups (popup pattern elsewhere in legacy UI)

**Impact:** Editing density on smaller screens; diverges from legacy interaction model designers expect.

---

## 5. Multiple menu bars (context-sensitive toolbars)

**Legacy:** **Multiple menu/toolbar rows** — main menu bar plus a **format toolbar** (and context-specific strips) that **merge from the active MDI child** (Form vs Document). Row 2 appears below the menu bar when a Form or Document child is active.

**Browser Designer:** Simplified single toolbar; no per-child merged Format / Tables / Tools strips.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — main icon toolbar, Format toolbar (row 2 per MDI child), browser gap table
- `DESIGNER_FORM_FORMAT_TOOLBAR.md` — Form vs Document format toolbar
- `DESIGNER_DOCUMENT_EDITOR.md` — Document format toolbar on row 2

**Impact:** Slower design and editing workflow; fewer commands one click away compared to legacy.

---

## Already recorded elsewhere (not duplicated here)

| Item | Where |
|------|--------|
| Insertion-point arrow | `ROADMAP.md` Phase 4 prerequisites |
| Move Up / Move Down | `ROADMAP.md` Phase 4 prerequisites |
| FIB free-mix layout, hint text | `docs/DESIGNER_BACKLOG_FORMS_FIBS.md`, `ROADMAP.md` |
| Fields palette drag targets | `ROADMAP.md` Phase 4 |
| `.tawala` import | `ROADMAP.md` Backlog |

---

*Last updated: July 2026 — captured after DirtBowl Registration Preview/Deploy parity phase.*
