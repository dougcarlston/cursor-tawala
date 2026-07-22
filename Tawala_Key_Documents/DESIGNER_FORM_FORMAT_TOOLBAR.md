# Designer — Form format toolbar (row 2)

Form MDI child merges **`formsToolStrip`** when a Form window is active. Document toolbar: **`DESIGNER_DOCUMENT_EDITOR.md`**.

---

## Toolbar items (left → right)

Same core as Document toolbar (Font Face through Insert or Delete Row or Column, **fx**). Tooltips match Document (`Font Face`, `Font Point Size`, `Paragraph Alignment`, etc.).

| # | Control | Notes |
|---|---------|-------|
| 1–13 | Font, color, B/I/U, indent, alignment, tables | See `DESIGNER_DOCUMENT_EDITOR.md` |
| 14 | **fx** | **Insert or edit a function** — enabled only in **Text** item body |

When cursor is in a **Heading** item, the **entire** format toolbar is greyed (source: `TargetTextEditor.Parent is HeadingView`). See `DESIGNER_FORM_ITEMS_HEADING.md`. When cursor is in a **Text** item, the toolbar is **live** — see `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` (July 2026 owner screenshots).

---

## Format menu extras (Form only)

Merged into main **Format** menu when Form MDI active (legacy screenshot Jul 17):

| Item | Notes |
|------|-------|
| Bold / Italic / Underline / Color / Reset Formatting | Icons + accelerators (Ctrl+B/I/U); live on Form rich text — **browser:** Formatting Palette |
| **Styles** → | Cascade (no icons): Fill in the Blank… / Multiple Choice… / Text… |
| **Page Header…** | Project-wide banner — **not** the Heading form item; see `DESIGNER_PAGE_HEADER.md` |
| **Project Themes** | Theme checklist (see `DESIGNER_MENU_SPEC.md`) |
| **Tabs…** | Tab stops in inches |

**Browser:** Format menu **removed**; **Project → Styles… / Page Header / Themes** own the retained items. **Tabs…** is documented but not exposed. **Styles…** opens from the **selected** FIB / MCQ / Text (no submenu).

Screenshot: [`assets/Format_-_Styles_submenu.png`](assets/Format_-_Styles_submenu.png)

Source typo in older Designer builds: **Reset Formattting** (three t’s) in `MDIFormView.Designer.cs`.

---

## Format → Styles dialogs (owner June–July 2026)

Opened from **Format → Styles →** cascade (legacy) or **Project → Styles…** (browser: opens the dialog matching the **selected** form item) when a **Form** MDI child is active.

**Authoritative FIB UI:** `FibStylesDialog` (compact Labels + Blanks + one preview). Older `FibItemStylesDialog` (scrollable radio-per-preview) is superseded by the owner screenshots below.

**Shared footer behavior (FIB / MCQ / Text):**

**Legacy (screenshots Jul 17):**

- Note: *Note: Style may be applied only to selected [item type] … The "Apply to All" feature has been disabled.*
- **Apply to All** — always disabled (removed Sept 2009 — project-wide apply “wreaks havoc in large projects”).
- **Apply to Selected** — hidden when no matching item selected; otherwise applies to the **one** selected canvas item (no multi-select).
- Button order: **Apply to Selected** · **Apply to All** · **Cancel**.

**Browser (`designer-web`, Jul 17 — form-scoped Apply to All + selection-routed Styles…):**

- **Project → Styles…** — single menu item; enabled only when a FIB / MCQ / Text is selected; opens that kind’s dialog (no FIB/MCQ/Text submenu). Heading / Break / etc. keep Styles greyed.
- **Apply to Selected** — same one-item contract; always available in the dialog (you opened it from a matching selection).
- **Apply to All** — **enabled** when the active form has ≥1 item of that kind; updates **every matching item on this form only** (not other forms / not project-wide). Safer than legacy project-wide `SetAll*Styles`.
- Note copy explains Selected vs All and the form-only scope.
- Design canvas does not paint FIB/MCQ layout tokens (owner Jul 18: won't do — interferes with editing; Preview is immediate). **Text** Instructional/Error Style **is** shown on Forms → Text.

---

### Fill in the Blank Styles

**Title:** `Fill in the Blank Styles`  
**Menu:** Format → Styles → **Fill in the Blank…** (browser: Project → Styles)  
**Source:** `FibStylesDialog.cs` + sample UserControls under `Dialogs/Fib*Sample*`

**Layout (≈449×311 fixed dialog):**

| Region | Content |
|--------|---------|
| Left | **Labels** group (radios) stacked above **Blanks** group (one checkbox) |
| Right | Single **lavender** preview panel (`RGB 200,200,255`) — swaps sample control as options change |
| Bottom | Note + buttons on grey footer strip |

#### Labels

| Radio | Preview sample | Stored `style` |
|-------|----------------|----------------|
| **Above** | `Name:` / `Address:` each above its blank (2 rows; blanks different widths) | `topLabels` |
| **Left justified** | `Name:` / `Address:` / `Phone:` left-aligned labels; blanks of varying width to the right | `leftAlignLabels` or `leftAlignLabelsJustified` |
| **Right justified** | Same three rows; labels right-aligned so colons line up | `rightAlignLabels` or `rightAlignLabelsJustified` |
| **Freeform** | Inline: *Columbus had three ships called the □ the □ and the □* | `freeform` |

Preview labels are **dark blue**; blanks are disabled textboxes (inset). Labels in the Styles preview are **not** auto-bolded; Deploy must not bold Name/Email/Phone by heuristic either (author B/I/U only).

**Deploy smoke (left/right Align, Jul 17):**

1. Multi-blank FIB (Name / Email / Phone on separate soft-rows or one soft-row) → Deploy shows **one label + one field per row** (not “Name: Email” with two boxes).
2. Label column hugs the longest label — fields sit close to labels (not a ~380px gutter). DirtBowl Registration themes may still use a wider `--reg-label-width`.
3. No unintended bold on Email/Phone/Name unless the Design prompt used Bold.
4. **Align right side** (`…Justified`): blanks stretch so right edges share a margin within that FIB table (CSS on `table.fib.justified`). Freeform does not offer Align right side.

#### Blanks

| Control | Rules |
|---------|--------|
| **Align right side** | Checkbox. **Enabled** only when **Left justified** or **Right justified** is selected; **greyed** for Above / Freeform (may retain checked appearance while disabled). When checked with Left/Right justified, blank **right edges** align and style token gains `…Justified`. |

#### Screenshots (owner Jul 17, 2026)

| Shot | File |
|------|------|
| Above | [`assets/Format_-_Styles_-_FIB-Above.png`](assets/Format_-_Styles_-_FIB-Above.png) |
| Left justified | [`assets/Format_-_Styles_-_FIB-LeftJustified.png`](assets/Format_-_Styles_-_FIB-LeftJustified.png) |
| Right justified | [`assets/Format_-_Styles_-_FIB-RightJustified.png`](assets/Format_-_Styles_-_FIB-RightJustified.png) |
| Right justified + Align right side | [`assets/Format_-_Styles_-_FIB-RightJustified-AlignRight.png`](assets/Format_-_Styles_-_FIB-RightJustified-AlignRight.png) |
| Freeform | [`assets/Format_-_Styles_-_FIB-Freeform.png`](assets/Format_-_Styles_-_FIB-Freeform.png) |

---

### Multiple Choice — Styles

**Title:** `Styles`  
**Menu:** Format → Styles → **Multiple Choice…**  
**Source:** `McqItemStylesDialog.cs`

**Layout:** Three **stacked** rows — each row is a **bold** radio on the left and a lavender preview panel on the right (**all three previews visible at once**; selecting a radio chooses which layout applies). Column-count combo sits under **Multi-column**. **Spacing** group box below the stack.

| Radio | Preview |
|-------|---------|
| **Vertical** | “What is your favorite color?” + Red / Orange / Yellow stacked (Red shown selected) |
| **Horizontal** | Same question; Red / Orange / Yellow in one row |
| **Multi-column** | Same question; six choices in two columns — left Red/Orange/Yellow, right Green/Blue/Violet |

**Multi-column** dropdown (enabled only when Multi-column selected; greyed otherwise): **Auto**, **2**, **3**, **4**, **5**. `Auto` → `columnCount` 0.

| Group | Control |
|-------|---------|
| **Spacing** | **Do not add blank space below question when displayed.** → `paddingBottom: false` when checked |

Maps to item `style` `vertical` / `horizontal` / `multicolumn` (+ optional `columnCount`).

**Deploy smoke (Jul 17):**

1. **Vertical** — stacked radios; labels clear of the control (OK in owner smoke).
2. **Horizontal** — choices in one row; radios must not overlap choice text (fixed: CSS now targets plain `<label>`, not only `label.choice`).
3. **Multi-column** — table cells have a readable gutter (~2em) between columns (was flush in default theme).
4. Design canvas shows a vertical list for all three — **layout paint won't be added** (owner Jul 18: interferes with editing; Preview is immediate). Check layouts on Deploy/Preview.

#### Screenshots (owner Jul 17, 2026)

| Shot | File |
|------|------|
| Vertical selected (no MCQ selected → Apply to Selected hidden) | [`assets/Format_-_Styles_-_MCQ-Vertical.png`](assets/Format_-_Styles_-_MCQ-Vertical.png) |
| Multi-column + column dropdown open (Apply to Selected visible) | [`assets/Format_-_Styles_-_MCQ-MultiColumn.png`](assets/Format_-_Styles_-_MCQ-MultiColumn.png) |

---

### Text — Styles

**Title:** `Styles`  
**Menu:** Format → Styles → **Text…**  
**Source:** `TextItemStylesDialog.cs`

**Layout:** Three stacked radio + lavender preview rows (all visible), then **Spacing** group, then footer.

| Radio | Preview text | Appearance |
|-------|--------------|------------|
| **Normal** | This is normal text | Plain black |
| **Instructional** | This is instructional text | **Bold italic blue** (legacy dialog: `SystemColors.Desktop`; browser/Deploy use navy `#000080`) |
| **Error** | This is error text | **Bold italic red** type (`#C00000`) — **no** red/pink background (lavender panel is dialog chrome only) |

**Browser Design + Preview + Deploy:** Text canvas rows and runtime CSS (`BASE_FORM_CSS` / Tomcat `default.css`) follow that type contract. Error must not pick up form-validation `.error` pink fill. DirtBowl Registration may still wrap instructional copy in a light banner; the type itself stays blue bold italic.

**Style vs local formatting (owner Jul 17):** Item Style is the default look. Palette / character formatting on the text (bold, italic, color, face, size) **wins** when present — same as Word style vs direct formatting. To see Instructional or Error from Styles, leave runs at **Default** (or use **Reset Formatting**). Applying Styles does not strip local formatting. **Smoke (Jul 21):** Instructional Text → select a word → Bold and/or Italic toggles still apply (and can clear) on that run; Style color remains unless Color is changed. Do not reintroduce `* { font-weight/font-style: inherit !important }` under `.text-style-instructional` / `.text-style-error`.

**Deploy note (Jul 17):** Designer API must be running code that includes Text Style color export (`applyTextItemStyleToXml`). If the API process predates that change, re-Deploy will still drop colors — restart `node server/index.mjs` (port 3001), then Deploy again.

**FIB multi-blank + Align right side:** Freeform is required for “Name ____ Email ____ Phone ____” on one line. Left/Right justified (+ Align right side) turn each blank into its own table row; do not Apply All justified styles onto a freeform multi-blank FIB.

| Group | Control |
|-------|---------|
| **Spacing** | **Do not add blank space below text when displayed.** → `paddingBottom: false` when checked |

Maps to `style` `normal` / `instructional` / `error`.

#### Screenshot (owner Jul 17, 2026)

| Shot | File |
|------|------|
| Instructional selected (Apply to Selected visible) | [`assets/Format_-_Styles_-_Text-Instructional.png`](assets/Format_-_Styles_-_Text-Instructional.png) |

---

### Browser (`designer-web`) — visual parity (Jul 17 polish)

Tokens under **Project → Styles**. Dialog chrome aligned to owner screenshots:

| Area | Status |
|------|--------|
| FIB Labels + Blanks groups, lavender Name/Address/Phone + Columbus freeform | Done |
| MCQ/Text stacked radio + three simultaneous lavender previews | Done |
| Apply to Selected **hidden** when no matching item; button order Apply to Selected · Apply to All · Cancel | Done |
| **Apply to All** = all matching items on **active form only** | Done (Jul 17) |
| Design canvas visual layout for each style token | **FIB/MCQ layout paint: won't do (owner Jul 18)** — would interfere with editing those rows, and Preview is immediate for Forms. **Text Style paint (Instructional/Error) on Forms → Text: kept** — canvas already shows blue bold-italic (Instructional) / red bold-italic (Error) so authors can tell the style apart. |
| Deploy left/right Align: one blank per table row; no Name/Email auto-bold | Done (Jul 17) |
| Deploy label column: content-sized (not fixed 380px gutter) | Done (Jul 17; DirtBowl reg forms keep `--reg-label-width`) |

*Legacy Preview tab (www.tawala.com) is unavailable offline — not required for this polish.*

---

### Tabs… (Form)

**Title:** `Tabs`  
**Menu (legacy):** Format → **Tabs…** · **Browser:** removed from the app (Jul 17)  
**Source:** `TabDialog.cs` / `TabDialog.Designer.cs`

**Layout (owner Jul 17 screenshot):**

- Label **Tab stop position:** + text field + unit **inch**
- List of stops (`0.00` format) with **Set** / **Clear** (greyed with no selection) / **Clear All**
- **OK** / **Cancel**

**Legacy rules:** Stops are inches, `0 < tab ≤ 6.5`, no duplicates.

**Browser decision (owner Jul 17):** Remove Tabs from the app for now. Tawala Designer is not intended to be a word processor, and the browser implementation did not provide a practical Tab action:

- Documents never exposed the command.
- Form Text / Heading could store `tabPositions`, but their export did not use them.
- FIB / MCQ could emit tab-stop metadata, but most exported paragraphs had no `<tab/>` marker to activate it.
- Tab / Shift+Tab in Text and Document remains reserved for table-cell navigation.

Keep the screenshot and legacy behavior documented. Preserve existing `tabPositions` fields and server conversion helpers for compatibility with imported projects; do not expose an editor dialog or Project menu item. A source search found the C# `TabDialog` itself but no code that opens it in the available legacy tree.

| Shot | File |
|------|------|
| Tabs dialog (0.50 / 2.00 / 4.00) | [`assets/Format_-_Tabs_dialog.png`](assets/Format_-_Tabs_dialog.png) |

---

## Enable rules (source)

| Control | Enabled when |
|---------|--------------|
| Format toolbar (most) | Design tab + rich-text editor focused |
| **Insert Table** | Parent is **TextItemView**; **disabled** while caret is inside an existing table (no nested tables) |
| **Tables** menu | Parent is **TextItemView** |
| **fx** / Insert → Function | Parent is **TextItemView** |
| Insert → Hyperlink / Invitation | Parent is **TextItemView** |
| Insert → Image | `CanInsertImage(TargetTextEditor)` |

Form Text tables share Document table behaviors: **Tab** / **Shift+Tab** cell navigation; multi-cell align (highlighted cells only); resize chrome plus one top-left ✥ move handle (no float wrap toggles).

**Heading**, **FIB**, **MCQ** rich regions may enable subset of formatting; **Function / Hyperlink / Invitation** require **Text** item specifically.

---

## Owner documentation gaps

- [x] **Format → Styles** FIB dialog (Jul 17 screenshots + `FibStylesDialog`)
- [x] **Format → Styles** MCQ / Text dialogs (Jul 17 screenshots + `McqItemStylesDialog` / `TextItemStylesDialog`)
- [x] **Preview tab** — owner: only www.tawala.com error (see `DESIGNER_STARTUP_AND_FORM_CANVAS.md` §8)
- [x] **Page Header** — `DESIGNER_PAGE_HEADER.md`

### Browser (`designer-web`) — Jul 17, 2026

- **Format** menu: **removed** (palette + Project).
- Live B/I/U/color/fonts/tables: **Formatting Palette**.
- **Project → Styles…**: **wired** — opens from selected FIB / MCQ / Text; Apply to Selected (one item); **Apply to All** (all matching on **active form** only).
- **Project → Tabs…**: **removed** (Jul 17). Legacy dialog documented; compatibility fields/converters retained for imported projects.
- **Page Header / Themes**: **stubs** on Project for **8080 / CSS** track.
- Styles **visual** dialog parity: **Jul 17**.
- **Design-canvas Style paint (owner Jul 18):** **Text** Instructional/Error **shown** on Forms → Text (can't otherwise tell them apart). **FIB/MCQ layout paint = won't do** — would interfere with editing; Preview is immediate for Forms and Documents mostly show it already.

---

*Last updated: July 18, 2026 — Design Text Style paint kept (Forms → Text); FIB/MCQ Design layout paint dropped per owner (interferes with editing; Preview immediate).*
