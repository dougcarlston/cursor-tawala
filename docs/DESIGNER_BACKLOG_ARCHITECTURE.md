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
2. **Drag-and-drop Fields → canvas/editors** — documented prerequisite for serious authoring. **Phase 2 (July 2026) — landed (first pass):** leaves drag **and** double-click to insert `<<name>>` at the caret in form-item property editors (item label, Text content, FIB **question**, MCQ question) and the rich-text surface (Text + Document body); process command JSON textarea accepts the token via native text drop. Deferred: Records/RecordSet drop context, structured itemization token editor, per-editor validation on invalid drops.

   **Allowed vs forbidden drop targets (owner rule, July 2026):**

   | Target | Field/variable drop? | Notes |
   |--------|----------------------|-------|
   | **Text** box content (rich-text surface + single-line) | ✅ Allowed | |
   | **Heading** box content / item label | ✅ Allowed | |
   | **FIB question** text | ✅ Allowed | The prompt shown above the response fields |
   | **MCQ** question | ✅ Allowed | |
   | Document body (rich-text) | ✅ Allowed | |
   | Process command **JSON** textarea | ✅ Allowed | via native `text/plain` token |
   | **Form / Process / Document name** (Explorer rename) | ❌ Forbidden | Name/identifier field; drop rejected (`fieldDropRejectHandlers`) |
   | **FIB capture-box labels** ("Label on form") | ❌ Forbidden | The labels on the actual input blanks — drop rejected |
   | **FIB stored name** (field identifier) | ❌ Forbidden | Used in processes/tables; drop rejected |

   **Deferred — FIB fine-grained drop map (BLOCKED on WYSIWYG item redesign):** distinguishing FIB question text vs blanks vs capture-label areas as separate drop zones is deferred until Forms/Documents items are true **WYSIWYG windows on the canvas**, where question text is visible in the item window but capture labels are not. Owner: this is hard to finalize/explain in the current non-WYSIWYG properties UI. Until then only the FIB **question** accepts drops and all capture-box/stored-name fields reject them. See **§ FIB / layout backlog** and `DESIGNER_MENU_SPEC.md` § Fields panel.
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
| Drag to editor | Inserts `<<…>>` token | **Drag source + drop targets** — inserts `<<name>>` at caret in item property editors, rich-text surface, process JSON | Records/RecordSet drop context; invalid-drop validation |
| Double-click insert | Yes | **Yes** — inserts into last-focused editor field (`insertFieldIntoActiveTarget`) | — |
| **First-open collapse** | All form folders **and** Variables **collapsed** | **All form folders + Variables collapsed** (Q3) | — |
| Collapse persistence | Session only; **not** saved with project | Session `useState` only | Aligned |
| Panel width | **Draggable left margin** (`FieldsPanel.cs`) | **Fixed** ~280px | Phase 1b resize |
| Records / record-set nodes | When editing processes | Not implemented | Backlog |

**Impact:** Without Variables and all-form scope, process/document/mail-merge authoring cannot reference project state the way legacy designers expect. Without per-node collapse, DirtBowl-scale projects overflow the panel and hide top-level categories. Without column resize, long field names truncate. Without drag (or double-click) into editors, field insertion remains manual — acceptable for early slices, not for full authoring parity.

**C# source:** `TawalaDesigner/Code/TAWALA/ProjectUI/FieldsPalette.cs` (tree population, Variables, drag); `FieldsPanel.cs` (left-margin resize).

---

## 2. Multi-window / MDI architecture

**Legacy:** Designers can open Forms, Processes, and Documents in **separate MDI child windows** with multiple windows open at once. The **Windows** menu lists open children; the center pane hosts `Form - …`, `Process - …`, `Document - …` windows simultaneously.

**Browser Designer (`designer-web`) — Pass 1 (July 2026): implemented.** The center canvas is now a **window manager** (`src/components/mdi/`). **Single-clicking** a form / process / document leaf in Project Explorer **opens** (or **focuses**) an overlapping MDI child window. Windows **drag** by title bar, **resize** from all edges/corners, **stack** with click-to-front z-order, **minimize** to a bottom taskbar, and **close**. Title bars read `Form - Name` / `Process - Name` / `Document - Name` (matching the legacy screenshot). Each window embeds the **real single-pane editor** (`FormEditor` / `ProcessEditor` / `DocumentEditor`), not a placeholder.

| Pass 1 — done | Deferred (Pass 2+) |
|---------------|--------------------|
| **Single-click** Explorer leaf opens window; re-open focuses existing window (`id = kind:name`, no duplicate) | **Windows menu** listing open children |
| Drag (title bar), resize (8 handles), click-to-front z-order | Yellow **connection banner** in process windows (§3, §6) |
| Minimize → taskbar; restore; close | Properties **popup** migration (§5) |
| Embedded real editors; title `Type - Name`; **cascade down-right** from previous window (resets when canvas empties) | Persist window layout to the project file |
| Windows re-key on entity **rename**; cleared on new/open project | **Snap / cascade / tile** commands; maximize |
| **Canvas starts EMPTY** (no auto-open); placeholder shows until first click | Per-window editor **tab** state (Design/Preview and selected item are still **global** — see limitations) |
| **Forms open in Design mode**; Process/Document windows have no Design/Preview tab | |

**Owner decisions — July 2026 (MDI window-open behavior; confirmed):**

| # | Decision | Legacy / owner rule | Pass 1 browser status |
|---|----------|---------------------|-----------------------|
| D1 | **Single-click open + cascade** | Single-click a Form / Process / Document in Explorer → opens on the canvas, raised to the top (focused). Each new window **cascades down-and-right** from the previous one, far enough to reveal the prior window's title bar (~24–30px). Re-opening an already-open entity **focuses** it (no duplicate). | **Done** — `openWindow` opens/raises on single-click; `cascadeIndex` steps each new window by `WINDOW_CASCADE_STEP` (28px), wraps after 8, and resets to origin when the canvas empties. |
| D2 | **Start empty** | On project load the canvas has **no windows open** (do not auto-open the first form). Empty-canvas placeholder shows when nothing is open. | **Done** — initial state, `newProject`, `importJson`, `setProject` all leave `openWindows: []`; `CanvasWindowManager` renders the "Select a form / process / document…" placeholder. |
| D3 | **Title format + Forms open in Design** | Title bar reads `{Type} - {name}` (e.g. `Form - Registration`, `Process - admin-post`, `Document - WhosComing`). **Forms always open in Design** (not Preview). **No Preview** for Processes or Documents — only Forms have Design/Preview tabs. | **Done** — MDI title bar already `Type - Name`; `openWindow` resets the (still-global) `editorTab` to `design` whenever a form opens; Process/Document editors have no Design/Preview tabs. Removed the redundant inner `Form — Name` heading (window title bar is now the single heading). |

**Known Pass 1 limitations (documented, acceptable for a shell):**
- `editorTab` (Design/Preview) and `selectedItemIndex` live in the **global** store, so all open **form** windows share the active Design/Preview tab and highlighted item. Per-window editor state is a Pass 2 refactor. **D3 mitigation:** opening a form always **resets the shared tab to Design**, so a newly opened form window is never left showing another form's Preview — but switching one form window to Preview still affects the others until Pass 2.
- Each embedded `FormEditor` registers a global Delete/Backspace key handler; duplicate handlers are **safe** (guarded on `selectedItemIndex === null`) but should be de-duplicated in Pass 2.
- No maximize button yet (minimize + close only).

**State/actions (`projectStore.ts`):** `openWindows: DesignerWindow[]` (`id, kind, name, x, y, w, h, z, minimized`), `activeWindowId`, and `openWindow / closeWindow / focusWindow / minimizeWindow / restoreWindow / setWindowBounds`.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — shell layout (Project Explorer | MDI windows | Fields), Windows menu, MDI gap table
- `DESIGNER_STARTUP_AND_FORM_CANVAS.md` — Form canvas as center MDI child on Design tab
- `DESIGNER_DOCUMENT_EDITOR.md` — Document MDI child window
- `DESIGNER_PROCESS_STATEMENTS_IF.md` — Process MDI window
- Owner MDI screenshot: `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Forms_-_Parent_Coachs-d61cde9a-0b21-48f6-a29f-c8c0a2a9ead1.png`

**Impact:** Pass 1 unblocks **large-project authoring** (open Registration + a process + a document side by side). Full parity (Windows menu, connection banners, per-window tab state, layout persistence) remains Pass 2+.

---

## 3. Forms ↔ Processes connection transparency

**Legacy:** Form ↔ Process links are explicit in **Form Properties** (PreProcess / PostProcess) and visible in the project tree. When a process is **attached** to a form, the Designer **auto-names** it `Pre-Process1` / `Post-Process1` (or the next available number); the designer may **rename** to any label and **need not keep** the `Pre-` / `Post-` prefix. **Pre vs Post role** is determined by the form linkage (`preProcess` / `process` on form JSON) and by **explorer icons** (gear vs form+gear) — **not** by parsing the process name (e.g. `RetrieveAdminSetupVariables` is a Pre-process with no `Pre-` prefix).

**Browser Designer:** **Phase 1 (July 2026)** reads `preProcess` / `process` on each form in project JSON and shows linked processes under the form in Project Explorer (`linkedProcessesForForm`), with Pre/Post **icons** driven by linkage. Deploy pipeline already serializes these fields. Still missing: **Form Properties** UI to edit links (including **auto-name on attach**), process-editor **yellow connection banner**, and Edit → Connect/Disconnect Pre/Post-Process menu actions.

**Spec cross-refs:**
- `DESIGNER_UI_REFERENCE.md` — Form Properties (PreProcess / PostProcess), PostProcess terminology
- `DESIGNER_MENU_SPEC.md` — explorer selection drives Items vs Statements columns; linked process icons
- This doc **§4** — explorer tree presentation (Phase 1 core done; icon/label polish backlog)

**Impact:** Explorer navigation of form–process wiring is **partially** addressed; editing and in-editor confirmation of links remain gaps.

---

## 4. Collapsible Explorer menus

**Legacy (owner screenshots, July 2026):** Explorer sections **collapse** with **`[-]` / `[+]`** — vital for large **Forms** trees. Each **form** is a parent node (form grid icon) that expands to show linked processes as children:

- **Pre-process** — solid purple **gear** icon (top-associated child); process **name** as label (default on attach: `Pre-ProcessN`; designer may rename — role is **not** inferred from the name string).
- **Post-process** — **form icon + gear overlay** (bottom-associated child); default on attach: `Post-ProcessN`; labels often `Post-{FormName}` after rename in older projects.

Forms, Processes, and Documents are separate collapsible folders. Dotted tree lines connect parents to children. Explorer toolbar above the tree has seven icon buttons (New Form/Process/Document, reorder, start point, block back). See `DESIGNER_MENU_SPEC.md` § Project Explorer and reference images listed there.

**Browser Designer — Phase 1 (July 2026, local `designer-web`):** Partial implementation landed:

| Done (Phase 1 core) | Not yet (polish / backlog) |
|---------------------|----------------------------|
| Collapsible **Forms** / **Processes** / **Documents** section folders | WinForms-style **`[-]` / `[+]`** (today: ▼/▶) |
| Linked processes nested under each form (`linkedProcessesForForm` from `preProcess` / `process` on form JSON) | **Auto-name on attach** (`Pre-ProcessN` / `Post-ProcessN`) when Connect Pre/Post UI ships |
| Per-form expand/collapse for linked process children | Dotted connector lines; full **7-icon** explorer toolbar (browser: F/P/D text only); rename, reorder, ★/block-back toolbar toggles |
| Process **name** labels; Pre/Post role from JSON linkage + **gear** / **form+gear** icons (not name prefix) | |
| Selection opens process same as flat Processes list | **MDI** multiple simultaneous Form + Process windows; yellow **connection banner** in process editor (§6, `DESIGNER_PROCESS_STATEMENTS_IF.md`) |
| **First-open default:** all form nodes **expanded** (Q3); form/folder/document node icons | |

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

## 7. Panel docking & resize (Explorer ↔ Items / Statements)

**Legacy (owner observation, July 2026):** Dragging the **right edge** of the **Project Explorer** column widens or narrows that column. The **Items** palette (form selected) and **Statements** palette (process selected) — both docked on the Explorer’s **right side** — **move and resize together** with the Explorer. Those middle-column palettes **cannot** be repositioned, widened, or expanded **independently** of the Explorer column today.

**Future capability (backlog — not Phase 1):** Independent resize, move, and expand of Items / Statements relative to the Project Explorer column. Until then, browser and legacy shells treat the Explorer + middle palette as a **coupled** left block.

**Spec cross-refs:**
- `DESIGNER_MENU_SPEC.md` — Project Explorer § Panel resize; middle column (Items / Statements)
- This doc **§2** — MDI / shell layout (Explorer | Items/Statements | MDI | Fields)

**Impact:** Low urgency for DirtBowl parity; matters for designers who want a wide Items strip or a narrow Explorer without dragging both together. Owner: “we’ll get to that eventually.”

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

*Last updated: July 2026 — Owner decisions Q1–Q4 (Fields leaves, variable scope, collapse defaults, `_InviteeID` ordering); Phase 1 browser status refreshed in §1 and §4 gap tables; §7 Explorer ↔ Items/Statements panel coupling (backlog).*

---

## Follow-up code changes — implemented (Explorer Phase 1, July 2026)

| File | Change | Status |
|------|--------|--------|
| `designer-web/src/lib/projectModel.ts` | Extended `collectProjectVariables` beyond Set/Append: recursively scans **Get**, **ForEach**, **Delete**, **If** conditions, and Where clauses for plain variable references (assignment targets + `<<variable>>` operands), parity with legacy `Process.Variables` / `Project.AllVariables`. Colon-qualified record / record-set names stay excluded. Sorts with **`_InviteeID` pinned first**, then `localeCompare`. | **Done** (Q2, Q4) |
| `designer-web/src/components/FieldsPalette.tsx` | `defaultCollapsed()` adds **all** `form:*` keys (not only non-active forms) so **every** form folder and **Variables** start collapsed on first open / project load. Session-only (`useState`), not persisted. | **Done** (Q3) |
| `designer-web/src/components/ProjectExplorer.tsx` | Initializes `expandedForms` with **all** `project.forms.map(f => f.name)` (and re-seeds on form-set change) so linked Pre/Post children are visible on first open. Added Pre/Post-process gear icons, form/folder/document node icons. | **Done** (Q3) |

*DirtBowl stress-test check (July 2026): Variables node lists 76 entries — `_InviteeID` first, remainder alphabetical, and now includes ForEach/Get-only references (e.g. `Candidate`, `Coach`, `Count`, `X`) that Set/Append scanning alone missed.*

---

## Follow-up code changes — implemented (Fields Phase 2 drag-and-drop, July 2026)

| File | Change | Status |
|------|--------|--------|
| `designer-web/src/lib/fieldInsertion.ts` | New helper: `FIELD_DRAG_MIME` custom drag payload + `text/plain` `<<name>>` token, `hasFieldDrag`/`readFieldDragName` (dragover-safe via `dataTransfer.types`), `insertTokenAtCaret`, and a module-level active-target registry (`setActiveFieldTarget` / `insertFieldIntoActiveTarget`) for double-click insert at last-focused editor. | **Done** (Phase 2) |
| `designer-web/src/components/FieldDropInputs.tsx` | `FieldTextInput` / `FieldTextArea` accept drops (shared `useFieldDropTarget`: caret `<<name>>` insertion, `field-drop-active` highlight, focus-time double-click target). Plus `fieldDropRejectHandlers` + `NameTextInput` that **reject** field drops on name/identifier fields (owner rule, July 2026). | **Done** (Phase 2) |
| `designer-web/src/components/FieldsPalette.tsx` | Leaves now emit the dual drag payload (`setFieldDragData`) and insert on **double-click** / Enter (`insertFieldIntoActiveTarget`); tooltip explains drag/double-click. | **Done** (Phase 2) |
| `designer-web/src/components/FormItemProperties.tsx` | Drop targets wired for item **label**, single-line **Content** (heading/MCQ text), FIB **question**, and MCQ **question**. FIB **capture-box label** ("Label on form") and **stored name** now use `NameTextInput` and **reject** drops (owner rule). | **Done** (Phase 2) |
| `designer-web/src/components/ProjectExplorer.tsx` | Inline Form/Process/Document **rename** input rejects field drops (`fieldDropRejectHandlers`) — name fields must not accept `<<field>>` tokens. | **Done** (Phase 2) |
| `designer-web/src/components/RichTextEditor.tsx` | contentEditable surface accepts field drops at the drop point (`caretRangeFromPoint` / `caretPositionFromPoint`) and registers as the double-click target on focus — covers Text content and Document body. | **Done** (Phase 2) |
| `designer-web/src/styles.css` | `.field-drop-active` dashed-accent highlight for valid drop targets. | **Done** (Phase 2) |

*Deferred (documented gaps): **FIB fine-grained drop map (BLOCKED on WYSIWYG item redesign)** — question text vs blanks vs capture-label drop zones can't be finalized until Forms/Documents items are true WYSIWYG windows on the canvas; Records / RecordSet tree nodes as drop context; drop into the structured **MULTIPLE QUESTION LIST** itemization token editor; invalid-drop validation/error surfacing; process command editing is JSON-textarea only (native token drop works, no structured condition field yet).*

---

## Follow-up code changes — implemented (MDI window shell Pass 1, July 2026)

| File | Change | Status |
|------|--------|--------|
| `designer-web/src/store/projectStore.ts` | Added MDI window model + actions: `openWindows` / `activeWindowId` state and `openWindow` (open-or-focus by `kind:name`, cascade offset), `closeWindow`, `focusWindow`, `minimizeWindow`, `restoreWindow`, `setWindowBounds`. Windows cleared on `setProject` / `newProject` / `importJson`; auto-open first form on load; **re-keyed on rename** (`renameForm` / `renameProcess` / `renameDocument`). | **Done** (Pass 1) |
| `designer-web/src/components/mdi/CanvasWindow.tsx` | Single MDI child: title bar drag, 8 resize handles (edges + corners, min 320×180), click-to-front, minimize/close controls, title `Type - Name`. Body embeds real `FormEditor` / `ProcessEditor` / `DocumentEditor`. | **Done** (Pass 1) |
| `designer-web/src/components/mdi/CanvasWindowManager.tsx` | Canvas host: renders visible windows absolutely inside `.designer-center`, minimized taskbar, empty-state placeholder. | **Done** (Pass 1) |
| `designer-web/src/App.tsx` | Center pane now renders `<CanvasWindowManager />` instead of the single selection-driven editor. | **Done** (Pass 1) |
| `designer-web/src/components/ProjectExplorer.tsx` | Form / process / document leaf clicks now call `openWindow(kind, name)` (open-or-focus) instead of bare `setSelection`. | **Done** (Pass 1) |
| `designer-web/src/styles.css` | `.mdi-*` window chrome: surface backdrop, title bar (active blue / inactive grey), controls, resize handles, taskbar. | **Done** (Pass 1) |

*Verified: `tsc -b && vite build` clean (July 2026). Live browser walkthrough not run in this session — Cursor IDE browser tab was unavailable (see CHAT_HANDOFF manual test steps). Deferred to Pass 2: Windows menu, process connection banner, per-window editor tab/item state, layout persistence, snap/cascade/tile, maximize.*

### Owner window-open decisions (D1–D3, July 2026)

Applied on top of the Pass 1 shell (commit `e88d3ba`) — **not committed** pending owner review of the Pass 1 checklist.

| File | Change | Decision |
|------|--------|----------|
| `designer-web/src/store/projectStore.ts` | **Start empty:** removed the seeded `Form 1` window and the auto-open in `newProject` / `importJson`; initial `openWindows: []`, `activeWindowId: null`. **Cascade:** added `cascadeIndex` state — new windows offset via `cascadeBounds(cascadeIndex)` (down-right, wraps after `WINDOW_CASCADE_WRAP`), incremented per new window, reset to 0 on `closeWindow` when the canvas empties and on `setProject` / `newProject` / `importJson`. **Forms → Design:** `openWindow` resets the global `editorTab` to `"design"` when a form opens (both open and re-focus paths); Process/Document leave it untouched. | D1, D2, D3 |
| `designer-web/src/components/FormEditor.tsx` | Removed the redundant inner `Form — {name}` heading (`.form-window-title`); the MDI window title bar (`Form - Name`) is now the single window heading. Editor body starts at the Design/Preview tabs. | D3 |
| `designer-web/src/styles.css` | Dropped the now-unused `.form-window-title` rule. | D3 |

*Verified: `tsc -b` clean (July 2026). Manual browser walkthrough left to the owner (see CHAT_HANDOFF § "MDI window-open decisions"). Title-bar format `Type - Name` already matched the owner spec (D3) — no change needed to `CanvasWindow.tsx`. Process/Document windows already had no Design/Preview tabs (D3). Per-window Design/Preview + selected-item state remains **global** (Pass 2); D3 only guarantees the initial/default is Design when a form opens.*
