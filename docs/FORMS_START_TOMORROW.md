# Forms track — start here (next session)

**Branch:** `cursor/forms-canvas-wysiwyg`  
**Run:** `cd ~/Projects/AI-Tawala/designer-web && npm run dev` → http://localhost:5173  
**Specs:** `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_*.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`

---

## Done on this branch (July 7, 2026)

- [x] MDI shell, docked Items palette (File Uploader greyed)
- [x] Shared **Formatting Palette** row 2 — B/I/U, font, color, align, indent (table/fx shell only)
- [x] **Heading** canvas WYSIWYG (`HeadingCanvasRow`)
- [x] **Text** canvas WYSIWYG (`TextCanvasRow`)
- [x] **FIB** canvas WYSIWYG (`FibCanvasRow`) — blanks, validation dialog, unique alternate labels, XML `<validator>`
- [x] **MCQ** canvas WYSIWYG (`McqCanvasRow`) — borderless question/choices, Enter adds choice, blank prune on exit, property strip
- [x] **Hidden Field** canvas row (`HiddenFieldCanvasRow`) — FIELD badge, Name: editor, Field1/2… auto-name, uniqueness validation
- [x] **Page Break** canvas row (`BreakCanvasRow`) — BREAK badge, hatched bar

---

## Tomorrow — priority order

### 1. Skip Instructions (last Items palette entry)

- [ ] Canvas row: fixed **SKIP** badge (dark red italic), peach body, **Edit** link
- [ ] Summary text on canvas: *(No instructions…)* / *(May skip to: T3)* / *(Skips to End of Form)*
- [ ] **Edit Skip Instructions** dialog — If / SkipTo / Set / Comment (subset of Process editor)
- [ ] Spec: `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` Skip section
- [ ] Replace JSON textarea in Properties panel with canvas hint (like other migrated items)

### 2. Formatting Palette — table tools

- [ ] Wire Insert Table, Delete Table, row/column split (#11–13 in toolbar spec)
- [ ] Set `cursorInTable` from active editor; enable rules already in `formattingPaletteContext.ts`
- [ ] Spec: `DESIGNER_FORM_FORMAT_TOOLBAR.md`

### 3. Formatting Palette — fx + Insert → Function

- [ ] Wire **fx** button and Insert menu Function picker for Text (and Document) body
- [ ] Spec: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`

### 4. MCQ — dynamic choice source

- [ ] **Configure Function** dialog when Choice source = “from stored data” (Display text, Value, Sort by, conditions)
- [ ] Enable **Edit** link; export dynamic `<data-provider>` XML (partial support exists in `mcToXml.mjs`)
- [ ] Reference: SportsDashboards divisions MCQ

### 5. FIB — Fields panel drop map

- [ ] Fine-grained drop targets: question vs blank vs capture label (was blocked until FIB WYSIWYG; now unblocked)
- [ ] Revisit `FibCanvasRow` + Fields panel drag behavior

### 6. Rich text → XML export parity

- [ ] Heading / Text / FIB prompt / MCQ question: canvas HTML formatting is not fully exported to legacy XML yet (MCQ question stripped to plain text today)
- [ ] Decide per-run vs strip vs font XML mapping

### 7. Polish & docs

- [ ] Palette polish: Default Font/Size clear, real icons for Items palette buttons
- [ ] Update `docs/CHAT_HANDOFF_FORMS.md` to match this branch (still lists FIB/MCQ as not done)
- [ ] Quick verify checklist at end of session

---

## Deferred / other tracks (not tomorrow unless you reopen)

| Item | Notes |
|------|--------|
| **File Uploader** | Palette button always disabled until implemented |
| **Per-item Properties popups** (D-Form-items) | Panel hints until full migration |
| **Move Up / Down** | Processes track — `CHAT_HANDOFF_PROCESSES.md` |
| **Insertion-point arrow** | Processes track |
| **8080 runtime parity** | Parked |
| **website-mock/** | Parked |

---

## Quick smoke test (2 min)

1. Insert Heading, Text, FIB, MCQ — edit on canvas; palette B/I/U on Text/FIB question/MCQ question.
2. FIB: duplicate alternate label → visible revert in property strip.
3. MCQ: Enter adds choice; blur prunes trailing blank.
4. Hidden Field: inserts as **FIELD** with `Field1`; duplicate name shows error.
5. Page Break: **BREAK** + hatched bar only.
6. `npm run build` in `designer-web/` → clean.

---

*Written end of July 7, 2026 session.*
