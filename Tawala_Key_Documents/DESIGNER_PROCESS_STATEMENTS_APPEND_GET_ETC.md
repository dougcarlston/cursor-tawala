# Designer process statements — Append, Get, ForEach, Delete, Set, Comment

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

**Jan 2011 build caveat:** Several statements are **partially implemented** in the owner’s Designer cut. Full behavior exists in `TawalaDesigner` source; runtime may work on deployed projects even when the local UI is incomplete.

Related: `DESIGNER_PROCESS_STATEMENTS_IF.md`, `DESIGNER_PROCESS_STATEMENTS_SHOW.md`, `DESIGNER_PROCESS_STATEMENTS_SEND.md`.

---

## Implementation status (owner’s build)

| Statement | UI status | Owner notes |
|-----------|-----------|-------------|
| **Append** | Complete | Fine and easy to use |
| **Get** | Mostly works | Part of Get/ForEach/Delete group not fully implemented in this Designer; **appears to work** |
| **ForEach** | **Incomplete** | First dropdown empty; cannot drag or type into it |
| **Delete** | **Incomplete** | Where row blanks/dropdowns not implemented |
| **Set** | Complete (simple) | Variable target only for **Add** |
| **Comment** | Complete | Designer-only notes in script |

---

## Append

### UI

Single **Documents** tab:

```
[appendage document ▼]  to  [target document ▼]     [Add]
```

Both dropdowns list project documents (real + virtual). Example: **Document 2** to **Header**.

### Script

```
Append Document 2 to Header
```

(XML: `<append document="Header" appendage="Document 2"/>`)

### Relation to Show / Send reset

**Show** and **Send** offer **Reset document to original state after Show/Send** — undoes appended content after a single use. Owner uses **Append** to build composite documents, then **Show and reset Document …** when a clean copy is needed next time.

### Add enabled

Both documents selected; valid insertion point (or Modify mode).

---

## Get

Retrieves stored submissions into a named **record list** for use by **ForEach** and record-qualified fields.

### UI

| Control | Purpose |
|---------|---------|
| **Record list name** | Text box — default auto-name e.g. `Record List 1` |
| **from** | Dropdown — source form (e.g. **Start**) |
| **Where** | Same condition rows as **If** (field, operator, value; **+** / **−**; **ALL** / **ANY** when multiple rows) |

### Fields palette after Get

Adds a **Record List 1** (or chosen name) branch with qualified fields, e.g.:

- `Start:username`
- `Start:UserEmail`
- `Start:Q3`

While editing Get (source form selected), Fields also shows a **`Record:`** branch for **Where** drops (`Record:Admin:Teams`, …) — same cue as Delete / Show Stored Record / ForEach loop variables.

### Script

```
Get Record List 1 from Start where Start:UserEmail is not blank
```

Operators on Where rows match **FIB/hybrid** set (equals, contains, is blank, …) — same as If when dragging form fields.

### Owner note

Get **appears to work** in this build even though the **Get / ForEach / Delete** family is not fully finished in the Designer UI.

### Add enabled

Valid record list name; form selected; Where rows complete (if present); valid insertion point.

---

## ForEach

Iterates a **record** variable over a **record list** produced by **Get**.

### UI — **Record** tab

```
[record variable ▼]  in  [record list ▼]     [Add]
```

| Control | Intended behavior (source) |
|---------|---------------------------|
| First dropdown | **Record** loop variable — should list `Process.Records` and allow typing a new record name |
| Second dropdown | **Record list** — should list `Process.RecordSets` (populated after **Get** in same process) |

### Owner build — broken / incomplete

- **First dropdown:** no entries; cannot drag or type.
- **Add** greyed (requires both fields per source).
- Second dropdown may show **Record List 1** after a **Get** line exists, but loop cannot be completed.

### Expected script (when working)

```
ForEach Record in Record List 1
```

Followed by statements inside the ForEach block (insertion inside parentheses).

---

## Delete

Deletes stored records from a form, optionally filtered by **Where**.

### UI

| Control | Label |
|---------|-------|
| Form dropdown | **records from form:** (e.g. **Followup Q**) |
| **Where** | Condition rows (same pattern as Get / If) |

### Owner build — incomplete

Where row controls appear **unwired**: empty text boxes and empty operator dropdown; cannot complete conditions. Form dropdown may work.

### Expected script (when working)

```
Delete records from Followup Q where …
```

---

## Set

Assigns a value to a **variable** (creates the variable if it does not exist).

### UI

| Control | Purpose |
|---------|---------|
| First box (green when focused) | **Target** — variable name |
| **to** | |
| Second box | **Value** — literal, expression, or dragged field reference |
| Checkbox | **Treat arithmetic expression as text (do not interpret +, -, * or / as math)** |

### Target rules (owner + source)

- Designer may **drag** any Fields palette item into the first box.
- **Add** enables only when the target is a **variable**:
  - Built-in **`_InviteeID`** (auto-created with projects), or
  - User-defined variable (e.g. **`Completed`** created by typing a new name), or
  - Existing variable in **Variables** folder.
- Form fields (e.g. `Start:username`) as target do **not** enable **Add** (unless they are assignable record fields in other contexts — owner practice: variables only).

### Value

Examples: literal `Yes`, quoted strings, `<<field>>` references in expression box.

### Script

```
Set Completed to "Yes"
```

### Variables in Fields palette

**Variables** folder at bottom of Fields panel lists project variables; new names appear after first **Set**.

---

## Comment

Embeds a **documentation line** in the process script. **Not executed** at runtime — visible only in Designer (owner: invisible to end users).

### UI

**Comment** tab with multi-line text box and **Add**.

### Script display

Comment text appears as its own line in the process list (screenshot: blue italic styling for comment lines).

Example (owner):

```
A couple of Statements aren't complete in this version of Designer: ForEach and Delete
```

(XML: `<comment>…</comment>`)

### Add enabled

Comment text non-empty.

---

## Example process (owner walkthrough)

Cumulative script from screenshots:

```
If Start:username is not blank AND Start:Q3 equals a
(
  Show Document Document 3
)
Append Document 2 to Header
Show and reset Document Header
Get Record List 1 from Start where Start:UserEmail is not blank
Set Completed to "Yes"
A couple of Statements aren't complete in this version of Designer: ForEach and Delete
```

*(Send / other lines may appear in longer versions of this test process.)*

---

## Browser Designer (`designer-web`) gaps

All six statement types: **not implemented** in `designer-web`. Implement from this spec + C# source when prioritizing process editor parity.

---

## Source cross-reference

| Statement | UI | Model |
|-----------|-----|-------|
| Append | `Processes/AppendStatementView.cs` | `Projects/Processes/AppendStatement.cs` |
| Get | `Processes/GetStatementView.cs` | `Projects/Processes/GetStatement.cs` |
| ForEach | `Processes/ForEachStatementView.cs` | `Projects/Processes/ForEachRecordStatement.cs` |
| Delete | `Processes/DeleteStatementView.cs` | `Projects/Processes/DeleteStatement.cs` |
| Set | `Processes/SetStatementView.cs` | `Projects/Processes/SetStatement.cs` |
| Comment | `Processes/CommentStatementView.cs` | `Projects/Processes/CommentStatement.cs` |

---

*Last updated: June 2026.*
