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

### File / Save (held — fix later)

- **Save / Save As ignore last-loaded file name** — After **Open…** / load, Save and Save As should default to that file’s name (and Chromium quiet-Save should keep targeting it). **Untitled** (or `Untitled.json`) only for **New Project**. Today suggested name comes from `project.name` only, so Save As has no memory of the path/name last loaded. **Bug; hold until later.**

- **Many `.json` files greyed out in Open / Save As pickers** — Native Chromium picker marks lots of valid project JSON as the wrong type (greyed), yet they can still be clicked and their names used to overwrite/save. Likely the File System Access `accept: { "application/json": [".json"] }` filter (macOS UTI/MIME mismatch — e.g. files typed as `text/plain`). Should treat normal `.json` project files as selectable without looking “invalid.” **Bug; hold until later.**

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

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only while editing? **UX bug; deferred.** (Idle Design correctly keeps literal `_` — Batch 2 hold-list Jul 18; boxes only in Preview/Deploy.)

- **Design-mode checkboxes and radios change state** on the canvas. **UX bug; deferred.**

### Hold-list (Jul 18 gated pass)

| Batch | Item | Status |
|-------|------|--------|
| 1 | Form DnD vs text selection (#1 + #10) | **Done** — badge-only reorder (`formItemReorder.ts`); Alt Label `select()` on focus |
| 2 | Design FIB idle keeps `_` (#3) | **Done** — idle paints prompt HTML with underscores |
| 3 | Preview FIB (#4 + #9) | **Done** — `fibPrompt` + runtime; leftAlign keeps interstitial text order |
| 4 | FIB insert highlight (#2) | **Done** — placeholder selected; trailing `_` not |
| 5 | Text indent face/size (#7, #8) | **Done** — `margin-left` indent (no `execCommand("indent")`) |
| 6 | Tables multi-select + overflow (#5, #6) | **Done** — cell selection center; Text wrap `max-height` + scroll |

### Functions (final Designer run-through)

- **RESPONSE TOTALS → Include only the records where: `<<field>>` is not blank** — Owner Jul 17 spotted **inappropriate list behavior** when that Where mode is used (exact wrong result TBD on retest). **Do not dig now** — park for final Designer run-through. See also `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § RESPONSE TOTALS.

### Edit / Undo (main toolbar smoke Jul 18)

- **Undo/Redo stack is per-item only** — Undo (icon bar, Edit → Undo, ⌘Z/Ctrl+Z) uses the browser’s `document.execCommand("undo")`, whose history lives on the **focused contenteditable**. Clicking out of one Form item into another blurs/unmounts that editor, so Chromium discards its undo stack; the next item starts empty. **No cross-item / project-level undo.** Owner Jul 18: acceptable for now if recorded. **Future:** store-level undo history (text commits, item move/delete) that survives focus changes — larger feature, not a toolbar wiring fix. (Legacy Jan 2011 build often had Undo/Redo always greyed anyway.)

- **Paste required a one-time browser permission prompt** — expected, not a bug: toolbar/menu Paste reads the system clipboard via `navigator.clipboard` (`clipboard-read`), which browsers gate; Cut/Copy write the current selection and are allowed silently. After **Allow**, Paste and ⌘V work without re-prompting. (Fixed Jul 18: Paste now uses the Clipboard API since `execCommand("paste")` is blocked from button clicks.)

### Full build / TypeScript (`npm run build`) — inventory Jul 17

`cd designer-web && npm run build` (`tsc -b`) fails with **14 errors in 6 files**. Day-to-day Design via Vite still works; this blocks a clean production build. **Inventory only — do not dig mid-feature;** fix in a short dedicated green-build pass (or near final Designer run-through).

| Category | Count | Files | Notes |
|----------|-------|--------|--------|
| **Real live-code** | 1 | `src/components/RichTextEditor.tsx` | Calls `reflowPlacedLinesBelow` but does not import it (function exists in `documentCanvas.ts`). Likely missing import / rename slip. |
| **Unused locals** | 2 | `src/lib/paletteCommands.ts` | Dead `CARET_ZWSP`; unused `root` param — safe cleanup. |
| **Test typing only** | ~10 | `documentCanvas.align.dom.test.ts`, `shellCommands.download.dom.test.ts` | `Element` vs `HTMLElement` / form mocks — tests, not product logic. |
| **Shell typing** | 1 | `src/lib/shellCommands.ts` | Keyboard pick missing `shiftKey` in a type. |
| **Table cast** | 1 | `src/lib/documentCanvas.ts` | `Element` → `HTMLTableElement` without cast. |

**Verdict:** Small set — one real missing import, rest mostly casts/unused. Not a multi-day detritus purge.

### Skip Instructions (parked — fix when Skip is reopened)

- **Skip dialog re-open does not restore insertion point.** **Real bug; parked with Skip.** (Smoke: Skip dialog itself felt solid July 10.)

- **Skip modal overlay quirks** (positioning / full-screen dim). Soft / polish; Close-only dismiss is intentional. **Parked with Skip.**

### Process / runtime navigation

- **Process statement: edit vs insert mode (legacy arrow)** — **Fixed Jul 16 (v2):** Selecting a script row enters **edit mode** — solid blue highlight and left **▶** on that statement; insert gap arrow is hidden. Clicking an insert gap / setting insert point clears selection (**insert mode** — ▶ on the separator between lines). Insert hit overlays no longer steal statement clicks (narrower hit band; rows above hits). Smoke: click “Show Form …” → arrow on statement + Modify; click between lines → arrow on gap, no statement highlight.

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

**Passed / fixed Jul 16 (code + owner smoke where noted):**

| # | Item | Notes |
|---|------|--------|
| 1 | **Horizontal FIB layout** | **Fixed:** Preview/Deploy blank widths from underscore `blank.length` (`size` + `ch`); stacked rows inline (not flex-stretch). Theme CSS in `themes/index.mjs`. |
| 2 | **FIB Required per blank** | **Owner OK.** First click selects item only; second click places caret under the click (`FibCanvasRow`). |
| 3 | **Required / empty submit** | **Owner OK.** `blank.required` blocks empty Submit; validators on Node + Java CSS (`.validateError` in `docker/tomcat/css/project/default.css`). |
| 4 | **MQL Where clause** | **Owner OK.** |
| 5 | **Print / Excel export links** | **Owner OK on Deploy.** Configure toggles persist; Preview emits Print / CSV export (`itemizationPreview.mjs`). |
| 7 | **Java Deploy (8080) parity** | **Owner OK** for Signup Form+MQL path (Tomcat up, `dev`/`dev`). Also: Document MQL field tokens + no nested font/division; FIB multi-blank soft-rows; Design B/I/U→Deploy (`fibRichPromptToXml`). **Re-smoke:** Redeploy after `fcebcfa` to confirm latest FIB formatting on 8080. |
| 8 | **Dev API on 3001 dies** | **Mitigated:** `designer-web/scripts/ensure-dev-api.sh` + README; check `/api/health` when Preview/Deploy fails. |
| 9 | **Session junk rows** | **Mitigated (ops):** cleared stale Java submissions / Preview sessions once; use `?reset=1` or fresh Deploy uniqueId if lists look wrong. |
| — | **FIB Design formatting → Deploy** | **Fixed Jul 16:** freeform prompts mirror B/I/U / face / size / color into Java font XML (`fibToXml` + tests). |

**Still open (leave MQL when these are done or explicitly deferred):**

| # | Item | Notes |
|---|------|--------|
| 6 | **`baseball` theme CSS** | Stub / fallback still weak vs full theme; optional — not blocking Signup default theme. |
| — | **Process caret + row highlight** | **Fixed Jul 16 (v2)** — legacy edit vs insert arrow (see Active bugs). |

**Then:** MQL/SignupSheet core is done enough to leave — continue owner review **#9** other untested functions that already emit Document XML (skip four deferred stubs: Categorizer / Roster / Link / PayPal).

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
