# Tawala Designer — Form Items: Text, FIB, MCQ

*From live Designer walkthrough on sample form (H1 + T1 + Q1 + MCQ), June 2026.*  
*See `DESIGNER_STARTUP_AND_FORM_CANVAS.md` for canvas basics; `DESIGNER_MENU_SPEC.md` for shell.*

---

## Common canvas conventions

| Element | Meaning |
|---------|---------|
| **H1**, **T1**, **Q1**, **Q2** | Auto-assigned **design labels** (orange bar when selected). Legacy often used **Qn** for both FIB and MCQ. |
| **Browser defaults (Jul 12, 2026)** | New inserts use **FIB1**, **FIB2**, … and **MCQ1**, **MCQ2**, … so the form item list distinguishes question types. Heading/Text stay **H**/**T**. |
| **Custom left label** | User can rename (e.g. Q1 row still shows **Surveyee** logic via alternate label; MCQ bar renamed **Angel Question**) |
| **Fields panel** | Lists **data field names** (alternate labels / MCQ names), not always the design label |
| **Black highlight** | Inline text selected for editing |
| **Format toolbar** | Active when rich text in an item has focus |

---

## Text item (T1)

*Owner reference (July 2026): `Text_Item_in_Forms-*.png`, `Text_Tools_Palette-*.png`. Formatting Palette spec: `DESIGNER_FORM_FORMAT_TOOLBAR.md`, `DESIGNER_DOCUMENT_EDITOR.md` § Document format toolbar.*

### On canvas

| UI | Content |
|----|---------|
| Left label | Orange **T1** badge |
| Body | Full **rich text** WYSIWYG; default placeholder: **`[Replace this with text of your own.]`** |
| Example after edit | `This is where directions will go.` |

### Owner screenshot — Text selected on Form canvas (July 2026)

Verified from `Text_Item_in_Forms-*.png`:

| UI region | State |
|-----------|--------|
| **Form window** | `Form - Form 1`, **Design** tab active |
| **Canvas** | **H1** heading row above (`Single Question Project`); **T1** orange badge with dotted selection border; placeholder **`[Replace this with text of your own.]`** **selected** (black highlight on white) |
| **Formatting Palette** | **Live** in main toolbar area (row 2, below menu bar) — font/size dropdowns, color, eraser, B/I/U, alignment, indent, table insert, **fx** all enabled |
| **Items bar** | Docked beside Explorer; **Text** icon available |
| **Fields panel** | Right-hand tree — unchanged |

### Behavior

- Added from **Items → Text** or **Insert** (form selected).
- **Canvas-inline WYSIWYG** — like Heading, but with **full rich text** (not plain heading styles). Cursor anywhere in body; insert/delete; box **expands vertically and horizontally** with content.
- **No per-item Properties popup** — label, body, and inline editing all live on the canvas in Design mode (owner, July 2026). Aligns with Heading as a canvas-inline exception under D-Form-items strategy (`DESIGNER_BACKLOG_ARCHITECTURE.md` §5).
- Formatting uses the shared **Formatting Palette** above the canvas (app shell row 2) — **not** embedded inside the form window. Same palette serves **Form Text items** and **Documents**.
- **Fields / variables:** Designer can **drag** field or variable references from the **Fields** panel into the text body (inserts `<<…>>` tokens). Text is not listed as its own field in Fields, but it can **reference** fields/variables inline.
- Does **not** appear as a named field under **Fields** (unlike FIB blanks / MCQ).

### Design-mode edit behavior (owner, July 2026)

**Shared activation:** Heading and Text both use **click the box** to make inline editing live when focus was elsewhere in Designer.

**Blur contrast:** Heading **collapses on blur** to badge + rendered text only (Heading Type dropdown hidden). Text **does not** collapse — the inline rich-text body stays visible whether or not the cursor is in the box; almost no visual difference between edit and non-edit — **except** while editing, the rich-text box uses a taller min-height / bottom padding so a lone function token still leaves clickable space to place the caret (Jul 13).

| State | Visible on canvas |
|-------|-------------------|
| **Insert / editing** | Orange **T1** badge; inline rich-text area; placeholder **selected on insert** (per screenshot) |
| **Not focused (blur)** | Same as editing — badge + rich-text body; no collapse to a static preview |

See contrast table in `DESIGNER_FORM_ITEMS_HEADING.md` § vs Text item.

### Formatting Palette — enable / disable rules

Palette lives on **row 2** of the app shell when a **Form** or **Document** MDI child is active. Source: `MDIFormView.Application_Idle`, `MDIDocumentView` idle handlers.

| Context | Palette state |
|---------|---------------|
| **Heading** item — cursor in heading text | **Entire palette greyed/disabled** (all controls) |
| **Text** item — cursor in text body | **Palette live** (full rich-text controls) |
| **Document** editor — cursor in body | **Palette live** |
| **FIB / MCQ** rich regions | **Subset** live: B/I/U only; font face, size, color, reset, indent, alignment, tables, **fx** greyed (`MDIFormView.cs` lines 281–287) |
| Nothing focused / Design tab inactive | **Greyed** (`textEditorIsInactive()`) |
| **Insert Table** (#11) | Enabled when parent is **TextItemView** (Form) or document editor (`CanInsertTable`) |
| **Delete Table** (#12) | Enabled only when `CursorInTable` |
| **Insert or Delete Row or Column** (#13) | Enabled only when `CursorInTable` — **greyed** on fresh text/document with no table |
| **fx** (#14) | Enabled when parent is **TextItemView** (Form Text) or document editor |
| **Reset Formatting** (#4) | **Greyed** on fresh document before any formatting applied (owner, June 2026) |

When **Heading** is selected (even if Text is also on the form), palette greys as soon as focus is in the Heading editor — not when merely selecting the Heading row without focus.

### Formatting Palette — button inventory (left → right)

Shared between Form Text and Document. Tooltips from `DESIGNER_DOCUMENT_EDITOR.md` (owner-verified June 2026); Form adds **fx** Text-only gate per `DESIGNER_FORM_FORMAT_TOOLBAR.md`.

| # | Control | Rollover tooltip | Notes |
|---|---------|------------------|-------|
| 1 | Font face dropdown | **Font Face** | Label **Default Font** when no specific face at cursor |
| 2 | Font size dropdown | **Font Point Size** | Label **Default Size** when no specific size at cursor |
| 3 | Font color split button | **Font Color** | **A** + color bar; arrow opens color submenu |
| 4 | Reset formatting | **Reset Formatting** | Eraser icon |
| — | *(separator)* | | |
| 5 | Bold | **Bold** | |
| 6 | Italic | **Italic** | |
| 7 | Underline | **Underline** | |
| — | *(separator)* | | |
| 8 | Outdent | **Outdent** | |
| 9 | Indent | **Indent** | |
| 10 | Alignment split button | **Paragraph Alignment** | Left / center / right / justify |
| — | *(separator)* | | |
| 11 | Insert table | **Insert Table** | Grid icon |
| 12 | Delete table | **Delete Table** | Grid + red **X**; greyed until cursor in table |
| 13 | Insert/delete row or column split | **Insert or Delete Row or Column** | Grid + green **+**; greyed until cursor in table |
| — | *(separator)* | | |
| 14 | **fx** | **Insert or Edit a Function** | Form: Text item only; Document: always when editor active |

**Not on palette** (Format menu / other): Page Header, Project Themes, Tabs, Styles dialogs — see `DESIGNER_FORM_FORMAT_TOOLBAR.md`.

Screenshot references:
- Form + Text selected: `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Item_in_Forms-e712e814-aeeb-4eae-8601-29da8826b108.png`
- Document palette detail: `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Tools_Palette-3a4f7923-cab1-46fb-b320-3555eaacc3be.png`
- Earlier captures: `assets/Text_Tools_Palette-*.png`, `assets/Formatting_-_*.png`

### vs Heading item

| | Heading | Text |
|---|---------|------|
| Label prefix | **H** | **T** |
| XML tag | `heading` | `text` |
| Properties popup | **No** (canvas-inline) | **No** (canvas-inline) |
| Click to activate (focus elsewhere) | **Click heading box** | **Click text box** |
| Blur / collapse | **Yes** — collapses to badge + text only; Heading Type hidden | **No** — almost no visual change between edit and non-edit |
| Formatting Palette while editing | **All greyed** | **Live** (full) |
| Heading Type dropdown | **Yes** (canvas, while editing) | No |
| Rich text scope | Theme heading styles only | Full character/paragraph formatting |
| Default placeholder | `[Replace this with heading of your own.]` | `[Replace this with text of your own.]` |

### Implementation dependency (browser Designer)

**Full Text canvas WYSIWYG is blocked on shared Formatting Palette implementation.** Palette is also critical for the **Documents** editor. Today `RichTextEditor.tsx` embeds a minimal B/I/U + font-size strip inside the Properties panel only — not the legacy shell-level palette.

See gap table below and `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` §6 (Formatting Palette).

### Text must-not-break smoke (browser Designer)

1. Set **Trebuchet MS** + **20 pt** on a Form Text body → type a line → **Return** → type again → second paragraph stays **Trebuchet 20**; palette Face/Size still shows that pair (not Arial / default 12). Cross-check Document smoke #12 in `DESIGNER_DOCUMENT_EDITOR.md`.

---

## Fill in the Blank — FIB (Q1)

### On canvas — prompt area

| UI | Content |
|----|---------|
| Left label | **Q1** (default) |
| Default prompt | `[Replace this with your question. Underscores create blanks.]` |
| Blanks | Underscore runs in prompt text create input fields (e.g. `Name: __________`) |
| Example | Visible label **Name:** + blank line |

### On canvas — property strip (below prompt)

| Control | Purpose |
|---------|---------|
| **Alternate Label** | Internal field name for **Fields** panel and **Processes** (e.g. `Surveyee` while user sees **Name:**) |
| **Height** | Line count for blank input area (`1` = single line; higher = multi-line for long answers) |
| **Required** | Checkbox — response required |
| **Validation** | Dropdown (see below) |
| **Edit...** | Opens validation editor when a validation type is selected (greyed when `-- No Validation --`) |

### Validation dropdown options

- `-- No Validation --`
- Email
- Phone Number
- Integer
- US State
- ZIP Code
- Proper Name
- Dollar Amount

### Fields panel

- Default blank slot names: **`Q1:a`**, **`Q1:b`**, … (letter per blank in item).
- Designer may set **Alternate Label** per blank; must be **unique** (duplicate names rejected).
- When alternate label is set, that name appears in Fields (e.g. `Surveyee`).

### Notes

- Blank length can be adjusted by **more/fewer underscores** and/or **Height**.
- Visible prompt text and alternate label are **independent** (important for export/runtime field names).
- **Multiple blanks** in one FIB item: **each blank** has its own Alternate Label (default `Qn:a`, `Qn:b`, …).
- **Edit vs idle:** while editing, underscore characters are typed literally. When the FIB row is **idle** on Design, underscore runs stay as `_` (length still drives blank metadata). **Preview** and **Deploy** convert `_` runs into input boxes.

### FIB must-not-break smoke (browser Designer)

Run before merging FIB canvas, `fibBlanks`, `fibPrompt`, or Form Preview/runtime FIB changes:

1. Design: insert FIB, type `Name ________` in the prompt, blur/deselect so the row is idle → underscores remain visible `_` (not Design-canvas text boxes).
2. Design: place caret in an underscore run while editing → Alternate Label / Height / Required still target that blank.
3. Preview: same item shows **Name** + one input — **no** leftover `________` beside the box. (Restart `designer-web` API on `:3001` after `server/runtime.mjs` / `fibPrompt.mjs` edits — Node does not HMR those.)
4. Deploy: a Design soft-row with two underscore blanks (e.g. `First ____ Last ____`) must stay **one line** with both boxes — not one blank per line (`fibToXml` / Redeploy to Java).
5. Deploy: B/I/U (and face/size/color when set) on FIB prompt labels in Design must appear on the Java form after Redeploy (`fibRichPromptToXml` / `fibToXml`).
5. Unit: `cd designer-web && npm test` (covers underscore → blanks metadata and Preview prompt parse).

### Backlog parity note (July 2026)

- **Backlog / parity only:** preserve support for a **secondary smaller and/or italic hint style** for FIB labels or parenthetical helper text.
- Use the DirtBowl example screenshot as a reference when revisiting FIB rendering parity in `designer-web`.
- Saved image path: `/Users/DougC1/.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Screenshot_2026-07-03_at_2.26.25_PM-a0b04c0f-0e0b-4528-81ff-ec95a8fd481b.png`

---

## Multiple Choice — MCQ (Q2+)

### On canvas — question area

| UI | Content |
|----|---------|
| Left label | **Q2** default; user may rename bar (e.g. **Angel Question**) — name appears in **Fields** |
| Default placeholder | `[Replace this with your question. Use Enter key to add choices below.]` |
| Question text | Click highlighted area to type question (e.g. *How many angels can dance on a pin?*) |

### Choices — manual entry (“Choices are entered above”)

| Interaction | Behavior |
|-------------|----------|
| Click after **a)** | Type first choice text |
| **Enter** | New line for next choice (**b)**, **c)**, …) |
| Choice labels | Auto-lettered **a)**, **b)**, **c)**, **d)** |

### On canvas — property strip

| Control | Purpose |
|---------|---------|
| **User may select more than one** | Checkbox → checkbox MCQ vs radio (only one) |
| **Response required** | Checkbox |
| **Choice source** dropdown | See below |
| **Edit** | Link; active when choices from stored data |

### Choice source dropdown

| Option | Meaning |
|--------|---------|
| **Choices are entered above** | Manual a/b/c choices in question body |
| **Choices are from stored data** | Dynamic MCQ from project data / SDT (e.g. Divisions in SportsDashboard) |

### Configure Function dialog (dynamic MCQ)

Opened via **Edit** when using stored data.

**Title:** Configure Function  
**Type:** DYNAMIC MCQ — *Populate choices from stored data*

| Field | Purpose |
|-------|---------|
| **Display text** | Field/expression for choice text shown to user (**REQUIRED**) |
| **Value** | Field/expression for stored value |
| **Sort by** | Field/expression for sort order |
| **Function Conditions** | Filter: field + operator + value; **+** / **−** for more rows |
| | Help link: “A compound expression” |

**Buttons:** OK, Cancel

Right panel shows context help for the focused field.

### Fields panel

- MCQ appears under form folder (e.g. **Angel Question** when bar renamed).
- FIB alternate label listed separately (e.g. **Surveyee**).
- Renaming the **left orange bar** likely updates the item’s **label** attribute in project XML (owner: “default label”; exact XML mapping TBD).

### Validation → Edit... (FIB)

Each validation type opens a **popup** for custom error messaging.

| Type | Dialog behavior |
|------|-----------------|
| *(general)* | Custom **error message** (e.g. Phone default: *“Please enter a valid number including area code.”*) |
| **Integer** | Also set **upper and lower limits** per field (e.g. birth-date range tests) |

*(Screenshot of validation dialog pending.)*

### Height vs underscores (FIB)

**Height** and underscore count are independent: a tall **Height** with a short underscore run can yield a tall, narrow input box (unverified in Preview while `www.tawala.com` is down).

---

## Example form state (from screenshots)

| Label | Type | Visible / notes | Fields name |
|-------|------|-----------------|-------------|
| H1 | Heading | Single Question Project | — |
| T1 | Text | Directions paragraph | — |
| Q1 | FIB | Name: + blank | **Surveyee** |
| Angel Question | MCQ | Angels on a pin; 4 choices; multi-select + required | **Angel Question** |

Form renamed **Form 1** → **Start** in later shots.

---

## File Uploader (deferred — out of browser Items palette)

**Status:** Not on the Jan 2011 reference Designer UI. Present in production projects (**SportsDashboards** divisions) and in repo C# source. **Owner Jul 17:** omitted from the browser Items palette (no greyed stub) and dropped from **Insert → Image** wording (“Tawala Upload”). Images in browser Designer: **From your PC…** (embed) or **From the Web…** (DISPLAY IMAGE URL). Keep this section for XML/`<file>` parity if a later build ever needs the form item.

### From source + XML (for browser Designer later)

| Aspect | Detail |
|--------|--------|
| Default label prefix | **F1**, **F2**, … (`alternateLabel` optional, e.g. `File1`) |
| Canvas | Inline rich-text instructions + embedded **file picker** (`<fileNameInput/>` in XML) |
| Default prompt | *“Select a file on your system for upload to the server.”* |
| Property strip | **Response required** checkbox only (no validation dropdown like FIB) |
| Fields panel | Listed like FIB/MCQ (`FormName:alternateLabel` or default label) |
| XML root | `<file label="F1" alternateLabel="…" required="false" style="topLabels">` … `<fileNameInput/>` |

Example (SportsDashboards template in test fixtures):

```xml
<file label="F1" alternateLabel="File1" required="false" style="topLabels">
  <paragraph>…instruction text…<fileNameInput/></paragraph>
</file>
```

**Next:** Owner documented **Hidden Field**, **Page Break**, and **Skip Instructions** in `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` (includes Skip + Break tutorial).

---

## Browser Designer (`designer-web`) gaps

| Feature | Legacy | Browser today |
|---------|--------|----------------|
| **Formatting Palette (row 2, shared)** | Full 14-control toolbar; Form Text + Document | **Missing** — `ToolBar.tsx` is standard commands only; no row 2 |
| Text canvas-inline WYSIWYG | Yes — badge + inline rich body on Design tab | **Static HTML preview** in `FormEditor.tsx` `CanvasItem` `"text"` branch; edit only in permanent Properties panel |
| Text — no Properties popup | Yes | **Wrong** — `FormItemProperties.tsx` still hosts `RichTextEditor` for Text |
| Text placeholder on insert | `[Replace this with text of your own.]` selected | `createDefaultItem` sets `content: ""`; no placeholder, no select-all |
| Text **T1** orange badge on canvas | Yes | Generic `[text] T1` debug label bar |
| Palette live when Text focused | Yes | N/A (no palette) |
| Palette greyed when Heading focused | Yes | N/A (no palette) |
| Table tools gated on `CursorInTable` | Yes | N/A |
| Text inline rich edit (Properties) | N/A (canvas only) | `RichTextEditor` with embedded mini-toolbar (B/I/U + size only) |
| FIB underscore → blanks | Yes | Design idle keeps `_`; Preview/`fibPrompt` strips `_` into blank inputs. Smoke + `npm test`. |
| FIB alternate label | Yes | `blank.name` / `alternateLabel` partial |
| FIB height | Yes | Not exposed |
| FIB validation types | Yes | Not exposed |
| MCQ inline choice entry | Yes | JSON choices array |
| MCQ multi-select / required | Yes | Partial in JSON |
| Dynamic MCQ + Configure Function | Yes | `dynamic` choices in JSON only |
| Rename item label on canvas | Yes | Uses `item.label` only |
| Default design labels for FIB / MCQ | Often **Qn** for both | **FIB**n / **MCQ**n on insert (`insertFormItem`) so lists distinguish types |

---

## Resolved / open (owner notes, June 2026)

| # | Topic | Answer |
|---|--------|--------|
| 1 | Text + Fields | Drag fields/variables into text; inserts references. No Preview to verify runtime. |
| 2 | Multi-blank FIB | Each blank: default `Q1:a`, `Q1:b`, …; alternate labels unique. |
| 3 | MCQ bar rename | Believed to change default **label** attribute (XML TBD). |
| 4 | Validation Edit | Popup per type: custom error message; Integer adds min/max. Image pending. |
| 5 | Height | Tall thin box possible; Preview unavailable. |
| 6 | Text blur / collapse | **No collapse** — unlike Heading, Text looks almost the same in edit vs non-edit. **Activation** same as Heading: **click the box** when focus was elsewhere. |

---

*Last updated: July 2026 — FIB underscore smoke + dual-pipeline tests; idle blank inputs restored; Preview `fibPrompt` strips `_` runs; regression-prevention Cursor rule.*
