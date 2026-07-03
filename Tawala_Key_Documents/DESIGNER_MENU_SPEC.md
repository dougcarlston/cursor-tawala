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

## Project Explorer toolbar

Icons are active/disabled based on project state. Hover shows the written name.

| # | Name | Icon | When visible / enabled |
|---|------|------|------------------------|
| 1 | New Form | Form page | Always (with 2–3 when project exists) |
| 2 | New Process | Gear | Always |
| 3 | New Document | Paper | Always |
| 4 | Move Node Up | ↑ | After nodes exist; greyed until reorder possible |
| 5 | Move Node Down | ↓ | Same as up |
| 6 | Toggle Form Starting Point | Flag | **Form selected only**; toggle on/off |
| 7 | Block Back Button | Red ← | **Form selected only**; toggle on/off |

- Icons **1–3** are the only active ones until forms/processes/documents exist.
- Icons **1–5** show once nodes exist; **4–5** grey until there are siblings to reorder.
- Icons **6–7** appear only when a **form** node is selected (hidden for Process or Document).

**New nodes:** Default names (`Form 1`, `Process 2`, …) in the correct folder.

**Rename:** Slow double-click on node name, or single-click if already highlighted → inline edit. Duplicate name reverts to default; empty name cancels.

**Tree structure:**

```
Forms
  [FormName]
    Pre-[ProcessName]    ← process attached to form
    Post-[ProcessName]
Processes
  [ProcessName]          ← same process type as under Forms
Documents
  [DocumentName]
```

Processes may be selected from **Processes** or from **under a Form**; behavior is identical.

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

```
[Form 1 name]
  [field]
  [field]
[Form 2 name]
  ...
Variables
  [variable]
```

- One folder per **form** in the project (all forms shown, not only the open one).
- Children are **field names** (e.g. `TitleText`, `Choice1`), not design labels (`Q1`, `H1`).
- Empty forms may show as empty folders.
- **Variables** at bottom lists saved project variables (e.g. `__InviteeID`, `Customize_Choice1`).

**Interactions**

| Action | Behavior |
|--------|----------|
| Drag field leaf | Drop into form or process editor → inserts `<<FieldReference>>` |
| Double-click field leaf | Insert at current editor focus |
| Invalid drop | Designer validates; may show error (e.g. XML parse error if dropped in wrong place) |

Only **field leaves** drag (not form folder headers or the Variables folder header).

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
| Project Explorer | 7 icons, rename, reorder, start point, block back | New F/P/D only; ★ for start point in tree |
| Middle column | Items / Statements / empty | Form items palette only; always visible |
| Fields | All forms + Variables; drag `<<…>>` | Selected form only; no drag |
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
| Explorer toolbar | `TawalaDesigner/Code/TAWALA/DesignerUI/ProjectExplorer.cs` |
| Main icon toolbar | `TawalaDesigner/Code/TAWALA/DesignerUI/DesignerView.cs` (`mainToolStrip`, `application_Idle`) |
| Document format toolbar | `TawalaDesigner/Code/TAWALA/Documents/MDIDocumentView.cs` |
| Deploy / Project Manager | `DesignerPresenter.cs` → deploy endpoint; `LaunchProjectManager()` → `Config.ProjectManagerURL` |
| Form `blockBackButton` / `startPoint` | `TawalaDesigner/Code/TAWALA/Projects/Forms/Form.cs` |

---

*Last updated: June 2026 — from owner walkthrough + screenshots (Emailer, Single Question Poll or Survey, DirtBowl-related themes).*
