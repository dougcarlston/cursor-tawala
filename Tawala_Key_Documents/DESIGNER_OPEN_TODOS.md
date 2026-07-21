# Designer open TODOs

Unfinished features, polish, and out-of-scope runtime gaps — **not** confirmed product bugs. Split out from chat bug inventories (July 10, 2026). See also **`DESIGNER_OPEN_BUGS.md`**.

Items marked **Deferred** were consciously postponed. Verify in the app before scheduling; later commits may have partially addressed some.

---

## ☀️ Jul 20–21 week — green build, then `.tawala` → Designer Open

**Undo policy locked** (`DESIGNER_OPEN_BUGS.md`). Document caret epic mostly landed — park invent/chip-select polish unless blocking.

**Owner Jul 20:** Process **Send** works in Browser Designer on :8080 (self-send OK; recipient lists not yet smoked). Close the old “Send no-op / mail unimplemented” deferral for Java Deploy path.

**This week target:** **File → Open** (or Import) a general `.tawala` into Browser Designer → editable JSON project → Save/Deploy. Unlocks owner’s library of full projects (beyond New Project stubs). Groundwork: `TAWALA_XML_TO_JSON_MAPPING.md`, SignupSheets one-off `scripts/convert-signupsheets-xml-to-json.mjs`, `jsonToXml.mjs` export. DirtBowl/Potluck JSON stubs stay non-targets until import works.

**Suggested order:**
1. **Short `tsc` green-build** (clear ~14 errors) — **Done Jul 21** (`npm run build` green).
2. **Generalize `.tawala` → format 2.0 JSON** (CLI first, then Open/Import in Designer) — **next**.
3. **Owner smoke:** Open Potluck / DirtBowl / Sign-up Sheet `.tawala` → Explorer populated → round-trip Deploy.
4. **Park:** Deferred UX, 3-browser, look-and-feel until after import smoke.

### End of day Jul 20 — checkpoint (read this first tomorrow)

**Git:** `3fd215f` pushed earlier (Document highlight-move / chips / Skip gaps). Follow-up docs commit tonight for Deploy/Send/Undo policy + week plan. Branch: `cursor/forms-canvas-wysiwyg`.

**Shipped / Owner Passed today (Design canvas + Deploy):**
- Document highlight-drag: multi-line move, drag-into-blank, no mid-line skip, whole-line only (not double-click word), chip-only relocate, function→text join.
- Skip re-edit: insert gaps again (`showAllInsertionGaps`); palette tool clears wrong Modify.
- Undo: **closed policy** — best-effort CE only; renames/Moves/Skip·Process/Explorer out of scope.
- Legacy Deploy: Potluck + full DirtBowl `.tawala` **flawless** (MQL, themes, AdminDash-scale). AdminDash “bug” was **corrupted JSON** (bare `Show`, blank AdminDashboard).
- Process **Send** self-mail on :8080 **works**.

**Important distinctions:**
- **New Project** Sign-up/Potluck JSON = stubs. Healthy smoke = `node scripts/deploy-tawala-template.mjs "…"` or path to `.tawala`.
- Full DirtBowl source: `designer-web/public/samples/legacy/DirtBowl.tawala`. Do not trust `dirtbowl_definition_v3.json` / open JSON copies.
- How to deploy `.tawala`: Tomcat on 8080 → `node scripts/deploy-tawala-template.mjs --list` / `"Potluck"` / path.

**Still open / park:**
- Font/Size mixed-run honesty (owner smoking § 7c/7j); intermittent chip jump after partial Size (workaround: drag back).
- Chip-inclusive drag-select highlight; long nowrap chips + MDI chrome under Fields.
- Document invent-on-click / live-caret B leftovers.
- No general `.tawala` → Designer Open yet (this week’s product goal after green-build).

**Key files touched earlier today:** `documentCanvas.ts`, `RichTextEditor.tsx`, `functionTokens.ts`, `SkipInstructionsDialog.tsx`, `SkipScriptView.gaps.dom.test.ts`, `documentCanvas.multiMove.dom.test.ts`, `functionTokens.move.dom.test.ts`, `DESIGNER_DOCUMENT_EDITOR.md`, `DESIGNER_OPEN_BUGS.md`, `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md`.

---

## Document editor & export

- **Document HTML → XML export incomplete** — only some function types emit real XML; others become comments. Tables/placed text improved in `documentHtmlToXml.mjs` but export is still partial. (Sources: Document palette & typewriter; Document WYSIWYG & palette)
- **fx / Insert Function not fully implemented** — picker popup works (smoke July 10); many functions lack full configure UI and/or real XML export. (Source: Document palette & typewriter; smoke item 14)
- **Field-token drag polish** — optional leftover; not confirmed broken. (Source: Document palette & typewriter)
- **Default Font / Default Size greyed rules** — legacy Reset greyed rules obsolete; Reset control **removed** July 10.
- **Indent / Outdent** — **verified July 10:** Document placed lines shift by 36 pt steps from the left margin (width still to right margin); Form paragraphs use `margin-left`.

### Document line model / reflow (epic — owner Jul 10)

**Not** legacy parity: vintage Designer used a hard-coded ~7″ text box with horizontal scroll. Owner prefers **margin-based reflow** so SportsDashboards (and similar) can target mobile/tablet by constraining overall form/window size instead of hand-rebuilding every layout.

Planned pieces (after single-line margin align):

- Invisible **line slots** (height from font size) between left/right margins — **not built as a grid**; empty husks after delete were pruned July 10. Snap uses real `.doc-placed-text` lines. 
- **Field snap-to-line** on drop so tokens join that line’s content — **done July 10**  
- **Move field token** after drop — **done July 10**  
- **Wrap on type** at right margin; push following lines down/aside — **verified July 10**  
- **Wrap on resize** when the MDI/window narrows — **verified July 10**  
- **Multi-line highlight** then **multi-line align** — **verified July 10**  

Single-line left/center/right/justify to margins shipped first; selection-only size + grow/shrink packing also landed July 10. Wrap-on-type soft-wraps within the margin box (from line `left` to right margin) and packs lines below when height changes. ResizeObserver re-wraps/packs on editor width change. Drag-select can span placed lines; align applies to all intersecting lines.

### Document caret model (epic — owner Jul 20) — **do first**

Today’s Document canvas is still **absolute placed-line islands** (`.doc-placed-text` + invent anchors). Owner wants **native-document editing** on top of (or instead of) that model.

**Priority (owner Jul 20):** Land this epic **before** further chip drag-select / mid-line-join bugfixes — those may dissolve or change shape once the caret model is flow-like.

| # | Request | Today | Target |
|---|---------|--------|--------|
| **A** | **Backspace across lines / Returns** | Backspace is largely confined to the current placed line; empty-line / cross-Return delete is limited or blocked | One Backspace at a time can delete through Returns and prior lines until the Document is empty |
| **B** | **Click → live caret (not invent anchors)** | Free-space click invents a placed-text anchor (`✥` / new `.doc-placed-text`); drag of a highlight often fails across islands | Click places a blinking caret in the document flow; drag-move of any highlighted run (text + chips) works |
| **C** | **Arrow keys across Returns, functions, fields** | Up/Down / Left/Right often stop at line boundaries or `contenteditable=false` chips | Caret moves continuously across soft Returns, hard Returns, function placeholders, variables, and field tokens |

**Implication:** A–C are one **caret/selection model** change (flow document + traversable chips), not three unrelated tweaks. Deploy HTML→XML must keep sorting/spacing contracts when the DOM model changes.

**Suggested build order:** A (cross-line Backspace / merge) → C (arrows through Returns + chips) → B (live caret / reduce invent anchors; unlock highlight drag).

**Progress (Jul 20):**
- **A landed** — Backspace/Delete at a placed-line edge merges the same-column neighbor (Return undo); empty Return husks remove and land on the previous line; table cells and side-by-side columns stay isolated (`handleDocumentDeleteBoundary` + `documentCanvas.deleteBoundary.dom.test.ts`). Smoke: `DESIGNER_DOCUMENT_EDITOR.md` § 22c.
- **C landed** — ArrowLeft/Right at line edges move to the previous/next same-column line; Left/Right jump over field/function chips; Up/Down prefer same-column neighbors (`handlePlacedTextArrowKey` + `documentCanvas.arrowKeys.dom.test.ts`). Smoke: § 22d.
- **B in progress** — ✥ invent-move anchors **removed** (Jul 20); highlight + hand-cursor drag moves selected placed lines (middle-line fill **Owner Passed Jul 20**). Still open: less invent-on-click / live caret flow; chip-inclusive drag-select highlight quirks.
- **Function chip mid-line join** — **Owner Passed Jul 20** (§ 22h).

**Related open bugs (expect to re-smoke after epic):** multiline drag-select highlight when the run includes field/function chips; long nowrap chips + unreachable MDI minimize/close (`DESIGNER_OPEN_BUGS.md`).

## Form items & Fields

- **Move Up / Move Down** for form items and process statements — **Done Jul 12** (↑/↓ + Alt+arrows + select-then-drag reorder; compact lists, caret only while dragging). **Document blocks — owner smoke Jul 15: pass.**
- **FIB hint-text styling** (smaller italic secondary font for parentheticals). **Deferred** → `docs/DESIGNER_BACKLOG_FORMS_FIBS.md`. (Source: Designer Sign-up DirtBowl)
- **Heading per-run Main/Sub size spans stripped** on export/runtime (legacy single-`type` heading can’t express mixed sizes). **Deferred.** (Source: Designer MDI and Heading)
- **FIB fine-grained Fields drop map** (question vs blank vs capture label). **Deferred** / unfinished. (Sources: Designer MDI and Heading; Forms canvas & Skip)
- **Per-item Properties popups** not migrated — permanent Properties panel still used for non-canvas-inline items. **Deferred.** (Source: Designer MDI and Heading)
- **Properties: Individual Items stay fully expanded** when not selected (should compress to a single line). UX polish — **superseded July 10:** right-column Properties panel removed; Fields owns the column. Per-item Properties popups remain a separate deferred item.
- **File Uploader** — **Omitted from Items palette (owner Jul 17).** Never wired in 2011 reference build or browser Designer. Spec only: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`. Use **Insert → Image → From your PC…** / **From the Web…** for images.
- **Items palette icons** are Unicode/CSS placeholders, not legacy assets. **Deferred.** (Source: Designer Forms foundation)
- **MCQ dynamic choice source** (“from stored data” + Configure Function). **Deferred.** (Source: Forms canvas & Skip)
- **Rich text HTML → legacy XML export incomplete** for Heading/Text/FIB prompt/MCQ question formatting (MCQ question still stripped to plain text). Unfinished. (Source: Forms canvas & Skip)

## Skip Instructions

- **Nested If inside then/else `( )` not supported.** **Deferred** (Skip parked). (Source: Forms canvas & Skip)
- **Modify on existing Skip script line missing** (legacy Add/Modify). **Deferred.** (Source: Forms canvas & Skip)
- **Skip toolbar Cut/Copy/Paste/Undo stubs** — icons present, not functional. **Deferred.** Covered by Undo closed policy (Jul 20) — not a separate Undo epic. (Source: Forms canvas & Skip)

## Process editor

- **Edit → Connect / Disconnect Pre/Post-Process** menu actions missing — yellow banner + connection dialog work; menu parity does not. **Deferred.** (Sources: Designer MDI and Heading; Process editor If/Set)
- **Send does nothing at runtime** — **Superseded Jul 20.** Browser Designer → Deploy **Send** works on :8080 (owner: self-send OK). Preview still does not send mail. Recipient-list / bulk To not yet owner-smoked. Ops: `docs/EMAIL_DELIVERY_OPS.md`. (Was: `runtimeEngine.mjs` no-op — Java path is the real delivery.)
- **Get `where` filter not applied in browser preview.** Designer Where UI works; runtime filter not wired. **Deferred.** (Source: Process statement panels)
- **Append / Show document merge not in browser preview.** Designer panels work; runtime merge not in this track. **Deferred.** (Source: Process statement panels)
- **Fields palette: no Record List / RecordSet branch after Get** (legacy when Get selected). ForEach record branches landed; Get RecordSet left as polish. **Deferred.** (Source: Process statement panels)

## Shell / MDI / chrome

- **MDI Pass 2** — no Windows menu; Design/Preview and selected item still global across form windows; no layout persistence; no maximize/tile/snap. **Deferred.** (Sources: Designer MDI and Heading; Document WYSIWYG & palette)
- **Long nowrap function chips push MDI chrome off-reach** — see open bug (Jul 20); workarounds Cascade / hide panels. Related to panel docking + title-bar clamp.
- **Panel docking** — Items/Statements cannot be resized or moved independently of Project Explorer. **Deferred.** (Source: Designer MDI and Heading)
- **Main icon toolbar (“frequently used”)** — legacy `mainToolStrip` below the menu bar, above Project Explorer, left of the Formatting Palette. **Done Jul 12** (`MainIconToolbar` + `shellCommands.ts`; shared with File/Edit/Project menus). Spec: `DESIGNER_MENU_SPEC.md`. Remaining: full home-page control audit (Owner review queue #2).
- **Processes palette, Project Explorer chrome, canvas item windows** not restyled to legacy look-and-feel. **Deferred** (Items palette only was Choice A). (Source: Designer Forms foundation)

## Architectural / DirtBowl (not Designer UI bugs)

- **DirtBowl Preview vs Deploy data/seed mismatch** (local Preview defaults vs Java persisted admin/division data). **Deferred** as architectural distinction. (Source: Designer Sign-up DirtBowl)

---

## Owner review queue (July 12, 2026)

Tasks the owner set (or agreed to schedule). Keep on this list until reviewed and either done, deferred, or gated.

| # | Task | Notes / sequencing |
|---|------|-------------------|
| 1 | **Wire Main icon toolbar** (“frequently used” strip) | **Done Jul 12** — `MainIconToolbar` shares handlers with File/Edit via `shellCommands.ts`. |
| 2 | **Home-page control audit — menus, tabs, and toolbars** | **Jul 17:** File/Edit/Insert/View/Project/Windows/Help. **View toggles wired.** Format removed (palette); Tables skipped; Project Tabs/Styles wired; Page Header/Themes = 8080 stubs; Help → About stub. |
| 3 | **Review remaining gated items** (3-browser smoke; look-and-feel parity) | Still **gated** until Designer is basically finished — owner asked to keep them visible on the review queue; discuss before starting. **Do not start during #9 smoke.** |
| 4 | **MCQ dynamic choice source** (“from stored data” + Configure Function) | Priority for SignupSheets-class apps; still **Deferred** in Form items until scheduled. |
| — | **Design-canvas Style paint** | **Owner Jul 18:** **Text** Instructional/Error shown on Forms → Text (already implemented). **FIB/MCQ layout paint = won't do** — interferes with editing; Preview immediate. See `DESIGNER_FORM_FORMAT_TOOLBAR.md`. |
| 5 | **HTML→XML export for functions we already Configure** | **Mostly done Jul 13–16** — 13 of 17 emit real XML; 4 deferred stubs. Remaining work is **owner smoke**, not emit. See function status matrix in `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`. |
| 6 | **Move Up / Down** for form items (process statements if cheap) | **Done Jul 12** — Form + Process: arrows and drag-reorder. **Document blocks — owner smoke Jul 15: pass.** |
| 7 | **Sample / template review (first pass)** | **Done Jul 12** (owner). **Re-review after #9 and #10** — functions + Deploy must work before a second full pass. |
| 8 | **Other structured Form Text tables** (e.g. choice tally) | Same click-to-Configure / rich-edit path as MQL + correlation when a template needs them. Part of #9. |
| 9 | **Wire the rest of the functions** | **WHERE re-smoke complete Jul 19** (see function matrix). **Jul 20:** TODO #11 MCQ Where **Passed**; TODO #12 Totals vs Bar Graph multi-select **Passed**. Core Configure+Deploy for ladder done earlier Jul 19. |
| 10 | **Get Deploy working** | **Usable Jul 12–16.** **Jul 20:** Potluck + DirtBowl legacy `.tawala` Deploy **Passed**; Process **Send** (self) **Passed**. Next: general `.tawala` → Designer Open this week. |
| 11 | **Implement MCQ-aware Function Where** | **Done Jul 19** — `mcConditionOperators` + `FunctionConditionsEditor` field-kind switch; XML emits `mc*`. **Owner Passed Jul 20.** |
| 12 | **RESPONSE TOTALS multi-select undercount** | **Done Jul 19 (investigation)** — no Totals-specific bug; same tally as Bar Graph; regression tests added. **Owner Passed Jul 20** (side-by-side Totals vs Bar Graph on multi MCQ — both pick up all choices). |**Cleanup plan Jul 16:** Home-page menu audit (#2) and gated items (#3 / After Designer finished) stay **parked** until the remaining #9 smoke-needed functions above are cleared or explicitly deferred. Do not start menu look-and-feel or 3-browser smoke without owner discussion.

---

## After Designer is basically finished (gated)

Owner (July 12, 2026): park these until the browser Designer is considered **basically finished**. **Do not start any of these without prior discussion with the owner.**

**Jul 16 cleanup:** Explicitly deferred until after #9 remaining function smokes (and owner says Designer is basically finished).

1. **Big smoke test on three different browsers** — full walkthrough of Designer (and critical Preview/Deploy paths as agreed) on three browsers; capture browser-specific defects. *(Also listed in Owner review queue #3.)*
2. **Conform Look and Feel** of the Designer shell and its windows to the legacy Designer application **without breaking** underlying behavior already shipped (layout, chrome, typography/colors — visual parity pass only after functional freeze). *(Also listed in Owner review queue #3.)*
3. **Main Page menus and tabs — no duplicates; identical behavior** — audit every main menu and tab for duplicate entries; on selection, each must operate exactly the same as its counterpart (no divergent handlers or stale duplicates). **Owner Jul 12:** schedule **after** Main icon toolbar is wired (Owner review queue #1–2); toolbar duplicates File/Edit and is part of the same audit. **Jul 17:** View menu stubs restored (all five); **wire View chrome toggles after menu review completes** (Owner review queue #2). Also parks Help stubs, Page Header/Themes, File↔toolbar parity.

---

## Source chats (inventories collected)

| Chat | Branch / notes |
|------|----------------|
| Document palette & typewriter | `cursor/forms-canvas-wysiwyg` · Jul 9–10 |
| Designer Sign-up DirtBowl | same · ~Jul 2–10 |
| Designer MDI and Heading | `main` checkpoint then continued on feature branch · ~Jul 7 |
| Designer Forms foundation | feature branch · ~Jul 7 |
| Forms canvas & Skip | feature branch · Jul 7–8 |
| Process editor If/Set | feature branch · Jul 8–9 |
| Process Show & If UX | feature branch · Jul 9 · **no open bugs** |
| Process statement panels | feature branch · Jul 9 |
| Document WYSIWYG & palette | feature branch · Jul 9 |

Skipped: Website library mock; 8080 Docker/Tomcat CSS; Findme Assistant.
