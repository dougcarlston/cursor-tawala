# Designer form items — Heading

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

**Not** the same as **Format → Page Header…** (project-wide banner on deployed pages). See `DESIGNER_PAGE_HEADER.md`.

Related: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`, `DESIGNER_MENU_SPEC.md` (Items palette #1).

Screenshots: `assets/Forms_-_Heading-*.png`, `Forms_-_Heading1-*.png`.

---

## Palette

| Item | Tooltip |
|------|---------|
| **Heading** | Add a heading item to the form to highlight sections of the form. |

**Insert → Heading** (Form context, Design tab).

---

## Canvas (Design tab)

When a Heading is added or selected:

| Element | Appearance |
|---------|------------|
| **Label bar** | Orange square with bold **`H1`** (increments H2, H3, … for additional headings) |
| **Text area** | Inline rich-text field; default placeholder: **`[Replace this with heading of your own.]`** |
| **Heading Type:** | Dropdown directly below the text — **`Main`** or **`Sub`** |

On add, focus moves into the text area and **all placeholder text is selected** (source: `AfterAddedToFormByUser` → `SelectAll()`).

---

## Heading Type

| Value | XML `type` attribute |
|-------|----------------------|
| **Main** | `Main` |
| **Sub** | `Sub` |

Example: `<heading label="H1" type="Main" …>`.

Default RTF uses 36pt (`fs36`) for Main heading text in resources.

---

## Format toolbar — **entirely greyed out** (owner + source)

When the cursor is in a **Heading** item (including while editing heading text), **all Format toolbar icons are disabled** — font face, size, color, bold/italic/underline, indent, alignment, tables, **fx**.

**Source:** `MDIFormView.Application_Idle` explicitly disables the format toolbar when `TargetTextEditor.Parent is HeadingView` — not a bug.

Heading styling is controlled by **Heading Type** (Main/Sub) and theme/global heading styles, not per-selection character formatting from the toolbar.

**Insert** menu rich-text items (Image, Hyperlink, Function) are also unavailable in Heading context (Text item only).

---

## vs Text item

| | Heading | Text |
|---|---------|------|
| Label prefix | `H` | `T` |
| XML tag | `heading` | `text` |
| Format toolbar while editing | **All greyed** | Active (full or partial by sub-region) |
| Heading Type dropdown | **Yes** | No |
| Default placeholder | `[Replace this with heading of your own.]` | Varies by style |

---

## Browser Designer gap

Heading not implemented as a distinct item type in `designer-web`.

---

## Source

- `TawalaDesigner/Code/TAWALA/Forms/HeadingView.cs`
- `TawalaDesigner/Code/TAWALA/Forms/HeadingOptions.cs`
- `TawalaDesigner/Code/TAWALA/Forms/MDIFormView.cs` (line ~245: disable format when `HeadingView`)
- `TawalaDesigner/Code/TAWALA/Projects/Forms/HeadingItem.cs`
- `Resources.HeadingItemDefaultRTF` — default placeholder text

---

*Last updated: June 2026.*
