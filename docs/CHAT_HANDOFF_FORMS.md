# Chat handoff — Forms only (browser Designer)

**Skinny handoff** for finishing the **Forms** portion of `designer-web/`. Full history: [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md). Status dashboard: [`ROADMAP.md`](ROADMAP.md) Phase 4.

**Suggested chat title:** `Designer — Forms canvas WYSIWYG`

---

## 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — Forms only (designer-web/)
Goal: Complete Forms canvas WYSIWYG + Formatting Palette for form items (FIB, MCQ, remaining items)
Read first: docs/CHAT_HANDOFF_FORMS.md, Tawala_Key_Documents/DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md, DESIGNER_FORM_FORMAT_TOOLBAR.md
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

**Active track:** Browser Designer — **Forms** slice only.  
**Parked:** 8080 runtime parity, `website-mock/`.

### Done — Forms shell & canvas-inline items

| Area | Status |
|------|--------|
| MDI Pass 1 (multi-window, cascade, empty start) | **Done** |
| Items palette docked beside Explorer; Processes palette swaps on Process window | **Done** |
| Items palette restyled (blue header, icon+label buttons; **File Uploader always greyed**) | **Done** |
| **Heading** canvas WYSIWYG (`HeadingCanvasRow`) — collapse on blur, Main/Sub per-run, badge label | **Done** |
| **Text** canvas WYSIWYG (`TextCanvasRow`) — no collapse, placeholder on insert, palette live | **Done** |
| **Formatting Palette** row 2 — shell + enable rules; indented to canvas left edge | **Done** |
| Palette commands wired: B/I/U, font face/size, color, reset, indent/outdent, alignment | **Done** |
| Palette shell only: Insert/Delete Table, row/column, **fx** | **Not wired** |
| Properties panel | **Stays** for FIB, MCQ, Hidden Field, Page Break, Skip Instructions, structured Text |

### Not done — remaining Forms work

| Priority | Item | Spec / notes |
|----------|------|--------------|
| **Next** | **FIB canvas row** (`FibCanvasRow`) — prompt + blanks WYSIWYG on canvas | `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` FIB section |
| **Next** | **MCQ canvas row** — question + choices on canvas | Same spec, MCQ section |
| High | Wire palette **table tools** (#11–13) + `cursorInTable` | `DESIGNER_FORM_FORMAT_TOOLBAR.md` |
| High | Wire palette **fx** + Insert → Function | `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` |
| Medium | Hidden Field, Page Break, Skip Instructions canvas or popup UX | `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` |
| Medium | Palette polish (Default Font/Size clear, real icons for Items buttons) | Owner screenshots in `.cursor/.../assets/` |
| Deferred | Per-item **Properties popups** (D-Form-items) — panel stays until migration complete | `DESIGNER_BACKLOG_ARCHITECTURE.md` §5 |
| Blocked | FIB fine-grained Fields drop map (question vs blank vs capture label) | Blocked until FIB is canvas WYSIWYG |

### Mention only — shared prerequisites (owned by Processes track)

These live in [`CHAT_HANDOFF_PROCESSES.md`](CHAT_HANDOFF_PROCESSES.md); Forms work does **not** require them first:

- **Insertion-point arrow** (where next insert lands)
- **Move Up / Down** (form items, process statements, document blocks)

---

## What's next (recommended order)

1. **FIB canvas row** — canvas-inline Q-badge, prompt, underscore blanks, property strip (Alternate Label, Height, Required, Validation).
2. **MCQ canvas row** — Q-badge, question, inline choices (Enter adds choice).
3. **Palette table tools** — insert/delete table, row/column split; enable when `cursorInTable`.
4. **Palette fx** — function picker for Text item body.
5. Remaining item types (Hidden Field, Page Break, Skip Instructions) — one at a time per D-Form-items strategy.

---

## Key files

| Area | Path |
|------|------|
| Form canvas | `designer-web/src/components/FormEditor.tsx` |
| Heading / Text rows | `HeadingCanvasRow.tsx`, `TextCanvasRow.tsx` |
| Items palette | `FormItemsPalette.tsx`, `FormInsertMenu.tsx` |
| Formatting Palette | `FormattingPalette.tsx`, `lib/formattingPaletteContext.ts`, `lib/paletteCommands.ts` |
| Properties (non-canvas items) | `FormItemProperties.tsx`, `FibFieldPreview.tsx` |
| State | `store/projectStore.ts`, `types/tawala.ts` |
| Styles | `styles.css` |
| Specs | `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_*.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md` |

---

## Constraints

- **Browser Designer only** — no Tomcat/CSS, no `website-mock/`.
- **Three-bucket discipline** — do not merge Designer, 8080, and website mock in one change.
- **Properties panel stays** until each item type migrates to canvas-inline or popup.
- **File Uploader** — show in Items palette but **always disabled** until implemented.
- **No commit or push** unless owner explicitly asks.
- Preview/deploy local only (5173 / 3001 / 8080) — not www.tawala.com.

---

## Quick verify

- [ ] Open form → Items palette shows icon buttons; File Uploader greyed.
- [ ] Insert Heading → palette **greyed** while editing heading text.
- [ ] Insert Text → palette **live**; B/I/U/font/color/align apply on canvas.
- [ ] FIB/MCQ still edit in Properties panel (until canvas rows land).
- [ ] `npm run build` in `designer-web/` → clean.

*Last updated: July 2026 — after Text canvas row, palette commands, Items palette restyle.*
