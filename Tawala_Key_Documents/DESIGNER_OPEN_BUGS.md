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

### Document functions (Jul 20)

- **Two identical MQL on Document 2 — different Deploy results** — One block showed `<>` (no table); the other rendered a table but the multi-select MCQ column (“All possibilities”) was blank. **Root cause (1):** formatting toolbar / styled wrapper `<span style="font-size…">` around a function token exported as nested `<font><font><itemization-table>…` — Java Font FACTORY drops the inner table. **Fixed:** `documentHtmlToXml.mjs` detects font-wrapped display components and skips the outer wrap. **Root cause (2):** multi-select MCQ values sometimes stored as one comma-separated string; Java `displayLabelsOnly` did not split them. **Fixed:** `DisplayMultipleChoiceLabel.java` splits comma-separated selections; Preview MQL uses `formatMcqCellValue` in `itemizationPreview.mjs`. **Owner OK Jul 20** after Tomcat WAR rebuild + Redeploy (re-entered one MQL manually).

- **Configure Function field target: Fields palette double-click does not replace** — **Fixed Jul 20.** Two causes: (1) Configure inputs live outside `.mdi-window.active`, so Fields double-click was refused as a “stale MDI target”; (2) column Contents / expression boxes appended at caret instead of replacing. Fix: `configureDialog` targets stay usable outside MDI + replace whole value on drop/double-click. **Smoke:** focus Contents or Where field → double-click a Fields leaf → prior value replaced (not appended / not no-op).

### File / Save (held — fix later)

- **Save / Save As ignore last-loaded file name** — After **Open…** / load, Save and Save As should default to that file’s name (and Chromium quiet-Save should keep targeting it). **Untitled** (or `Untitled.json`) only for **New Project**. **Partial fix (Jul 19):** Save / Save As / Open now sync JSON + Project Explorer root from the **file leaf name**, so Save As no longer leaves the tree on **Untitled**. Remaining gap: suggested name still comes from `project.name` (now usually aligned with the file); Chromium quiet-Save handle memory was already separate.

- **Many `.json` files greyed out in Open / Save As pickers** — **Fixed Jul 20:** Chromium + macOS often typed fresh saves as `text/plain`, so strict `application/json` filters greyed valid `.json` on the **first** Open after Save; second Open worked after Launch Services caught up. Open picker now shows all files (validate `.json` after pick); Save picker accepts `application/json`, `text/json`, and `text/plain`. Quiet-Save handle is released during Open so the file we just saved is not greyed as “already open.”

### Palette

- **Reset Formatting broken** — **Removed July 10** (control deleted from Formatting Palette; was always greyed and unreliable).

- **Font/size dropdown on mixed-format runs** — may still show Default Font/Size when the selection mixes faces/sizes. Single-run caret sync and Document typing persistence verified OK July 10 (after hard refresh; earlier “regression” was a stale tab).

### Document canvas & palette (smoke test July 10)

Owner could not fully test overnight (hooks-order / “too many hooks” error); retested on tip `b48656b` after hard refresh. Font face/size for plain typing OK; remaining Document issues:

- **Backspace deletes function chip then caret vanishes / arrows die** — **Fixed Jul 20.** After Backspace removed a trailing `<<…>>` chip (esp. chip-only placed line), focus often sat after `contenteditable=false` with no ZWSP landing (or selection cleared) → no blinking caret; ArrowLeft appeared dead. Fix: explicit chip Backspace/Delete keeps a live caret landing; `focusPlacedBlock` / `focusPlacedBlockEnd` ensure chip ZWSP pads. **Smoke:** `DESIGNER_DOCUMENT_EDITOR.md` § 22e.

- **Multiline / drag-select highlighting buggy** — **Verified July 10** for plain text: cross-block drag-select works; multi-line align applies to all intersecting placed lines. **Reopened Jul 19 (owner):** drag highlighting still does **not** work properly when the selection includes **Field tokens** and/or **function labels** (`<<…>>`). **Jul 20:** folded into Document caret-model epic (live caret + drag of highlighted content) — see `DESIGNER_OPEN_TODOS.md` § Document caret model.

- **Cannot drag Function label onto same line as text** — **Jul 19 (owner):** Function tokens (`<<…>>`) cannot be dragged onto an existing text line (stay separate / won’t join the line). Field drop/snap-to-line was verified Jul 10; function labels need the same mid-line join behavior. **Jul 20:** same epic as caret model / chip traversal.

- **Cannot rename Form / Process / Document in Explorer** — **Fixed Jul 19:** rename was only a 500ms long-press on an already-selected row (easy to miss, and HTML5 drag canceled it). Now: **click a selected** Form/Process/Document name to edit, or press **F2**. Enter commits, Escape cancels.

- **Cannot rename Process in Explorer (Forms/Documents OK)** — **Fixed Jul 20:** a linked process appears twice (under the form and under **Processes**). Inline rename matched `kind + name`, so both rows entered edit mode; the second input stole focus and blur-cancelled the first. Now matches the unique render `key` so only one `RenameInput` mounts. **Smoke:** select a Post/Pre (or Processes-folder leaf) → click again or F2 → type new name → Enter; Explorer + open window title update.

- **Document rename does not update Process Show / Send / Append** — **Fixed Jul 19:** renaming a Document in Explorer now cascades into Process command refs (`documentRenameCascade.ts` via `renameDocument`), including nested If / ForEach. **Owner Passed Jul 20.**

- **Form rename resizes Document / Form function chips** — **Fixed Jul 20.** Renaming a Form remounts its MDI window; open Documents’ `ResizeObserver` packed layout and **committed**, and commit stripped chip `font-size: 12pt` so chips inherited a larger parent line. Fix: (1) never strip font-size from `.function-token` / `.field-token`; (2) only persist Document reflow when that surface’s **width** changed. Also added **form rename cascade** (`formRenameCascade.ts`) so `Form:Field` refs / function chips update like legacy. **Smoke:** open Document + Form with MQL chips → rename another Form in Explorer → chip sizes and Document layout stay put; rename a referenced Form → chip labels update without growing.

- **Process Connect dialog blocked multi-form Pre/Post** — **Fixed Jul 19:** legacy allows one process on many forms (checklist; Potluck `Show Results`). Dialog was a single-form dropdown/Attach flow. Now Pre and Post **form checklists** (check several forms); banner uses plural “N Forms” when appropriate. **Also:** drag a Process onto a Form in Explorer attaches as **Post-process** when that form’s Post slot is empty (legacy drop).

- **Field rename does not update Functions / function labels** — **Fixed Jul 19:** renaming a Hidden Field, FIB blank alt label, or MCQ field name cascades into Document/Form Text function chips (`data-function-config` + visible `<<NAME(…)>>`), field tokens, and Process command field refs (`fieldRenameCascade.ts` via `updateFormItem`).

- **Function chips insert at random sizes / resize nearby text** — **Fixed Jul 19 (regression):** Form Text still used badge `10px` (Document already inherited). Insert also painted sticky size onto the parent line. Now Form Text chips inherit like Document; insert no longer resizes the paragraph; default insert leaves chip size unset so it matches surrounding text.

- **Ghost / deleted Document text still visible** — **Jul 19 false alarm (owner):** the unexpected question phrasing on Deploy came from **Form Item** MCQ text (Response Totals injects the Form question), not from deleted Document prose. Separately, Design still got a small hardening Jul 19 (discard orphan glyphs after delete; prune stacked duplicate placed lines; missing `reflowPlacedLinesBelow` import) — keep as regression prevention, not as confirmation of that screenshot.

- **Fields and variables font face/size** — **Face/size match verified July 10** after fix. Placement still broken (below).

- **Field token drop/placement on Document** — **Snap-to-line + move-after-drop verified July 10:** drop joins the nearest line; drag relocates an existing token; mid-line drop, typing after token, and joint highlight/reformat with text all work.

- **Font Color selector** — **Verified July 10:** highlight recolors only the selection; **A** applies current color; **▾** chooses a new color; icon swatch tracks color; new typing keeps the color.

- **Alignment tools** — **Verified July 10:** single- and multi-line left/center/right/justify to margins; justify wraps at content width; last line left-aligned.

- **Font size / line packing** — **Verified July 10:** selection-only size; one enlarged word can push lines down; pull-up only when the line box shrinks; reset to default works (mixed highlight shows Mixed, not false default).

- **Can still overwrite existing text without deleting it** — typewriter Return now **pushes** lines below instead of deleting/stacking (**verified July 10**). Owner recheck July 10 afternoon: **no residual overwrite** observed.

- **Empty placed-line husks after delete** — select-all + Delete left an invisible `.doc-placed-text` snap target. **Fixed July 10:** delete/cut/backspace-to-empty prunes husks (Return blank lines kept until deleted).

- **Arrow keys leave Document line / spawn nearby lines** — **Verified July 10:** confine arrows/Home/End in placed lines; Up/Down move between lines (including within soft-wrapped blocks); click on same row snaps into existing line.

### Form canvas (UX backlog)

- **Del/× on selected function chip deletes whole Text row** — **Fixed Jul 20:** highlighting a `<<…>>` function (or field) chip and pressing Del/Backspace, or clicking the row/toolbar **×**, used to remove the entire Text item. Now removes only the selected chip(s); whole-row delete still applies when no chip is highlighted.

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

- **Function Where on MCQ fields** — **Implemented Jul 19 (TODO #11).** Function Conditions switches to legacy `mcEquals` / `mcContains` / `mcIsBlank` / … when the left field is an MCQ (`onlyone` → MCOne vs MCMany). Value placeholder = choice letter. FIB Where unchanged. **Owner Passed Jul 20** (non-numeric Where + 2 numeric tests OK).

- **RESPONSE TOTALS multi-select undercount** — **Investigated Jul 19 (TODO #12): no code bug found.** Java + Preview tally both loop all `getValues` / array choices (same as Bar Graph). Added regression tests. **Owner Passed Jul 20** — Totals and Bar Graph both pick up all choices on the same multi-select MCQ.

- **RESPONSE TOTALS → Include only the records where: `<<field>>` is not blank** — Owner Jul 17 spotted **inappropriate list behavior** when that Where mode is used (exact wrong result TBD on retest). May be the same MCQ/`mcIsNotBlank` gap as above. **Park** with TODO #11.

### Edit / Undo (main toolbar smoke Jul 18)

- **Undo/Redo stack is per-item only** — Undo (icon bar, Edit → Undo, ⌘Z/Ctrl+Z) uses the browser’s `document.execCommand("undo")`, whose history lives on the **focused contenteditable**. Clicking out of one Form item into another blurs/unmounts that editor, so Chromium discards its undo stack; the next item starts empty. **No cross-item / project-level undo.** Owner Jul 18: acceptable for now if recorded. **Future:** store-level undo history (text commits, item move/delete) that survives focus changes — larger feature, not a toolbar wiring fix. (Legacy Jan 2011 build often had Undo/Redo always greyed anyway.)

- **Paste required a one-time browser permission prompt** — expected, not a bug: toolbar/menu Paste reads the system clipboard via `navigator.clipboard` (`clipboard-read`), which browsers gate; Cut/Copy write the current selection and are allowed silently. After **Allow**, Paste and ⌘V work without re-prompting. (Fixed Jul 18: Paste now uses the Clipboard API since `execCommand("paste")` is blocked from button clicks.)

### Full build / TypeScript (`npm run build`) — inventory Jul 17

`cd designer-web && npm run build` (`tsc -b`) fails with **14 errors in 6 files**. Day-to-day Design via Vite still works; this blocks a clean production build. **Inventory only — do not dig mid-feature;** fix in a short dedicated green-build pass (or near final Designer run-through).

| Category | Count | Files | Notes |
|----------|-------|--------|--------|
| **Real live-code** | 0 | — | Missing `reflowPlacedLinesBelow` import in `RichTextEditor.tsx` **fixed Jul 19**. |
| **Unused locals** | 2 | `src/lib/paletteCommands.ts` | Dead `CARET_ZWSP`; unused `root` param — safe cleanup. |
| **Test typing only** | ~10 | `documentCanvas.align.dom.test.ts`, `shellCommands.download.dom.test.ts` | `Element` vs `HTMLElement` / form mocks — tests, not product logic. |
| **Shell typing** | 1 | `src/lib/shellCommands.ts` | Keyboard pick missing `shiftKey` in a type. |
| **Table cast** | 1 | `src/lib/documentCanvas.ts` | `Element` → `HTMLTableElement` without cast. |

**Verdict:** Small set — one real missing import, rest mostly casts/unused. Not a multi-day detritus purge.

### Skip Instructions (parked — fix when Skip is reopened)

- **Skip dialog: click / edit / delete statement ignored** — **Fixed Jul 19:** Edit Skip Instructions had no select / Modify / line delete (toolbar Delete disabled; Add always appended). Now Process-parity: click a line to edit (Modify), × / toolbar Delete, ↑↓, insert at the arrow (not always append). Also: SkipTo no longer resets the dropdown to the first FIB on open (that made Skip-after-FIB1 look like a no-op).

- **Skip dialog re-open does not restore insertion point.** Soft leftover; session now also stores `insertIndex` + selection. Re-smoke when convenient.

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
| MDI windows slide under Fields (unreachable close/minimize) | **Fixed Jul 19** — drag/resize clamp keeps frame (and title-bar controls) inside `.mdi-surface` |

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
