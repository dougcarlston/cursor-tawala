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
| Palette commands: B/I/U, font, color, reset, indent, alignment | **Done** on Form Text canvas + Document `RichTextEditor` |
| Form **Text** canvas-inline (`TextCanvasRow`) drives palette | **Done** |
| Form **Heading** greys entire palette | **Done** |

### Not done — integration gaps

| Priority | Item | Notes |
|----------|------|-------|
| **High** | **Document editor** full parity — RTF body, tables, functions | `DocumentEditor.tsx` is MVP (simple HTML / JSON fallback) |
| **High** | **Lift `RichTextEditor` mini-toolbar** — palette is sole format UI | Embedded B/I/U/size strip still in `RichTextEditor.tsx` |
| **High** | Wire palette **table tools** (#11–13) for Text + Document | Shell enabled; commands not implemented |
| **High** | Wire palette **fx** + Insert Function for Text + Document | Shell enabled; picker not implemented |
| Medium | **Default Font / Default Size** clear behavior on palette dropdowns | Spec: grey reset until mixed formatting |
| Medium | Palette **font face/size reflect cursor** (sync dropdown labels to selection) | Legacy idle-handler behavior |
| Medium | Document **Fields drag** into body (partial — RichTextEditor accepts drops when focused) | Full Document MDI parity in spec |
| Medium | Space **above Project Explorer** for future row-1 icon/tools palette | Owner note: left indent reserves this |
| Deferred | Per-window Design/Preview + selected item state (MDI Pass 2) | Global store today |
| Deferred | Windows menu, layout persistence | MDI Pass 2 |

### Sibling tracks (do not duplicate here)

| Track | Handoff | Focus |
|-------|---------|-------|
| Forms | [`CHAT_HANDOFF_FORMS.md`](CHAT_HANDOFF_FORMS.md) | FIB/MCQ canvas rows, remaining form items |
| Processes | [`CHAT_HANDOFF_PROCESSES.md`](CHAT_HANDOFF_PROCESSES.md) | Insertion arrow, Move Up/Down, Form↔Process UI |

Use **this** handoff when the task touches **Documents** or **shared palette** behavior across Form Text and Document body.

---

## What's next (recommended order)

1. **Remove `RichTextEditor` embedded toolbar** — Document body (and any remaining Properties rich surfaces) use palette only.
2. **Document editor WYSIWYG** — single contentEditable body matching legacy Document MDI; register `formattingKind="document"`.
3. **Table insert/delete/row-column** on palette — shared `paletteCommands` for Text + Document.
4. **fx / Insert Function** — shared picker wired to palette button #14.
5. Palette state sync (font/size at caret, reset greyed on fresh doc).
6. Optional: row-1 tools palette shell above Explorer (legacy icon bar) — coordinate with palette indent math in `FormattingPalette.tsx`.

---

## Key files

| Area | Path |
|------|------|
| Formatting Palette | `FormattingPalette.tsx`, `lib/formattingPaletteContext.ts`, `lib/paletteCommands.ts` |
| Document editor | `DocumentEditor.tsx`, `RichTextEditor.tsx` |
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
- [ ] `RichTextEditor` surfaces have no duplicate mini-toolbar (after lift task).
- [ ] `npm run build` in `designer-web/` → clean.

*Last updated: July 2026 — palette + Text canvas done; Document full parity is primary gap.*
