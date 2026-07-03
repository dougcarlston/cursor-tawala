# Designer process statements — Send

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

Related: `DESIGNER_PROCESS_STATEMENTS_SHOW.md`, `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` (email validation on FIB).

---

## Overview

**Send** means **send by email only**. There is a single **Email** tab (no SMS or other channels in this UI).

Select **Send** in the Statements palette. **Add** inserts at the blue-arrow insertion point in the process script.

---

## Email tab — fields

| Label | Control | Behavior |
|-------|---------|----------|
| **To:** | Field text box | **Addressee** — owner: **must be a field** from Fields palette (drag or double-click). Example: `Start:UserEmail` (green highlight when field-bound). |
| **Cc:** | Field text box | Optional; field drag/double-click or typed literal — **not validated** in Designer (owner: probably should be, like **From**). |
| **From (Address):** | Field text box | **Addressor** — may be **typed directly** (e.g. `doug@carlston.net`); **not validated** in Designer (owner: probably should be). May also accept a dragged field. |
| **(Name):** | Field text box | Optional display name for From; literal or field. |
| **Subject:** | Expression text box | Free text; may embed field references. Example: `A test Message`. |
| **Document to be used as Body text:** | Dropdown | Project document used as email body (e.g. Document 2). |

### Checkboxes

| Checkbox | Sub-label |
|----------|-----------|
| **Include Page Header** | Enabled only when project has page header content |
| **Reset document to original state after Send** | *(removes any appended documents)* |

When reset is checked, script may append **and reset Document** (same pattern as Show).

---

## Add button

**Enabled when** (valid insertion point or Modify mode):

- **To** non-empty
- **Subject** non-empty
- **Document** selected in dropdown

**From (Address)** is **not** required for Add (owner may leave blank or type unvalidated address).

---

## Owner pattern: validated recipient email

Add a **FIB** on the source form (e.g. form **Start**) with alternate label **UserEmail** and **Validation → Email** on the blank. Collect the address at submit time; reference **`Start:UserEmail`** in **To:**.

Designer does **not** re-validate **From** or **Cc** when typed as literals. Owner note: both probably **should** be validated (email format) — only **To** is practically enforced via a form FIB with Email validation.

---

## Script line

```
Send Document {doc name} to {recipient}
```

Example:

```
Send Document Document 2 to Start:UserEmail
```

With reset:

```
Send Document Document 2 to Start:UserEmail and reset Document
```

---

## Example in process (owner)

```
Show Document Header
If Start:username is not blank AND Start:Q3 equals a
(
  Show Document Header
)
Show Form Followup Q
Send Document Document 2 to Start:UserEmail
```

---

## Fields palette interaction

- Focused row highlighted **light green** (To, Cc, From, Name, Subject in turn).
- Double-click field in palette fills the focused row.
- Typing in a box clears field binding (`KeyPress` clears Tag) — for **To**, owner uses field-only practice.

---

## Browser Designer (`designer-web`) gaps

| Feature | Legacy | Browser today |
|---------|--------|----------------|
| Send email statement | Email tab | Not implemented |
| Document as body | Yes | N/A |
| To / From / Cc / Subject | Yes | N/A |

---

## Source cross-reference

| Topic | Location |
|-------|----------|
| Send UI | `TawalaDesigner/Code/TAWALA/Processes/SendStatementView.cs` |
| Send model / XML | `…/Projects/Processes/SendStatement.cs` |
| Document body | `…/Projects/Processes/SendBody.cs` (`SendDocumentBody`) |

---

## Open questions

1. Runtime: is **To** enforced as field-only, or can literals work if typed?
2. **Include Page Header** — what header content is included?
3. Should **browser Designer** add email validation for **From** and **Cc** even if legacy did not?

---

*Last updated: June 2026.*
