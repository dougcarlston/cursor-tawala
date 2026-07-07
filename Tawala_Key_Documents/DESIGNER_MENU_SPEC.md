# Tawala Designer — Menu & Shell Spec (hands-on)

*Captured from live **Tawala Project Designer #251 (DEV)** on the legacy Windows app, June 2026.*  
*Complements `DESIGNER_UI_REFERENCE.md` (XML/demo reconstruction) and `designer-web/` (browser reimplementation).*

---

## Application shell

| Region | Title | When visible |
|--------|-------|----------------|
| Left | **Project Explorer** | View → Project Explorer ✓ |
| Middle-left | **Items** or **Statements** | View → Items Palette ✓; content depends on selection |
| Center | MDI windows (`Form - …`, `Process - …`, `Document - …`) | Design / Preview tabs on forms |
| Right | **Fields** | View → Fields Palette ✓ |
| Bottom | **Status bar** | View → Status Bar ✓ |

**Title bar:** `Tawala Project Designer #251 (DEV) - [Project Name]`

**Menu bar:** File · Edit · View · Insert · Format · Tables · Tools · Windows · Help

---

## Project Explorer

**Owner reference screenshots (July 2026, SportsDashboards-scale project):**

| Image | Path | Shows |
|-------|------|-------|
| Explorer close-up | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Project_Explorer-16cbb1fb-21b3-4372-ae93-f067205db3de.png` | Forms tree with linked Pre/Post processes, explorer toolbar icons |
| Explorer toolbar + Toggle Start Point tooltip | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Project_Explorer_Menu-b12501c1-827e-463b-a59c-7141d2d9c4ab.png` | Full 7-icon explorer toolbar (New Form/Process/Document, Move Up/Down, Toggle Form Starting Point, Block Back Button) with separators; `Toggle Form Starting Point` native tooltip over a selected form |
| Full MDI layout | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Forms_-_Parent_Coachs-d61cde9a-0b21-48f6-a29f-c8c0a2a9ead1.png` | Form + two Process windows, Items palette, Fields panel, connection banners |

Panel title bar: **Project Explorer** (blue title bar, white text). Tree uses **dotted connector lines** between parent and child nodes (WinForms `TreeView`).

### Explorer toolbar

Icons sit in a strip **directly above** the tree. Hover shows the written name. Icons are active/disabled based on project state.

| # | Name | Icon (legacy) | When visible / enabled |
|---|------|---------------|------------------------|
| 1 | New Form | Form page with yellow star/sparkle | Always (with 2–3 when project exists) |
| 2 | New Process | Purple gear | Always |
| 3 | New Document | Paper / document page | Always |
| 4 | Move Node Up | ↑ | After nodes exist; greyed until reorder possible |
| 5 | Move Node Down | ↓ | Same as up |
| 6 | Toggle Form Starting Point | Flag overlay on form icon | **Form selected only**; toggle on/off |
| 7 | Block Back Button | Red ← overlay on form icon | **Form selected only**; toggle on/off |

- Icons **1–3** are the only active ones until forms/processes/documents exist.
- Icons **1–5** show once nodes exist; **4–5** grey until there are siblings to reorder.
- Icons **6–7** appear only when a **form** node is selected (hidden for Process or Document).
- July 2026 close-up crop shows icons **1–5** clearly; **6–7** require a selected form node (not shown in that crop).

**New nodes:** Default names (`Form 1`, `Process 2`, …) in the correct folder.

**Rename:** Slow double-click on node name, or single-click if already highlighted → inline edit. Duplicate name reverts to default; empty name cancels.

### Tree structure and icons

Top-level folders under the project root: **Forms**, **Processes**, **Documents** (each folder collapses with **`[-]`** / **`[+]`**).

**Form nodes (under Forms):**

| Node | Icon | Collapse |
|------|------|----------|
| Form name | Small window with internal grid lines (`Form_InTree`; overlays for ★ start point, pre-populate, block-back) | **`[-]`** when expanded and has linked processes (or always expandable); **`[+]`** when collapsed |
| Linked **Pre-process** (child) | **Solid purple gear** (`Form_PreProcess`) | Leaf — no collapse box |
| Linked **Post-process** (child) | **Form icon + small purple gear** overlaid bottom-right (`Form_PostProcess`) | Leaf — no collapse box |

**Child order under a form:** Pre-process first (if any), then Post-process (if any). A form may have only a Post-process, only a Pre-process, or both.

**Naming (owner correction, July 2026):** Child nodes show the **process name** as stored in the project — not a `Pre:` / `Post:` text prefix in the label.

When a process is **attached to a Form** (connected as Pre- or Post-process), the Designer **automatically assigns** a default name:

| Role | Default name pattern |
|------|----------------------|
| Pre-process | `Pre-Process1` (or `Pre-Process2`, … next available number) |
| Post-process | `Post-Process1` (or `Post-Process2`, … next available number) |

**Why auto-naming:** Processes are often created **before** they are attached to a form; the default name helps the designer track intended placement.

The designer **may rename** the process to **any** name — they **do not have to keep** the `Pre-` or `Post-` prefix in the label. **Pre vs Post role is not encoded in the name string** — it comes from the form’s `preProcess` / `process` linkage in project JSON and from **tree icons** (see table above):

| Role | Icon (placement semantics) |
|------|----------------------------|
| Pre-process | Solid purple **gear** (top / top-associated child position) |
| Post-process | **Form + gear** overlay (bottom / bottom-associated child position) |

**Do not infer** Pre vs Post from the process **name** alone (e.g. `RetrieveAdminSetupVariables` has no `Pre-` prefix but is a Pre-process under `NavigationToPaymentByCheck`).

| Name pattern | Example | Notes |
|--------------|---------|-------|
| Auto-name on attach | `Pre-Process1`, `Post-Process2` | Default when linking; designer may rename immediately |
| `Pre-{FormName}` | `Pre-ParentCoaches`, `Pre-ShowAllRosters` | Common after rename or legacy projects |
| Descriptive (no prefix) | `RetrieveAdminSetupVariables` | Pre-process under `NavigationToPaymentByCheck` — **gear** icon, not name, marks role |
| `Post-{FormName}` | `Post-ParentCoaches`, `Post-NavigationToPaymentByCheck` | Common Post-process label |
| Post only | `Post-DuplicateRegistrationCheck` | Under `DuplicateRegistrationCheck`; no Pre-process child |

**Examples from July 2026 explorer screenshot:**

```
Forms  [-]
  ParentCoaches  [-]
    (gear) Pre-ParentCoaches
    (form+gear) Post-ParentCoaches
  NavigationToPaymentByCheck  [-]
    (gear) RetrieveAdminSetupVariables
    (form+gear) Post-NavigationToPaymentByCheck
  RefreshAdminDash  [-]
    (form+gear) Post-RefreshAdminDash
  UtilityToSetPlayerAges  [-]
    (form+gear) Post-UtilityToSetPlayerAges
  ShowAllRosters  [-]
    (gear) Pre-ShowAllRosters
    (form+gear) Post-ShowAllRosters
  PlayerCommunicator  [-]
    (gear) Pre-PlayerCommunicator
    (form+gear) Post-PlayerCommunicator
  DuplicateRegistrationCheck  [-]
    (form+gear) Post-DuplicateRegistrationCheck
Processes  [-]
  … (flat list of all processes — same names as under forms)
Documents  [-]
  …
```

**Standalone Processes folder:** Every process appears here as well as under its connected form (if linked). Selecting a process from **either** location is equivalent — middle column shows **Statements**, MDI opens `Process - {name}`.

**Selection:** Standard WinForms tree row highlight when a node is selected (not visible in the July 2026 close-up crop; confirmed in C# `ProjectExplorer.cs` `AfterSelect`).

### Collapse behavior (owner decisions, July 2026)

| When | Behavior |
|------|----------|
| **First open** (project load) | **Forms** section expanded; **each form node expanded** so linked Pre/Post processes are visible (`[-]` on forms with children in legacy screenshot). |
| **During session** | User collapse/expand via **`[-]` / `[+]`** (or ▼/▶ in browser) **persists until project close**. |
| **Save / close / reopen** | Collapse state is **not** saved — tree returns to first-open defaults. |

**Browser gap:** `ProjectExplorer.tsx` initializes `expandedForms` as an **empty** `Set`, so per-form nodes start **collapsed** instead of expanded. See `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` §4.

**Image cross-ref:** `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Project_Explorer-16cbb1fb-21b3-4372-ae93-f067205db3de.png`.

### Panel resize (legacy layout, July 2026)

Dragging the **right edge** of the Project Explorer column resizes the Explorer; the **Items** / **Statements** middle column (see below) is docked to that edge and **moves and resizes with it**. Those palettes **cannot** be widened, moved, or expanded independently of the Explorer today.

**Backlog:** Independent resize/move/expand of Items and Statements relative to the Explorer — `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` **§7**.

### MDI workspace (backlog context — not Phase 1)

From the full MDI screenshot (`ParentCoaches` example):

- **Multiple MDI children** open at once: e.g. `Form - ParentCoaches` ( **Design** / **Preview** tabs), `Process - Pre-ParentCoaches`, `Process - Post-ParentCoaches` — overlapping tiled windows in the center pane.
- **Items** palette column sits between Project Explorer and MDI (8 form item buttons when a form is active).
- **Fields** panel on the right lists all project forms and variables (see Fields section below).
- Each **Process** window has a **yellow connection banner** immediately below the title bar (clickable):

| State | Banner text |
|-------|-------------|
| Not connected as Post-Process | *Not connected as Post-Process to any Form. Click here to change.* |
| Connected as Post-Process | *Connected as Post-Process to Form 'ParentCoaches'. Click here to change.* |

Pre-processes that are not Post-processes show the **Not connected** banner text (e.g. `Process - Pre-ParentCoaches` in the screenshot). Full banner behavior: `DESIGNER_PROCESS_STATEMENTS_IF.md` § Process connection.

---

## Middle column (Items / Statements)

| Explorer selection | Column title | Contents |
|--------------------|--------------|----------|
| **Form** | **Items** | 8 form item types (icons + tooltips) |
| **Process** | **Statements** | 9 statement types (text only, no icons/tooltips) |
| **Document** | *(empty)* | No palette — see `DESIGNER_DOCUMENT_EDITOR.md` |

**Document open:** middle column stays blank; rich-text **format toolbar** (row 2) and document-specific **Insert** / **Format** / **Tables** / **View** menu merges apply instead. **Fields** palette remains on the right for drag-in field tokens and **fx** functions.

### Items (form selected)

**Top group**

| # | Item | Tooltip (rollover) |
|---|------|-------------------|
| 1 | Heading | Add a heading item to the form to highlight sections of the form. | `DESIGNER_FORM_ITEMS_HEADING.md` |
| 2 | Text | Add a text item for displaying formatted text. |
| 3 | Fill in the Blank | Add questions with one or more blanks. |
| 4 | Multiple Choice | Add single or multiple choice question. |
| 5 | File Uploader | Add a form item that requests a file upload from the user and includes room for a paragraph of instructions. |

> **Owner note (June 2026):** On the **Jan 24, 2011** Designer build available on the reference PC, **File Uploader is not implemented** (palette entry may exist but does not work). A later build clearly shipped it — e.g. **SportsDashboards** projects use `<file>` items for division uploads. Until a newer Designer is found, document File Uploader from **project XML** and **`TawalaDesigner` source** (`FileUploadItem.cs`, `FileUploadItemView.cs`). See `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` § File Uploader (deferred).

*(line break)*

**Bottom group**

| # | Item | Tooltip |
|---|------|---------|
| 6 | Hidden Field | Add hidden field to store additional data associated with the form. |
| 7 | Page Break | Add a page break which causes a submit button to appear. |
| 8 | Skip Instructions | Add processing instructions to skip over form items. |

### Statements (process selected)

Grouped with visual gaps (same order as **Insert → Process**):

| Group | Statements |
|-------|------------|
| 1 | **If** |
| | *(gap)* |
| 2 | **Show**, **Send** |
| | *(gap)* |
| 3 | **Append** |
| | *(gap)* |
| 4 | **Get**, **ForEach**, **Delete** |
| | *(gap)* |
| 5 | **Set** |
| | *(gap)* |
| 6 | **Comment** |

Clicking a statement button inserts at the cursor in the process editor.

**If statement:** `DESIGNER_PROCESS_STATEMENTS_IF.md`  
**Show statement:** `DESIGNER_PROCESS_STATEMENTS_SHOW.md`  
**Send statement:** `DESIGNER_PROCESS_STATEMENTS_SEND.md`  
**Append, Get, ForEach, Delete, Set, Comment:** `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`  
**Document editor:** `DESIGNER_DOCUMENT_EDITOR.md`  
**Insert / functions:** `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`  
**Templates + deploy:** `DESIGNER_TEMPLATE_MATRIX.md` · `docs/ROADMAP.md`

---

## Fields panel (right)

**Purpose:** Tree of **field names** for `<<…>>` references in text, choices, processes, documents.

**UI chrome (owner screenshot, July 2026):** Title bar **Fields**; WinForms tree with **`[-]` / `[+]`** expand/collapse and **dotted connector lines**; selection highlight on the active node. A **grey vertical bar** on the panel’s **left edge** is the resize handle (drag horizontally to widen or narrow the column).

**Panel layout:** Docked on the **right** of the shell. **Column width** is user-adjustable by dragging the **left margin** (C# `FieldsPanel.cs` — `Padding.Left` hit zone, east-west resize cursor). Default width is narrow (~120px in designer resources); designers widen it when field names are long.

**Tree collapse (scalability):** Click **`[-]`** on a branch (e.g. **Survey**, **Variables**) to collapse that submenu; **`[+]`** expands again. With many forms and variables (DirtBowl-scale projects), folding unused branches is **required** to see more top-level categories within the fixed panel height.

**Example tree** (forms **Survey** and **Report** in one project):

```
Survey
  Q1
  Q2
  Name
Report
Variables
  _InviteeID
```

- One folder per **form** in the project (all forms shown, not only the open one).
- Children are **field names** (e.g. `Q1`, `Name`, `TitleText`) — the tokens used in `<<…>>`, not necessarily the design label shown on the canvas.
- In tree terms, a **leaf** is a **terminal node** (no children): one leaf per **unique capturable field** — each FIB blank with its own label is its own leaf; MCQ and Hidden Field each contribute one leaf. Owner confirmed July 2026 (Q1).
- Empty forms may show as empty folders (e.g. **Report** with no visible children).
- **Variables** lists project variables used in processes and skip instructions — not only Set/Append targets but also **Get**, **ForEach** (e.g. `Where [Record:Name] Equals [variable]`), and related statements. Owner tested ForEach variable references July 2026 (Q2).
- **`_InviteeID`** is always listed **first** under Variables; remaining variables are **alphabetical** (Q4).

### Collapse behavior (owner decisions, July 2026)

| When | Behavior |
|------|----------|
| **First open** | **All** form folders **collapsed**; **Variables** collapsed. |
| **During session** | Per-node **`[-]` / `[+]`** state remembered until project close. |
| **Save / close / reopen** | Collapse state **not** persisted — returns to all-collapsed defaults. |

Rationale (owner): ease of use on large projects — fold everything, then expand only what you need.

**Browser gap:** `FieldsPalette.tsx` `defaultCollapsed()` leaves the **active form expanded** on load; should collapse **all** forms. See `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` §1 Q3.

**Image cross-refs:** example tree below; full shell with Fields column — `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Forms_-_Parent_Coachs-d61cde9a-0b21-48f6-a29f-c8c0a2a9ead1.png`.

**Interactions**

| Action | Behavior |
|--------|----------|
| Drag field leaf | Drop into form or process editor → inserts `<<FieldReference>>` |
| Double-click field leaf | Insert at current editor focus |
| Invalid drop | Designer validates; may show error (e.g. XML parse error if dropped in wrong place) |

Only **field leaves** drag (not form folder headers or the Variables folder header).

**Browser Designer backlog:** `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` **§1** — Phase 1 tree (all forms + Variables + `[-]`/`[+]`) largely landed; remaining: variable discovery scope (Q2), collapse defaults (Q3), `_InviteeID` pin (Q4), left-margin resize (Phase 1b), drag-and-drop into editors (Phase 2).

### Owner decisions — July 2026 (Fields + Explorer)

| # | Decision | Spec detail |
|---|----------|-------------|
| Q1 | **Leaf = capturable field** | Terminal node per unique data-capture name under each form; not nested under canvas item labels. |
| Q2 | **Variable sources** | Include foreach/get (and related) process references, not only set/append assignment targets. |
| Q3 | **Collapse defaults** | Session-only persistence. Fields: all collapsed on first open. Explorer: all **Forms** expanded on first open. |
| Q4 | **`_InviteeID` order** | Always first in Variables list; others alphabetical. |

C# source: `FieldsPalette.cs` (`getSortedVariables`, form field leaves); `Project.cs` (`AllVariables` from `Process.Variables`); `FieldsPanel.cs` (resize).

---

## Menu: File

| Item | Shortcut | Icon | Notes |
|------|----------|------|-------|
| New Project... | Ctrl+N | Page + star | |
| Open Project... | Ctrl+O | Folder | |
| *(separator)* | | | |
| Add New → | | | Submenu |
| → Document | | Paper | |
| → Form | | Form grid + star | |
| → Process | | Gear + star | |
| *(separator)* | | | |
| Save | Ctrl+S | Floppy | |
| Save As... | | | |
| *(separator)* | | | |
| Deploy Project | | Globe + arrow | |
| *(separator)* | | | |
| Print Preview... | | | Greyed out |
| Print... | | | Greyed out |
| *(separator)* | | | |
| Exit | | | |

---

## Menu: Edit

| Item | Shortcut | Notes |
|------|----------|-------|
| Cut | Ctrl+X | |
| Copy | Ctrl+C | |
| Delete | Del | |
| Paste | Ctrl+V | Greyed when clipboard empty |
| Rename | F2 | Tree node rename |
| *(separator)* | | |
| Undo | Ctrl+Z | Greyed when nothing to undo |
| Redo | Ctrl+Y | Greyed when nothing to redo |
| *(separator)* | | |
| Connect Pre-Process | | Greyed when not applicable |
| Disconnect Pre-Process | | |
| Connect Post-Process | | Greyed when not applicable |
| Disconnect Post-Process | | |
| *(separator)* | | |
| Starting Point | | Checkbox; form only |
| Pre-populate With Last Entry | | Checkbox; form only |
| Block Back Button | | Checkbox; form only |

*Enable states vary with selection (example: form with Pre-/Post- processes connected).*

---

## Menu: View

All items are **show/hide toggles** (checked = visible):

| Item |
|------|
| Project Explorer |
| Fields Palette |
| *(separator)* |
| Toolbar |
| Status Bar |
| Items Palette |

---

## Menu: Insert (context-sensitive)

Insert contents depend on the **active MDI child** (Form, Process, or Document). Full dialogs and function catalog: **`DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`**.

### Enable rules summary (owner + source)

| Context | Always available | Requires rich-text focus |
|---------|------------------|--------------------------|
| **Form** | Top 7 form items (Heading … Skip Instructions) | Image…, Invitation…, Hyperlink…, Function… |
| **Process** | Statement types only | — |
| **Document** | Image…, Invitation…, Hyperlink…, Function… (per rules) | **Field** when Fields palette selection |

**Field** appears **only** on Document Insert — not listed on Form Insert.

**Form:** **Function…** and format-toolbar **fx** enable when cursor is in a **Text** item body. **Image…** enables in other rich-text contexts per `CanInsertImage`.

### A) Form selected

| Item | Notes |
|------|-------|
| Heading | |
| Text | |
| Fill in the Blank | |
| Multiple Choice | |
| Hidden Field | |
| Page Break | |
| Skip Instructions | |
| *(separator)* | | |
| Image... → | See Image submenu | Greyed unless cursor in rich text |
| Invitation... | | Greyed unless cursor in **Text** item |
| Hyperlink... | | Greyed unless cursor in **Text** item |
| Function... | | Greyed unless cursor in **Text** item |

Matches **Items** palette except **File Uploader** appears on palette but not in this Insert list. **Field** not present (Document only).

### B) Process selected

*(From Processes folder or Pre-/Post- under a Form.)*

Insert uses internal `*StatementView` names; palette uses short names:

| Insert label | Palette label |
|--------------|---------------|
| IfStatementView | If |
| *(separator)* | |
| ShowStatementView | Show |
| SendStatementView | Send |
| *(separator)* | |
| AppendStatementView | Append |
| *(separator)* | |
| GetStatementView | Get |
| ForEachStatementView | ForEach |
| DeleteStatementView | Delete |
| *(separator)* | |
| SetStatementView | Set |
| *(separator)* | |
| CommentStatementView | Comment |

### C) Document selected

| Item | Notes |
|------|-------|
| **Field** | Document only; enabled when field selected in Fields palette |
| Image... → | Submenu |
| Invitation... | Start-link invitation dialog |
| Hyperlink... | URL hyperlink dialog |
| Function... | Insert Function picker; requires ≥1 form in project |

**Insert → Image…**, **Invitation…**, **Hyperlink…**, **Function…** dialogs: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`.

---

## Menu: Format

Active when rich text has focus (e.g. text inside a form item). Some items greyed when nothing formattable is selected.

| Item | Shortcut | Notes |
|------|----------|-------|
| Bold | Ctrl+B | |
| Italic | Ctrl+I | |
| Underline | Ctrl+U | |
| Reset Formatting | | Eraser icon; greyed when inactive |
| *(separator)* | | |
| **Page Header…** | Project-wide banner — `DESIGNER_PAGE_HEADER.md` |
| Project Themes | → | Submenu |
| Color | → | Submenu |
| Tabs... | | Dialog |

*An earlier capture also listed **Styles** → submenu; not re-verified in June 2026 session.*

### Format → Color →

| Item | Opens |
|------|--------|
| Theme | Theme default color |
| Recent | Last used color |
| *(separator)* | |
| Choose... | Standard Windows **Color** dialog |

### Format → Project Themes →

Checklist; one theme active (✓). Themes observed:

Baseball, Basic Blue, Basic Green, Basic Pink, Basic Yellow, Big Q, Blue Lined Paper, Chocolate, Dark, **Default**, Dirtbowl, Dirtbowl - Variable Width, Full Moon, Green Lined Paper, Green Tea, Light Green, Lime, MVSC, Orange Swirl, Plain, Purple Haze, Red, Red Rays, Salzburg, Soup's On, Tennis, Tin Car Bell, Yellow

### Format → Tabs... dialog

Tab stops in **inches** for paragraph/FIB layout (maps to XML `tabPositions` / twips on export).

| Control | Behavior |
|---------|----------|
| Tab stop position | Numeric input (e.g. `0.00`) |
| List | Existing stops (example: 0.50–7.00 in 0.50" steps) |
| Set | Add stop at entered position |
| Clear | Remove selected (greyed if none selected) |
| Clear All | Remove all |
| OK / Cancel | |

---

## Menu: Tables

### Tables → Insert →

| Item | Notes |
|------|-------|
| Table | Active |
| Column Before | Greyed until inside a table |
| Column After | Greyed |
| Row Before | Greyed |
| Row After | Greyed |

### Tables → Delete →

| Item |
|------|
| Table |
| Column |
| Row |

---

## Menu: Tools

| Item |
|------|
| Project Manager... |

---

## Menu: Windows

| Item | Notes |
|------|-------|
| Cascade | |
| Tile Horizontally | |
| Tile Vertically | |
| Close All | |
| *(separator)* | |
| 1 Form - [name] | MDI window list; checkmark = active |
| 2 Document - [name] | |
| … | |

---

## Menu: Help

| Item |
|------|
| About |

---

## Main icon toolbar (row below menu bar)

Single horizontal toolbar (`mainToolStrip` in `DesignerView`). **11 icons**, four groups separated by vertical rules.

Screenshot reference: `assets/Icon_Tool_Bar-*.png`.

### Icons left → right

| # | Tooltip | Icon (description) | Default enabled | Notes |
|---|---------|-------------------|-----------------|-------|
| 1 | **New Project** | Document + purple gear + star | Yes | Same as File → New |
| 2 | **Open Project** | Yellow folder + blue arrow | Yes | Same as File → Open |
| 3 | **Save Project** | Blue floppy disk | Yes | Same as File → Save |
| 4 | **Deploy Project** | Green globe + cursor | When project has ≥1 form | Uploads to **www.tawala.com** — **fails offline** (owner: Jan 2011 build cannot reach site) |
| | *(separator)* | | | |
| 5 | **Cut** | Blue scissors | Context-dependent | See enable rules below |
| 6 | **Copy** | Two pages | Context-dependent | |
| 7 | **Paste** | Clipboard | Context-dependent | Often grey when nothing to paste |
| 8 | **Delete** | Red **X** | Context-dependent | |
| | *(separator)* | | | |
| 9 | **Undo** | Curved left arrow | Context-dependent | Owner: **always greyed** on Jan 2011 build — likely not wired / not implemented there |
| 10 | **Redo** | Curved right arrow | Context-dependent | Same as Undo |
| | *(separator)* | | | |
| 11 | **Project Manager** | Document + gear + small globe | Yes (not greyed) | Opens **Library** on website (`Config.ProjectManagerURL`) — **fails** when site unreachable |

Duplicates **File** / **Edit** menu actions where applicable.

### Enable / grey-out rules (owner observations + source)

Toolbar state refreshes on **application idle** (`DesignerView.application_Idle`). Edit buttons query the focused control implementing **`IEditMenu`** (`getIEditMenu` walks focus chain).

| Focus context | Cut / Copy / Paste | Delete | Undo / Redo |
|---------------|-------------------|--------|-------------|
| **Project Explorer**, **Items**, **Statements**, or **Fields** column (no MDI child focused) | **Greyed out** | Greyed out | Greyed out (owner: always on 2011 build) |
| **Form**, **Process**, or **Document** MDI window selected / focused with editable target | **Enabled** when `CanCut()` / `CanCopy()` / `CanPaste()` | When `CanDelete()` | When `CanUndo()` / `CanRedo()` (if implemented) |
| Rich text in form item (Text, FIB prompt, etc.) | Per text selection | Per context | Per text editor undo stack |

**Owner caveat:** Items and Statements column focus does **not always** refresh Cut/Copy/Paste — toolbar sometimes **sticks** at previous state. Exact rules TBD.

**Deploy:** Enabled when `FormList.Count > 0` and `networkIsUp` (source); still requires live Tawala deploy endpoint.

**Project Manager:** Always clickable; launches default browser to project library URL — not greyed when offline, but action fails like Deploy.

### Browser Designer note

`designer-web` should use **local save/export** instead of www.tawala.com deploy; undo/redo should be implemented for canvas editors even if legacy build lacked them.

---

## Format toolbar (row 2 — form or document MDI)

Visible when a **Form** or **Document** MDI child is active and rich text has focus. Merges from the active child (`MDIFormView` or `MdiDocumentView`). Full control list: **`DESIGNER_DOCUMENT_EDITOR.md`** (document); form adds bullets, fill color, and **Styles**.

**Document toolbar (left → right):** owner tooltips in `DESIGNER_DOCUMENT_EDITOR.md`. Summary:

| Group | Controls |
|-------|----------|
| Font | Font Face, Font Point Size, Font Color, Reset Formatting |
| Style | Bold, Italic, Underline |
| Paragraph | Outdent, Indent, Paragraph Alignment |
| Tables | Insert Table, Delete Table, Insert or Delete Row or Column |
| Insert | Insert or Edit a Function (**fx**) |

On fresh document open: **Reset Formatting**, **Delete Table**, and **Insert or Delete Row or Column** start greyed.

**Form-only extras (not on Document toolbar):** bullets, background/fill color, form-item Styles.

---

## Browser Designer (`designer-web`) gaps

| Area | Legacy | Browser today |
|------|--------|----------------|
| Project Explorer | 7 toolbar icons; `[-]`/`[+]` folders and per-form expand; form grid + gear / form+gear icons on linked processes; process **name** labels (role from icons + JSON, not name prefix); **auto-name on attach** (`Pre-ProcessN` / `Post-ProcessN`); **forms expanded on first open**; rename, reorder, start point, block back | **Phase 1 (July 2026):** collapsible Forms/Processes/Documents; linked Pre/Post under each form via `preProcess`/`process` JSON; process name labels + Pre/Post **gear** icons (role from linkage, not name); ▼/▶ toggles; F/P/D text toolbar only. **Gaps:** auto-name on attach when Connect Pre/Post UI exists; `[-]`/`[+]` chrome, dotted lines, toolbar icons 4–7, rename/reorder, default-expanded forms on first open |
| Middle column | Items / Statements / empty | Form items palette only; always visible |
| Fields | All forms + Variables; flat field-name **leaves**; `[-]`/`[+]` collapse; **all collapsed on first open**; `_InviteeID` first; left-margin resize; drag `<<…>>` | **Phase 1 (July 2026):** all forms + Variables; flat leaves; `[-]`/`[+]`; active form expanded on load (Q3 gap); plain alpha sort (Q4 gap); variable scan set/append only (Q2 gap); fixed column width; drag source only, no drop targets |
| Insert | Context-sensitive (3 menus) | Partial; no Process insert |
| Format / Themes / Tabs | Full | Minimal |
| MDI Windows menu | Yes | Single editor pane |
| Toolbars | Main icon bar + format bar (per MDI child) | Simplified |

---

## Source cross-reference

| Topic | C# source (repo) |
|-------|------------------|
| Statement order / separators | `TawalaDesigner/Code/TAWALA/Processes/MDIForm.cs` → `StatementViewTypes` |
| Fields drag/drop | `TawalaDesigner/NewCode/TawalaDesigner/ProjectUI/FieldsPalette.cs` |
| Fields column resize | `TawalaDesigner/Code/TAWALA/ProjectUI/FieldsPanel.cs` |
| Explorer toolbar | `TawalaDesigner/Code/TAWALA/DesignerUI/ProjectExplorer.cs` |
| Main icon toolbar | `TawalaDesigner/Code/TAWALA/DesignerUI/DesignerView.cs` (`mainToolStrip`, `application_Idle`) |
| Document format toolbar | `TawalaDesigner/Code/TAWALA/Documents/MDIDocumentView.cs` |
| Deploy / Project Manager | `DesignerPresenter.cs` → deploy endpoint; `LaunchProjectManager()` → `Config.ProjectManagerURL` |
| Form `blockBackButton` / `startPoint` | `TawalaDesigner/Code/TAWALA/Projects/Forms/Form.cs` |

---

*Last updated: July 2026 — Owner decisions Q1–Q4 (Fields leaves, variable scope, collapse defaults, `_InviteeID` ordering); Phase 1 browser gap table refreshed.*
