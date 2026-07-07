# Designer form items — Heading

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

**Not** the same as **Format → Page Header…** (project-wide banner on deployed pages). See `DESIGNER_PAGE_HEADER.md`.

Related: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`, `DESIGNER_MENU_SPEC.md` (Items palette #1).

Screenshots:

| Asset | What it shows |
|-------|----------------|
| `assets/Forms_-_Heading-*.png`, `assets/Forms_-_Heading1-*.png` | Earlier Heading captures (June 2026) |
| `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Drag_Heading_to_Canvas-e9347d50-26e5-4d29-b855-df7089e8b8d1.png` | **Insert / editing (July 2026)** — click Heading in docked Items bar → inline WYSIWYG row with placeholder selected + **Heading Type** dropdown visible |
| `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Finished_Heading-be3d8494-10d0-4818-8857-bc062ba4c222.png` | **Finished / blur (July 2026)** — collapsed row: **H1** badge + heading text only (`Welcome Campers!`); no Heading Type dropdown |
| `assets/ClickNewForm-*.png` | Items palette docked beside Explorer (see `DESIGNER_BACKLOG_ARCHITECTURE.md` D-Items-palette-placement) |

---

## Palette

| Item | Tooltip |
|------|---------|
| **Heading** | Add a heading item to the form to highlight sections of the form. |

**Insert → Heading** (Form context, Design tab).

---

## Canvas (Design tab)

Form windows expose **Design** and **Preview** tabs above the canvas (browser Designer: `FormEditor.tsx` `form-tabs`).

Headings have so few properties that legacy **does not use a per-item Properties popup** — label, text, and heading type all live **on the canvas in Design mode**. The permanent right-hand Properties/Inspector panel remains for other form items; Heading is the **canvas-inline exception** (owner, July 2026; aligns with D-Form-items incremental popup migration in `DESIGNER_BACKLOG_ARCHITECTURE.md` §5).

### Design-mode state machine

| State | When | Visible on canvas |
|-------|------|-------------------|
| **inserting → editing** | User clicks **Heading** in Items bar, or **clicks the heading box** on canvas when focus was elsewhere (same click-to-activate as Text) | Orange **H1** badge; inline rich-text area with placeholder **`[Replace this with heading of your own.]`** **selected** (black highlight); **Heading Type:** label + dropdown below text (**Main** / **Sub**) |
| **collapsed (finished)** | User has typed heading text and **cursor leaves** the heading box (blur / validate) | **H1** badge + **rendered heading text only** — no Heading Type row, no placeholder highlight, no editing chrome |

Transitions:

1. **Insert:** `AfterAddedToFormByUser` → focus text editor → `SelectAll()` on default RTF (`HeadingView.cs`).
2. **Edit:** `GotFocus` on text editor → show **Heading Type** options panel (`enableOptionsPanel(true)`; `HeadingOptions` embedded below text).
3. **Collapse:** `OnValidated` / `releaseOptionsPanel()` → hide options panel; row height shrinks to badge + text (`GetPreferredSize` omits options height when panel released).

**Click-to-activate:** When focus was elsewhere, **click the heading box** to restore the **editing** row (badge + inline editor + Heading Type), not stay collapsed — same activation model as Text.

**Source:** `HeadingView.cs` (`AfterAddedToFormByUser`, `itemTextEditor_GotFocus`, `OnValidated` → `releaseOptionsPanel`).

### Properties on canvas (not Properties panel)

| Property | UI on canvas | Storage / notes |
|----------|--------------|-----------------|
| **Label** | Orange (or light gray when collapsed) square, bold **`H1`**, **`H2`**, … | XML `label` attribute; `H` = Heading, digit = sequence among headings on the form |
| **Content** | Inline rich-text field (editing) or styled heading text (collapsed) | RTF in legacy; plain/HTML in browser Designer until full RTF parity |
| **Heading Type** | Label **Heading Type:** + dropdown **below** text — visible **only while editing** | XML `type` = `Main` or `Sub`; maps to theme heading styles (36pt Main in default RTF) |

Default placeholder on insert: **`[Replace this with heading of your own.]`** (`Resources.HeadingItemDefaultRTF`).

### Owner screenshot — click Heading in Items bar (July 2026)

Verified from `Drag_Heading_to_Canvas-*.png` (**inserting / editing** state):

| UI region | State |
|-----------|--------|
| **Form window** | `Form - Form 1`, **Design** tab active |
| **Items bar** | Docked beside Project Explorer (not inside the form window); **Heading** icon has **blue border** (last-clicked / active palette item) |
| **Canvas** | One Heading row at top; orange **H1** badge; placeholder text **selected**; **Heading Type: Main** dropdown visible below text; blank white canvas below |
| **Fields panel** | Right-hand tree (Variables / Form 1) — unchanged |

### Owner screenshot — finished / blur (July 2026)

Verified from `Finished_Heading-*.png` (**collapsed** state):

| UI region | State |
|-----------|--------|
| **Form window** | `Form - Form 2`, **Design** tab active |
| **Canvas row** | **H1** in light gray bordered box (left); large heading text **`Welcome Campers!`** to the right |
| **Not visible** | **Heading Type** dropdown, placeholder brackets, inline editor chrome, Delete/debug label bar |
| **Format toolbar** | Present at top of app shell; individual icons mostly greyed (full toolbar parity deferred) |

Blur behavior: once the user replaces the placeholder and focus leaves the heading, the row **collapses** to label + rendered text until the user **clicks the heading box** again to edit.

### Canvas WYSIWYG row layout (legacy target)

Everything below lives **on the canvas inside the form window**, not in the permanent Properties panel:

| Element | Editing state | Collapsed state |
|---------|---------------|-----------------|
| **H1 badge** | Orange square, bold **`H1`** on the left of the row | Light gray bordered box, bold **`H1`** (screenshot); increments **H2**, **H3**, … (`label` attribute) |
| **Inline text** | Rich-text field; default **`[Replace this with heading of your own.]`** selected on insert | Rendered heading at theme size (e.g. **`Welcome Campers!`**) |
| **Heading Type:** | Label + dropdown **below** the text | **Hidden** until item re-enters edit mode |

**Popup vs canvas:** Legacy keeps **Heading Type** on the canvas while editing (`HeadingOptions` panel embedded in `HeadingView`; hidden on blur via `releaseOptionsPanel`). Per owner decision **D-Form-items strategy**, other items may gain per-item Properties **popups** later — but Heading stays **canvas-inline** because its property count does not warrant a popup. Do not move Heading editing to Properties-only.

**Format toolbar:** Entirely greyed while cursor is in Heading text (see below) — shown in finished screenshot but full parity deferred this pass.

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

**Shared activation (owner, July 2026):** Heading and Text both use **click the box** to make inline editing live when focus was elsewhere. **Blur contrast:** Heading **collapses** (badge + rendered text only; Heading Type hidden). Text has **almost no visual change** on blur — badge + rich-text body stay visible.

| | Heading | Text |
|---|---------|------|
| Label prefix | `H` | `T` |
| XML tag | `heading` | `text` |
| Click to activate (focus elsewhere) | **Click heading box** | **Click text box** |
| Blur / collapse | **Yes** — badge + rendered text only; Heading Type hidden | **No** — inline body stays visible; almost no visual change |
| Format toolbar while editing | **All greyed** | Active (full or partial by sub-region) |
| Heading Type dropdown | **Yes** | No |
| Default placeholder | `[Replace this with heading of your own.]` | `[Replace this with text of your own.]` |

---

## Browser Designer gap

**Items dock (IMPLEMENTED, July 2026):** `FormItemsPalette` docks beside Explorer; click **Heading** → `insertFormItem("heading")` appends to the active form (`selection.name` synced via `focusWindow`). See `DESIGNER_BACKLOG_ARCHITECTURE.md` D-Items-palette-placement.

**Canvas WYSIWYG (NOT implemented — first form-item target):**

| Legacy / owner spec | `designer-web` today (`FormEditor.tsx`) |
|---------------------|----------------------------------------|
| Design-mode **state machine**: inserting → editing (chrome visible) → **collapsed on blur** (badge + text only) | Single static render; no edit/collapse states |
| Orange **H1** badge on left of row (gray when collapsed) | Generic `[heading] H1` debug strip in `.form-item-label` above content |
| Inline rich-text; placeholder `[Replace this with heading of your own.]` | Static `<h2>` / `<h3>` preview (`preview-heading-main/sub`); no inline edit |
| Placeholder **selected on insert** | `createDefaultItem` sets `content: "New heading"`; no auto-focus or select-all |
| **Heading Type:** dropdown **on canvas while editing**; **hidden on blur** | **Level** dropdown in permanent **Properties** panel only (`FormItemProperties`); always available, wrong label |
| Collapsed row: no Delete chrome, no type dropdown, no dashed debug wrapper | Dashed `.form-item-block` wrapper + **Delete** button + `[type]` label bar always visible |
| Items bar: clicked icon **blue border** | Text buttons; no icon grid; no last-clicked highlight |
| Click heading box → restore editing row with Heading Type | Selection only toggles `.selected` CSS on debug block |
| Format toolbar greyed in Heading context | Not wired to Heading focus |

**Implementation hooks (when scheduled):**

- `FormEditor.tsx` — replace `CanvasItem` `"heading"` branch with `HeadingCanvasRow` (props: `item`, `index`, `selected`, `onUpdate`, `onBlurCollapse`).
- `createDefaultItem` — default `content` to `[Replace this with heading of your own.]`.
- Local component state: `mode: "editing" | "collapsed"`; enter editing on insert + on row click/focus; collapse on text `blur` (mirror `HeadingView.OnValidated` / `releaseOptionsPanel`).
- Optional: `insertFormItem` flag or store field to auto-focus new heading row.
- CSS: `.heading-canvas-row`, `.heading-badge`, `.heading-type-row` (hidden when collapsed).
- Palette highlight state in `FormItemsPalette`.

**Properties popup:** Not planned for Heading — canvas-inline is intentional. Permanent Inspector stays for other items until D-Form-items migration.

**Deferred this pass:** Full rich-text WYSIWYG toolbar, RTF round-trip, per-character formatting in Heading text.

---

## Source

- `TawalaDesigner/Code/TAWALA/Forms/HeadingView.cs`
- `TawalaDesigner/Code/TAWALA/Forms/HeadingOptions.cs`
- `TawalaDesigner/Code/TAWALA/Forms/MDIFormView.cs` (line ~245: disable format when `HeadingView`)
- `TawalaDesigner/Code/TAWALA/Projects/Forms/HeadingItem.cs`
- `Resources.HeadingItemDefaultRTF` — default placeholder text

---

*Last updated: July 2026 (owner insert + finished/blur screenshots; design-mode state machine; unified click-to-activate with Text; blur contrast).*
