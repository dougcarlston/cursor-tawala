# Designer open bugs

Confirmed or likely **broken / incorrect behavior** in the browser Designer (`designer-web/`). Reviewed with owner July 10, 2026.

Unfinished features and polish: **`DESIGNER_OPEN_TODOS.md`**.  
Skipped chats (not Designer track): Website library mock; 8080 templates/Docker/Tomcat CSS; Findme Assistant.

---

## Active / deferred bugs

### Palette

- **Reset Formatting broken** — Document and Form rich text. Control is **always greyed** on purpose until fixed. **Real bug; deferred.**

- **Font/size dropdown on mixed-format runs** — may still show Default Font/Size when the selection mixes faces/sizes. **Unconfirmed — verify once** in the app; demote to TODO or drop if OK.

### Form canvas (UX backlog)

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only. **UX bug; deferred.**

- **Design-mode checkboxes and radios change state** on the canvas. **UX bug; deferred.**

### Skip Instructions (parked — fix when Skip is reopened)

- **Skip dialog re-open does not restore insertion point.** **Real bug; parked with Skip.**

- **Skip modal overlay quirks** (positioning / full-screen dim). Soft / polish; Close-only dismiss is intentional. **Parked with Skip.**

### Process / runtime navigation

- **AdminDash start point → empty Thank you; Coach Contact hard to reach after deploy** — Process / navigation start-point from DirtBowl stress pass. **Real bug;** separate track from Document/palette work.

---

## Removed from bugs list (July 10 review)

| Item | Disposition |
|------|-------------|
| Properties: Individual Items stay fully expanded | Moved to **TODOs** (UX polish, not broken) |
| Dev server restart blanks Designer tab | Not a product bug (HMR); dropped |
| Empty MDI until form clicked in Explorer | By design; dropped |

---

## Earlier prune (fixed / superseded)

| Item | Why removed |
|------|-------------|
| Formatting Palette table tools not wired | Landed in later Document/palette work |
| Formatting Palette fx / Insert → Function not wired | Landed in Document palette & typewriter (`3f49995`) |
| Process UX bugs (If/Show panels, connection overlay, etc.) | Fixed in Process chats |
| Yellow Connect banner / connection dialog | Landed; menu parity → TODOs |
| Document HTML→XML “paragraph-only” | Partial fix; remainder → TODOs |
| Field-token drag polish | → TODOs |
