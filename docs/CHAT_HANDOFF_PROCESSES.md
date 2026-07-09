# Chat handoff — Processes only (browser Designer)

**Skinny handoff** for **Process** authoring in `designer-web/`. Full history: [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md). Forms canvas work: [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md).

**Suggested chat title:** `Designer — Process editor & statements`

---

## 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — Processes only (designer-web/)
Goal: Process editor parity — statement property panels (If/Set first), Edit→Connect/Disconnect, Records in Fields
Read first: docs/CHAT_HANDOFF_PROCESSES.md, Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_IF.md, DESIGNER_BACKLOG_ARCHITECTURE.md §3
Constraints: designer-web/ only; do not mix 8080 CSS or website-mock; no commit unless I ask
```

---

## Run locally

```bash
cd ~/Projects/AI-Tawala/designer-web && npm run dev
```

| URL | Role |
|-----|------|
| http://localhost:5173 | Designer UI |
| http://localhost:3001 | Dev API |

---

## Current status (July 2026)

**Active track:** Browser Designer — **Processes** slice.  
**Parked:** 8080 runtime parity, `website-mock/`.

### Done — shell & navigation (shared with Forms)

| Area | Status |
|------|--------|
| MDI Pass 1 — Process windows open/focus/cascade like Forms | **Done** |
| **Process Statements palette** replaces Items when Process window active | **Done** |
| Explorer shows linked Pre/Post processes under each form (icons, names) | **Done** (Phase 1) |
| Fields Phase 2 — drag/double-click `<<name>>` into process JSON textarea | **Done** |
| Deploy pipeline serializes `preProcess` / `process` on forms | **Done** |

### Done — Process editor shell (July 8, 2026)

| Area | Status |
|------|--------|
| **Insertion-point arrow** — blue ▶ in script pane; click gaps / `( )` interiors | **Done** (`SkipScriptView`, `processInsertPath` in store) |
| **Move Up / Down** — reorder statements within a block; Alt+↑/↓ | **Done** (`moveSelectedProcessCommand`) |
| **Yellow connection banner** + **Connect Process** dialog (attach Pre/Post, disconnect each role) | **Done** (`ProcessEditor.tsx`, `ProcessConnectionDialog.tsx`) |
| **Form ↔ Process linking removed from right-column Properties** (July 9 architecture) | **Done** — linkage only via Process window banner; `FormProperties.tsx` keeps Starting Point only |
| Structured **script pane** (pseudo-code lines) vs flat command list | **Done** (`processScript.ts`); JSON textarea for non-If/Set lines |
| **If / Set property panels** (shared with Skip Instructions) | **Done** (July 9) — `IfStatementBuilder`, `SetStatementBuilder`; palette opens panel; **Modify** when script line selected |

### Not done — Process authoring (this track)

| Priority | Item | Spec / notes |
|----------|------|--------------|
| High | **Show / Send / Append / Get / …** property panels | One type at a time per `DESIGNER_PROCESS_STATEMENTS_*.md` |
| Medium | Edit → Connect / Disconnect Pre/Post-Process menu actions | `DESIGNER_MENU_SPEC.md` |
| Medium | Statement-type configure dialogs (If, Show, Send, Append, Get, …) | `DESIGNER_PROCESS_STATEMENTS_*.md` |
| Medium | Records / RecordSet context in Fields panel when editing processes | Backlog §1 |
| Deferred | Per-statement Properties popups | §5 Properties strategy |

### Architecture (July 9, 2026)

- **Process statement properties** (If, Set, Show, …) live in the **Process MDI window** (statement builders / script pane), not the right-column Properties panel.
- **Form ↔ Process linkage** (Pre- or Post-process attach/detach) is **only** via the yellow **“Click here to change”** banner on the active Process window → `ProcessConnectionDialog`.
- Right column when a Process window is active: minimal placeholder + **Fields** palette; no process-linking dropdowns.
- **Form Properties** (when a Form window is active): form-level settings only (e.g. Starting Point); no Pre/Post dropdowns.

### Forms dependency (parallel track — do not mix in Process chats)

Serious DirtBowl-scale **form** authoring (FIB/MCQ canvas rows) continues in [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md). **Insertion-point** and **Move Up/Down** prerequisites are **done** for Processes (July 8, 2026); the two tracks can proceed in parallel without blocking each other.

---

## What's next (recommended order)

1. **Show statement** property panel (Document / Form / URL tabs per `DESIGNER_PROCESS_STATEMENTS_SHOW.md`).
2. **Send / Append / Get / ForEach / Delete / Comment** — one at a time.
3. Edit → **Connect / Disconnect** Pre/Post-Process menu actions.
4. **Records / RecordSet** nodes in Fields panel when a Process window is active (backlog §1).
5. Shared gaps: nested If insertion; dialog/panel re-open state; Cut/Copy/Paste/Undo.

---

## Key files

| Area | Path |
|------|------|
| Process editor | `designer-web/src/components/ProcessEditor.tsx` |
| Process script / move | `lib/processScript.ts`, `lib/skipInsertPath.ts` (`do` branch) |
| If / Set builders | `IfStatementBuilder.tsx`, `SetStatementBuilder.tsx`, `lib/statementBuilders.ts` |
| Statements palette | `ProcessStatementsPalette.tsx`, `processStatements.ts` |
| Explorer / links | `ProjectExplorer.tsx`, `lib/projectModel.ts` (`linkedProcessesForForm`, `formLinksForProcess`) |
| Form ↔ Process attach | `ProcessConnectionDialog.tsx`, `linkProcessToForm` / `unlinkProcessFromForm` in `projectStore.ts` |
| MDI shell | `components/mdi/`, `store/projectStore.ts` |
| Fields drop | `FieldsPalette.tsx`, `fieldInsertion.ts` |
| Specs | `Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_*.md`, `DESIGNER_MENU_SPEC.md` § Explorer |

---

## Constraints

- **Browser Designer only** — no Tomcat/CSS, no `website-mock/`.
- **Do not refactor Forms canvas items** in this chat unless a shared primitive (insertion arrow, Move Up/Down) requires it.
- Process JSON editing today is **textarea MVP** — improve incrementally, don't big-bang rewrite unless planned.
- **No commit or push** unless owner explicitly asks.

---

## Quick verify

- [ ] Single-click process in Explorer → `Process - Name` window opens.
- [ ] Active Process window → middle column shows **Processes/Statements** palette (not Items).
- [ ] Blue insertion arrow visible; click moves insert point; palette insert lands at arrow.
- [ ] **If** / **Set** palette buttons highlight and open shared property panel (not instant template insert).
- [ ] If builder: condition rows, ALL/ANY, Otherwise, **Add ↓**; click existing If line → **Modify ↓**.
- [ ] Set builder: field + value + arithmetic checkbox; click existing Set line → **Modify ↓**.
- [ ] Skip Instructions dialog still works (same If/Set builders).
- [ ] Yellow banner shows form link (or “Not connected…”); banner opens dialog to attach Pre/Post or disconnect each role independently.
- [ ] Active Process window → right column shows placeholder (no Pre/Post dropdowns); Form window → Form Properties shows Starting Point only.
- [ ] Fields double-click inserts `<<name>>` into focused process JSON field.
- [ ] `npm run build` in `designer-web/` → clean.

*Last updated: July 9, 2026 — Process properties in statement window; Form↔Process linkage via banner dialog only (not right-column Properties).*
