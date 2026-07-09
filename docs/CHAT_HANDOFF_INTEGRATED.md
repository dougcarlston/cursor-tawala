# Chat handoff — Integrated (Forms + Processes + Documents)

**Skinny handoff** for cross-cutting Designer work: **Documents**, shared **Formatting Palette**, and shell integration across all three MDI child types. Narrower tracks: [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md), [`CHAT_HANDOFF_PROCESSES.md`](CHAT_HANDOFF_PROCESSES.md).

**Suggested chat title:** `Designer — Documents & shared palette`

---

## 5-line paste opener

```
Project: AI-Tawala (~/Projects/AI-Tawala)
Track: Browser Designer — integrated Forms/Processes/Documents (designer-web/)
Goal: Document editor + full shared Formatting Palette parity; unify rich-text across Form Text and Documents
Read first: docs/CHAT_HANDOFF_INTEGRATED.md, Tawala_Key_Documents/DESIGNER_DOCUMENT_EDITOR.md, DESIGNER_FORM_FORMAT_TOOLBAR.md
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

**Active track:** Browser Designer — **integration** slice (Documents + shared palette + shell).  
**Parked:** 8080 runtime parity, `website-mock/`.

### Done — shared infrastructure

| Area | Status |
|------|--------|
| MDI Pass 1 — Form / Process / Document windows | **Done** |
| Docked palette **context swap**: Items (Form) ↔ Statements (Process) ↔ **none** (Document) | **Done** |
| **Formatting Palette** row 2 — visible for Form + Document MDI; indented to canvas left edge | **Done** |
| Palette focus context (`heading` / `text` / `document` / `none`) + enable rules | **Done** |
| Palette commands: B/I/U, font, color, reset, indent, alignment | **Done** on Form Text canvas + Document |
| Form **Text** canvas-inline (`TextCanvasRow`) drives palette | **Done** |
| Form **Heading** greys entire palette | **Done** |
| **Lift `RichTextEditor` mini-toolbar** — row 2 is sole format UI | **Done** (July 2026) |
| **Document editor** — single full-bleed WYSIWYG canvas (no Preview split) | **Done** (July 2026) |
| Document HTML persistence (tables/formatting survive save) | **Done** (July 2026) |
| Palette **table tools** (#11–13): insert dialog, delete, row/column submenu | **Done** (July 2026) |
| **Table handles overlay** — move, resize, column/row dividers, float L/block/R | **Done** (July 2026) |
| Document **click-to-place text** + **field drop at coordinates** | **Done** (July 2026; polish ongoing) |
| Palette **font/size reflect cursor** (dropdown labels sync to selection) | **Done** (July 2026; not perfect) |

### Not done — integration gaps

| Priority | Item | Notes |
|----------|------|-------|
| **High** | **fx** + Insert Function for Text + Document (#14) | Shell enabled; picker not implemented |
| **High** | Document **RTF/export parity** — rich structure in deploy XML | HTML WYSIWYG today; `jsonToXml` still paragraph-limited |
| Medium | **Default Font / Default Size** grey reset on fresh doc | Spec: reset greyed until mixed formatting |
| Medium | **Drag-to-reposition** placed text/field blocks | Tables have ✥ handle; `doc-placed-text` blocks do not |
| Medium | Field tokens as non-overlapping inline widgets | Plain `<<name>>` text today; can stack/overlap |
| Medium | Palette font/size sync edge cases | Stable snapshot added; mixed-format runs may still show Default |
| Medium | Space **above Project Explorer** for future row-1 icon/tools palette | Owner note: left indent reserves this |
| Deferred | Per-window Design/Preview + selected item state (MDI Pass 2) | Global store today |
| Deferred | Windows menu, layout persistence | MDI Pass 2 |

### Session summary — July 9, 2026 (Documents + palette integration)

**Removed** embedded `RichTextEditor` mini-toolbar; Formatting Palette (row 2) is the only format UI for Document + Properties rich text.

**Document window:** Replaced split editor/Preview with one white WYSIWYG canvas; content stored as HTML string (tables/formatting persist).

**Tables:** Legacy Insert Table dialog (width inches, rows, columns); palette delete table + row/column submenu; `TableHandlesOverlay` for move (relative offset), edge/corner resize, column/row dividers, float left/block/right.

**Free placement:** Click empty canvas → absolutely positioned text block; field drops use viewport coordinates (not caret sniffing near floats).

**Palette polish:** Table row/column menu visibility CSS fix; font face/size dropdowns sync to caret (cached snapshot for `useSyncExternalStore`); hooks-order and infinite-loop fixes.

**New files:** `InsertTableDialog.tsx`, `TableHandlesOverlay.tsx`, `lib/tableLayout.ts`, `lib/documentCanvas.ts`.

### Sibling tracks (do not duplicate here)

| Track | Handoff | Focus |
|-------|---------|-------|
| Forms | [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md) | FIB/MCQ canvas rows, remaining form items |
| Processes | [`CHAT_HANDOFF_PROCESSES.md`](CHAT_HANDOFF_PROCESSES.md) | Insertion arrow, Move Up/Down, Form↔Process UI |

Use **this** handoff when the task touches **Documents** or **shared palette** behavior across Form Text and Document body.

---

## What's next (recommended order)

1. **fx / Insert Function** — shared picker wired to palette button #14.
2. Document field tokens — styled/non-editable spans; drag-to-reposition `doc-placed-text` blocks.
3. Palette reset greyed on fresh doc; font/size edge cases (mixed runs).
4. `jsonToXml` / deploy path for document HTML tables and placed blocks.
5. Optional: row-1 tools palette shell above Explorer (legacy icon bar).

---

## Key files

| Area | Path |
|------|------|
| Formatting Palette | `FormattingPalette.tsx`, `lib/formattingPaletteContext.ts`, `lib/paletteCommands.ts` |
| Document editor | `DocumentEditor.tsx`, `RichTextEditor.tsx`, `lib/documentCanvas.ts` |
| Tables | `InsertTableDialog.tsx`, `TableHandlesOverlay.tsx`, `lib/tableLayout.ts` |
| Form Text (palette consumer) | `TextCanvasRow.tsx` |
| App shell / palette visibility | `App.tsx` |
| MDI | `components/mdi/CanvasWindow.tsx` |
| Specs | `DESIGNER_DOCUMENT_EDITOR.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`, `DESIGNER_MENU_SPEC.md` § toolbars |

---

## Palette rules (shared Form Text + Document)

| Context | Palette |
|---------|---------|
| Heading focused | Entire palette **greyed** |
| Text item body | **Live** (full) |
| Document body | **Live** (full) |
| FIB/MCQ rich region (future) | B/I/U only |
| Form Preview tab | **Greyed** |
| Table delete / row-column (#12–13) | Enabled only when `cursorInTable` |
| **fx** (#14) | Text item or Document only |

---

## Constraints

- **Browser Designer only** — no Tomcat/CSS, no `website-mock/`.
- **One shared `FormattingPalette` component** — do not fork Form vs Document toolbars.
- Document window → **no Items/Statements middle column** (legacy empty middle palette).
- **No commit or push** unless owner explicitly asks.

---

## Quick verify

- [ ] Open Document window → no middle Items column; Formatting Palette visible.
- [ ] Focus Document body → palette live; B/I/U/font apply.
- [ ] Focus Form Text on canvas → same palette controls work.
- [ ] Focus Heading → palette greyed.
- [ ] `RichTextEditor` surfaces have no duplicate mini-toolbar.
- [ ] Insert Table dialog → table move/resize handles; field drop at click point.
- [ ] `npm run build` in `designer-web/` → clean.

*Last updated: July 9, 2026 — Document WYSIWYG canvas, tables, placement, palette table tools + font sync.*
