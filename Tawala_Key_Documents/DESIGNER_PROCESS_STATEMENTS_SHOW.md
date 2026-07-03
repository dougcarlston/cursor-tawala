# Designer process statements — Show

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

Related: `DESIGNER_PROCESS_STATEMENTS_IF.md`, `DESIGNER_MENU_SPEC.md`.

---

## Overview

**Show** displays something to the end user at runtime: a **document**, another **form**, a **stored record** editor, or a **URL**.

Select **Show** in the Statements palette (orange highlight). The property panel uses **four tabs** — each tab is a different Show variant. **Add** commits the line at the blue-arrow **insertion point** in the process script (lower pane).

Same pattern as **If**: top panel follows the highlighted Statements button; the script pane shows what is actually in the process.

---

## Property panel — four tabs

| Tab | Script line pattern (examples) |
|-----|------------------------------|
| **Document** | `Show Document Header` |
| **Form** | `Show Form Followup Q` |
| **Stored Record** | `Show stored record from Start where Record:Start:Q2 equals a` |
| **URL** | `Show URL www.Tswala.com` |

Each tab has its own **Add** button (becomes **Modify** when editing an existing line).

**Add enabled when:**

| Tab | Requirements |
|-----|----------------|
| Document | Document selected in dropdown; valid insertion point (or Modify mode) |
| Form | Form selected in dropdown; valid insertion point (or Modify mode) |
| Stored Record | **from** form selected; **Where** optional (may be blank); valid insertion point (or Modify mode) |
| URL | URL text box non-empty |

---

## Tab: Document

| Control | Label / behavior |
|---------|------------------|
| Dropdown | Lists all project **documents** (e.g. Document 1, Header) |
| Checkbox | **Reset document to original state after Show** |
| Sub-label | *(removes any appended documents)* |

When reset is checked, script text includes **and reset**:

`Show and reset Document Header`

(XML: `<show document="…" reset="true|false"/>`)

**Preview:** Opening or selecting a document may show an MDI window (e.g. **Document - Document 1** with rich text *This is Document 1*).

---

## Tab: Form

| Control | Behavior |
|---------|----------|
| Dropdown | Lists all project **forms** (Form 2, Start, Form 3, Form 4, Followup Q, …) |

Commits:

`Show Form {form name}`

(XML: `<show form="…"/>`)

Used in owner example after an **If** block to show a follow-up form when conditions pass.

---

## Tab: Stored Record

Edit an existing stored submission or create a new record, scoped by form and optional **Where** clause.

### Layout

| Control | Label / options |
|---------|-----------------|
| **from** | Dropdown — source **form** whose stored records are searched (Form 2, Start, …) |
| **Upon submit:** | **Modify existing record** (default) or **Create new record** |
| **Where** | Condition builder (same row UI as **If** — field, operator, value, **+** / **−**) |

**Where** is **optional** (owner confirmed): **Add** stays enabled with **from** selected and Where left blank — selects among all stored records for that form with no filter.

When conditions are present, use **+** / **−** and **ALL** / **ANY** as in **If**.

### Fields palette (Stored Record tab active)

When **from** form is selected, Fields expands a **Record:** branch with record-qualified fields, e.g.:

- `Record:Start:username`
- `Record:Start:Q2`

Drag into **Where** rows (owner example: `Record:Start:Q2` **equals** `a`).

**Choices** under an MCQ may still appear for literal values.

### Script format

```
Show stored record from {FormName} where {conditions}
```

Example with Where:

```
Show stored record from Start where Record:Start:Q2 equals a
```

Example without Where (owner confirmed valid):

```
Show stored record from Start
```

(XML: `<edit form="Start" submit="modify|new">` with nested `<conditions>…</conditions>`)

**submit attribute:** `modify` = Modify existing record; `new` = Create new record.

---

## Tab: URL

| Control | Label |
|---------|-------|
| Label | **Type or paste URL here:** |
| Text box | Free-form URL string |

Commits:

`Show URL {url}`

Example from screenshot: `Show URL www.Tswala.com` (owner typing — not validated in Designer).

(XML: `<show><url><string value="…"/></url></show>`)

**Add** enabled when URL text is non-empty (insertion-point rules for URL tab are looser than Document/Form in source).

---

## Example process (owner walkthrough)

Post-process on form **Start**. Cumulative script:

```
Show Document Header
If Start:username is not blank AND Start:Q2 equals a
(
  Show Document Header
)
Show Form Followup Q
Show stored record from Start where Record:Start:Q2 equals a
Show URL www.Tswala.com
```

**Flow reading:**

1. Always show document **Header** first.
2. If username filled and Q2 = **a**, show **Header** again inside the If.
3. Show form **Followup Q**.
4. Open stored record from **Start** matching Q2 = **a** (modify existing).
5. Show external URL.

Document window **Document - Header** can remain open behind the process editor as a design-time preview.

---

## Browser Designer (`designer-web`) gaps

| Feature | Legacy | Browser today |
|---------|--------|----------------|
| Show statement | Four-tab UI | Not implemented |
| Show Document + reset | Yes | N/A |
| Show Form | Yes | N/A |
| Show Stored Record + Where | Yes | N/A |
| Show URL | Yes | N/A |

---

## Source cross-reference

| Topic | Location |
|-------|----------|
| Show UI | `TawalaDesigner/Code/TAWALA/Processes/ShowStatementView.cs` |
| Document / Form show | `…/Projects/Processes/ShowStatement.cs` |
| Stored record show | `…/Projects/Processes/ShowRecordStatement.cs` |
| URL show | `…/Projects/Processes/ShowUrlStatement.cs` |

---

## Open questions

1. Runtime behavior of **Show URL** without `http://` prefix?
2. Does **Reset document after Show** interact with **Append** statements elsewhere in the process?

---

*Last updated: June 2026.*
