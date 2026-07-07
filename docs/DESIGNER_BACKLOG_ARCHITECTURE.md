# Designer Backlog — Architecture (browser vs legacy C#)

*Owner findings from stress-testing with **DirtBowl Registration** (July 2026). These are **general Designer architecture gaps**, not template-specific blockers. DirtBowl served its purpose: surfacing structural failures that did not appear in smaller templates (Simple Survey, Sign-up Sheet, Get Together all passed Preview + Deploy earlier).*

**Related specs:** `Tawala_Key_Documents/DESIGNER_*.md` (especially `DESIGNER_MENU_SPEC.md`, `DESIGNER_STARTUP_AND_FORM_CANVAS.md`, `DESIGNER_UI_REFERENCE.md`, `DESIGNER_DOCUMENT_EDITOR.md`).

**Related runtime docs:** [`ROADMAP.md`](ROADMAP.md) Phase 4, [`COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md).

---

## 1. Fields panel architecture

**Legacy (owner screenshot, July 2026):** Right-hand **Fields** panel — title bar **Fields**, WinForms `TreeView` with **`[-]` / `[+]`** expand/collapse controls and **dotted connector lines**. Tree items show a **selection highlight** (e.g. form folder selected).

**Example tree** (DirtBowl-style project with forms **Survey** and **Report**):

```
Survey              ← form name (expanded, selected)
  Q1                ← form field names (from active form items)
  Q2
  Name
Report              ← sibling form node (may be empty or collapsed)
Variables           ← expanded
  _InviteeID        ← underscore-prefixed project variable
```

**Legacy behavior (C# `FieldsPalette.cs`):**

- One **top-level node per form** in the project — **all forms shown**, not only the open MDI child.
- Children under each form are **field names** (`GetFormItemFields()`), e.g. `Q1`, `Name` — the names used in `<<…>>` references.
- **Variables** node at the bottom lists **project variables** (e.g. `_InviteeID`); appears when the project has variables. Built-in / process-created names use a leading underscore per `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`.
- Additional top-level nodes may appear for **Records** / record-set contexts when editing processes (`addRecordItems`, `addRecordSetItems`) — owner screenshot shows the common Survey + Report + Variables case.
- **Drag** a field **leaf** into form, process, or document editor → inserts `<<FieldReference>>`.
- **Double-click** a leaf inserts at current editor focus.
- Only **leaves** drag (not form folder headers or the Variables folder header).
- **Collapse via `[-]` / expand via `[+]`** on each branch node (form folders, **Variables**, **Records**, etc.). Collapsing submenus is **essential for large projects** (DirtBowl stress-test): the panel height is fixed, so designers must fold closed forms they are not using to scan top-level categories and find fields quickly.
- **Resizable column width** — drag the **left margin** of the Fields panel (grey vertical splitter bar between Fields and the center canvas/explorer). Implemented in C# `FieldsPanel.cs` (`Padding.Left` hit zone, horizontal resize cursor); min width ~60px.

**Browser Designer (`designer-web`) — Phase 1 (July 2026):** `FieldsPalette.tsx` in `InspectorPanel.tsx` — **all forms** as top-level branches, **flat field-name leaves** per form (`formFieldNames` in `projectModel.ts`), **Variables** branch (`collectProjectVariables`), **`[-]` / `[+]`** per-node collapse, selection highlight, leaf drag sources. Still **no editor drop targets**; **no** dotted connector lines; right column **fixed width** (`styles.css` `.designer-right` ~280px) — **no left-margin drag resize**.

**Owner decisions — July 2026** (confirmed requirements; see gap table for `designer-web` alignment):

| # | Topic | Legacy / owner rule | Phase 1 browser status |
|---|--------|---------------------|-------------------------|
| Q1 | **“Leaf” = capturable field** | One **terminal tree node** per unique data-capture name under each form branch (each FIB blank with its own label → its own leaf; MCQ / Hidden Field → one leaf each). Not grouped under item design labels. | **Aligned** — `formFieldNames()` emits one leaf per `GetFormItemFields()`-style name. |
| Q2 | **Variables scope** | Variables used in **Set**, **Append**, **Get**, **ForEach** (e.g. `Where [Record:Name] Equals [variable]`), and related process/skip statements appear under **Variables**. Legacy `Project.AllVariables` aggregates `Process.Variables` from all statement types. | **Done** — `collectProjectVariables()` recursively scans assignment targets **and** `<<variable>>` references across Get/ForEach/Delete/If conditions and Where clauses. |
| Q3 | **Collapse defaults** | **Session-only** collapse (not saved with project). **First open — Fields:** all form folders **and** Variables **collapsed**. **First open — Project Explorer:** all **Forms expanded** (linked Pre/Post visible). Rationale: ease of use. | **Done** — Fields `defaultCollapsed()` collapses **all** forms + Variables; Explorer seeds `expandedForms` with all forms. Session-only (`useState`), not persisted. |
| Q4 | **`_InviteeID` ordering** | **`_InviteeID` always first** in Variables; all other variables **alphabetical**. | **Done** — `_InviteeID` always present and pinned first; remainder sorted via `localeCompare`. |

**Image cross-refs:** Fields tree example — `DESIGNER_MENU_SPEC.md` § Fields panel; full layout with Fields column — `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Forms_-_Parent_Coachs-d61cde9a-0b21-48f6-a29f-c8c0a2a9ead1.png`.

**Owner requirements (Item 1 backlog):**

1. **Variables section** — **Done** — branch present; variable **discovery scope** now covers Set/Append/arithmetic targets plus `<<variable>>` references in Get/ForEach/Delete/If (Q2), `_InviteeID` pinned first (Q4).
2. **Drag-and-drop Fields → canvas/editors** — documented prerequisite for serious authoring. **Phase 2** — drag source exists; drop targets not wired.
3. **Per-node collapse/expand (`[-]` / `[+]`)** — **Done** — first-open defaults aligned (Q3): Fields all-collapsed, Explorer all-forms-expanded.
4. **Left-margin column resize** — **Phase 1b**. No column splitter today (fixed CSS width).

**Spec cross-refs:**

- `DESIGNER_MENU_SPEC.md` — Fields panel (right), browser gap table
- `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md` — Variables folder, `_InviteeID`
- `DESIGNER_DOCUMENT_EDITOR.md` — palette field drag into document body
- `ROADMAP.md` Phase 4 — Fields palette drag targets

**Gap table — Fields panel (legacy vs `designer-web` Phase 1):**

| Aspect | Legacy | Browser Phase 1 | Remaining gap |
|--------|--------|-----------------|---------------|
| Panel title | **Fields** | **Fields** (Inspector right column) | — |
| Tree chrome | `[-]`/`[+]` tree, dotted connector lines | `[-]`/`[+]` per branch | Dotted connector lines |
| Form scope | **All forms** in project (e.g. Survey, Report) | **All forms** | — |
| Form children | **Field names** as flat leaves (Q1, Q2, Name) | Flat **field-name leaves** (`formFieldNames`) | Validate multi-blank FIB edge cases vs C# |
| **Variables** | **Variables** node; `_InviteeID` first, then A–Z | **Variables** branch; recursive discovery (Q2) + `_InviteeID` pin, then A–Z (Q4) | — |
| Tree selection | Highlight on selected node | Leaf selection highlight | — |
| Drag to editor | Inserts `<<…>>` token | Drag **source** on leaves only | **No drop targets** (Phase 2) |
| Double-click insert | Yes | No | Phase 2 |
| **First-open collapse** | All form folders **and** Variables **collapsed** | **All form folders + Variables collapsed** (Q3) | — |
| Collapse persistence | Session only; **not** saved with project | Session `useState` only | Aligned |
| Panel width | **Draggable left margin** (`FieldsPanel.cs`) | **Fixed** ~280px | Phase 1b resize |
| Records / record-set nodes | When editing processes | Not implemented | Backlog |

**Impact:** Without Variables and all-form scope, process/document/mail-merge authoring cannot reference project state the way legacy designers expect. Without per-node collapse, DirtBowl-scale projects overflow the panel and hide top-level categories. Without column resize, long field names truncate. Without drag (or double-click) into editors, field insertion remains manual — acceptable for early slices, not for full authoring parity.

**C# source:** `TawalaDesigner/Code/TAWALA/ProjectUI/FieldsPalette.cs` (tree population, Variables, drag); `FieldsPanel.cs` (left-margin resize).

---

## 2. Multi-window / MDI architecture

**Legacy:** Designers can open Forms, Processes, and Documents in **separate MDI child windows** with multiple windows open at once. The **Windows** menu lists open children; the center pane hosts `Form - …`, `Process - …`, `Document - …` windows simultaneously.

**Browser Designer (`designer-web`):** Server-side layout uses a **single editor pane** — no MDI, no Windows menu, no parallel Form + Process + Document windows.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — shell layout (Project Explorer | MDI windows | Fields), Windows menu, MDI gap table
- `DESIGNER_STARTUP_AND_FORM_CANVAS.md` — Form canvas as center MDI child on Design tab
- `DESIGNER_DOCUMENT_EDITOR.md` — Document MDI child window
- `DESIGNER_PROCESS_STATEMENTS_IF.md` — Process MDI window

**Impact:** Blocks meaningful **large-project authoring** (e.g. full DirtBowl edit pass). Does **not** block using DirtBowl as a Preview / Deploy parity sample.

---

## 3. Forms ↔ Processes connection transparency

**Legacy:** Form ↔ Process links are explicit in **Form Properties** (PreProcess / PostProcess) and visible in the project tree. Processes are named and tied to forms in a way designers can inspect and navigate.

**Browser Designer:** **Phase 1 (July 2026)** reads `preProcess` / `process` on each form in project JSON and shows linked processes under the form in Project Explorer (`linkedProcessesForForm`). Deploy pipeline already serializes these fields. Still missing: **Form Properties** UI to edit links, process-editor **yellow connection banner**, and Edit → Connect/Disconnect Pre/Post-Process menu actions.

**Spec cross-refs:**
- `DESIGNER_UI_REFERENCE.md` — Form Properties (PreProcess / PostProcess), PostProcess terminology
- `DESIGNER_MENU_SPEC.md` — explorer selection drives Items vs Statements columns; linked process icons
- This doc **§4** — explorer tree presentation (Phase 1 core done; icon/label polish backlog)

**Impact:** Explorer navigation of form–process wiring is **partially** addressed; editing and in-editor confirmation of links remain gaps.

---

## 4. Collapsible Explorer menus

**Legacy (owner screenshots, July 2026):** Explorer sections **collapse** with **`[-]` / `[+]`** — vital for large **Forms** trees. Each **form** is a parent node (form grid icon) that expands to show linked processes as children:

- **Pre-process** — solid purple **gear** icon; process name as label (often `Pre-{FormName}`, sometimes descriptive e.g. `RetrieveAdminSetupVariables`).
- **Post-process** — **form icon + gear overlay**; label typically `Post-{FormName}`.

Forms, Processes, and Documents are separate collapsible folders. Dotted tree lines connect parents to children. Explorer toolbar above the tree has seven icon buttons (New Form/Process/Document, reorder, start point, block back). See `DESIGNER_MENU_SPEC.md` § Project Explorer and reference images listed there.

**Browser Designer — Phase 1 (July 2026, local `designer-web`):** Partial implementation landed:

| Done (Phase 1 core) | Not yet (polish / backlog) |
|---------------------|----------------------------|
| Collapsible **Forms** / **Processes** / **Documents** section folders | WinForms-style **`[-]` / `[+]`** (today: ▼/▶) |
| Linked processes nested under each form (`linkedProcessesForForm` from `preProcess` / `process` on form JSON) | **Gear** and **form+gear** icons on child nodes |
| Per-form expand/collapse for linked process children | Process **name only** in tree (legacy); browser uses `Pre: name` / `Post: name` text prefixes |
| Selection opens process same as flat Processes list | Dotted connector lines; full **7-icon** explorer toolbar (browser: F/P/D text only); rename, reorder, ★/block-back toolbar toggles |
| **First-open default:** all form nodes **expanded** (Q3); Pre/Post-process **gear** + form/folder/document node icons | |
| | **MDI** multiple simultaneous Form + Process windows; yellow **connection banner** in process editor (§6, `DESIGNER_PROCESS_STATEMENTS_IF.md`) |

**Owner decisions — July 2026 (Explorer collapse):**

| Rule | Legacy / owner | Browser Phase 1 |
|------|----------------|-----------------|
| Collapse persistence | **Session only** — not saved when project is saved/closed | Aligned (`useState` in `ProjectExplorer.tsx`) |
| **First open — Forms** | Each form node **expanded** (Pre/Post children visible when linked) | **Done** — `expandedForms` seeds with all form names (re-seeds on form-set change) |
| **First open — section folders** | Forms / Processes / Documents folders expanded (owner SportsDashboards screenshot) | Aligned — `expanded.forms` / `processes` / `documents` default `true` |

**Image cross-ref:** `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Project_Explorer-16cbb1fb-21b3-4372-ae93-f067205db3de.png` (forms expanded with `[-]` on each form).

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — Project Explorer (toolbar, icons, examples, MDI banner), browser gap table
- `DESIGNER_UI_REFERENCE.md` — project tree / explorer panel structure
- This doc **§1** — Fields panel tree (separate from Project Explorer)
- This doc **§3** — Form ↔ Process linking in Form Properties (explorer shows the link; banner confirms in process MDI)

**Impact:** Phase 1 makes form–process hierarchy **navigable** in the browser; polish pass needed before parity with legacy scanability on DirtBowl-scale projects.

---

## 5. Properties panel vs popup dialogs

**Legacy:** **Item Properties** and **Form Properties** open as **popup dialogs** when an item or form is selected — they do not permanently consume layout space.

**Browser Designer:** A **permanent Properties panel** wastes too much horizontal space compared to legacy popup-on-select behavior.

**Spec cross-refs:**
- `DESIGNER_UI_REFERENCE.md` — Form Properties, Item Properties dialogs (confirmed fields)
- `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` — validation popups (popup pattern elsewhere in legacy UI)

**Impact:** Editing density on smaller screens; diverges from legacy interaction model designers expect.

---

## 6. Multiple menu bars (context-sensitive toolbars)

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
| Fields palette drag targets | `ROADMAP.md` Phase 4; **§1** above (Phase 2 of Fields panel) |
| `.tawala` import | `ROADMAP.md` Backlog |

---

*Last updated: July 2026 — Owner decisions Q1–Q4 (Fields leaves, variable scope, collapse defaults, `_InviteeID` ordering); Phase 1 browser status refreshed in §1 and §4 gap tables.*

---

## Follow-up code changes — implemented (Explorer Phase 1, July 2026)

| File | Change | Status |
|------|--------|--------|
| `designer-web/src/lib/projectModel.ts` | Extended `collectProjectVariables` beyond Set/Append: recursively scans **Get**, **ForEach**, **Delete**, **If** conditions, and Where clauses for plain variable references (assignment targets + `<<variable>>` operands), parity with legacy `Process.Variables` / `Project.AllVariables`. Colon-qualified record / record-set names stay excluded. Sorts with **`_InviteeID` pinned first**, then `localeCompare`. | **Done** (Q2, Q4) |
| `designer-web/src/components/FieldsPalette.tsx` | `defaultCollapsed()` adds **all** `form:*` keys (not only non-active forms) so **every** form folder and **Variables** start collapsed on first open / project load. Session-only (`useState`), not persisted. | **Done** (Q3) |
| `designer-web/src/components/ProjectExplorer.tsx` | Initializes `expandedForms` with **all** `project.forms.map(f => f.name)` (and re-seeds on form-set change) so linked Pre/Post children are visible on first open. Added Pre/Post-process gear icons, form/folder/document node icons. | **Done** (Q3) |

*DirtBowl stress-test check (July 2026): Variables node lists 76 entries — `_InviteeID` first, remainder alphabetical, and now includes ForEach/Get-only references (e.g. `Candidate`, `Coach`, `Count`, `X`) that Set/Append scanning alone missed.*
