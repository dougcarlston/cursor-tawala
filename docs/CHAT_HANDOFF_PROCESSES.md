# Chat handoff — Processes only (browser Designer)

**Use this file** to start a fresh Process chat and drop older threads. Forms: [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md).

**Suggested chat title:** `Designer — Process statements (Send next)`

---

## Paste opener (new chat)

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — Processes only (designer-web/)
Goal: Send statement property panel next; then Append / Get / ForEach / Delete / Comment
Read first: docs/CHAT_HANDOFF_PROCESSES.md, Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_SEND.md
Branch: cursor/forms-canvas-wysiwyg (or main after merge)
Constraints: designer-web/ only; no 8080 / website-mock; no commit unless I ask
```

---

## Run locally

```bash
cd ~/Projects/AI-Tawala/designer-web && npm run dev
```

Reload after pull: **Cmd+Shift+R** (unsaved in-memory work is lost).

---

## Done (July 9, 2026)

| Area | Notes |
|------|--------|
| **If / Set / Show property panels** | In Process MDI window (`IfStatementBuilder`, `SetStatementBuilder`, `ShowStatementBuilder`). Palette toggles panel; other statements still template-insert. |
| **Script line → panel** | Clicking If/Set/Show line switches panel; If panel sticky when clicking nested Show/Set inside If. |
| **Indexed insertion** | `processInsertPath` + `processInsertIndex`; click gaps anywhere (including above/below whole If block); `insertCommandAtPoint`. |
| **If block UI** | Continuous blue shade when If header selected; compact single-spaced script (`.process-script-area`). |
| **If Modify** | Updates selected If when header/line selected + builder valid (uses `selectedProcessCommandPath`, not panel-only). |
| **If field drop** | Green + white boxes accept Fields drag without pre-click; value box `<<token>>`. |
| **Show script text** | Document (+reset), Form, stored record (`edit` cmd), URL — `processScript.ts` + `jsonToXml` edit. |
| **Form ↔ Process** | Yellow banner + `ProcessConnectionDialog` only (not right-column Properties). |

### Key files

| Area | Path |
|------|------|
| Process editor | `ProcessEditor.tsx` |
| Script + gaps | `SkipScriptView.tsx`, `lib/skipScript.ts`, `lib/processInsert.ts` |
| Builders | `IfStatementBuilder.tsx`, `SetStatementBuilder.tsx`, `ShowStatementBuilder.tsx`, `lib/statementBuilders.ts` |
| Store | `projectStore.ts` — `processInsertPoint`, `processStatementPanel`, `setSelectedProcessCommandPath` |
| Palette | `ProcessStatementsPalette.tsx`, `processStatements.ts` |
| Specs | `Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_*.md` |

---

## Next (recommended order)

1. **Send** property panel — Email tab per `DESIGNER_PROCESS_STATEMENTS_SEND.md` (mirror Show/If pattern). **No email provider needed for UI.**
2. **Append / Get / ForEach / Delete / Comment** — one at a time.
3. Edit → Connect / Disconnect Pre/Post-Process menu.
4. Records / RecordSet in Fields panel when Process active (backlog §1).

### Send runtime (later — not blocking Designer UI)

- Primary provider research: **Resend** (dev); **Amazon SES** at scale.
- Stub today: `runtimeEngine.mjs` `case "send"` is no-op.
- Adapter sketch: `emailService.mjs` + document HTML from `documentRenderer.mjs`.

---

## Architecture reminders

- Statement properties live in **Process window**, not right-column Properties.
- JSON is internal only — users see script lines + builders.
- Shared primitives: `FieldDropInputs`, `statementBuilders.ts`, `SkipScriptView` insertion gaps.

---

## Quick verify

- [ ] Open process → insertion arrow at **top** (`root`, index 0).
- [ ] Click gap **above** or **below** shaded If block → insert at root before/after If.
- [ ] If header selected → **Modify** updates condition text in script.
- [ ] Show: all four tabs Add/Modify; script lines match spec.
- [ ] `npm run build` in `designer-web/` passes.

*Last updated: July 9, 2026 (afternoon) — Show panel, indexed insertion, If script UX fixes.*
