# Designer open bugs

Confirmed or likely **broken / incorrect behavior** in the browser Designer (`designer-web/`). Reviewed with owner July 10, 2026.

Unfinished features and polish: **`DESIGNER_OPEN_TODOS.md`**.  
Skipped chats (not Designer track): Website library mock; 8080 templates/Docker/Tomcat CSS; Findme Assistant.

---

## Smoke test July 10 (`b48656b`) — pass summary

| # | Area | Result |
|---|------|--------|
| 1–4 | Shell / MDI / palettes | Pass |
| 5–6 | Form insert/edit, field drag | Pass |
| 7 | Skip dialog | Pass (solid) |
| 8 | Process If/Set/Show | Pass |
| 9 | Send/Get panels | Layout OK; email validation incomplete; mail backend unimplemented → **TODOs** |
| 10–12 | Document typewriter Enter, font stick, blank click | Pass |
| 13 | Drag-select highlight | **Pass** (cross-block + multi-line align, July 10 tip) |
| 14 | fx popup | Opens; catalog/config not fully implemented → **TODOs** |
| 15 | Reset | Intentionally greyed; may **remove** if unfixable |
| 16 | Save / reload | Pass |
| 17 | `npm run build` | Optional — see owner note |

---

## Active / deferred bugs

### Palette

- **Reset Formatting broken** — Document and Form rich text. Always greyed on purpose (was breaking other features). **Deferred;** owner may **delete the control** if we cannot fix it cleanly.

- **Font/size dropdown on mixed-format runs** — may still show Default Font/Size when the selection mixes faces/sizes. Single-run caret sync and Document typing persistence verified OK July 10 (after hard refresh; earlier “regression” was a stale tab).

### Document canvas & palette (smoke test July 10)

Owner could not fully test overnight (hooks-order / “too many hooks” error); retested on tip `b48656b` after hard refresh. Font face/size for plain typing OK; remaining Document issues:

- **Multiline / drag-select highlighting buggy** — **Verified July 10:** cross-block drag-select works; multi-line align applies to all intersecting placed lines.

- **Fields and variables font face/size** — **Face/size match verified July 10** after fix. Placement still broken (below).

- **Field token drop/placement on Document** — **Snap-to-line + move-after-drop verified July 10:** drop joins the nearest line; drag relocates an existing token; mid-line drop, typing after token, and joint highlight/reformat with text all work.

- **Font Color selector** — **Verified July 10:** highlight recolors only the selection; **A** applies current color; **▾** chooses a new color; icon swatch tracks color; new typing keeps the color.

- **Alignment tools** — **Verified July 10:** single- and multi-line left/center/right/justify to margins; justify wraps at content width; last line left-aligned.

- **Font size / line packing** — **Verified July 10:** selection-only size; one enlarged word can push lines down; pull-up only when the line box shrinks; reset to default works (mixed highlight shows Mixed, not false default).

- **Can still overwrite existing text without deleting it** — typewriter Return now **pushes** lines below instead of deleting/stacking (**verified July 10**). Other overlap cases may remain.

- **Arrow keys leave Document line / spawn nearby lines** — **Verified July 10:** confine arrows/Home/End in placed lines; Up/Down move between lines (including within soft-wrapped blocks); click on same row snaps into existing line.

### Form canvas (UX backlog)

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only. **UX bug; deferred.**

- **Design-mode checkboxes and radios change state** on the canvas. **UX bug; deferred.**

### Skip Instructions (parked — fix when Skip is reopened)

- **Skip dialog re-open does not restore insertion point.** **Real bug; parked with Skip.** (Smoke: Skip dialog itself felt solid July 10.)

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
