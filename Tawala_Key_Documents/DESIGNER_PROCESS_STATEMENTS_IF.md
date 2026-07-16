# Designer process statements — If

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026). Other statements (Show, Send, …) documented separately as they are captured.

Related: `DESIGNER_MENU_SPEC.md` (Statements palette), `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` (If in Skip Instructions — same condition UI, smaller statement set).

---

## Process editor shell

When a **Process** node is selected in Project Explorer:

| Area | Content |
|------|---------|
| Middle column | **Statements** palette (text buttons, grouped) |
| Center | MDI window **Process - [name]** |
| Right | **Fields** (all forms + Variables) |

### Statements palette (process selected)

Same order as **Insert → Process**:

| Group | Statements |
|-------|------------|
| 1 | **If** |
| | *(gap)* |
| 2 | **Show**, **Send** |
| | *(gap)* |
| 3 | **Append** |
| | *(gap)* |
| 4 | **Get**, **ForEach**, **Delete** |
| | *(gap)* |
| 5 | **Set** |
| | *(gap)* |
| 6 | **Comment** |

Clicking a statement button selects that statement type and opens its **property panel** in the upper part of the process window. The lower part shows the **process script** (pseudo-code lines) with a blue arrow marking the **insertion point**.

### Post-process banner

Yellow bar at top of process window:

| State | Text |
|-------|------|
| Not connected | *Not connected as Post-Process to any Form. Click here to change.* |
| Connected | *Connected as Post-Process to Form 'Start'. Click here to change.* |

Post-process runs after the connected form is submitted.

### Empty process

Instruction text: *To create a new statement, click one of the buttons in the Statements palette.*

---

## If statement — property panel

Selecting **If** in the Statements palette opens the **If** tab in the details panel (top half of the process window).

### Header copy

**Single condition:**

*If the following condition is true, execute the first set of commands:*

**Multiple conditions** (after **+** adds a second row):

*If **[ALL / ANY]** of the following conditions are true, execute the first set of commands:*

| Combobox | Meaning |
|----------|---------|
| **ALL** | AND — every condition must be true |
| **ANY** | OR — at least one condition must be true |

### Condition row

Each row has:

| Control | Purpose |
|---------|---------|
| Field box (light green when focused) | Qualified field name — **drag from Fields** or double-click a field in Fields palette |
| Operator dropdown | Comparison operator (set depends on field type — see below) |
| Value box (white) | Comparison value — typed text, choice letter, or drag a field reference |
| **+** / **−** | Add or remove condition rows |

**Field input:** Only **Fields palette** references appear practical (drag/double-click). Variables use the same operator sets as FIB text fields (`HybridOperator` in source).

### Otherwise (else branch)

Checkbox: **Otherwise execute second set of commands**

When checked, script preview shows an **Otherwise** block for commands when the If condition is false.

### Add / Modify button

- Label is **Add** when inserting a new If at the insertion point.
- Label is **Modify** when editing an existing If line (double-click or select line in script).
- **Enabled only when:**
  1. Insertion point is valid for adding **or** panel is in **Modify** mode, **and**
  2. Every condition row has a field filled in, **and**
  3. Every row that needs a value has the value box filled (`is blank` / `is not blank` hide the value box).

After **Add**, the If block is written into the script and the insertion point moves **inside** the opening `( … )` so nested statements (e.g. **Show**) can be added.

---

## Operators by field type (from source)

Operator list is chosen from the dragged field’s type (`IOperatorDataSource` on each field).

### FIB blank, Variable, and most text-like fields

**HybridOperator** — 12 operators:

| Operator | Typical use (owner / source) |
|----------|------------------------------|
| equals | Text or numeric equality |
| does not equal | |
| contains | Text |
| does not contain | Text |
| begins with | Text |
| ends with | Text |
| is less than | Numeric / ordered |
| is less than or equal to | Numeric / ordered |
| is greater than | Numeric / ordered |
| is greater than or equal to | Numeric / ordered |
| is blank | No value box |
| is not blank | No value box |

### MCQ — single-select (`selectOnlyOne`)

**MCOneOperator** — 4 operators:

| Operator |
|----------|
| equals |
| does not equal |
| is blank |
| is not blank |

When an MCQ field is selected in the condition, **Fields** palette may expand **Choices** (a, b, c, …) for picking comparison values.

### MCQ — multi-select

**MCManyOperator** — 6 operators: equals, does not equal, is blank, is not blank, **contains**, **does not contain**.

### Owner notes

- Text-only operators (contains, begins with, …) are offered for FIB/Variable rows; runtime behavior on non-text data not verified on Jan 2011 build.
- Numeric operators appear in the same FIB/Variable list; suitability depends on stored answer format.
- Only one field reference per condition row in the field box; literals or choice letters go in the value box.

---

## Example workflow (owner tutorial)

**Setup:** Process 1 connected as **Post-Process** to form **Start**. Fields: `username` (FIB), `Q2` (MCQ).

### 1. Simple If + Show

```
If Start:username is blank
(
  Show Document Document 1
)
```

1. Click **If** → drag `Start:username` → operator **is blank** → **Add**.
2. Click **Show** → tab **Document** → dropdown **Document 1** → **Add**.

### 2. If / Otherwise with two Show documents

```
If Start:username is blank
(
  Show Document Document 1
)
Otherwise
(
  Show Document Document 2
)
```

Check **Otherwise execute second set of commands** before first **Add**. Place insertion point in each `( … )` block to add the corresponding **Show**.

### 3. MCQ condition

```
If Start:Q2 equals a
(
  Show Document Document 1
)
```

Drag `Start:Q2` → operators limited to MC set → **equals** → value `a`.

### 4. Multiple conditions (AND)

```
If Start:Q2 equals a AND Start:username is not blank
(
  Show Document Document 1
)
```

Add second row with **+** → set combobox to **ALL** → `Start:username` **is not blank**.

---

## Show statement (brief — used inside If)

See **`DESIGNER_PROCESS_STATEMENTS_SHOW.md`** for full Document / Form / Stored Record / URL tabs.

---

## If panel vs other statements (owner clarification)

The **If** property panel stays visible in the **top** details area while **If** is selected in the Statements palette — even when the insertion point in the **script** (lower pane) has moved on to another statement type. This can look like being “stuck inside If” when you are actually configuring or viewing a different statement.

**Not a bug (June 2026):** Owner confirmed that a greyed **Add** button in this situation was due to misreading the UI — the process had only one command at that point, not an unfinished If block.

**How to tell where you are:**

| Cue | Meaning |
|-----|---------|
| **Orange/highlighted button** in Statements palette | Which statement type’s property panel is shown on top |
| **Blue arrow** in lower script pane | **Insert mode:** ▶ points at the gap between lines (where **Add** inserts). **Edit mode:** ▶ points at the highlighted statement (**Modify**). Never both at once (legacy). |
| **Add** vs **Modify** on the button | Add = new line at insertion point; Modify = editing the selected script line (click that line) |

Click another statement button (**Show**, **Set**, …) or click in the script to move the insertion point when leaving If configuration.

---

## XML / source cross-reference

| Topic | Location |
|-------|----------|
| If statement model | `TawalaDesigner/Code/TAWALA/Projects/Processes/IfStatement.cs` |
| If UI | `TawalaDesigner/Code/TAWALA/Processes/IfStatementView.cs` |
| Condition rows | `TawalaDesigner/Code/TAWALA/Controls/ConditionGroup.cs` |
| Operators | `TawalaDesigner/Code/TAWALA/Projects/Expressions/ComparisonOperator.cs` |
| Process editor / insertion point | `TawalaDesigner/Code/TAWALA/Processes/ProcessEditor.cs` |
| Statement palette order | `TawalaDesigner/Code/TAWALA/Processes/MDIForm.cs` → `StatementViewTypes` |

---

## Open questions

1. Runtime errors when using `contains` / numeric operators on wrong data types?
2. Can the value box accept `<<field>>` references for all operator types?

---

*Last updated: June 2026.*
