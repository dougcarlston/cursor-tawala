# Designer — Document editor

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

Documents are rich-text output pages used in **Show**, **Send**, and **Append** process statements. The editor is an MDI child window (`Document - [name]`).

Related: `DESIGNER_MENU_SPEC.md`, `DESIGNER_UI_REFERENCE.md` (display functions), `DESIGNER_PROCESS_STATEMENTS_SHOW.md`.

---

## Shell when a Document is open

| Region | Behavior |
|--------|----------|
| **Project Explorer** | Unchanged — Documents folder, selected document highlighted |
| **Middle column (Items / Statements)** | **Empty** — no palette (unlike Form → Items, Process → Statements) |
| **Center** | **Document - [name]** MDI window — rich text editor canvas |
| **Right** | **Fields** palette — still active; drag fields / variables into document |
| **Row 1 toolbar** | Main **icon bar** (New, Open, Save, Cut, Copy, …) — shared with whole app |
| **Row 2 toolbar** | **Document format toolbar** — appears below menu bar when Document MDI child is active |

Screenshot references:
- Toolbar overview: `assets/Text_Tools_Palette-*.png`
- Font face: `assets/Formatting_-_Fonts-*.png`
- Font size: `assets/Formatting_-_Font_Size1-*.png`, `Formatting_-_Font_Size2-*.png`
- Font color: `assets/Formatting_-_Font_Color-*.png`, `Formatting_-_Font_Color_-_Recent-*.png`, `Formatting_-_Font_Color_-_Choose_Color-*.png`
- Alignment: `assets/Formatting_-_Paragraph_Alignment-*.png`
- Table edit: `assets/Formatting_-_Edit_Table-*.png`

---

## Document format toolbar (row 2)

Position: immediately **to the right of the Project Manager icon** on row 1, on **row 2** below the menu bar when a Document MDI child is active.

Owner-verified **rollover tooltips** (June 2026), left → right:

| # | Control | Rollover tooltip | Notes |
|---|---------|------------------|-------|
| 1 | Font face dropdown | **Font Face** | Visible label **Default Font** when no specific face at cursor |
| 2 | Font size dropdown | **Font Point Size** | Visible label **Default Size** when no specific size at cursor |
| 3 | Font color split button | **Font Color** | **A** + color bar; arrow opens color submenu |
| 4 | Reset formatting | **Reset Formatting** | **Greyed** on fresh document open |
| | *(separator)* | | |
| 5 | Bold | **Bold** | |
| 6 | Italic | **Italic** | |
| 7 | Underline | **Underline** | |
| | *(separator)* | | |
| 8 | Outdent | **Outdent** | |
| 9 | Indent | **Indent** | |
| 10 | Alignment split button | **Paragraph Alignment** | |
| | *(separator)* | | |
| 11 | Insert table | **Insert Table** | |
| 12 | Delete table | **Delete Table** | **Greyed** until cursor inside a table; **no submenu** — deletes entire table containing cursor |
| 13 | Insert/delete row or column split | **Insert or Delete Row or Column** | **Greyed** until cursor inside a table |
| | *(separator)* | | |
| 14 | **fx** | **Insert or Edit a Function** | |

### Font Face dropdown

Full list (owner screenshot, web-safe fonts on reference PC):

| # | Entry |
|---|--------|
| 1 | **Default Font** |
| 2 | Arial |
| 3 | Arial Black |
| 4 | Comic Sans MS |
| 5 | Courier New |
| 6 | Georgia |
| 7 | Impact |
| 8 | Tahoma |
| 9 | Times New Roman |
| 10 | Trebuchet MS |
| 11 | Verdana |

**Display behavior (owner):**

- After formatting text, clicking **inside** that text updates the dropdown to show that text’s font face.
- Clicking the **document title bar** (window chrome) shows whatever font face last appeared while a document was open — not necessarily the font at the current cursor.
- Closing and reopening a document may revert the dropdown to **Default Font**, especially when the document mixes multiple fonts and/or sizes.

Source loads project default font plus `Fonts.WebSafeFonts` from installed fonts (`MDIDocumentView.OnLoad`).

### Font Point Size dropdown

Full list (owner screenshots):

| # | Entry |
|---|--------|
| 1 | **Default Size** |
| 2 | 8 |
| 3 | 9 |
| 4 | 10 |
| 5 | 11 |
| 6 | 12 |
| 7 | 14 |
| 8 | 16 |
| 9 | 18 |
| 10 | 20 |
| 11 | 22 |
| 12 | 24 |
| 13 | 26 |
| 14 | 28 |
| 15 | 36 |
| 16 | 48 |
| 17 | 72 |

**Display behavior (owner):** Same rules as Font Face — reflects selection at cursor; title-bar click shows last-shown size; reopen may reset to **Default Size** when mixed formatting in document.

### Font Color split button

**Main button click:** applies current color to selection.

**Arrow menu:**

| Item | Behavior |
|------|----------|
| **Theme Color** | **Greyed out** (owner: always disabled in observed sessions) |
| **Recent Color** | Swatch matches the color currently in use; **Font Color** icon bar shows same color. On a **new document** before color is set, icon may have no color bar — Recent then reflects that default state |
| **Choose Color…** | Opens standard Windows **Color** dialog (basic colors grid, custom colors, HSL/RGB, OK/Cancel) |

Owner example: after choosing purple via Choose Color, **Recent Color** swatch and toolbar **A** underline both show purple.

### Paragraph Alignment split button

Arrow menu (owner screenshot):

| Item |
|------|
| Left Align |
| Center |
| Right Align |
| Justify |

Toolbar icon updates to match current paragraph alignment.

### Table controls

| Control | Enable | Action |
|---------|--------|--------|
| **Insert Table** | When insert allowed at cursor | Inserts table at cursor |
| **Delete Table** | When cursor **inside** a table | **Single click** — deletes the **entire** table containing the cursor (no submenu) |
| **Insert or Delete Row or Column** | When cursor **inside** a table | Arrow opens submenu; all commands operate relative to **cursor position** |

**Insert or Delete Row or Column** submenu:

| Section | Item |
|---------|------|
| Insert | Insert Column Before |
| Insert | Insert Column After |
| Insert | Insert Row Before |
| Insert | Insert Row After |
| *(separator)* | |
| Delete | Delete Column |
| Delete | Delete Row |

### Initial state (fresh document open)

| Control | State |
|---------|--------|
| Reset Formatting | Greyed |
| Delete Table | Greyed |
| Insert or Delete Row or Column | Greyed |

**Delete Table** and **Insert or Delete Row or Column** become active after inserting a table and placing the cursor inside it.

Other controls per normal rules (e.g. **Insert Table** when cursor can accept a table; **fx** when project has ≥1 form).

**Not on Document toolbar (vs Form rich-text toolbar):** bullets/numbering, fill/background color, form-item **Styles** submenu.

Toolbar state syncs on application idle: bold/italic/underline checked state, font dropdowns, alignment icon, table commands enabled/disabled per cursor context.

Cross-check: `MDIDocumentView.Designer.cs` (`ToolTipText` properties).

---

## Menus merged when Document is focused

Document MDI child merges its menu strip into the main menu bar (`MergeAction.MatchOnly` / `Insert`). Owner screenshot shows **Format**, **Tables**, **Tools**, **Windows**, **Help** visible; **File** and **Edit** remain from the shell.

### View → (document only)

| Item | Purpose |
|------|---------|
| **Normal View** | Default editing view |
| **Page View** | Page-layout preview mode |

Document loads in **Normal View** by default (`normalViewToolStripMenuItem.PerformClick()` on load).

### Insert → (document context)

See **`DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`** — Field, Image, Invitation, Hyperlink, Function enable rules and dialogs.

### Format → (document subset)

| Item | Shortcut |
|------|----------|
| Bold | Ctrl+B |
| Italic | Ctrl+I |
| Underline | Ctrl+U |
| Color → Theme / Recent / Choose… | |
| Reset Formatting | |
| *(separator)* | |
| Tabs… | Tab-stop dialog (inches) |

**Not merged from Form editor:** Page Header…, Project Themes, Styles.

### Tables →

Same structure as global Tables menu — **Insert** (Table, Column Before/After, Row Before/After) and **Delete** (Table, Column, Row). Row/column items greyed until cursor is inside a table.

---

## Fields palette interaction

- **Drag** a form field or variable from **Fields** into the document → inserts a **field token** at drop position (qualified name, e.g. `Start:UserEmail`).
- **Double-click** a field in Fields palette → inserts at cursor (when function dialog not open).
- Accepts: palette fields (`IPaletteField`), **Variables**, or plain text drag.

Field tokens update when form items are renamed or processes change **Set** / arithmetic statements (`DocumentEditor.updateFieldsAndFunctions`).

---

## Display functions (`fx`)

The **fx** button and **Insert → Function…** open the function configuration dialog. Functions render as inline tokens in the document body (e.g. `<<display-image …>>`) — see `DESIGNER_UI_REFERENCE.md`.

- **Insert:** new function instance at cursor.
- **Edit:** when cursor is on an existing function field, opens editor for that instance.
- **Image from upload:** Insert → Image → From the Web or Tawala Upload… uses `display-image` function path.

Requires at least one form in the project for **Function** / **fx** to enable.

---

## Edit / clipboard (document body)

`DocumentEditor` implements `IEditMenu` for the rich-text control:

- **Cut / Copy / Paste / Delete** work when text or fields are selected in the document body.
- **Ctrl+A** selects all.
- **Undo / Redo** reported as **not available** through `IEditMenu` on `DocumentEditor` (main toolbar Undo/Redo may remain greyed on Jan 2011 build).

`MdiDocumentView` itself does **not** route cut/copy at the window level — focus must be in the text editor.

---

## Persistence

Document content stored as **RTF** on the `RtfDocument` model. Saves on project sync / serialize (`documentEditor.Save()` → `document.Rtf = GetRTF()`).

Virtual documents (**Header**, etc.) follow the same editor when opened from the explorer.

---

## Example workflow (owner)

1. Select **Document 3** in explorer → MDI window opens; middle column empty; format toolbar visible.
2. Type prose; apply **Default Font** / bold / alignment from toolbar.
3. Drag **Start:UserEmail** from Fields into body.
4. Click **fx** to insert a display function.
5. Use document in process: `Show Document Document 3`, `Send Document …`, `Append Document 2 to Header`.

---

## Browser Designer (`designer-web`) gaps

| Area | Legacy | Browser today |
|------|--------|----------------|
| Document MDI editor | Full RTF + toolbar + tables + functions | Not implemented |
| Middle column | Empty for documents | Form items palette always shown |
| Fields drag into document | Yes | No drag |
| Format toolbar row 2 | Per-document child toolbar | Minimal / form-only |

---

## Source cross-reference

| Topic | C# source |
|-------|-----------|
| Document MDI + toolbar | `TawalaDesigner/Code/TAWALA/Documents/MDIDocumentView.cs`, `MDIDocumentView.Designer.cs` |
| Rich text + drag/drop | `TawalaDesigner/Code/TAWALA/Documents/DocumentEditor.cs` |
| Empty middle palette | `TawalaDesigner/Code/TAWALA/Documents/DocumentPalette.cs` |
| Text engine | `TawalaDesigner/Code/TAWALA/TextEditor/TextEdit.cs` |
| Font color presenter | `TawalaDesigner/Code/TAWALA/TextEditor/TextEditorToolStripPresenter.cs` |
| Document model | `TawalaDesigner/Code/TAWALA/Projects/Documents/RtfDocument.cs` |

---

*Last updated: June 2026.*
