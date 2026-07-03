# Tawala XML → JSON Format Mapping
*Canonical translation reference between the legacy `.tawala` XML format and the JSON project definition schema (format 2.0)*
*Cross-referenced against: Designer source (C# NewCode), Java runtime (build-1700), DirtBowl definition v2*

---

## Top-Level Project Structure

### XML
```xml
<project name="DirtBowl" themePath="dirtbowl2" format="1.11" designerBuild="...">
  <pageHeader .../>
  <styles fibItemStyle="topLabels" mcItemStyle="vertical"/>
  <form .../>
  <process .../>
  <document .../>
  <imagedef .../>
</project>
```

### JSON
```json
{
  "name": "DirtBowl",
  "format": "2.0",
  "themePath": "dirtbowl2",
  "forms": [ ... ],
  "processes": [ ... ],
  "documents": [ ... ]
}
```

**Mapping notes:**
- `format` — XML had separate `format` (Designer authoring version, e.g. `"1.11"`) and `designerBuild` (integer build number). JSON uses a single `format` field representing the JSON schema version. Set to `"2.0"` for all new projects.
- `_originalFormat` — preserved as a non-functional annotation on projects converted from XML (e.g. `"_originalFormat": "1.10"`). Ignored by the runtime.
- `<pageHeader>` — page-level styling metadata. Not represented in JSON (absorbed into `themePath`).
- `<styles>` — global default item styles (`fibItemStyle`, `mcItemStyle`, `textItemStyle`). Not currently in JSON schema; treat as implicit defaults if needed.
- `<imagedef>` elements — image asset library. Not yet represented in JSON schema.

---

## Form Structure

### XML
```xml
<form name="Registration" startPoint="true"
      process="Post-Registration" preProcess="RetrieveAdminSetupVariables"
      dataEntryOnly="false" dataSourceName="SharedPlayers"
      themePath="dirtbowl2">
  <items>
    ...
  </items>
</form>
```

### JSON
```json
{
  "name": "Registration",
  "startPoint": true,
  "process": "Post-Registration",
  "preProcess": "RetrieveAdminSetupVariables",
  "themePath": "dirtbowl2",
  "items": [ ... ]
}
```

**Mapping notes:**
- `startPoint` — `"true"`/`"false"` string in XML → boolean in JSON. Always present in JSON.
- `process` / `preProcess` — omitted in both XML and JSON when not connected.
- `dataEntryOnly` — omitted in JSON when false (the default).
- `dataSourceName` — omitted in JSON when empty. When present, marks the form as a shared data source.
- `themePath` — can appear at both project and form level. Form-level overrides project-level.
- `<items>` wrapper — flattened; items are a direct array in JSON.

---

## Form Items

### 1. FIB — Fill-in-Blank Question

#### XML
```xml
<fib label="Q1" alternateLabel="PlayerName" style="topLabels">
  <paragraph indent="0" align="left">
    First: <blank label="a" length="20" required="false" alternateLabel="FirstName"/>
    Last:  <blank label="b" length="20" required="true"  alternateLabel="LastName"/>
  </paragraph>
  <displayConditions>
    <equals field="Registration:Division" value="8U"/>
  </displayConditions>
</fib>
```

#### JSON
```json
{
  "type": "fib",
  "label": "Q1",
  "style": "leftAlignLabels",
  "prompt": "First: / Last:",
  "blanks": [
    { "name": "FirstName", "length": 20 },
    { "name": "LastName",  "length": 20, "required": true }
  ],
  "displayCondition": {
    "field": "Registration:Division",
    "op": "equals",
    "value": "8U"
  }
}
```

**Mapping notes:**
- `label` — positional address (Q1, Q2, ...). Same in both.
- `alternateLabel` (XML, item-level) — semantic name for the whole FIB item. Not used in JSON (the item is addressed by `label` or by blank names directly).
- `style` — XML values: `"topLabels"`, `"alignedLabels"`, `"freeform"`, `"justified"`. JSON values include `"leftAlignLabels"`, `"leftAlignLabelsJustified"`, `"topLabels"`. These are renderer hints — map as-is when converting.
- `prompt` — JSON-only. A flat string containing the label text interleaved with `/` separators between blanks. In XML, label text lives in the `<paragraph>` text nodes around `<blank>` elements. The `/` separator pattern was developed for the JSON runtime; when converting from XML, extract the inter-blank text and join with `/`.
- Blank `label` (a, b, c...) — positional letter. In XML: `label="a"`. In JSON: implied by array index, `name` is the semantic name.
- Blank `alternateLabel` (XML) → blank `name` (JSON). The semantic field name used in process references.
- `required` — per-blank in XML. Per-blank in JSON too. Default `false`; omit when false.
- `length` — blank input width in characters. Same in both.
- `height` — multi-line blank (textarea). Present in XML; omit in JSON when 0/1.
- `displayCondition` — see Conditions section below. XML tag is `<displayConditions>` (plural); JSON key is `displayCondition` (singular).

---

### 2. MC — Multiple Choice Question

#### XML
```xml
<mc label="Q2" alternateLabel="SexMCQ" onlyone="true" required="false"
    style="vertical" columnCount="0">
  <contents>
    <question>
      <paragraph indent="0" align="left">Sex:</paragraph>
    </question>
    <choice label="a">Boy</choice>
    <choice label="b">Girl</choice>
  </contents>
  <displayConditions>...</displayConditions>
</mc>
```

#### JSON
```json
{
  "type": "mc",
  "label": "Q2",
  "name": "SexMCQ",
  "question": "Sex:",
  "onlyone": true,
  "required": false,
  "displayAs": "radio",
  "choices": [
    { "label": "a", "text": "Boy" },
    { "label": "b", "text": "Girl" }
  ],
  "displayCondition": { ... }
}
```

**Mapping notes:**
- `alternateLabel` (XML) → `name` (JSON). The semantic field name.
- `onlyone` — `"true"`/`"false"` string in XML → boolean in JSON. `true` = radio buttons, `false` = checkboxes.
- `displayAs` — JSON-only derived field. Set to `"radio"` when `onlyone=true`, `"checkbox"` when false, `"dropdown"` when `style="dropdown"`.
- `style` (XML: `"vertical"`, `"horizontal"`, `"dropdown"`) — partially absorbed into `displayAs`. `"vertical"` and `"horizontal"` map to column layout hints.
- `columnCount` (XML) — integer, 0 = auto. Not currently in JSON schema.
- `question` — JSON: flat string extracted from `<question><paragraph>` text. XML allows rich formatting (fonts, field refs) inside the question; JSON collapses to plain text for simple cases.
- `choices` — `label` (a, b, c...) + `text` in both. Choice text in XML is full rich content; JSON collapses to plain string.
- `required` → `requireAtLeastOne` in XML. Boolean in JSON.
- **Dynamic choices** (`<data-provider><dynamic-mcq>`) — choice list populated at runtime from stored records. JSON replaces the `choices` array with a single entry of `"type": "dynamic"`:

```json
{
  "choices": [
    {
      "type": "dynamic",
      "sourceForm": "Divisions",
      "displayExpr": "<<Record:Divisions:DivNames>>",
      "valueExpr":   "<<Record:Divisions:DivisionID>>",
      "sortExpr":    "<<Record:Divisions:DivNames>>",
      "where": { "field": "...", "op": "...", "value": "..." }
    }
  ]
}
```

| Field | XML source | Meaning |
|---|---|---|
| `sourceForm` | `<record-selector><form name="..."/>` | Form whose records populate the list |
| `displayExpr` | `<display-expression>` field/string nodes | Template for what the user sees — uses `<<>>` field refs |
| `valueExpr` | `<value-expression>` field/string nodes | Template for what gets stored |
| `sortExpr` | `<sort-expression>` | Sort key (empty = natural order); `#` separates multi-key: `"<<Last>>#<<First>>"` |
| `where` | `<record-selector><conditions>` | Optional filter using the standard condition schema |

XML note: `<data-provider>` is a **direct child of `<mc>`**, not nested under `<contents>`. The `<question>` element is also a direct child. This differs from the `<contents>` wrapper used for static choices.

---

### 3. Text Block

#### XML
```xml
<text label="T1" style="normal">
  <paragraph indent="0" align="left">Registration closes December 1st.</paragraph>
</text>
```

#### JSON
```json
{
  "type": "text",
  "label": "T1",
  "style": "normal",
  "content": "Registration closes December 1st."
}
```

**Mapping notes:**
- `content` — JSON: flat string. XML: full rich paragraph content (fonts, field refs, tables). When converting complex XML text items, the `content` field captures the plain-text summary; rich content is lost unless a separate rich-content field is added.
- `style` — `"normal"`, `"boxed"`, `"shaded"`, `"instructional"` (instructional is JSON-only, maps to a visual callout style).
- `displayCondition` — same pattern as FIB/MC.

---

### 4. Heading

#### XML
```xml
<heading label="H1" style="main">Player Registration</heading>
<heading label="H2" style="sub">Step 2</heading>
```

#### JSON
```json
{ "type": "heading",    "label": "H1", "content": "Player Registration" }
{ "type": "subheading", "label": "H1", "content": "Step 2" }
```

**Mapping notes:**
- XML `style="main"` → JSON `type: "heading"`.
- XML `style="sub"` → JSON `type: "subheading"`.
- `content` — plain text string. XML may contain inline formatting; JSON is plain text.

---

### 5. Hidden Field

#### XML
```xml
<field name="TotalCost"/>
```

#### JSON
```json
{
  "type": "field",
  "label": "",
  "name": "TotalCost"
}
```

**Mapping notes:**
- `label` — auto-assigned in XML (inherits from position). JSON sets it explicitly (often `""`).
- `name` — the semantic field name. Addressable as `FormName:TotalCost` in processes.

---

### 6. Page Break

#### XML
```xml
<break/>
```

#### JSON
```json
{ "type": "break" }
```

No attributes in either format.

---

### 7. Skip Instructions

#### XML
```xml
<skipInstructions>
  <if>
    <conditions><equals field="Registration:Division" value="8U"/></conditions>
    <trueSet><skip to="Q7"/></trueSet>
    <falseSet/>
  </if>
</skipInstructions>
```

#### JSON
```json
{
  "type": "skipInstructions",
  "label": "skip1",
  "commands": [
    {
      "cmd": "if",
      "condition": { "field": "Registration:Division", "op": "equals", "value": "8U" },
      "then": [ { "cmd": "skip", "to": "Q7" } ]
    }
  ]
}
```

**Mapping notes:**
- `label` — JSON-only. Auto-assigned (skip1, skip2, ...).
- `commands` — uses the same command/condition schema as Process commands (see Process Commands section). Only `if`, `skip`, and `set` appear in practice within skipInstructions.
- XML `<trueSet>` → JSON `then`. XML `<falseSet>` → JSON `else` (omit when empty).
- XML `<conditions>` (plural wrapper) → JSON `condition` (singular object). The condition structure is the same.
- `"__EndOfForm__"` — a special sentinel value for `skip.to` meaning "skip to the end of the form / submit". Used in JSON to terminate a form page early.

---

## Conditions (displayConditions and if/get conditions)

The condition schema is identical whether used in `displayCondition` on a form item, `condition` on an `if` command, or `where`/`conditions` on a `get` command.

### Simple condition

#### XML
```xml
<equals field="Registration:Division" value="8U"/>
<mcContains field="Registration:SexMCQ" value="a"/>
<isBlank field="Registration:Email"/>
```

#### JSON
```json
{ "field": "Registration:Division", "op": "equals",     "value": "8U" }
{ "field": "Registration:SexMCQ",   "op": "mcContains", "value": "a"  }
{ "field": "Registration:Email",    "op": "isBlank"                    }
```

**Operator mapping (XML tag → JSON `op`):**

| XML tag | JSON `op` | Notes |
|---|---|---|
| `equals` | `"equals"` | FIB field vs value or field |
| `doesNotEqual` | `"doesNotEqual"` | |
| `contains` | `"contains"` | |
| `doesNotContain` | `"doesNotContain"` | |
| `beginsWith` | `"beginsWith"` | |
| `endsWith` | `"endsWith"` | |
| `isLessThan` | `"isLessThan"` | Numeric comparison |
| `isLessThanOrEqualTo` | `"isLessThanOrEqualTo"` | |
| `isGreaterThan` | `"isGreaterThan"` | |
| `isGreaterThanOrEqualTo` | `"isGreaterThanOrEqualTo"` | |
| `isBlank` | `"isBlank"` | No `value` field |
| `isNotBlank` | `"isNotBlank"` | No `value` field |
| `mcEquals` | `"mcEquals"` | MC field vs choice label (e.g. `"a"`) |
| `mcDoesNotEqual` | `"mcDoesNotEqual"` | |
| `mcContains` | `"mcContains"` | MC checkbox — any selected choice matches |
| `mcDoesNotContain` | `"mcDoesNotContain"` | |
| `mcIsBlank` | `"mcIsBlank"` | No `value` field |
| `mcIsNotBlank` | `"mcIsNotBlank"` | No `value` field |

**MC `value` field:** For MC operators, `value` is a **choice label** (`"a"`, `"b"`, ...) not the choice text. `mcEquals field="Q2" value="a"` means "choice A was selected".

### Compound condition (and/or)

#### XML
```xml
<conditions>
  <and>
    <equals field="PlayerFirst" value="<<Registree:Registration:FirstName>>"/>
    <equals field="ToAddress"   value="<<Registree:Registration:ParentEmail>>"/>
  </and>
</conditions>
```

#### JSON
```json
{
  "and": [
    { "field": "PlayerFirst", "op": "equals", "value": "<<Registree:Registration:FirstName>>" },
    { "field": "ToAddress",   "op": "equals", "value": "<<Registree:Registration:ParentEmail>>" }
  ]
}
```

**Mapping notes:**
- XML `<and>` and `<or>` are sibling elements inside `<conditions>`. JSON uses `"and": [...]` or `"or": [...]` as the top-level key of the condition object.
- Nesting is recursive: an `or` can contain another `or` or `and`.
- A `LogicalOperator` between simple conditions in XML (e.g. `<equals/><and/><equals/>`) maps to a wrapping `"and"` object in JSON.
- Right-hand `value` in conditions can be a field reference using `<<FormName:FieldName>>` syntax — same as in `set` command values.

---

## Process Structure

### XML
```xml
<process name="Post-Registration">
  <set field="..." .../>
  <get recordList="..." .../>
  <if> ... </if>
</process>
```

### JSON
```json
{
  "name": "Post-Registration",
  "commands": [ ... ]
}
```

---

## Process Commands

### `comment`

```json
{ "cmd": "comment", "text": "-- Retrieve admin variables" }
```

Ignored at runtime. Used for authoring notes.

---

### `set`

#### XML
```xml
<set field="AdminName">
  <set><string field="Admin:AdminSetup:AdminsName"/></set>
</set>
```

#### JSON
```json
{ "cmd": "set", "field": "AdminName", "value": "<<Admin:AdminSetup:AdminsName>>", "concat": false }
```

**Mapping notes:**
- `value` — a string that may contain `<<FormName:FieldName>>` field references and literal text. Mixed example: `"value": "SeasonYr - <<Registration:RegAgeYr>>"`.
- `concat` — boolean. `false` = replace the field value. `true` = append to the existing value (maps to XML `<append>` command in some contexts).
- Field references use `<<recordName:FormName:FieldName>>` for record-scoped fields (inside a `foreach`), or `<<FormName:FieldName>>` for global form fields.
- Arithmetic: XML uses `<add>`, `<sub>`, `<mul>`, `<div>` child elements on `<set>` containing `<operand field/value/>` nodes. Converted to expression strings:
  - `<add><operand field="Count"/><operand value="1"/></add>` → `"value": "<<Count>> + 1"`
  - `<sub><sub><operand field="SeasonYr"/><operand field="RegAgeYr"/></sub><operand value="1"/></sub>` → `"value": "<<SeasonYr>> - <<RegAgeYr>> - 1"` (nested operations flatten left-to-right)
  - Operators: `add`→`+`, `sub`→`-`, `mul`→`*`, `div`→`/`
  - `arithmeticAsText="true"` on `<set>` means string concatenation (not numeric) — ignored in JSON since the expression string serves both purposes.

---

### `get`

#### XML
```xml
<get recordList="SetupRecords">
  <forms>
    <form name="AdminSetup"/>
  </forms>
</get>

<get recordList="TeamAndCoachList">
  <forms>
    <form name="Team"/>
    <form name="Coach"/>
  </forms>
  <conditions>
    <equals field="TeamAndCoachList:Team:CoachId" value="<<TeamAndCoachList:Coach:CoachId>>"/>
  </conditions>
</get>
```

#### JSON
```json
{ "cmd": "get", "recordList": "SetupRecords", "sourceForms": ["AdminSetup"] }

{
  "cmd": "get",
  "recordList": "TeamAndCoachList",
  "sourceForms": ["Team", "Coach"],
  "where": {
    "field": "TeamAndCoachList:Team:CoachId",
    "op": "equals",
    "value": "<<TeamAndCoachList:Coach:CoachId>>"
  }
}
```

**Mapping notes:**
- `recordList` — the named collection created by this get. Referenced by `foreach` and record-scoped field addresses.
- `sourceForms` — list of form names to query. Multi-form gets create a joined result set.
- `where` — optional filter condition. Uses the same condition schema as `if`. XML `<conditions>` → JSON `where`.
- Record-scoped field references: `<<RecordName:FormName:FieldName>>` — `RecordName` is the iterator variable from `foreach`.

---

### `foreach`

#### XML
```xml
<foreach record="Admin" recordList="SetupRecords">
  <set field="AdminName">...</set>
</foreach>
```

#### JSON
```json
{
  "cmd": "foreach",
  "recordName": "Admin",
  "recordList": "SetupRecords",
  "do": [
    { "cmd": "set", "field": "AdminName", "value": "<<Admin:AdminSetup:AdminsName>>", "concat": false }
  ]
}
```

**Mapping notes:**
- `recordName` — the loop variable name (XML: `record` attribute → JSON: `recordName`).
- `recordList` — references a record list created by a preceding `get`.
- `do` — the command body (JSON). XML has child elements directly.

---

### `if`

#### JSON
```json
{
  "cmd": "if",
  "condition": { "field": "Registration:Division", "op": "equals", "value": "8U" },
  "then": [ ... ],
  "else": [ ... ]
}
```

**Mapping notes:**
- XML `<trueSet>` → JSON `then`. XML `<falseSet>` → JSON `else`.
- `else` is omitted when the falseSet is empty.
- `condition` uses the full condition schema (simple, and, or, nested).

---

### `send`

#### XML
```xml
<send>
  <to><field name="Registration:ParentEmail"/></to>
  <from><field name="AdminEmail"/></from>
  <subject><string value="Confirmed: "/><string field="Registration:FirstName"/></subject>
  <body><document name="ConfirmationLetter"/></body>
</send>
```

#### JSON
```json
{
  "cmd": "send",
  "to":      { "fieldRef": "Registration:ParentEmail" },
  "from":    { "fieldRef": "AdminEmail" },
  "subject": "Confirmed: <<Registration:FirstName>>",
  "body":    { "document": "ConfirmationLetter" }
}
```

**Address variants:**

| XML | JSON |
|---|---|
| `<field name="FormName:FieldName"/>` | `{ "fieldRef": "FormName:FieldName" }` |
| `<literal value="admin@example.com"/>` | `{ "literal": "admin@example.com" }` |
| Multiple recipients (list) | `[ { "fieldRef": "..." }, { "fieldRef": "..." } ]` |

**Mapping notes:**
- `subject` — JSON: string with optional `<<FieldRef>>` interpolation. XML: compound expression with `<string value="..."/>` and `<string field="..."/>` children.
- `body.document` — references a document by name. Must exist in the project's `documents` array.
- `cc` / `bcc` — same address object/array structure as `to`.

---

### `show` (navigate to form)

#### XML
```xml
<show form="PlayerDashboard"/>
```

#### JSON
```json
{ "cmd": "show", "form": "PlayerDashboard" }
```

---

### `showDocument` (display document page)

#### XML
```xml
<show type="document" name="Receipt" reset="false"/>
```

#### JSON
```json
{ "cmd": "showDocument", "document": "Receipt", "reset": false }
```

**Mapping notes:**
- `reset` — boolean. When `true`, clears the current session data after displaying.
- XML uses `<show>` with a `type` discriminator; JSON uses a separate `showDocument` command name.

---

### `show` (external URL)

#### XML
```xml
<show><url><string value="https://example.com/"/><string field="Registration:Token"/></url></show>
```

#### JSON
```json
{ "cmd": "show", "url": "https://example.com/<<Registration:Token>>" }
```

---

### `skip`

#### JSON
```json
{ "cmd": "skip", "to": "Q7" }
{ "cmd": "skip", "to": "__EndOfForm__" }
```

`to` is either a form item label (`Q7`, `H2`, `T3`, `skip2`) or the sentinel `"__EndOfForm__"`.

---

### `append`

#### JSON
```json
{ "cmd": "append", "field": "Notes", "value": "<<Registration:Comment>>" }
```

Appends `value` to the existing field value. Equivalent to `set` with `concat: true`.

---

### `delete`

#### JSON
```json
{
  "cmd": "delete",
  "from": "RecruitmentDB",
  "where": {
    "field": "Record:RecruitmentDB:BadRecord?",
    "op": "equals",
    "value": "Delete"
  }
}
```

Deletes all records in `from` matching the `where` condition. `from` is a form name (the record store).

---

### `edit`

#### JSON
```json
{ "cmd": "edit", "form": "AdminSetup", "submit": "modify" }
```

Navigates to a form in edit mode, pre-populated with an existing record. `submit` can be `"modify"` (update) or `"add"` (new record).

---

## Document Structure

### XML
```xml
<document name="ConfirmationLetter">
  <xmlData>
    <paragraph indent="0" align="left">
      Dear <field name="Registration:FirstName"/>,
    </paragraph>
    <table indent="0">
      <row>
        <cell width="3000"><paragraph indent="0" align="left">Division:</paragraph></cell>
        <cell width="3000"><paragraph indent="0" align="left"><field name="Registration:Division"/></paragraph></cell>
      </row>
    </table>
  </xmlData>
</document>
```

### JSON
```json
{
  "name": "ConfirmationLetter",
  "_rawSummary": "Dear [FirstName], ..."
}
```

**Current state:** JSON documents are currently stored with only a `_rawSummary` (plain-text preview). The full rich-text content from `<xmlData>` is not yet represented in JSON. This is the largest gap in the current JSON schema.

**Design for full document content in JSON:**
```json
{
  "name": "ConfirmationLetter",
  "content": [
    {
      "type": "paragraph",
      "align": "left",
      "indent": 0,
      "nodes": [
        { "type": "text", "text": "Dear " },
        { "type": "field", "name": "Registration:FirstName" },
        { "type": "text", "text": "," }
      ]
    },
    {
      "type": "table",
      "indent": 0,
      "rows": [
        {
          "cells": [
            { "width": 3000, "content": [ { "type": "paragraph", "nodes": [ { "type": "text", "text": "Division:" } ] } ] },
            { "width": 3000, "content": [ { "type": "paragraph", "nodes": [ { "type": "field", "name": "Registration:Division" } ] } ] }
          ]
        }
      ]
    }
  ]
}
```

**Document content node types (proposed JSON):**

| XML element | JSON `type` | Key fields |
|---|---|---|
| `<paragraph indent align>` | `"paragraph"` | `indent` (twips), `align` (`"left"`, `"right"`, `"center"`), `nodes: []` |
| `<table indent>` | `"table"` | `indent`, `rows: []` |
| `<row>` | *(implicit in `rows` array)* | `cells: []` |
| `<cell width>` | `"cell"` | `width` (twips), `content: []` (array of paragraphs) |
| `<font face size color>` | `"font"` | `face`, `size` (points, not twips), `color` (CSS hex `#RRGGBB`), `nodes: []` |
| `<b>` | `"bold"` | `nodes: []` |
| `<i>` | `"italic"` | `nodes: []` |
| `<u>` | `"underline"` | `nodes: []` |
| `<field name>` | `"field"` | `name` (qualified: `"FormName:FieldName"`) |
| `<image id width height>` | `"image"` | `id`, `width`, `height` |
| `<invitation form project>` | `"invitation"` | `form`, `project`, `text`, `private`, `authToken` |
| `<link>` | `"hyperlink"` | `url`, `text`, `newWindow` |
| text node | `"text"` | `text` |

**Font size:** Convert twips to points when building JSON (`size_pt = twips / 20`).
**Font color:** Convert bare hex to CSS hex (`"FF0000"` → `"#FF0000"`).

---

## Field Reference Syntax

Field references appear in `set` values, `send` subject strings, and condition values.

| Context | Syntax | Meaning |
|---|---|---|
| Current form field | `<<FormName:FieldName>>` | A blank or MC field on `FormName` |
| Hidden field | `<<FieldName>>` | A `<field>` item (no form prefix if unqualified) |
| Variable | `<<VariableName>>` | A process-local variable |
| Record-scoped | `<<RecordVar:FormName:FieldName>>` | Field from a `foreach` record |
| Blank label (no alternateLabel) | `<<FormName:Q1:a>>` | Fallback positional address |

In the JSON schema, field references are embedded as `<<...>>` in string values, identical to the XML convention. No encoding change is needed.

---

## Schema Version Notes

| XML `format` value | Meaning | Runtime compatibility |
|---|---|---|
| `1.3` | Minimum safe (oldest the runtime accepts) | ✓ |
| `1.4` | Current runtime target | ✓ |
| `1.10` – `1.11` | Designer-ahead versions | ✓ (runtime ignores unknown elements) |
| `2.0` | JSON schema version (new) | JSON runtime only |

---

## Conversion Checklist (XML → JSON)

When converting a `.tawala` file to the JSON format:

- [ ] Parse `<project>` attributes → top-level JSON fields
- [ ] For each `<form>`: map attributes, flatten `<items>` array
- [ ] For each form item: map `type`, `label`, `style`, flatten rich content to `content`/`prompt` strings
- [ ] For FIB: extract blank `alternateLabel` → blank `name`; extract inter-blank text → `prompt` with `/` separators
- [ ] For MC: extract `alternateLabel` → `name`; collapse `<question>` text → `question`; map `onlyone` → `displayAs`
- [ ] Map `<displayConditions>` → `displayCondition` using condition operator table
- [ ] For each `<process>`: map commands recursively
- [ ] Map `<set>` XML compound expressions → `value` strings with `<<>>` interpolation
- [ ] Map `<if>` XML (`<trueSet>/<falseSet>`) → `then`/`else`
- [ ] Map `<get>` XML (`<forms>/<conditions>`) → `sourceForms`/`where`
- [ ] Map `<foreach>` XML (`record` attr + child elements) → `recordName`/`do`
- [ ] Map `<send>` XML address/subject/body sub-elements → flat JSON fields
- [ ] For each `<document>`: preserve `name`; convert `<xmlData>` to `content` node tree
- [ ] Strip `<rawHtmlData>` (legacy, ignored by runtime)
- [ ] Set `"format": "2.0"` on output
- [ ] Preserve `"_originalFormat"` as annotation
