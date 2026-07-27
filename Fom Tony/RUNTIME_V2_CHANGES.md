# tawalaRuntime v2 — Changes & Analysis Notes

Deployed: 2026-06-20

---

## What Changed in v2

### 1. Skip Logic — Now Fully Implemented

`runSkipInstructions()` iterates a form's items for `type: "skipInstructions"` or `type: "skip"` items. Each item's `commands` array is evaluated by `evalSkipBlock()`, which handles:
- `{ cmd: "skip", to: "LabelName" }` — unconditional skip
- `{ cmd: "if", condition, then, else }` — conditional skip (both branches are skip blocks)
- `set`, math commands — side effects allowed inside skip blocks

Skip runs **before** the post-process. Returns `skipTarget` to the client.

Client behavior:
- `skipTarget === null` → show all sections normally
- `skipTarget === "__EndOfForm__"` → proceed to post-process navigation
- `skipTarget === "SomeLabelName"` → re-display same form starting from named section

### 2. `get` WHERE clause — Now Correctly Evaluated

`evalCondOnRecord()` resolves field references from a record object, supporting:
- Three-part: `RecordName:FormName:fieldName`
- Two-part: `FormName:fieldName`
- Dot notation: `FormName.fieldName`
- Bare: `fieldName`

### 3. `foreach` Three-Part Field Resolution

`getValue()` now resolves `RecordName:FormName:fieldName` via `ctx.recordBindings[RecordName]`.

### 4. Unified `compareValues()` for All Operators

Single shared function used by both `evalCond()` and `evalCondOnRecord()`.
MC operators now properly split on commas and compare per-choice.

### 5. Boot Fix

`getForm` is the correct boot entry point. Empty `formName` in `submitForm` safely delegates to start-form display without running the post-process.

---

## Skip Instructions JSON Schema

```json
{ "type": "skipInstructions", "label": "skip1", "commands": [
    { "cmd": "if",
      "condition": { "field": "FormName:Q:blank", "op": "equals", "value": "Yes" },
      "then": [{ "cmd": "skip", "to": "DestLabel" }],
      "else": [{ "cmd": "skip", "to": "__EndOfForm__" }] }
] }
```

## All Condition Operators

FIB: `equals`, `doesNotEqual`, `contains`, `doesNotContain`, `beginsWith`, `endsWith`, `isBlank`, `isNotBlank`, `isGreaterThan`, `isLessThan`, `isGreaterThanOrEqualTo`, `isLessThanOrEqualTo`

MC: `mcEquals`, `mcDoesNotEqual`, `mcContains`, `mcDoesNotContain`, `mcIsBlank`, `mcIsNotBlank`

Logical: `and`, `or`
