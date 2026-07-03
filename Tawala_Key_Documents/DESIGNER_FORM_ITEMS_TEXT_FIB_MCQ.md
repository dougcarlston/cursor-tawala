# Tawala Designer — Form Items: Text, FIB, MCQ

*From live Designer walkthrough on sample form (H1 + T1 + Q1 + MCQ), June 2026.*  
*See `DESIGNER_STARTUP_AND_FORM_CANVAS.md` for canvas basics; `DESIGNER_MENU_SPEC.md` for shell.*

---

## Common canvas conventions

| Element | Meaning |
|---------|---------|
| **H1**, **T1**, **Q1**, **Q2** | Auto-assigned **design labels** (orange bar when selected) |
| **Custom left label** | User can rename (e.g. Q1 row still shows **Surveyee** logic via alternate label; MCQ bar renamed **Angel Question**) |
| **Fields panel** | Lists **data field names** (alternate labels / MCQ names), not always the design label |
| **Black highlight** | Inline text selected for editing |
| **Format toolbar** | Active when rich text in an item has focus |

---

## Text item (T1)

### On canvas

| UI | Content |
|----|---------|
| Left label | **T1** |
| Body | Rich text; default placeholder: `[Replace this with text of your own.]` |
| Example after edit | `This is where directions will go.` |

### Behavior

- Added from **Items → Text** or **Insert** (form selected).
- Edited **inline** on canvas; use standard **Format** menu / format toolbar (bold, italic, alignment, etc.).
- **No** on-canvas property strip like FIB/MCQ in these screenshots.
- **Fields / variables:** Designer can **drag** field or variable references from the **Fields** panel into the text body (inserts `<<…>>` tokens). Text is not listed as its own field in Fields, but it can **reference** fields/variables inline.
- Does **not** appear as a named field under **Fields** (unlike FIB blanks / MCQ).

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

## File Uploader (deferred — not on 2011 Designer build)

**Status:** Not documentable from live Designer UI on the Jan 2011 build. Confirmed in production projects (**SportsDashboards** divisions) and fully present in repo source.

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
| Text inline rich edit | Yes | Basic `contentEditable` |
| FIB underscore → blanks | Yes | Prompt string only |
| FIB alternate label | Yes | `blank.name` / `alternateLabel` partial |
| FIB height | Yes | Not exposed |
| FIB validation types | Yes | Not exposed |
| MCQ inline choice entry | Yes | JSON choices array |
| MCQ multi-select / required | Yes | Partial in JSON |
| Dynamic MCQ + Configure Function | Yes | `dynamic` choices in JSON only |
| Rename item label on canvas | Yes | Uses `item.label` only |

---

## Resolved / open (owner notes, June 2026)

| # | Topic | Answer |
|---|--------|--------|
| 1 | Text + Fields | Drag fields/variables into text; inserts references. No Preview to verify runtime. |
| 2 | Multi-blank FIB | Each blank: default `Q1:a`, `Q1:b`, …; alternate labels unique. |
| 3 | MCQ bar rename | Believed to change default **label** attribute (XML TBD). |
| 4 | Validation Edit | Popup per type: custom error message; Integer adds min/max. Image pending. |
| 5 | Height | Tall thin box possible; Preview unavailable. |

---

*Last updated: June 2026.*
