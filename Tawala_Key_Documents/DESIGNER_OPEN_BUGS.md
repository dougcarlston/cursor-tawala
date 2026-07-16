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
| 15 | Reset | **Removed** from palette July 10 |
| 16 | Save / reload | Pass |
| 17 | `npm run build` | Optional — see owner note |

---

## Active / deferred bugs

### Palette

- **Reset Formatting broken** — **Removed July 10** (control deleted from Formatting Palette; was always greyed and unreliable).

- **Font/size dropdown on mixed-format runs** — may still show Default Font/Size when the selection mixes faces/sizes. Single-run caret sync and Document typing persistence verified OK July 10 (after hard refresh; earlier “regression” was a stale tab).

### Document canvas & palette (smoke test July 10)

Owner could not fully test overnight (hooks-order / “too many hooks” error); retested on tip `b48656b` after hard refresh. Font face/size for plain typing OK; remaining Document issues:

- **Multiline / drag-select highlighting buggy** — **Verified July 10:** cross-block drag-select works; multi-line align applies to all intersecting placed lines.

- **Fields and variables font face/size** — **Face/size match verified July 10** after fix. Placement still broken (below).

- **Field token drop/placement on Document** — **Snap-to-line + move-after-drop verified July 10:** drop joins the nearest line; drag relocates an existing token; mid-line drop, typing after token, and joint highlight/reformat with text all work.

- **Font Color selector** — **Verified July 10:** highlight recolors only the selection; **A** applies current color; **▾** chooses a new color; icon swatch tracks color; new typing keeps the color.

- **Alignment tools** — **Verified July 10:** single- and multi-line left/center/right/justify to margins; justify wraps at content width; last line left-aligned.

- **Font size / line packing** — **Verified July 10:** selection-only size; one enlarged word can push lines down; pull-up only when the line box shrinks; reset to default works (mixed highlight shows Mixed, not false default).

- **Can still overwrite existing text without deleting it** — typewriter Return now **pushes** lines below instead of deleting/stacking (**verified July 10**). Owner recheck July 10 afternoon: **no residual overwrite** observed.

- **Empty placed-line husks after delete** — select-all + Delete left an invisible `.doc-placed-text` snap target. **Fixed July 10:** delete/cut/backspace-to-empty prunes husks (Return blank lines kept until deleted).

- **Arrow keys leave Document line / spawn nearby lines** — **Verified July 10:** confine arrows/Home/End in placed lines; Up/Down move between lines (including within soft-wrapped blocks); click on same row snaps into existing line.

### Form canvas (UX backlog)

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only. **UX bug; deferred.**

- **Design-mode checkboxes and radios change state** on the canvas. **UX bug; deferred.**

### Skip Instructions (parked — fix when Skip is reopened)

- **Skip dialog re-open does not restore insertion point.** **Real bug; parked with Skip.** (Smoke: Skip dialog itself felt solid July 10.)

- **Skip modal overlay quirks** (positioning / full-screen dim). Soft / polish; Close-only dismiss is intentional. **Parked with Skip.**

### Process / runtime navigation

- **Process statement: block highlight and text caret conflict** — Selecting a Process script row (e.g. “Show Form Form 1”) shows both the light-blue row selection chrome **and** a text insertion caret in the label at once. They fight visually; only one selection mode should win (block select vs inline edit). **Owner Jul 16** (screenshot). **Open.**

- **AdminDash start point → empty Thank you; Coach Contact hard to reach after deploy** — Process / navigation start-point from DirtBowl stress pass. **Real bug;** separate track from Document/palette work.

### Fixed Jul 15 (Signup Sheet smoke — held bugs)

- **MQL Configure column toolbar blank + no tips** — footer +/−/↑/↓ used SVG + native `title` (invisible under modal CSS; tips clipped / absent on disabled). Fixed: glyph buttons + upward `win-tip` tooltips.
- **Signup Sheet submit showed “Registration step complete”** — `renderSubmitAck` DirtBowl copy. Fixed: generic “Thank you” / “Back to {form}” unless Registration.
- **Return to Form 1 kept prior values** — session formFields not cleared. Fixed: append record + `clearFormAnswers` on complete; back link `?fresh=1`.
- **Preview/Deploy lost Form Item spacing** — runtime CSS had no inter-item margins. Fixed: `.tawala-form > …` spacing in `runtime.mjs` page shell (Design canvas unchanged).
- **Signup MQL headers with empty thin rows** — `topLabels` Preview inputs were `readonly` (empty submits → blank records). Fixed: live `blankInput`; skip all-empty `appendFormRecord`.
- **Document injected `Continue →`** — Show Document then Show Form used a separate Continue page. Fixed: stack blank Form under Document on the same response (no Continue artifact).
- **Document MQL config with `<<Field>>` broke parse** — span regex used `[^>]*`. Fixed: quote-aware `htmlSpanReplace.mjs`.
- **Email/Phone validators ignored in Node Preview** — only Registration had checks. Fixed: `fibBlankValidation.mjs` on submit when blank.validation is set.
- **Horizontal FIB layout missing on default/baseball** — vertical spacing only. Fixed: flex rules in `BASE_FORM_CSS`.
- **MQL columns blank despite stored signups** — blanks named `a`/`b`/`c`/`d` with alternateLabel `First`/`Last`/…; MQL asked for `<<Form 1:First>>`. Fixed: `blankAliasesFromForm` at render + append. Also force-restarted API (stale process still injected Continue →).
- **Show Form after Document left prior answers** — `clearFormAnswers` cleared `a`/`b` but not alternateLabel keys (`First`/`Last`), so `blankInput` refilled. Fixed: clear aliases + Form stacks blank.
- **Email/Phone accepted garbage on Signup** — Node Preview skipped validators unless `blank.validation` set. Fixed: infer Email/Tel from name/alternateLabel; reject `..` emails.

**Smoke (SignupSheet Preview/Deploy):** Fill Form 1 → Submit → Document table shows the new row (plus prior rows) **above** a **blank** Form 1; no `Continue →`. Bad email/phone shows error and stays on Form. Soft-refresh 5173 after API restart.

### Pickup Jul 16 — MULTIPLE QUESTION LIST / SignupSheet (owner resume)

**Passed Jul 15 (owner):** MQL table shows stored rows; Document stacks above blank Form 1 (no Continue →); Delete on form items asks “Are you sure?” (blur/Del mishap mitigated).

**Still fix / re-smoke before leaving MQL:**

| # | Item | Notes |
|---|------|--------|
| 1 | **Horizontal FIB layout** | **Owner Jul 16:** Design aligns left+right edges of two FIB rows via underscore lengths; Preview should size blanks from underscore counts the same way. **Fix in progress:** `size`+`ch` width from `blank.length`; stacked rows use inline flow (not flex stretch). |
| 2 | **FIB Required per blank** | **Owner Jul 16:** Not dual-Required — first click parked caret at end of last blank (Phone) while user thought Email was active. **Fix:** first click selects item only; second click places caret under the click. |
| 3 | **Required / empty submit** | **Owner Jul 16: confirmed fails.** **Fix:** `blank.required` blocks empty Submit (`"{label} is required."`). |
| 4 | **MQL Where clause** | **Owner Jul 16: OK.** |
| 5 | **Print / Excel export links** | Configure has print/export toggles; Node Preview table does not emit those links yet. |
| 6 | **`baseball` theme CSS** | Missing file → `resolveTheme` falls back to default; optional real baseball stylesheet. |
| 7 | **Java Deploy (8080) parity** | Tonight’s smoke was Node runtime on 5173/3001; when Tomcat is up, re-check MQL + validators on 8080. |
| 8 | **Dev API on 3001 dies** | Vite 5173 often stays up while Express dies → Preview/Deploy fail; restart detached API if `/api/health` ≠ 200. |
| 9 | **Session junk rows** | Earlier empty/bad signups may still be in Deploy session; `?reset=1` or new Deploy uniqueId for a clean list. |

**Then:** finish MQL checklist above → continue **#9** other untested functions that already emit Document XML (skip four deferred stubs: Categorizer / Roster / Link / PayPal).

- **Form item Delete had no confirm** — Del/× removed canvas items immediately (easy after FIB Required focus). Fixed Jul 15: `confirmAndDeleteFormItem` + strip/button Del ignore.

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
