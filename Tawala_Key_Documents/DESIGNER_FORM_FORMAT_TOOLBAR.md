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

Merged into main **Format** menu when Form MDI active:

| Item | Notes |
|------|-------|
| Bold / Italic / Underline / Color / Reset | Same as Document |
| **Page Header…** | Project-wide banner (text + optional image) — **not** the Heading form item; see `DESIGNER_PAGE_HEADER.md` |
| **Project Themes** | Theme checklist (see `DESIGNER_MENU_SPEC.md`) |
| **Tabs…** | Tab stops in inches |
| **Styles** → | Form-item style dialogs only on Form |
| → **Fill in the Blank…** | Global FIB layout style |
| → **Multiple Choice…** | Global MCQ layout style |
| → **Text…** | Text style dialog (title **Styles**) |

Source typo in menu: **Reset Formattting** (three t’s) in `MDIFormView.Designer.cs`.

Screenshots: `assets/Format_-_Styles_-_FIB-*.png`, `Format_-_Styles_-_MCQ-*.png`, `Format_-_Styles_-_Text-*.png`.

---

## Format → Styles dialogs (owner June 2026)

Opened from **Format → Styles →** sub-menu when a **Form** MDI child is active.

**Shared behavior (all three):**

- Live **preview** panel (lavender background) updates as options change.
- Footer note: *Style may be applied only to selected [item type] questions in the active form, if any. The "Apply to All" feature has been disabled.*
- **Apply to All** — **always greyed** (removed in source Sept 2009 — “inadvertent click wreaks havoc in large projects”).
- **Apply to Selected** — enabled when a layout/style radio is chosen; applies to **selected** items of that type on the active form (greyed if none selected / no style picked).
- **Cancel** — closes without applying.

---

### Fill in the Blank Styles

**Title:** `Fill in the Blank Styles`  
**Menu:** Format → Styles → **Fill in the Blank…**

| Group | Control | Options |
|-------|---------|---------|
| **Labels** | Radio | **Above** (default in screenshot), **Left justified**, **Right justified**, **Freeform** |
| **Blanks** | Checkbox | **Align right side** — enabled only when **Left justified** or **Right justified** selected; greyed for Above/Freeform |

Preview shows sample FIB layout for the selected label/blank combination.

---

### Multiple Choice — Styles

**Title:** `Styles`  
**Menu:** Format → Styles → **Multiple Choice…**

| Layout | Preview |
|--------|---------|
| **Vertical** | Question + choices stacked (sample: “What is your favorite color?”) |
| **Horizontal** | Choices in one row |
| **Multi-column** | Choices in columns; sub-dropdown **greyed** until Multi-column selected |

**Multi-column** column-count dropdown (enabled when Multi-column selected): **Auto**, **2**, **3**, **4**, **5**

| Group | Control |
|-------|---------|
| **Spacing** | **Do not add blank space below question when displayed.** (checkbox) |

Maps to project/form `mcItemStyle` (e.g. `vertical`, `horizontal`, multi-column).

---

### Text — Styles

**Title:** `Styles`  
**Menu:** Format → Styles → **Text…**

| Style | Preview appearance |
|-------|-------------------|
| **Normal** | “This is normal text” — plain |
| **Instructional** | “This is instructional text” — **bold italic** |
| **Error** | “This is error text” — **bold italic red** |

| Group | Control |
|-------|---------|
| **Spacing** | **Do not add blank space below text when displayed.** (checkbox) |

Maps to `textItemStyle` (`normal`, `instructional`, `error`).

---

## Enable rules (source)

| Control | Enabled when |
|---------|--------------|
| Format toolbar (most) | Design tab + rich-text editor focused |
| **Insert Table** | Parent is **TextItemView** |
| **Tables** menu | Parent is **TextItemView** |
| **fx** / Insert → Function | Parent is **TextItemView** |
| Insert → Hyperlink / Invitation | Parent is **TextItemView** |
| Insert → Image | `CanInsertImage(TargetTextEditor)` |

**Heading**, **FIB**, **MCQ** rich regions may enable subset of formatting; **Function / Hyperlink / Invitation** require **Text** item specifically.

---

## Owner documentation gaps

- [x] **Format → Styles** sub-dialogs (FIB / MCQ / Text)
- [x] **Preview tab** — owner: only www.tawala.com error (see `DESIGNER_STARTUP_AND_FORM_CANVAS.md` §8)
- [x] **Page Header** — `DESIGNER_PAGE_HEADER.md`

---

*Last updated: June 2026 — from `MDIFormView.cs`, `Dialogs/*StylesDialog.cs`.*
