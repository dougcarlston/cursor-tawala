# Tawala Designer — Source Code Analysis
*From C# source: TawalaDesigner-complete.zip → NewCode/TawalaDesigner (593 .cs files)*
*Cross-referenced against Java runtime (build-1700) and .tawala XML format*

---

## Overview

The Designer is a C# .NET Windows Forms application using a strict **MVP (Model-View-Presenter)** architecture. It is organized into distinct authoring modules, each with its own Presenter, View interface, and backing Model objects. The core project model lives entirely in `Tawala.Projects.*` and is shared across all modules.

The Designer's primary job is to author and serialize `.tawala` XML files. Every model object knows how to serialize itself via `ToXml()`. The project is saved by calling `Project.Current.ToXml()` which recursively calls `ToXml()` down the entire object graph.

---

## Application Modules

| Module | Namespace | Purpose |
|---|---|---|
| `FormDesigner` | `Tawala.FormDesigner` | Visual editor for Forms and their items |
| `ProcessDesigner` | `Tawala.ProcessDesigner` | Statement editor for Processes |
| `DocumentDesigner` | `Tawala.DocumentDesigner` | Editor for printable Document templates |
| `ProjectExplorer` | `Tawala.ProjectExplorer` | Tree view of all forms, processes, documents |
| `Browser` | `Tawala.Browser` | Embedded IE preview browser |
| `Processes` | `Tawala.Processes` | Individual process statement views (Set, Get, Send, etc.) |
| `ProjectUI` | `Tawala.ProjectUI` | Fields palette, component palette |
| `ComponentDesigner` | `Tawala.ComponentDesigner` | Reusable component authoring |
| `FunctionControls` | `Tawala.FunctionControls` | Function/formula editor UI |
| `FunctionRuntime` | `Tawala.FunctionRuntime` | Client-side function evaluation |

---

## The Project Model

### `Project` (singleton: `Project.Current`)

The entire authoring state is held in a single static `Project.Current` instance. Key fields:

```csharp
public const string XmlFormatVersion = "1.11";  // Designer format version

// Content lists
FormList formList;          // all forms
ProcessList processList;    // all processes
DocumentList documentList;  // all document templates
ImageDefinitionCollection NewImages;  // image assets
PageHeader pageHeader;      // page-level header settings

// Global style defaults
string GlobalFibItemStyle;  // e.g. "topLabels"
string GlobalMCItemStyle;   // e.g. "vertical"
string GlobalTextItemStyle;

// Global field lookup tables (updated as items are added/removed)
static ProjectFieldMapById FieldMapById;
static ProjectFieldMapByName FieldMapByName;
```

**Serialization** (called on Save or Upload):
```xml
<project name="..." themePath="..." format="1.11" designerBuild="...">
  <pageHeader .../>
  <styles fibItemStyle="..." mcItemStyle="..." textItemStyle="..."/>
  <form .../>   <!-- one per form -->
  <process .../>  <!-- one per process -->
  <document .../>
  <imagedef .../>
</project>
```

**Format version note:** The Designer writes `format="1.11"`. The Java runtime's current accepted version is `1.4`. This gap is reconciled during upload — the server-side `XMLTransceiver` converts/validates on receipt. The Designer version is a superset; the runtime ignores elements it doesn't know.

---

## Form Items — Complete Type Registry

`FormItemFactory` is the single source of truth for all item types the Designer can produce:

```csharp
formItemFactory.Register("heading",          typeof(NewHeadingItem));
formItemFactory.Register("text",             typeof(NewTextItem));
formItemFactory.Register("fib",              typeof(NewFibItem));
formItemFactory.Register("mc",               typeof(NewMcqItem));
formItemFactory.Register("field",            typeof(NewHiddenField));
formItemFactory.Register("break",            typeof(BreakItem));
formItemFactory.Register("skipInstructions", typeof(NewSkipInstructionsItem));
```

### `NewFibItem` — Fill-in-Blank Question (`<fib>`)

The workhorse item. Contains rich-text paragraph content with embedded `<blank>` elements.

**Serialized form:**
```xml
<fib label="Q1" alternateLabel="PlayerName" style="topLabels">
  <paragraph indent="0" align="left">
    What is the player's name?
    <blank label="a" length="20" required="false" alternateLabel="FirstName"/>
    <blank label="b" length="20" required="false" alternateLabel="LastName"/>
  </paragraph>
  <displayConditions>...</displayConditions>
</fib>
```

**Key properties:**
- `label` — auto-assigned sequential ID (Q1, Q2, ...) — the positional address
- `alternateLabel` — semantic name for the whole FIB item (optional)
- `style` — layout hint: `"topLabels"`, `"freeform"`, `"justified"`, `"alignedLabels"`
- Blanks have their own `label` (a, b, c...) and `alternateLabel` (the blank's semantic field name)
- `displayConditions` — conditional visibility (see Conditions section)

**Field addressing:** The blank's `alternateLabel` becomes the field name. Referenced as `FormName:AlternateLabel` in processes (e.g. `Registration:FirstName`). If no alternateLabel, fallback is `FormName:Q1:a`.

**Default template on creation:**
```xml
<paragraph indent="0" align="left">
  [Replace this with your question. Underscores create blanks.]
  <blank label="a" length="20" required="false"/>
</paragraph>
```

### `NewMcqItem` — Multiple Choice Question (`<mc>`)

Supports both radio buttons (single-select) and checkboxes (multi-select).

**Serialized form:**
```xml
<mc label="Q3" alternateLabel="ShirtSize" onlyone="true" required="false"
    style="vertical" columnCount="2">
  <contents>
    <question>
      <paragraph ...>What shirt size?</paragraph>
    </question>
    <choice label="a">Small</choice>
    <choice label="b">Medium</choice>
    <choice label="c">Large</choice>
  </contents>
  <displayConditions>...</displayConditions>
</mc>
```

**Key properties:**
- `onlyone` — `true` = radio buttons (single-select); `false` = checkboxes (multi-select)
- `required` — at least one selection required
- `style` — `"vertical"`, `"horizontal"`, `"dropdown"`
- `columnCount` — multi-column layout (0 = use style default)
- `choiceSourceType` — `0` = static choices; `1` = dynamic (`<data-provider>` driven)

**Default template on creation:**
```
question: [Replace this with your question. Use Enter key to add choices below.]
choice a: (empty)
```

### `NewTextItem` — Text Block (`<text>`)

Non-question content: instructions, formatted text, images, links, field display references.

**Serialized form:**
```xml
<text label="T1" style="normal">
  <paragraph ...>...</paragraph>
</text>
```

- `style` — `"normal"`, `"boxed"`, `"shaded"`
- Can contain `<field name="..."/>` references for display-only computed values
- Supports `<displayConditions>` for conditional text blocks

### `NewHeadingItem` — Section Heading (`<heading>`)

```xml
<heading label="H1" style="main">Section Title</heading>
<heading label="H2" style="sub">Subsection</heading>
```

- `style` — `"main"` (larger, primary) or `"sub"` (smaller, secondary)

### `NewHiddenField` — Hidden/Computed Field (`<field>`)

A named field slot with no visible UI. Used to store values set by process commands.

```xml
<field name="TotalCost"/>
```

- `name` serves as both the `alternateLabel` and the field's reference name
- Addressable as `FormName:TotalCost` in processes
- Auto-generates a default name (`Field 1`, `Field 2`, ...) if none given

### `BreakItem` — Page Break (`<break/>`)

```xml
<break/>
```

Splits a long form into multiple browser pages. The runtime renders a "Next" button here. No content, no label, no attributes.

### `NewSkipInstructionsItem` — In-Form Skip Logic (`<skipInstructions>`)

A structured mini-process embedded directly in the form's item list. Contains `ProcessLine` objects rendered as a visual "If X → Skip to Y" panel in the Designer UI.

```xml
<skipInstructions>
  <if>
    <conditions><equals field="Sex" value="Female"/></conditions>
    <trueSet><skip to="Q7"/></trueSet>
    <falseSet/>
  </if>
</skipInstructions>
```

This is the Designer's encapsulation of the runtime's `SkipBlocks` / `SkipIf` mechanism. The Designer shows it as a panel between questions, distinct from a full process statement. At runtime, it is processed by `FormSegments` when rendering the form — skip blocks scan the item list and conditionally jump to a target label.

---

## The Conditions System (Designer Side)

`Conditions` is the Designer's model for both `displayConditions` (item-level visibility) and process `if`/`get` conditions. Both use the same condition factory.

**Full operator registry (from `Conditions.cs` static constructor):**

| XML Tag | Designer Class | Right-hand operand |
|---|---|---|
| `equals` | `FibCondition` | field or literal value |
| `doesNotEqual` | `FibCondition` | field or literal value |
| `contains` | `FibCondition` | field or literal value |
| `doesNotContain` | `FibCondition` | field or literal value |
| `beginsWith` | `FibCondition` | field or literal value |
| `endsWith` | `FibCondition` | field or literal value |
| `isLessThan` | `FibCondition` | numeric |
| `isLessThanOrEqualTo` | `FibCondition` | numeric |
| `isGreaterThan` | `FibCondition` | numeric |
| `isGreaterThanOrEqualTo` | `FibCondition` | numeric |
| `isBlank` | `FibNoExpressionCondition` | *(none)* |
| `isNotBlank` | `FibNoExpressionCondition` | *(none)* |
| `mcEquals` (with `value` attr) | `MCValueCondition` | literal string |
| `mcDoesNotEqual` (with `value`) | `MCValueCondition` | literal string |
| `mcContains` (with `value`) | `MCValueCondition` | literal string |
| `mcDoesNotContain` (with `value`) | `MCValueCondition` | literal string |
| `mcEquals` (no `value`) | `MCFieldCondition` | field reference |
| `mcDoesNotEqual` (no `value`) | `MCFieldCondition` | field reference |
| `mcContains` (no `value`) | `MCFieldCondition` | field reference |
| `mcDoesNotContain` (no `value`) | `MCFieldCondition` | field reference |
| `mcIsBlank` | `MCFieldNoExpressionCondition` | *(none)* |
| `mcIsNotBlank` | `MCFieldNoExpressionCondition` | *(none)* |
| `and` | `LogicalOperator` | groups child conditions |
| `or` | `LogicalOperator` | groups child conditions |

This maps exactly 1:1 to the Java runtime's `BooleanExpression` operator registry. The XML tag names are the contract — they must be identical on both sides.

---

## The Process Statement System (Designer Side)

`ProcessStatementFactory` mirrors the Java `ProcessCommandList.FACTORY` exactly:

```csharp
processStatementFactory.Register("addTo",          typeof(AddStatement));
processStatementFactory.Register("append",          typeof(AppendStatement));
processStatementFactory.Register("comment",         typeof(CommentStatement));
processStatementFactory.Register("delete",          typeof(DeleteStatement));
processStatementFactory.Register("divideBy",        typeof(DivideStatement));
processStatementFactory.Register("foreach",         typeof(ForEachRecordStatement));
processStatementFactory.Register("get",             typeof(GetStatement));
processStatementFactory.Register("if",              typeof(IfStatement));
processStatementFactory.Register("multiplyBy",      typeof(MultiplyStatement));
processStatementFactory.Register("send",            typeof(SendStatement));
processStatementFactory.Register("set",             typeof(SetStatement));
processStatementFactory.Register("edit",            typeof(ShowRecordStatement));
processStatementFactory.Register("show","document", typeof(ShowDocumentStatement));
processStatementFactory.Register("show","form",     typeof(ShowFormStatement));
processStatementFactory.Register("show",            typeof(ShowUrlStatement));
processStatementFactory.Register("skip",            typeof(SkipToStatement));
processStatementFactory.Register("subtractFrom",    typeof(SubtractStatement));
```

### `SetStatement`

Uses an `Expression` object supporting the `<<FieldReference>>` UI-text-box syntax, which compiles to nested `<set field="...">` XML. Arithmetic operators (`add`, `sub`, `mul`, `div`) are nested XML children. The target field is resolved at design time via `Project.FieldMapByName`. Unqualified field names (no colon) are treated as variables and added to the process's variable list.

### `GetStatement`

Holds a `RecordSet` (named record list + list of source forms + conditions). Registered in `process.RecordSets` so the Designer can expose the record set to subsequent `foreach` and field references. Supports external forms (cross-project shared data sources) via `FieldProviders.ExternalForms` — these are re-resolved if the shared project changes, via a `FieldProvidersChanged` event.

### Expression System

Two syntaxes exist in the Designer UI:
1. **Text-box input:** `<<FieldRef>>` + literal text → compiles to `<string field="..."/>` / `<string value="..."/>` XML operands
2. **Arithmetic:** nested `<add>`, `<sub>`, `<mul>`, `<div>` with `<operand field="..."/>` / `<operand value="..."/>` children

These compile directly to the `Operator` / `StringConcatenationExpression` structure in the Java runtime.

---

## The Form XML — Complete Structure

`Form.ToXml()` output:

```xml
<form name="Registration" startPoint="true"
      process="Post-Registration" preProcess="Init-Form"
      dataEntryOnly="false" dataSourceName="SharedPlayers">
  <items>
    <heading label="H1" style="main">Player Registration</heading>
    <text label="T1" style="normal">
      <paragraph>...</paragraph>
    </text>
    <fib label="Q1" alternateLabel="PlayerName" style="topLabels">
      <paragraph>
        First name: <blank label="a" alternateLabel="FirstName" length="20" required="true"/>
        Last name:  <blank label="b" alternateLabel="LastName"  length="20" required="true"/>
      </paragraph>
    </fib>
    <mc label="Q2" alternateLabel="Division" onlyone="true" required="true" style="vertical">
      <contents>
        <question><paragraph>Select division:</paragraph></question>
        <choice label="a">8U</choice>
        <choice label="b">10U</choice>
      </contents>
    </mc>
    <break/>
    <skipInstructions>
      <if>
        <conditions><mcEquals field="Division" value="8U"/></conditions>
        <trueSet><skip to="Q5"/></trueSet>
        <falseSet/>
      </if>
    </skipInstructions>
    <field name="ComputedTotal"/>
  </items>
</form>
```

**Key serialization rules:**
- `startPoint` is always emitted (no default omission)
- `dataEntryOnly` and `dataSourceName` are omitted when default (false / empty)
- `process` and `preProcess` are omitted when not connected
- Items appear in authoring order within `<items>`
- The `<items>` wrapper element is always present even when empty

---

## Format Version Divergence

| Side | Version | Notes |
|---|---|---|
| Designer (`XmlFormatVersion`) | `"1.11"` | Always written by Designer |
| Java runtime (`CURRENT_VERSION`) | `"1.4"` | What the runtime expects |
| Java runtime (`MINIMUM_SAFE`) | `"1.3"` | Oldest the runtime will accept |

The Designer was version-ahead of the runtime intentionally — new Designer features could write new format elements that the runtime safely ignored until it caught up. The `XMLTransceiver` on upload handled version negotiation.

**For reimplementation:** Treat the JSON schema as the canonical new format (`"format": "2.0"` or similar). The Designer-to-runtime XML contract documented here should be the reference for mapping existing `.tawala` projects to the new JSON format.

---

## The `displayConditions` Round-Trip — The Critical Contract

The same condition structure traverses the full pipeline:

1. **Authored** in the Designer's `ConditionsBuilder` UI (visual condition builder control)
2. **Serialized** by `Conditions.ToXml("displayConditions")` → child element of the form item
3. **Deserialized** by `FormItem.getDisplayConditions()` in the Designer on project load
4. **Transmitted** unchanged as part of the XML blob in the `.tawala` file
5. **Parsed** by `FormItem(ConfigElement element)` in the Java runtime → `BooleanExpression.load()`
6. **Evaluated** at render time by `FormItem.shouldBeDisplayed(context)`

The condition operator XML tag names (`equals`, `mcContains`, `isBlank`, etc.) are **identical** in the C# `Conditions` factory and the Java `BooleanExpression` factory. This is a strict contract — any name change breaks the round-trip for existing projects.

---

## Complete Designer → Runtime XML Mapping

| Designer C# class | XML element produced | Java runtime class |
|---|---|---|
| `Project` | `<project format="1.11" designerBuild="...">` | `Project` |
| `Form` | `<form name="..." startPoint="...">` | `Form` |
| `NewFibItem` | `<fib label="..." style="...">` | `FillInBlank` |
| `NewMcqItem` | `<mc label="..." onlyone="..." required="...">` | `MultipleChoice` |
| `NewTextItem` | `<text label="..." style="...">` | `TextBlock` |
| `NewHeadingItem` (main) | `<heading label="..." style="main">` | `HeadingItem` |
| `NewHeadingItem` (sub) | `<heading label="..." style="sub">` | `SubheadingItem` |
| `NewHiddenField` | `<field name="..."/>` | `StoredField` |
| `BreakItem` | `<break/>` | page break in `FormSegments` |
| `NewSkipInstructionsItem` | `<skipInstructions>` | `SkipBlocks` / `SkipIf` |
| `Conditions` | `<displayConditions>` | `BooleanExpression` (in `FormItem`) |
| `Process` | `<process name="...">` | `Process` |
| `SetStatement` | `<set field="..." ...>` | `SetCommand` |
| `GetStatement` | `<get recordList="...">` | `Get` |
| `SendStatement` | `<send>` | `Send` |
| `IfStatement` | `<if>` | `If` |
| `ForEachRecordStatement` | `<foreach recordList="..." record="...">` | `ForEach` |
| `SkipToStatement` | `<skip to="..."/>` | `SkipCommand` |
| `ShowFormStatement` | `<show form="..."/>` | `ShowForm` |
| `ShowDocumentStatement` | `<show document="..."/>` | `ShowDocument` |
| `ShowUrlStatement` | `<show><url>...</url></show>` | `Show` |
| `ShowRecordStatement` | `<edit>` | `EditRecord` |
| `AppendStatement` | `<append>` | `Append` |
| `DeleteStatement` | `<delete>` | `Delete` |
| `AddStatement` | `<addTo>` | `MathCommand.AddTo` |
| `SubtractStatement` | `<subtractFrom>` | `MathCommand.SubtractFrom` |
| `MultiplyStatement` | `<multiplyBy>` | `MathCommand.MultiplyBy` |
| `DivideStatement` | `<divideBy>` | `MathCommand.DivideBy` |
| `CommentStatement` | `<comment>` | *(ignored at runtime)* |
