# Designer open bugs

Verified or owner-reported **broken / incorrect behavior** in the browser Designer (`designer-web/`), after pruning chat inventories (July 10, 2026).

Deferred = we chose not to fix yet (or greyed the control on purpose).  
Unfinished features and polish live in **`DESIGNER_OPEN_TODOS.md`**.

Skipped chats (not Designer track): Website library mock; 8080 templates/Docker/Tomcat CSS; Findme Assistant.

---

## Palette

- **Reset Formatting broken** — Document and Form rich text. Control is **always greyed** on purpose until fixed. **Deferred.** (Sources: Document palette & typewriter; Document WYSIWYG & palette)

- **Font/size dropdown on mixed-format runs** — may still show Default Font/Size when the selection mixes faces/sizes. Caret sync for single runs works; mixed-run behavior not fully confirmed. (Source: Document WYSIWYG & palette)

## Form canvas

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only. **Deferred** / backlog. (Source: Designer Sign-up DirtBowl)

- **Design-mode checkboxes and radios change state** on the canvas. **Deferred** as general Designer UX. (Source: Designer Sign-up DirtBowl)

- **Properties: Individual Items stay fully expanded** when not selected (should compress to a single line). **Deferred** as general Designer UX. (Source: Designer Sign-up DirtBowl)

## Skip Instructions

- **Skip dialog re-open does not restore insertion point.** Still open when Skip was parked. (Source: Forms canvas & Skip)

- **Skip modal overlay quirks** (positioning / full-screen dim). Under investigation; Close-only dismiss is intentional. (Source: Forms canvas & Skip)

## Process / runtime navigation

- **AdminDash start point → empty Thank you; Coach Contact hard to reach after deploy** — Process / navigation start-point issue from DirtBowl stress pass. Still open. (Source: Designer Sign-up DirtBowl)

## Not product bugs (kept for awareness)

- **Dev server restart blanks Designer tab** — HMR/port kill; hard refresh. Dev tooling only. (Source: Forms canvas & Skip)
- **Empty MDI until a form is clicked in Explorer** — by design after new/import (`openWindows: []`). (Source: Forms canvas & Skip)

---

## Pruned as fixed or superseded

| Item | Why removed |
|------|-------------|
| Formatting Palette table tools not wired | Landed in later Document/palette work |
| Formatting Palette fx / Insert → Function not wired | Landed in Document palette & typewriter (`3f49995`) |
| Process UX bugs (If/Show panels, connection overlay, etc.) | Fixed in Process editor / Show & If / statement-panel chats |
| Yellow Connect banner / connection dialog | Landed after MDI chat; menu parity still open → moved to TODOs |
| Document HTML→XML “paragraph-only” from early Document chat | Partially replaced by `documentHtmlToXml.mjs`; remaining gap → TODOs |
| Field-token drag polish | Unconfirmed; moved to TODOs as polish |
