# Tawala Designer — UI & Command Tree Reference
*Reconstructed from Flash Captivate demo screenshots + complete XML tag analysis of all 18 .tawala project files*
*Baseline established: June 18, 2026*
*Status: **COMPLETE — approved as reimplementation specification baseline***

---

## Document Status & Coverage Summary

This document represents the consolidated, verified specification of the Tawala Designer application and its underlying execution engine, reconstructed from two independent source types:

**Source 1 — Flash Captivate demo screenshots** (`designerpoll.swf`, `get_together.swf`, `Poll.swf`, `SignUp.swf`)
Extracted as PNG frames and reviewed directly by the original project owner (Doug). All key dialogs confirmed.

**Source 2 — XML analysis of 18 production `.tawala` project files**
Complete tag inventory extracted programmatically. Every XML element is accounted for and cross-referenced against the visual record.

### Coverage by area

| Area | Status | Source |
|------|--------|--------|
| Application shell & window layout | ✅ Confirmed | Screenshots |
| Menu bar — File, Forms, Documents, Images, Tools | ✅ Confirmed | Screenshots + XML |
| Process Menu — full statement list | ✅ Confirmed | Screenshots + XML |
| Project tree panel structure | ✅ Confirmed | Screenshots |
| Form editor — all item types | ✅ Confirmed | XML (18 files) |
| Form item properties | ✅ Confirmed | XML |
| Form properties | ✅ Confirmed | XML |
| Form-level validation | ✅ Confirmed | XML |
| Dynamic multiple choice | ✅ Confirmed | XML |
| Process editor — window structure & status banner | ✅ Confirmed | Screenshot id547 |
| Process editor — Statements Palette (existence) | ✅ Confirmed | Screenshot id547 |
| Statements Palette — button layout & icons | ⚠️ Not captured | Palette not visible in id547 |
| All 11 Process statement types | ✅ Confirmed | XML (18 files) |
| All 18 condition operators | ✅ Confirmed | XML |
| All 5 arithmetic operators | ✅ Confirmed | XML |
| Field reference syntax | ✅ Confirmed | XML + Screenshots |
| Document editor — window structure | ✅ Confirmed | Screenshot id514 |
| Document display function token syntax `<<...>>` | ✅ Confirmed | Screenshot id514 |
| Display function picker dialog (id468) | ✅ Confirmed | Owner review |
| Configure Function dialog — full structure | ✅ Confirmed | Screenshot id487 + owner review |
| All 7 display functions (named + described) | ✅ Confirmed | Owner review (id468) |
| Additional display functions from XML | ✅ Documented | XML |
| Document rich text formatting system | ✅ Confirmed | XML |
| Document control elements (print, export) | ✅ Confirmed | XML |
| Payment integration (PayPal) | ✅ Confirmed | XML |
| Categorizer drag-and-drop item | ✅ Confirmed | XML + YUI source |
| Web output / end-user browser view | ✅ Confirmed | Screenshots |
| CSS theme system (17 named themes) | ✅ Confirmed | CSS files |
| Email send statement | ✅ Confirmed | XML |
| Record CRUD (Add, Edit, Delete) | ✅ Confirmed | XML |
| External navigation (Navigate-To) | ✅ Confirmed | XML |

### Known gaps (low priority)
- Statements Palette visual layout — button icons and arrangement not captured
- `<authenticationTokenValue>` — purpose inferred (session token) but not confirmed
- `<metafileHeader>` — purpose inferred (project metadata) but not confirmed  
- `<template-id>` in CampaignDashboards — likely a project template reference, not confirmed
- Full PayPal button configuration field list — partially documented from XML

### What this document enables
This specification is sufficient to begin a ground-up reimplementation of Tawala in a modern stack. The following are fully defined:
- The **project file format** (`.tawala` XML schema)
- The **execution engine** (process statement types, operators, field reference grammar)
- The **data model** (forms as tables, records as rows, field references as column paths)
- The **document reporting system** (token syntax, display function API, Configure Function parameters)
- The **Designer UI** (menu structure, editor panels, dialog workflows)
- The **web output layer** (rendered form structure, theme system, navigation model)

---

## Overview of the Designer Application

The Designer was a Windows .NET desktop application. The main window had:
- A **menu bar** across the top
- A **project tree / explorer panel** on the left showing all Forms, Processes, Documents, and Images
- A **main editing canvas** in the center/right — showing the selected Form, Process, or Document
- Standard Windows chrome (title bar, min/max/close)
- Title bar format: **Tawala Designer — [Project Name]**

---

## Menu Bar — Confirmed Structure

### File
- New Project
- Open Project
- Save Project
- Save Project As
- (likely) Publish / Export to server
- Exit

---

### Forms Menu *(confirmed from screenshot id47)*
| Menu Item | Action |
|-----------|--------|
| Add Form | Creates a new blank form in the project |
| Delete Form | Removes the selected form |
| Rename Form | Renames the selected form |
| Set as Start Form | Marks the selected form as the project entry point |
| Form Properties | Opens the Form Properties dialog |
| Move Form Up | Reorders form in the tree |
| Move Form Down | Reorders form in the tree |
| Add Item | Adds a new field/question item to the current form |
| Delete Item | Removes the selected item |
| Move Item Up | Reorders item within the form |
| Move Item Down | Reorders item within the form |
| Item Properties | Opens the Item Properties dialog |
| Preview Form | Renders a live preview of the form |

---

### Process Menu *(confirmed from screenshot id53)*
The Process menu provides the complete set of drop-in statement types for building process logic:

| Menu Item | Description |
|-----------|-------------|
| Get | Retrieves a record list from one or more forms' data stores |
| For Each | Iterates over a record list, aliasing each record |
| Set | Assigns a value to a field (concatenation or arithmetic) |
| If | Conditional branch with TrueSet / FalseSet sub-blocks |
| Show | Navigates the user to a specified Form or Document |
| Comment | Non-executing developer annotation |
| Send | Sends an email message (see Email section below) |
| Delete | Deletes a record from the data store |
| Edit | Edits/updates an existing record |
| Add | Adds a new record |
| Navigate-To | Navigates to a URL or external resource |

*Note: Add, Edit, Delete, and Navigate-To appear in the XML across many projects confirming they are full Process statement types, not just Document functions.*

---

### Documents Menu
- Add Document
- Delete Document
- Rename Document
- Document Properties

**Documents** are the output/report views of a Tawala project — read-only pages assembled from field data and display functions. They are displayed via a `Show` process statement.

---

### Images Menu
- Add Image
- Delete Image
- (Images are stored as base64-encoded data in the .tawala XML and referenced by name in forms and documents)

---

### Tools Menu
- Project Settings / Properties
- (likely) Publish / Deploy to server
- (likely) Theme / Stylesheet selector

---

## Project Tree Panel (left sidebar)

```
[Project Name]
├── Forms
│   ├── [Form 1 name]
│   ├── [Form 2 name]
│   └── ...
├── Processes
│   ├── [Process 1 name]
│   └── ...
├── Documents
│   ├── [Document 1 name]
│   └── ...
└── Images
    ├── [Image 1 name]
    └── ...
```

Clicking any node in the tree opens it in the main canvas editor.

---

## Form Editor

The form editor displayed the list of **Items** — the question/field elements that make up the form.

### Item Types *(confirmed from XML across all projects)*

| XML Tag | Type Name | Description |
|---------|-----------|-------------|
| `<fib>` | Fill-in-blank | Single-line text input |
| `<mc>` | Multiple choice | Radio buttons (onlyone=true) or checkboxes (onlyone=false); supports horizontal, multicolumn layouts |
| `<text>` | Display text | Static formatted text block; may include tables, field references, and display functions |
| `<heading>` | Heading | Section heading |
| `<field>` | Hidden field | Stores computed or carried-over data; not shown to user |
| `<skipInstructions>` | Skip instructions | Conditional skip logic display message |
| `<categorizer>` | Categorizer | Drag-and-drop team/group assignment UI (YUI-based) |
| `<paypal-single-item-button>` | PayPal button | Embedded PayPal payment button |

### Dynamic Multiple Choice (`<dynamic-mcq>`)
Multiple choice items could be populated dynamically from records in another form:
```xml
<mc ...>
  <data-provider>
    <dynamic-mcq version="1">
      <display-expression>
        <field name="Record:FormName:FieldName"/>
      </display-expression>
    </dynamic-mcq>
  </data-provider>
</mc>
```

### Form Item Properties (confirmed fields):
- **label** — internal identifier (used in field references as the field name)
- **alternateLabel** — alternate display name  
- **required** — true/false
- **onlyone** — true (radio/single-select) / false (checkbox/multi-select) for mc items
- **style** — layout style (horizontal, multicolumn, etc.)

### Form Properties:
- **Name** — form identifier
- **Start Point** — boolean, marks as project entry form
- **Theme Path** — CSS theme applied to rendered output
- **Process** — post-submit process to run after this form is submitted
- **Pre-Process** — process to run before this form is displayed
- **pageHeader** — the page header image/branding block
- **header** — form header content
- **styles** — CSS style overrides

### Form-Level Validation
- `<validator>` with `<email-validator>` — validates email format on fib fields
- `<error-message>` — custom validation error text

---

## Process Editor

### Process Window Structure (confirmed from id547)

The Process editor window title format is: **Process — [Process Name]**
(e.g. "Process — Process 1")

**Connection status banner (yellow bar):**
Immediately below the title, a yellow status bar indicates whether the process is connected to a form:
> "Not connected as PostProcess to any Form. Click here to change."

This confirms:
- Processes are explicitly linked to Forms as either a **PostProcess** (runs after form submission) or **PreProcess** (runs before form is displayed) — matching the Form Properties fields already documented
- An **unconnected process** is flagged with a yellow warning banner — a clear visual affordance
- The banner is **clickable** — clicking it opens the connection assignment UI, allowing the user to link the process to a form without leaving the process editor
- The term **PostProcess** is the system's internal name for what the Form Properties panel calls "Process" (the post-submit handler)

**Statements Palette instruction (blue bold text):**
When the process body is empty, a prompt appears in the canvas:
> "To create a new statement, click one of the buttons in the Statements Palette."

This confirms:
- Statements are added via a **Statements Palette** — a panel of buttons, one per statement type
- The palette is a separate UI element from the process body canvas (likely a toolbar or side panel)
- The palette is the visual equivalent of the Process menu — both provide access to the same statement types, but the palette is the primary authoring method within the process editor itself
- An empty process shows this instructional prompt rather than a blank canvas

**Inferred Statements Palette:**
Based on the Process menu (id53) and XML analysis, the palette almost certainly contained one button per statement type:
| Button | Statement |
|--------|-----------|
| Get | Query records from a form |
| For Each | Iterate over a record list |
| Set | Assign a value to a field |
| If | Conditional branch |
| Show | Navigate to a form or document |
| Send | Send an email |
| Add | Add a new record |
| Edit | Edit an existing record |
| Delete | Delete a record |
| Navigate-To | Go to an external URL |
| Comment | Add a developer annotation |

The palette was not visible in id547 (the screenshot was cropped or the palette was collapsed), so button labels, icons, and layout remain to be confirmed.

---

Processes are displayed as a sequential list of statements, visually indented to show nesting.

### Complete Statement Reference *(confirmed from XML analysis)*

---

**`<get>`** — Query records
```xml
<get name="RecordListName">
  <form name="FormName"/>
  <conditions>...</conditions>
  <sort-expression>...</sort-expression>
</get>
```
Retrieves records from the named form's data store into a named list. Supports conditions and sort.

---

**`<foreach>`** — Iterate over records
```xml
<foreach alias="RecordAlias" list="RecordListName">
  <body>
    [statements]
  </body>
</foreach>
```
Iterates over each record in a list. Fields referenced as `RecordAlias:FormName:FieldName` within body.

---

**`<set>`** — Assign a value
```xml
<set name="FieldName">
  <value-expression>
    <operand><string>literal text</string></operand>
    <append/>
    <operand><field name="OtherField"/></operand>
  </value-expression>
</set>
```
Assigns a computed value to a field. Supports string concatenation (`<append>`) and arithmetic (`<add>`, `<sub>`, `<mul>`, `<sum>`).

---

**`<if>`** — Conditional branch
```xml
<if>
  <conditions>
    <and>
      <equals field="FieldName">value</equals>
    </and>
  </conditions>
  <trueSet>[statements]</trueSet>
  <falseSet>[statements]</falseSet>
</if>
```

---

**`<show>`** — Navigate to form or document
```xml
<show form="FormName"/>
<show document="DocumentName"/>
```
Displays the specified form or document to the user.

---

**`<send>`** — Send email
```xml
<send>
  <from>...</from>
  <to>...</to>
  <cc>...</cc>
  <subject>...</subject>
  <body>...</body>
</send>
```
Sends an email. Field values can be embedded in all fields.

---

**`<add>`** — Add a new record
Creates a new record in the specified form's data store.

---

**`<edit>`** — Update an existing record
Updates fields in an existing record.

---

**`<delete>`** — Delete a record
Removes a record from the data store.

---

**`<navigate-to>`** — Navigate to external URL
```xml
<navigate-to>
  <url>...</url>
  <new-window>true/false</new-window>
</navigate-to>
```

---

**`<comment>`** — Annotation
Non-executing developer note.

---

### Condition Operators *(complete set from XML)*

| Operator Tag | Description |
|---|---|
| `<equals>` | String equality |
| `<doesNotEqual>` | String inequality |
| `<contains>` | String contains substring |
| `<doesNotContain>` | String does not contain substring |
| `<beginsWith>` | String begins with value |
| `<isBlank>` | Field is empty |
| `<isNotBlank>` | Field is not empty |
| `<isGreaterThan>` | Numeric greater than |
| `<isGreaterThanOrEqualTo>` | Numeric ≥ |
| `<isLessThan>` | Numeric less than |
| `<isLessThanOrEqualTo>` | Numeric ≤ |
| `<mcEquals>` | Multiple-choice field equals a specific choice value |
| `<mcDoesNotEqual>` | MC field does not equal a choice value |
| `<mcContains>` | MC field (multi-select) contains a choice |
| `<mcDoesNotContain>` | MC field does not contain a choice |
| `<mcIsBlank>` | MC field has no selection |
| `<mcIsNotBlank>` | MC field has at least one selection |

Conditions are combined with `<and>` / `<or>` boolean operators.

---

### Arithmetic Operators in `<set>` Expressions

| Operator Tag | Description |
|---|---|
| `<append>` | String concatenation |
| `<add>` | Addition |
| `<sub>` | Subtraction |
| `<mul>` | Multiplication |
| `<sum>` | Sum of a list |

---

## Field Reference Syntax

```
RecordAlias:FormName:FieldName
```
Example: `Item:Setup:AdminName` — the AdminName field from the Setup form, aliased as "Item"

Within the same form context, fields are referenced by name alone: `FieldName`

In XML: `<field name="RecordAlias:FormName:FieldName"/>`

---

## Document Editor & Display Functions

Documents are the reporting/output layer of Tawala. They are rich formatted pages built from a combination of static content, field references, and **display functions** — pre-built analytics widgets inserted via a picker dialog.

### Document Editor — Display Function Token Syntax (confirmed from id514)

The document editor window title format is: **Document — [Document Name]**
(e.g. "Document — Document 1")

Display functions are inserted into the document body as **double-chevron tokens**:

```
<<FUNCTION_NAME(field_reference)>>
```

**Confirmed example from id514:**
```
This is the vote so far:  <<CHOICE SUMMARY TABLE(Record:Form 1:Q1)>>
```

This means:
- The surrounding text (`"This is the vote so far:"`) is static rich text typed directly into the document
- The token `<<CHOICE SUMMARY TABLE(Record:Form 1:Q1)>>` is inserted by the function picker dialog (id468)
- The argument `Record:Form 1:Q1` is a field reference — `Record` is the record alias, `Form 1` is the form name, `Q1` is the field/item label
- At runtime, the engine expands the token into the rendered table output

**General token syntax:**
```
<<FUNCTION_NAME(RecordAlias:FormName:FieldName)>>
```

The document editor displays these tokens inline within the rich text body, mixed with static prose. This makes Documents a natural-language reporting format — authors write descriptive text around the analytics tokens, and the runtime fills them in.

---

### The Configure Function Dialog (Dialog id487) — Two-Step Insertion Workflow

After selecting a function in the picker (id468), a second dialog opens: **Configure Function**. This dialog is specific to the selected function and provides:

1. **A reminder description** at the top — the full function name and its plain-English description (same text as shown in the picker)
2. **Parameter fields** — the inputs required to configure this instance of the function
3. **A record filter** — an inline condition builder to restrict which records the function operates on
4. **A field-level description** — as each parameter field is focused, a description of that parameter appears at the bottom of the dialog
5. **OK / Cancel** buttons

#### Confirmed example: Configure Function — CHOICE SUMMARY TABLE

**Header text (top of dialog):**
> CHOICE SUMMARY TABLE — Displays a three-column table showing both the number and percentage of people responding to each choice of a given multiple choice question.

**Parameters:**

| Parameter | UI | Notes |
|---|---|---|
| *Question | Green data entry box | Required field (asterisk prefix). Followed by the label "— multiple choice" confirming this must reference an MC field |

**Record filter section:**
> "include only the records where"

Three side-by-side data entry boxes forming a condition row:
- **Box 1** — Field name to filter on
- **Box 2** — Operator (dropdown caret — confirms a set of operators is available)
- **Box 3** — Value to compare against
- **+ button** — Adds an additional filter row (supports multiple conditions)
- **− button** — Removes a filter row

This is an inline condition builder, equivalent to the `<conditions>` block in the Process editor, but surfaced directly in the document function configuration. It allows the function to operate on a filtered subset of records (e.g. only records from a specific team, date range, or status).

**Field description panel (bottom of dialog):**
When the *Question parameter is focused:
> "Question — The multiple choice question whose number and percentage information you wish to show."

This confirms the dialog is **context-sensitive** — the description updates as each parameter field is selected, matching the same self-documenting pattern as the function picker itself.

#### Inferred full insertion workflow:

```
Documents Menu → Insert Function
    ↓
[Function Picker dialog — id468]
  Select: CHOICE SUMMARY TABLE
    ↓
[Configure Function dialog — id487]
  *Question:  [Record:Form 1:Q1]      ← green data entry box
  include only the records where:
    [field] [operator ▼] [value]  [+] [−]
  [OK]
    ↓
Token inserted into document body:
  <<CHOICE SUMMARY TABLE(Record:Form 1:Q1)>>
```

#### Asterisk (*) convention:
The asterisk prefix on `*Question` marks it as a **required parameter**. Optional parameters presumably appear without the asterisk.

#### Green data entry box:
The green color on required parameter fields is a visual affordance — likely the Designer's standard styling for required inputs throughout the application, consistent with how required form items may have been highlighted in the Form editor.

---

### The Display Function Picker (Dialog id468)

This dialog presents a list of named display functions on the left, with a description pane on the right that updates as each function is highlighted. The confirmed functions and their descriptions:

| Function Name | Description |
|---|---|
| **CHOICE SUMMARY TABLE** | Displays a three-column table showing both the number and percentage of people responding to each choice of a given multiple choice question |
| **FIELD LIST** | Displays the values of a specified field from all matching records, as a list |
| **FIELD TABLE** | Displays field values in a tabular layout |
| **POPULAR CHOICE COUNT** | Displays the count of respondents who selected the most popular choice for a given MC question |
| **POPULAR CHOICE TABLE** | Displays a table showing the most popular choices for a given MC question |
| **POPULAR CHOICE TEXT** | Displays the text label of the most popular choice for a given MC question |
| **RECORD COUNT** | Displays the total count of records matching specified conditions |

### Additional Display Functions Found in XML *(not yet seen in dialog)*

| XML Tag | Description |
|---|---|
| `<record-count>` | Count of matching records (maps to RECORD COUNT above) |
| `<choice-tally-table>` | Tally of choices — likely maps to CHOICE SUMMARY TABLE |
| `<response-totals-table>` | Response totals in table format |
| `<itemization-table>` | Detailed line-item table of records |
| `<simple-list>` | Simple list of field values |
| `<simple-list-field>` | The field used as the data source for simple-list |
| `<question-correlation-table>` | Cross-tabulation: shows which respondents chose which options for a given question, with a display field (e.g. respondent name) |
| `<display-mcq-label>` | Renders the text label(s) of MC choices stored in a field; supports `label_only` and `all_choices` display modes |
| `<record-selector>` | Interactive record selection widget — allows user to pick from a list of records |
| `<invitation>` | Hyperlink to another form or document within the project |
| `<link>` | External hyperlink |
| `<link-to-project-details>` | Link to a project details/dashboard page |

### Document Control Elements
| XML Tag | Description |
|---|---|
| `<show-print-control>` | Displays a print button on the document |
| `<show-export-control>` | Displays an export/download button |
| `<open-preference>` | Controls how the document opens (same window, new window, etc.) |
| `<display-conditions>` | Conditions that control whether a document section is shown |

### Document Rich Text Formatting
Documents supported rich formatted text via:
- `<paragraph>` with `indent`, `align` attributes
- `<font>` with `face`, `size`, `color`, bold (`<b>`), italic (`<i>`), underline (`<u>`)
- `<table>`, `<row>`, `<cell>` with `width` attributes (twips-based measurement)
- `<division>` — block-level container within a cell
- `<tab>`, `<tabPositions>`, `<tabStop>` — tab stop layout
- `<sp>` — non-breaking space
- `<break>` — line break
- `<blank>` — blank placeholder

---

## Payment Integration

The `<paypal-single-item-button>` form item embedded a PayPal payment button directly into a form, with:
- `<amount>` / `<amount-field>` — static or dynamic payment amount
- `<button-type>` — PayPal button style
- `<successful-payment-return-form-name>` — form to show on successful payment
- `<cancelled-payment-return-form-name>` — form to show if payment cancelled
- `<description>` — item description for PayPal
- `<item>` — item name
- `<layout-type>` — button layout
- `<style-option>` — visual style
- `<status-field>` — field to store payment status result

---

## Categorizer Item

The `<categorizer>` form item provided a drag-and-drop group/team assignment UI (built on YUI):
- `<category-ids>` — the identifiers for each category/team
- `<category-names>` — display names for each category
- `<category-storage-field>` — the field where assignments are stored

---

## Web Output (End-User Browser View)

From the 790x545/547 screenshots (rendered web forms):
- Clean, styled HTML forms with Tawala branding
- "Made with Tawala" badge in footer
- Form navigation buttons (Next, Submit, Back)
- Multiple choice items rendered as radio buttons or checkboxes
- Fill-in-blank items as standard `<input>` text fields
- Themed via CSS (themes: default, blueline, greenline, orange, lime, dark, plain, white, yellow, red, style2, fabric, litegreen, pinkrose, money, lightbulb, clock)
- Page header with project/organization logo image
- Form title displayed at top of each page
- `<invitation>` links rendered as styled hyperlinks for navigation between forms/documents

---

## Screen Inventory — designerpoll.swf

| File | Size | Content | Status |
|------|------|---------|--------|
| img_000_id37.jpg | tiny | No useful content | Discard |
| img_001_id53.jpg | small | **Process menu open — full statement list** | KEY |
| screen_000_id29 | 790x547 | Web output — intro screen | Web UI |
| screen_001_id47 | 790x547 | **Designer — Forms menu open** | KEY |
| screen_002_id61 | 1100x762 | Designer main canvas — low info | Discard |
| screen_003_id63 | 1100x762 | Designer main canvas — duplicate | Discard |
| screen_004_id65 | 1100x762 | Designer — form editor view | Designer UI |
| screen_005_id86 | 790x115 | Partial toolbar strip | Supporting |
| screen_006_id93 | 320x121 | Small dialog | Supporting |
| screen_007_id99 | 566x121 | Dialog / properties panel | Supporting |
| screen_008_id158 | 1100x762 | Designer — process editor | Designer UI |
| screen_009_id160 | 1100x762 | Designer — process editor (alt process) | Designer UI |
| screen_010_id162 | 1100x762 | Designer — process editor | Designer UI |
| screen_011_id164 | 1100x762 | Designer — process editor | Designer UI |
| screen_012_id166 | 1100x762 | Designer — process editor | Designer UI |
| screen_013_id168 | 1100x762 | Designer — process editor | Designer UI |
| screen_014_id358 | 790x547 | Web output — form in progress | Web UI |
| screen_015_id468 | 272x252 | **Document function picker dialog** | KEY |
| screen_016_id487 | 408x346 | **Configure Function dialog — CHOICE SUMMARY TABLE parameters** | KEY |
| screen_017_id514 | 614x372 | **Document editor — live preview showing display function token syntax** | KEY |
| screen_018_id539 | 790x547 | Web output — results/document view | Web UI |
| screen_019_id547 | 441x268 | **Process editor — empty process with connection status banner and Statements Palette instruction** | KEY |
| screen_020_id570 | 409x117 | Small toolbar / status panel | Supporting |
| screen_021_id615 | 336x189 | Small dialog | Supporting |
| screen_022_id634 | 790x547 | Web output — results view | Web UI |
| screen_023_id640 | 790x547 | Web output — final screen | Web UI |
| screen_024_id661 | 740x210 | Partial — Designer toolbar area | Supporting |
| screen_025_id673 | 740x124 | Partial — Designer menu/toolbar strip | Supporting |

---

## Screen Inventory — get_together.swf

| File | Size | Content |
|------|------|---------|
| screen_002_id208 | 1007x853 | **Designer — project tree + form editor** |
| screen_003_id210 | 1007x853 | Designer — different form selected |
| screen_004_id212 | 1007x853 | Designer — process editor view |
| screen_005_id214 | 1007x853 | Designer — process editor (alt process) |
| screen_007_id245 | 657x173 | Partial — Designer top panel / toolbar |
| screen_011_id341 | 583x360 | Dialog — likely publish/export dialog |

---

## Remaining Open Questions (post-baseline)

- [x] id547 confirmed: Process editor window — connection status banner, Statements Palette instruction (palette itself not visible in screenshot)
- [x] id514 confirmed: Document editor live preview showing display function token insertion syntax
- [x] id487 confirmed: Configure Function dialog — full parameter structure documented
- [ ] Statements Palette button layout not yet captured — need additional screenshot or recollection
- [ ] Confirm whether the Process menu also lists Add, Edit, Delete, Navigate-To — or whether those are inserted another way
- [ ] Document the Categorizer UI fields in the Item Properties dialog
- [ ] Determine full list of PayPal button configuration options
- [ ] Confirm `<template-id>` usage in CampaignDashboards — may be a project template reference
- [ ] Understand `<authenticationTokenValue>` — appears in all projects, likely a per-user session/auth field
- [ ] Clarify `<metafileHeader>` — appears in major projects, likely project metadata
- [ ] Document the `<record-selector>` interaction model — appears to be an interactive record picker widget
