# Designer — Form item conditional display

Captured from legacy **Tawala Project Designer** screenshots (owner, Aug 10, 2026) plus C# `Forms/Dialogs/FormItemConditionalDisplay*`.

**Why it matters:** Conditional display on Form Items left `<displayConditions>` in XML that the browser convert path did not surface for a long time — roughly **seven** owner projects were hard or impossible to convert cleanly until `displayCondition` was preserved on JSON items. Those projects were **sequestered** until the Designer could edit the conditions.

**Aug 10, 2026 — released from sequestration:** Item-level conditional display is editable (badge menu + dialog + `{Qn}` braces). Projects held only for this gap may return to normal Library / My Tawala staging. (Per-column itemization `displayCondition` in Configure Function remains cue-only — not a reason to sequester whole projects.)

Screenshots:

| File | Content |
|------|---------|
| [`assets/Conditional_Display_RClick.png`](assets/Conditional_Display_RClick.png) | Right-click menu on a form-item label badge |
| [`assets/Conditional_Display_Dialog.png`](assets/Conditional_Display_Dialog.png) | **Conditional Display of Form Item** dialog |
| [`assets/Conditional_Display_Braces_Labels.png`](assets/Conditional_Display_Braces_Labels.png) | Canvas badges with `{Q2}`-style braces when conditional |

Related: `DESIGNER_OPEN_TODOS.md` § Form items; `DESIGNER_OPEN_BUGS.md` § Hierarchical convert / preserved-warning cues; convert helpers in `designer-web/src/lib/preservedImportGaps.ts`.

---

## Canvas cue (legacy)

When a Form Item has conditional display enabled, its **left-column badge** (T / Q / H / …) is wrapped in **curly braces**:

| Always shown | Conditional |
|--------------|-------------|
| `Q1`, `Q3`, `T2`, `H1` | `{Q2}`, `{Q4}`, `{Q5}`, `{Q6}`, `{Q7}` |

Example (CreateStaffDashboard): Title, Second Email, phones show as `{Q…}` while Name / Email stay plain.

**Not** parentheses around the badge (earlier TODO wording was wrong).

---

## Entry point (legacy)

**Right-click** the form-item **label/badge** column (not only the prompt text). Context menu:

| Item | Notes |
|------|--------|
| Cut (Ctrl+X) | |
| Copy (Ctrl+C) | |
| Paste (Ctrl+V) | Greyed when clipboard empty |
| Delete (Del) | |
| *(separator)* | |
| **Display conditionally…** | Opens the dialog below |

---

## Dialog: Conditional Display of Form Item

Window title: **Conditional Display of Form Item** (gear icon).

| Control | Copy / behavior |
|---------|-----------------|
| Checkbox | **Display this item conditionally** (trailing space in Designer resource string) |
| Label | **Display only when** |
| Combinator | **ALL** / **ANY** — shown when multiple condition rows (“…of the following conditions are true:”) |
| Condition rows | Field (green drop target) · operator · value · **+** / **−** |
| Buttons | **OK** · **Cancel** |

Example from screenshot: field `QEmail2`, operator **equals**, value **Yes**.

Operators and field-drop behavior match Skip / If / Get **Where** rows (same condition vocabulary).

Unchecking **Display this item conditionally** clears conditional display (badge braces go away).

C# sources:

- `TawalaDesigner/Code/TAWALA/Forms/Dialogs/FormItemConditionalDisplayView.cs`
- `FormItemConditionalDisplayPresenter.cs`
- `IFormItemConditionalDisplayPresenter.cs`

---

## Deploy / XML

Form items emit nested:

```xml
<displayConditions>
  <equals field="QEmail2"><string value="Yes"/></equals>
</displayConditions>
```

(and other ops: `isBlank`, `doesNotEqual`, nested `and`/`or`, etc.)

Browser:

| Path | Status |
|------|--------|
| `.tawala` → JSON (`tawalaXmlToJson`) | Preserves `item.displayCondition`; convert warn logged |
| JSON → Push XML (`jsonToXml`) | Re-emits `<displayConditions>` |
| Design canvas cue | Amber **`cond`** chip beside T/Q badge (not legacy `{Qn}` braces yet) |
| Edit UI (right-click → dialog) | **Yes** (Aug 10) — see Browser gaps below |

Per-column itemization `displayCondition` is a related but separate Configure Function gap (“Column is always displayed” link).

---

## Browser Designer gaps

| Legacy | Browser (Aug 10, 2026) |
|--------|------------------------|
| Right-click → **Display conditionally…** | **Yes** — badge context menu on FIB / Text / MCQ / Heading / Structured Text |
| `{Qn}` braces on badge | **Yes** — braces **inside** the badge cell (`{Q4}`), matching legacy |
| Dialog with checkbox + Where rows | **Yes** — `ConditionalDisplayDialog` (FunctionConditionsEditor) |
| Edit / clear / multi-row ALL/ANY | **Yes** — OK writes `item.displayCondition`; uncheck clears |
| Amber `cond` chip | Optional (`showCondChip`); braces are the primary cue |

### Smoke (browser)

1. Form with FIB Q2 → right-click badge → **Display conditionally…** → check box → Where `QEmail2` equals `Yes` → OK → badge shows `{Q2}`.
2. Uncheck → OK → braces clear.
3. Push: item hidden until condition true (existing Java `displayConditions`).
4. Import CreateStaffDashboard-style project → braces on Q2/Q4–Q7; right-click edits preserved conditions.

---

## Smoke (legacy reference)

1. Open a form with conditional items (e.g. CreateStaffDashboard) → badges show `{Q2}`, `{Q4}`… 
2. Right-click `{Q2}` → **Display conditionally…** → checkbox on; Where `QEmail2` equals `Yes`.
3. OK → braces remain; uncheck → braces clear.
