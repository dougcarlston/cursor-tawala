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
- Toolbar overview: `assets/Text_Tools_Palette-*.png`, `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Tools_Palette-3a4f7923-cab1-46fb-b320-3555eaacc3be.png` (July 2026 owner ref)
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

**Smoke (Font Point Size — browser):**
1. Select text in a Document or Form rich-text surface → choose **8** from Font Point Size → body stays ~8pt (not ~36pt); banner shows **8**.
2. Choose **10** then **11** → glyphs and banner show those sizes (not **12 pt (default)**). Re-select plain 12-pt text → banner returns to **12 pt (default)**. Document commit must **keep** inline `10pt` / `11pt` (must not strip them as legacy size 3).
3. Highlight **one word** (double-click) in a multi-word Document placed line → Size **20** (or Face) → only that word changes; **highlight stays on that word** (does not vanish, does not expand to the whole paragraph).
4. After (3), change Face again → still only the same highlighted word changes (no ghost apply after the highlight was lost). Size-only follow-up applies to the same highlight.
5. Drag-select **several words** → Face or Size → formatting applies to the full drag; highlight stays on that full span (does not collapse to only the last word).
6. Spot-check **12**, **20**, **36** — banner label matches the sized run.

### Font Color split button

**Main button click (A + bar):** applies the **current** sticky color to the selection (does not open a menu).

**A-bar current color:** the underline on **A** is sticky — it stays after Choose Color / Recent-slot picks, and does **not** follow ordinary caret moves into differently colored text. To sync the A-bar from existing text: **long-press** (~450ms, no drag) on that colored text (Document or Form rich-text). Long-press samples only; it does not recolor the selection.

**Arrow menu (▾):**

| Item | Behavior |
|------|----------|
| **Theme Color** | **Greyed out** (owner: always disabled in observed sessions) |
| **Choose Color…** | Opens an **in-app Color Picker** dialog (SV spectrum + hue + R·G·B, optional Eyedropper). **No Recent row in this ▾ menu.** |

**Why in-app Color Picker (not OS Colors):** a web app cannot inject HTML/controls into the macOS (or other OS) Color window. To put Recent under RGB, Choose Color… must open Designer chrome.

**Color Picker dialog:**

| Region | Behavior |
|--------|----------|
| **SV + hue + RGB** | Mac-like spectrum / brightness / R·G·B. Live pick updates sticky **A** bar and applies to the selection. |
| **Eyedropper** | `EyeDropper` API when available; greyed when not. |
| **Recent swatch row** | Fixed row of **8 small** (~15px) boxes at the **bottom of this dialog** (under RGB). Empty = light-border empty square (**no diagonal slash**). Newest fills left-to-right, deduped, capped at 8, persisted in `localStorage`. |
| **Dismiss** | Click outside dialog or Escape. |

Each committed pick fills a Recent slot and updates the sticky **A** bar. Click a filled Recent box to apply that color (same as **A** / color command). Empty boxes are inert.

Owner example: Choose Color… → pick purple → **A** underline stays purple; reopen Choose Color… → purple in the Recent row. Highlight other text → click that box (or **A**) → that text turns purple.

**Smoke (Font Color — browser, July 2026):**
1. Document (or Form Text): ▾ Font Color → **Theme Color** (greyed) and **Choose Color…** only — **no** Recent boxes in the ▾ menu.
2. Choose Color… → **in-app** Color Picker (not the system Color panel). Empty Recent boxes under RGB are small, clean squares (**no diagonals**).
3. Pick colors → boxes fill small and clean; reopen → click a recent box to reuse; **A** bar sticky + apply still works.
4. Color some text purple, some black. Click (short) into black text → **A** bar still purple. Long-press (~½ s) on purple text → **A** bar becomes purple; selection is not force-recolored by the long-press alone.

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
| **Insert Table** | When insert allowed at cursor (**not** inside an existing table — no nested tables) | Inserts table at cursor |
| **Delete Table** | When cursor **inside** a table | **Single click** — confirms (**Are you sure you want to delete this table?**); **OK** deletes the entire table containing the cursor; **Cancel** leaves it |
| **Insert or Delete Row or Column** | When cursor **inside** a table | Arrow opens submenu; all commands operate relative to **cursor position** |
| **Borders** | When cursor **inside** a table | Arrow opens submenu: **Border 1** (thin grid), **Border 2** (thick outer + thin cells), **No Border**. Class on `table.user` (`user-border-1` / `user-border-2` / `user-border-none`) |

**Multi-cell format:** drag across cells to highlight a rectangle; Bold / Italic / Underline / color / face / size / alignment apply to **all selected cells only** (cell-level `text-align` for alignment — never the whole table or row).

**Tab navigation:** Inside a table, **Tab** moves the caret to the next cell (end of row → first cell of next row). **Shift+Tab** moves to the previous cell. At the last/first cell, caret stays in the table (no leave / no auto-insert row).

**Table chrome:** When the caret is in a table: **one** top-left ✥ **move** handle (anchor corner — drag repositions the table; absolute X/Y is kept — no forced pack under preceding text), plus resize (edges / SE corner) and column/row dividers. **No** float wrap toggles (left/block/right). Prose above/beside/below stays editable via normal click/type + Indent/Outdent. Multi-select that includes a table → Indent/Outdent also nudges the table left edge (same 36 pt steps as placed lines).

**Document table layout:** Tables and `.doc-placed-text` share absolute positions. Packing is **collision-aware** (same horizontal column only): side-by-side L/R prose is allowed; intentional drag gaps are not closed; same-column overlaps still clear so leave/return never layers text under a table. Unused empty invent anchors prune on leave/other click (intentional Double-Return `data-doc-blank` gaps are kept). **Click/drop hit-test:** inside `.doc-placed-text` → edit that line; inside a `table.user` cell → caret/highlight that cell; otherwise invent in free space (including L/R of tables). Left-edge invent may use a tiny inset so the caret is visible — mid-canvas / free-space invents keep the click X. Blank-canvas / prose clicks must not stay trapped in the table when `caretRangeFromPoint` false-positives into an absolute table.

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
| Color → Theme / Choose… | |
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

**Browser (Jul 19):** `updateFormItem` → `cascadeFieldRenameInProject` rewrites Document/Form Text function chips + field tokens and Process field refs when a Hidden Field / FIB blank / MCQ Fields name changes.

---

## Display functions (`fx`)

The **fx** button and **Insert → Function…** open the function configuration dialog. Functions render as inline tokens in the document body (e.g. `<<display-image …>>`) — see `DESIGNER_UI_REFERENCE.md`.

- **Insert:** new function instance at cursor.
- **Edit:** when cursor is on an existing function field, opens editor for that instance.
- **Images:** Insert → Image → **From your PC…** stores project `images[]` + inline `<img data-tawala-image-id>` (Deploy `<image>`/`<imagedef>`). **From the Web…** uses the `display-image` function path (URL).

Requires at least one form in the project for **Function** / **fx** to enable.

---

## Edit / clipboard (document body)

`DocumentEditor` implements `IEditMenu` for the rich-text control:

- **Cut / Copy / Paste / Delete** work when text or fields are selected in the document body.
- **Ctrl+A** selects all.
- **Undo / Redo** reported as **not available** through `IEditMenu` on `DocumentEditor` (main toolbar Undo/Redo may remain greyed on Jan 2011 build).
- **`designer-web` gap (owner Jul 20):** Undo/Redo still **does not reverse Document canvas edits** (toolbar / Edit menu / ⌘Z). See `DESIGNER_OPEN_BUGS.md` § Edit / Undo.

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
| Document MDI editor | Full RTF + toolbar + tables + functions | Rich canvas with placed lines, tables, fx tokens (ongoing parity). **Jul 20 owner epic:** move toward live caret / cross-line Backspace / arrows through chips — `DESIGNER_OPEN_TODOS.md` § Document caret model |
| Middle column | Empty for documents | Form items palette hidden for Document windows |
| Fields drag into document | Yes | Drag + double-click insert `<<name>>` field tokens |
| Format toolbar row 2 | Per-document child toolbar | Shared Formatting Palette (Document + Form Text) |

### Must-not-break smoke (Document canvas — July 2026)

**Toolbar state vs caret**

1. Type mixed B/I/face runs (e.g. italic “pretty”, bold “much”, plain “everywhere”). Caret in plain “everywhere” → **B** and **I** not lit; Font Face shows default/Arial (not a prior Comic face).
2. Select Comic Sans on one word, then click a plain/default run → Font Face returns to **Arial (default)** (not sticky Comic).

**Double-click word select / triple-click paragraph**

3. Double-click a word that straddles style boundaries (e.g. first letters plain, rest italic) → whole word selects, not a mid-word chop at the span edge.
3b. Triple-click in a placed Document line (plain text, and a line with `<<field>>` chips) → whole paragraph/line selects. Same for Form Text body. Mid-text single click still places a caret only (no new `✥`). Field double-click still selects the chip only.
3c. Double-click blank Document canvas (no text under the pointer) → invent/place a text anchor and show a caret (`|`) so typing works. Single-click free space still invents/places a caret too.
3d. Click the **left** side of a blank Document (near the window border, before Indent) → blinking `|` is clearly visible inside the content area (not clipped under the MDI/chrome border). Mid-canvas invent and free-space L/R of a table still place at the click X.
3e. **Click/drop hit-test (July 14):** Click or field-drop into **existing placed text** → caret/edit in that line (no second overlapping invent). Click/drop a **table cell** → caret or cell highlight in that cell (not invent elsewhere). Click/drop **free space** (including beside a table — **left and right**) → invent at that point. Left-edge invent may inset for caret visibility; other free-space invents must keep the click X. Dragging an existing ✥ line to the right of a table must keep working; click-invent in the empty white space to the right must invent (not stay dead from caretRange / full-width hit-box false positives).

**Field tokens**

4. Insert `<<Form:Field>>` into a 20 pt line → chip sits on the text baseline and matches surrounding face/size (not tiny mono badge or sticky wrong size). **Hard-refresh first.**
5. Double-click an existing field token → selects that token only; does **not** spawn a second `<<…>>` elsewhere. **With that chip selected** (or caret in a plain `<<Field>>` run), Fields drop / double-click **replaces** the chip — never nests `<<A<<B>>…>>` mid-token (table cells included).
6. Mid-sentence continuity: type `The player of the week was ` → insert field → type `, who rallied…` → stays **one** placed line/paragraph with the same face/size/margin (does **not** open a new block after the chip).
7. Select a run that includes field chips (or select-all on the placed line) → choose **20 pt** → chips resize to **20 pt** to match body text (not stuck at insert-time 12/16). Same for Font Face.
7b. Field chip baseline (16→20 pt): place `<<FIB1:a>>` / `b` / `c` mid-sentence at **16 pt Trebuchet**, select the placed line → **20 pt** → chips stay on the same baseline as body glyphs; selection highlight is a smooth block (no stepped/taller line through chips; no vertical drop after `<<FIB1:c>>`). **Hard-refresh first.**
7c. Face/Size banner honesty (July 14): select a **uniform** Comic + larger-pt line that includes field chips → Face shows **Comic Sans MS** (not **Mixed**); Size shows the real point size (not false **12 pt (default)**). Repeated Face/Size changes keep chips and text matched. Mixed only when styles truly differ. Mid-span Comic+26 with a field → **click away** → **re-drag** the same span → Face still **Comic Sans MS** and Size **26** (not Mixed/Mixed).
7d. B/I caret: after mixed bold/italic/plain runs, caret in plain text → **B** / **I** not lit (computed/probe style, not sticky queryCommandState).
7e. Selection-scoped Size/Face (July 14): in a Document placed line, highlight **one word** → choose a non-default Size (or Face) → **only that word** changes; highlight must **not** expand to the whole placed paragraph; highlight must **remain** on that word after the palette command.
7f. **10 pt** and **11 pt**: choose either Size → glyphs and Size banner show **10** / **11** (not stuck as **12 pt (default)**). Other stops (8, 9, 12, 14, …) still work. Click another run → banner follows that selection (not sticky 10). Document HTML must retain `font-size: 10pt` / `11pt` after commit (not stripped).
7g. B/I/U on Fields: select a field chip alone (or a run that includes chips) → **Bold** / **Italic** / **Underline** apply to the chip(s). Button state lights when the chip carries that mark.
7h. Selection restore after Face/Size (July 14): double-click one word → Face or Size → highlight remains. Drag several words → Face or Size → highlight stays on the full drag (not last-word-only). After a successful restore, a further Face change applies only to the still-highlighted span (no ghost range).
7i. **Field-gap selection stability (July 14 retest):** triple-click a Document placed line that includes several `<<FIB…>>` chips → Size (or Face then Size) repeatedly (20→22→24→26). Highlight must stay **end-to-end** including chips and the trailing words (e.g. still covers `modification.`); must **not** creep inward by ~2 invisible pads per chip each pass (`modific`|`ation.`). After Comic Face then Size to 26, no orphan default-face/default-size fragments at the start or end of the original run.
7j. **Face→Size with chips (July 14 SS1–SS4 / retest):** In a Document line that already has face/size (e.g. 18 pt Arial) and **three** field chips (`<<FIB1:a>>` / `c` / `b`), drag a **partial** highlight that includes text **and** at least one chip → **Face** Times New Roman (highlight kept, chip included in blue highlight) → **Size** **26 pt**. Selection must stay the same meaningful span (starts with `some`, ends with `while` — not mid-word `"th"`…`"w"`); glyphs that were in the highlight must remain Times+26 (not revert to default beside the selection); chip in the highlight must paint Times+26 and stay in the highlight (no white gap that excludes the chip from the next Size). Soft-wrap mid-word line breaks are OK only when both halves share the same face/size and stay selected. Harness: `designer-web/public/face-then-size-ss4.html`.
7j2. **Partial Size + function chip stay put (open, intermittent Jul 20):** Paragraph with a function placeholder → change Size on **some words only** (not the whole line) → chip must stay in its original text position. If it jumps, drag it back (§ 22h). Report a reliable repro when found (`DESIGNER_OPEN_BUGS.md`).
7k. **Left-of-table defaults (July 14):** Invent/type a word left of a table (e.g. **Left**) → Face **Comic Sans MS** + Size **20** → then Face **Arial (default)** + Size **12 pt (default)**. Glyphs and Face/Size banner must show Arial/12 (not stuck on Comic/20). Persist after Save / soft-refresh. One-word default inside a longer Comic line must **not** strip the whole placed line’s face/size.

**Indent (Batch 5 — keep face/size)**

8. Indent a Document placed line → left edge steps in; wrap width to right margin; lines below **do not overlap**; face/size on the indented run stay unchanged.
8a. Drag-select placed lines **and** a Document table together → **Indent** / **Outdent** moves the table’s left edge in the same 36 pt steps as the lines (table stays aligned with the chips/prose). Caret alone inside a cell must **not** move the table.
9. Form Text indent via margin-left / soft-line wrap → keeps face/size (no `blockquote` rewrite).

**Mid-text caret (must not invent a second paragraph)**

10. Type a sentence in a Document placed line. Click mid-word (e.g. to fix a typo) → caret appears **in that same placed line** at the click point; **no** new `✥` / `Npt × Mpt` paragraph anchor appears on top of the text. Delete/type to correct the word. **Hard-refresh before testing.**
11. Mid-sentence field continuity still holds (smoke #6) after the click-caret fix.

**Return keeps face/size (multi-paragraph)**

12. Set **Trebuchet MS** + **20 pt** → type a line → **Return** → type again → second placed line stays **Trebuchet 20**; Formatting Palette Face/Size banner still shows that face/size (not Arial / default 12). Same for Form Text body paragraphs.

**Double-Return blank line (July 14)**

13. Type a paragraph → **Return twice** (visible blank line) → type a second paragraph → edit anywhere in the second paragraph → the blank-line gap between the two **must remain** (intentional empty placed line / flow paragraph is not collapsed or pruned).

**Paragraph alignment / Center (July 14 leftover #6)**

13a. Soft-refresh. Document placed line → Align **Center** (palette) → glyphs sit centered between left/right margins (not a left-stripped shrink box). **Left** / **Right** / **Justify** still work. **Return** on a centered line → new line stays centered. Soft-refresh / leave-return keeps Center. Table cell Align Center still affects only highlighted cells (smoke #15).
13a2. Soft-refresh. Placed line (or Form Text) with **leading spaces/tabs** → Align **Left** → glyphs flush at left edge (leading whitespace characters gone). **Center** / **Right** still OK. Palette **Indent** (`margin-left` / `data-doc-indent`) still indents — Align Left must not clear that margin.

**Window / MDI resize reflow (July 14 leftover #7)**

13b. Soft-refresh. Document with wrapping placed text + a table beside (or with an intentional ✥ gap) → narrow the Document/MDI window (or browser). Text re-wraps; same-column overlaps clear; table does **not** snap under side-by-side prose; intentional vertical gap under text is kept.
13b2. Soft-refresh. Side-by-side text + table → **narrow** until they stack (table/text pushed down) → **widen** again → tops restore toward their prior homes (not permanently ratcheted down). Intentional ✥ gaps must still stay.

**Continue after break (July 14 leftover #8)**

13c. Soft-refresh. Set **Trebuchet** + **20** → type a sentence → click away (another window / blank) → click back at the **end** of the sentence → keep typing → same face/size on the **same** placed line (no new wrong-margin/font block). Mid-sentence after a field chip still continues on one line (smoke #6).

**Tables (July 14)**

14. Caret inside a table → **Insert Table** greyed / no-op tip (“Cannot insert a table inside another table”).
15. Drag across several cells → highlight → Bold / Size / Align Center apply to **all selected cells only** (other cells unchanged).
16. Borders menu → **Border 1** / **Border 2** / **No Border**; reload / soft-refresh still shows the chosen border class on the table.
17. Caret in a cell → **Tab** / **Shift+Tab** moves to next/previous cell; last/first cell stays put (does not jump outside the table).
18. With caret in a table → **one** top-left ✥ move handle (no ◧/▭/◨ float icons); drag moves the table; type/drag/indent text above or below the table still works.
19. Insert a table among Document text → click another window → click Document back → table still **reserves vertical space** (text not visible underneath / not layered under the table).
20. Document with a table + prose → click in text above/beside/below (outside cells) → caret lands in that prose and typing works (not trapped in the table).

**Free-space table / text layout (July 14 batch)**

21. Place / type text **left** of a table when there is room → text stays beside the table (does **not** jump below).
22. Place / type text **right** of a table when there is room → stays beside.
22b. **Delete beside table (July 14):** Invent/type a short word (e.g. `Test`) in free space **right** of a table → Backspace/Delete erases **in that placed line** (word must **not** hop into a table cell). Nearby labels (Left / Right side / Bottom) keep their font face. Clicking a cell still edits that cell.
22c. **Cross-Return Backspace (Jul 20 caret epic A):** Type a line → Return → type a second line → place caret at the **start** of the second line → Backspace → lines merge into one (caret at the join). Empty Return line + Backspace removes the blank and lands on the previous line. Repeat until the Document can be emptied. Side-by-side L/R columns must **not** merge into each other; table cells still must not receive hopped text (22b).
22d. **Arrows across Returns + chips (Jul 20 caret epic C):** Two stacked lines → ArrowRight at end of first → caret at start of second; ArrowLeft at start of second → end of first. Line with `<<field>>` or function chip → ArrowRight/Left jumps over the chip (does not stick). Side-by-side columns must not steal Left/Right.
22e. **Backspace function chip caret (Jul 20):** Document line with only a function chip → caret after chip → Backspace removes the chip and leaves a **visible blinking caret** (ArrowLeft still works). Must not leave focus stuck on the ✥ move handle with no caret.
22f. **Space wrap + paragraph Backspace (Jul 20):** Type `Old fashioned names` → delete the space before `names` → re-type the space → `names` must stay on the **same** visual line when there is room (no sticky mid-word wrap). Blank line between two paragraphs → Backspace at start of the lower paragraph removes **only** the blank (lower text stays). Content Return → Backspace at start of lower line merges with a joining space (`Hello` + `World` → `Hello World`).
22g. **Multi-paragraph move (Jul 20):** Drag-select two or more stacked Document lines → pointer over the highlight shows a **hand (grab)** → drag with **grabbing** hand → **all** selected lines move together (gaps kept; no snap-back pack). **No ✥ / position anchors** on placed lines (they interfered with highlight sizing). **Whole-line** select (e.g. triple-click) → highlight drag moves that line only. **Double-click a single word** → drag must **not** move the whole paragraph (word stays a text selection; use Cut/Paste or retype to relocate a word). **Drag into blank gap:** select a lower line → drag up onto a Double-Return blank above → blank husk is consumed (same outcome as Backspace on that blank); line must **stay** at the drop Y (no snap-back under the blank). **No skipped middle lines:** drag-select from a top line through a bottom line must highlight **and** move every same-column line in between (even if invent/reorder left that middle line later in the DOM). **Owner Passed Jul 20** (middle-line fill); word-vs-paragraph move fixed same day.
22h. **Chip-only relocate (Jul 20):** Select a field or function placeholder (`<<…>>`) → drag the chip (not the surrounding words) → only the chip moves; sibling text on that line stays put. Drop after `these types are:` (or mid-line) joins that line — does **not** drag the whole paragraph / wrap fragment with the chip. **Owner Passed Jul 20** (including function label onto a text line).
22i. **Long chip layout / window chrome (open):** A long nowrap function chip may appear on the next visual line until the MDI window is widened (then snaps onto its line). After widen, confirm **minimize / close** remain reachable without hiding Explorer/Items/Fields or using **Windows → Cascade** (see `DESIGNER_OPEN_BUGS.md`).
23. Drag table with ✥ to leave a gap below preceding text (or beside) → after release, table keeps that X/Y (does **not** snap under the text above).
23b. Soft-refresh. Place several “Text here” lines in the table’s column → drag table ✥ over them (they yield downward) → drag table clear / back → yielded prose snaps back toward prior homes (not left orphaned far down the canvas).
24. Click blank canvas to invent a text anchor → click away / elsewhere **without typing** → empty anchor is gone (intentional Double-Return blank between paragraphs still kept).
25. Soft-refresh → leave/return Document → still **no** table-over-text overlay.
26. Caret in a table → **Delete Table** → **Are you sure you want to delete this table?** → **Cancel** keeps table; **OK** removes it.

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

*Last updated: July 2026 — invent caret left inset (3d); click/drop hit-test edit vs invent (3e); selection-scoped Face/Size, 10/11 pt snap fix, B/I/U on field chips; Face/Size chip inherit readout; Return keeps face/size; Double-Return blank gap; paragraph Center/align (#6); resize reflow (#7) + widen home-restore (13b2); continue-after-break (#8); mid-text click caret; no nested tables; multi-cell format; table Borders 1/2/none; Tab cell nav; align only on highlighted cells; one top-left table move handle (no float toggles); Document free-space L/R text + collision-aware table placement; table ✥ yield/restore (23b); empty invent prune; Delete Table confirm; field-gap ZWSP Size shrink fix (7i); Face→Size with chips (7j / SS1–SS4 retest); left-of-table Arial/12 defaults apply (7k); delete beside table stays in placed line (22b); cross-Return Backspace merge (22c); arrows across Returns + chips (22d).*
