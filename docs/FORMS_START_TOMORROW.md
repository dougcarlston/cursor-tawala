# Forms track — start here (next session)

**Branch:** `cursor/forms-canvas-wysiwyg`  
**Run:** `cd ~/Projects/AI-Tawala/designer-web && npm run dev` → [http://localhost:5173](http://localhost:5173)  
**Specs:** `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_*.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`

---

## Done on this branch (July 7–8, 2026)

- [x] MDI shell, docked Items palette (File Uploader greyed)
- [x] Shared **Formatting Palette** row 2 — B/I/U, font, color, align, indent (table/fx shell only)
- [x] **Heading** canvas WYSIWYG (`HeadingCanvasRow`)
- [x] **Text** canvas WYSIWYG (`TextCanvasRow`)
- [x] **FIB** canvas WYSIWYG (`FibCanvasRow`) — blanks, validation dialog, unique alternate labels, XML `<validator>`
- [x] **MCQ** canvas WYSIWYG (`McqCanvasRow`) — borderless question/choices, Enter adds choice, blank prune on exit, property strip
- [x] **Hidden Field** canvas row (`HiddenFieldCanvasRow`) — FIELD badge, Name: editor, Field1/2… auto-name, uniqueness validation
- [x] **Page Break** canvas row (`BreakCanvasRow`) — BREAK badge, hatched bar
- [x] **Cursor-driven form insertion** (`FormInsertionPoint`, `insertBeforeIndex` in `projectStore`) — blue arrow between rows, palette inserts at point, Alt+↑/↓ reorder
- [x] **Skip Instructions** (`SkipCanvasRow`, `SkipInstructionsDialog`, `SkipScriptView`, `skipScript`/`skipSummary`) — SKIP badge, summary, Edit dialog (If/SkipTo/Set/Comment); properties panel canvas hint
- [x] **Skip dialog Set + Comment builders** (July 8 PM) — Set field/expression panel, Comment text panel

### Known issues (July 8)

| Issue | Notes |
| ----- | ----- |
| Dev server restart → blank tab | HMR/full reload can leave a white page until manual refresh |
| Empty MDI until form clicked | Canvas area blank until a form (or other node) is opened from Explorer |
| Skip modal overlay | Dialog positioning/overlay quirks under investigation |
| Skip script insertion point | Otherwise/then `( )` interior click fixed July 8 (skip-block-interior); If→Set wipe fixed July 8 PM; remaining: nested If, dialog re-open restore |
| **Fields panel → Text body drag-drop** | ~~User test July 8 PM~~ **Fixed July 8 PM commit** — blur on Fields-panel interaction was collapsing the contenteditable before drop; `retainEditorFocusOnBlur` keeps editor mounted. |
| **Delete form item — not discoverable** | ~~User test July 8 PM~~ **Fixed July 8 PM commit** — Design tab toolbar **×** next to ↑/↓; red **×** on selected Text/Heading rows; border click selects without focusing editor so **Del** deletes. |

### Stability fixes (July 8)

- [x] **FormEditor** — Rules of Hooks: all `useEffect` hooks run before the `form not found` guard
- [x] **ErrorBoundary** — top-level wrapper in `main.tsx`; recoverable error UI instead of white screen

---



## Tomorrow — priority order

**Skip Instructions** — substantially done on branch. Set + Comment builders added July 8 PM. Open dialog bugs: nested If insertion; Modify on existing script line (click line to edit, legacy Add/Modify). Spec gap table still open (`DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md`).

### 0. User-testing fixes (July 8 PM) — bump before table/fx polish

- [x] **Fields panel drag-drop → Text (verify Heading/FIB/MCQ too)** — fixed: `retainEditorFocusOnBlur` in `fieldInsertion.ts`; blur no longer collapses editor during Fields-panel drag/double-click.
- [x] **Delete selected form item — discoverability** — Design tab **×** button, per-row **×** on Text/Heading, border-click select-without-edit for **Del**.

### 1. Formatting Palette — table tools

- [ ] Wire Insert Table, Delete Table, row/column split (#11–13 in toolbar spec)
- [ ] Set `cursorInTable` from active editor; enable rules already in `formattingPaletteContext.ts`
- [ ] Spec: `DESIGNER_FORM_FORMAT_TOOLBAR.md`



### 2. Formatting Palette — fx + Insert → Function

- [ ] Wire **fx** button and Insert menu Function picker for Text (and Document) body
- [ ] Spec: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`



### 3. MCQ — dynamic choice source

- [ ] **Configure Function** dialog when Choice source = “from stored data” (Display text, Value, Sort by, conditions)
- [ ] Enable **Edit** link; export dynamic `<data-provider>` XML (partial support exists in `mcToXml.mjs`)
- [ ] Reference: SportsDashboards divisions MCQ



### 4. FIB — Fields panel drop map

- [ ] Fine-grained drop targets: question vs blank vs capture label (was blocked until FIB WYSIWYG; now unblocked)
- [ ] Revisit `FibCanvasRow` + Fields panel drag behavior (depends on §0 field-drop fix if basic question drop is still broken)



### 5. Rich text → XML export parity

- [ ] Heading / Text / FIB prompt / MCQ question: canvas HTML formatting is not fully exported to legacy XML yet (MCQ question stripped to plain text today)
- [ ] Decide per-run vs strip vs font XML mapping



### 6. Polish & docs

- [ ] Palette polish: Default Font/Size clear, real icons for Items palette buttons
- [ ] Update `docs/CHAT_HANDOFF_FORMS.md` to match this branch (still lists FIB/MCQ as not done)
- [ ] Quick verify checklist at end of session

---



## Deferred / other tracks (not tomorrow unless you reopen)


| Item                                          | Notes                                            |
| --------------------------------------------- | ------------------------------------------------ |
| **File Uploader**                             | Palette button always disabled until implemented |
| **Per-item Properties popups** (D-Form-items) | Panel hints until full migration                 |
| **Move Up / Down**                            | Form canvas: Alt+↑/↓ done; Process editor — `CHAT_HANDOFF_PROCESSES.md` |
| **Insertion-point arrow**                     | Form canvas done; Process editor still open      |
| **8080 runtime parity**                       | Parked                                           |
| **website-mock/**                             | Parked                                           |




---



## Quick smoke test (2 min)

1. Insert Heading, Text, FIB, MCQ — edit on canvas; palette B/I/U on Text/FIB question/MCQ question.
2. FIB: duplicate alternate label → visible revert in property strip.
3. MCQ: Enter adds choice; blur prunes trailing blank.
4. Hidden Field: inserts as **FIELD** with `Field1`; duplicate name shows error.
5. Page Break: **BREAK** + hatched bar only.
6. Skip Instructions: **SKIP** row, summary, **Edit** opens dialog.
7. Blue insertion arrow between rows; palette insert lands at arrow; Alt+↑/↓ reorders.
8. **Fields → Text:** click T5 body, drag `Form 1:Name` (or variable) from Fields — expect `<<Form 1:Name>>` at caret.
9. **Delete item:** select row, click **×** in Design tab toolbar or row chrome, or click row border and press **Del**.
10. `npm run build` in `designer-web/` → clean.

---

## July 8 PM session (commit)

**Skip** — done for now (Set + Comment builders, If→Set wipe fix). **Items palette** — complete except File Uploader (greyed). **§0 bugs fixed this commit:** Fields→Text/Heading drag-drop + double-click insert; delete discoverability (toolbar ×, row ×, border-select + Del). **Next:** Formatting Palette table/fx tools; Explorer icon toolbar.

---

*Updated July 8, 2026 (PM session commit) — §0 field-drop + delete fixes; Skip parked; next = table/fx + Explorer toolbar.*