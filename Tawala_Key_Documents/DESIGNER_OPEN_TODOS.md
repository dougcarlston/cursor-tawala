# Designer open TODOs

Unfinished features, polish, and out-of-scope runtime gaps — **not** confirmed product bugs. Split out from chat bug inventories (July 10, 2026). See also **`DESIGNER_OPEN_BUGS.md`**.

Items marked **Deferred** were consciously postponed. Verify in the app before scheduling; later commits may have partially addressed some.

---

## ☀️ Jul 21 — `.tawala` → Designer Open **landed**

**Green-build:** Done Jul 21 (`npm run build` green).

**Import:** Shared converter [`designer-web/src/lib/tawalaXmlToJson.mjs`](../designer-web/src/lib/tawalaXmlToJson.mjs).

```bash
# CLI (repo root)
node scripts/tawala-to-json.mjs path/to/Project.tawala [out.json]

# Designer
File → Open → choose .json or .tawala (or .tawala.xml)
```

After Open of `.tawala`, status shows `Imported … (N warnings)`; quiet-Save handle is **cleared** — next Save is Save As `.json` (does not overwrite the legacy file).

**CLI smoke Jul 21:**
| Source | Forms | Processes | Documents | Images |
|--------|------:|----------:|----------:|-------:|
| SignupSheets.tawala.xml | 9 | 12 | 8 | 0 |
| Potluck Template.tawala | 2 | 3 | 3 | 2 |
| DirtBowl.tawala | 69 | 79 | 44 | 2 |

**Known lossy warnings (expected):** styles dropped; multi-form itemization limited; Send subject notes; Document table→paragraph approximations. SportsDashboards deep parity still out of scope. Dynamic MCQ imports open in Configure Function (Edit); nested `where` trees may need a re-save of conditions. **Jul 24:** `<pageHeader>` imports into JSON (no longer dropped). **Jul 25:** Form Text / Document rich content with fields, invitations, and hyperlinks import as HTML tokens (not structured arrays except MQL); invitation `displayText` and `<link>` description/url no longer warn as empty.

**Jul 21 import follow-ups (fixed same day):** FIB import now emits Design underscore runs from `<blank length>`; Form Text inline `<image id>` embeds as `data-tawala-image-id` imgs with data-URLs from `project.images`; Form Text `<table>` with `<division>` cells → editable HTML (Name / `<<fields>>`); structured-content fallback now shows badge + × delete. Re-run CLI / re-Open `.tawala` to pick up. Skip script shows friendly `equals` for imported `mcEquals`.

**Owner smoke next:** File → Open Potluck / DirtBowl `.tawala` → Explorer populated → optional Deploy. **Jul 24:** New Project → Potluck JSON is no longer a stub (owner `00` file installed as `templates/potluck.json`).


**Park:** Deferred UX, 3-browser, look-and-feel; Document invent/chip polish unless blocking.

### End of day Jul 20 — checkpoint (historical)

**Git:** `3fd215f` pushed earlier (Document highlight-move / chips / Skip gaps). Follow-up docs commit tonight for Deploy/Send/Undo policy + week plan. Branch: `cursor/forms-canvas-wysiwyg`.

**Shipped / Owner Passed today (Design canvas + Deploy):**
- Document highlight-drag: multi-line move, drag-into-blank, no mid-line skip, whole-line only (not double-click word), chip-only relocate, function→text join.
- Skip re-edit: insert gaps again (`showAllInsertionGaps`); palette tool clears wrong Modify.
- Undo: **closed policy** — best-effort CE only; renames/Moves/Skip·Process/Explorer out of scope.
- Legacy Deploy: Potluck + full DirtBowl `.tawala` **flawless** (MQL, themes, AdminDash-scale). AdminDash “bug” was **corrupted JSON** (bare `Show`, blank AdminDashboard).
- Process **Send** self-mail on :8080 **works**.

**Important distinctions:**
- **New Project** Potluck JSON = **updated Jul 24** (owner `00-WebDesigner-MainMenu_Potluck.json`). Other starters vary; when in doubt smoke via `node scripts/deploy-tawala-template.mjs "…"` or path to `.tawala`.
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

- **Document HTML → XML export** — **13 of 17** function types emit real XML (`documentHtmlToXml.mjs`). **4 Deferred stubs** (categorizer / roster / link / paypal) emit comments only and are hidden from Insert → Function (Jul 24). Tables/placed text improved; remaining gaps are stub product, not missing emitters for the shipped set. (Sources: Document palette & typewriter; Document WYSIWYG & palette)
- **fx / Insert Function** — picker + Configure for all catalog entries; Insert hides the four parked stubs. Full configure UI for stubs not required this build. (Source: Document palette & typewriter; smoke item 14)
- **Field-token drag polish** — optional leftover; not confirmed broken. (Source: Document palette & typewriter)
- **Default Font / Default Size greyed rules** — legacy Reset greyed rules obsolete; Reset control **removed** July 10.
- **Indent / Outdent** — **verified July 10:** Document placed lines shift by 36 pt steps from the left margin (width still to right margin); Form paragraphs use `margin-left`.

### Document line model / reflow (epic — owner Jul 10)

**Not** legacy parity: vintage Designer used a hard-coded ~7″ text box with horizontal scroll. Owner prefers **margin-based reflow** so SportsDashboards (and similar) can target mobile/tablet by constraining overall form/window size instead of hand-rebuilding every layout.

**Related (not the same epic):** multi-device **output sizing** (separate computer / tablet / phone layouts, optional autoswitch) is a larger product redesign that was **not** in the 2011 Designer. Spec’d and **parked** Jul 24 — see § **Device output sizing** below. Do not conflate margin wrap in Design with that future runtime/authoring model.

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
- **Heading Main/Sub** — **Jul 24:** Design Type applies to the **selection** (or pends at caret). Mixed Main+Sub lines in one box export as multiple `<heading>` / Preview `h1.heading` + `h2.subheading` with stack gap. See `DESIGNER_FORM_ITEMS_HEADING.md`. (Source: Designer MDI and Heading)
- **FIB fine-grained Fields drop map** (question vs blank vs capture label). **Deferred** / unfinished. (Sources: Designer MDI and Heading; Forms canvas & Skip)
- **Per-item Properties popups** not migrated — permanent Properties panel still used for non-canvas-inline items. **Deferred.** (Source: Designer MDI and Heading)
- **Properties: Individual Items stay fully expanded** when not selected (should compress to a single line). UX polish — **superseded July 10:** right-column Properties panel removed; Fields owns the column. Per-item Properties popups remain a separate deferred item.
- **File Uploader** — **Deferred / out of scope for this build (owner Jul 27).** Hidden on Forms→Items palette (no greyed stub; owner Jul 17). Very complex; unused on owner Mac except SportsDashboards communicator forms. Never wired in 2011 reference build or browser Designer. Spec only: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` § File Uploader. Use **Insert → Image → From your PC…** / **From the Web…** for images.
- **Items palette icons** — **Done Jul 24** (legacy-style PNG tiles in `designer-web/public/icons/form-item-*.png`). (Was: Unicode/CSS placeholders.)
- **MCQ dynamic choice source** (“from stored data” + Configure Function). **Done Jul 23** — Choice source / Edit → `ConfigureFunctionDialog` (`dynamic-mcq`); Deploy `mcToXml`. Preview expands rows from session records (condition filter still Preview-only soft). (Source: Forms canvas & Skip)
- **Rich text HTML → legacy XML export** for Form items — **MCQ question Done Jul 24**; **Heading Main/Sub split Done Jul 24**. Text/FIB already on the rich path. (Source: Forms canvas & Skip)

## Skip Instructions

Skip Instructions is **wired** (canvas Edit dialog: If / SkipTo / Set / Comment; Modify / delete / reorder / insert gaps including nested If). Soft leftovers only (e.g. re-open insert-point polish) — see `DESIGNER_OPEN_BUGS.md` if needed.

**Closed / not TODOs (owner Jul 23):**
- **Toolbar Cut / Copy / Paste / Undo** — legacy chrome only; never implemented in the 2011 Designer and **not needed** inside Skip. Do not schedule.
- **Nested If** — in active use in browser Designer; not a deferred gap.
- **Modify existing Skip lines** — done Jul 19 (was a stale Deferred line).

## Process editor

- **Edit → Connect / Disconnect Pre/Post-Process** menu actions missing — yellow banner + connection dialog work; menu parity does not. **Deferred.** (Sources: Designer MDI and Heading; Process editor If/Set)
- **Send does nothing at runtime** — **Superseded Jul 20.** Browser Designer → Deploy **Send** works on :8080 (owner: self-send OK). Preview still does not send mail. Recipient-list / bulk To not yet owner-smoked. Ops: `docs/EMAIL_DELIVERY_OPS.md`. (Was: `runtimeEngine.mjs` no-op — Java path is the real delivery.)
- **Get `where` filter not applied in browser preview.** Designer Where UI works; runtime filter not wired. **Deferred.** (Source: Process statement panels)
- **Append / Show document merge not in browser preview.** Designer panels work; runtime merge not in this track. **Deferred.** (Source: Process statement panels)
- **Fields palette: no Record List / RecordSet branch after Get** (legacy when Get selected). ForEach record branches landed; Get RecordSet left as polish. **Deferred.** (Source: Process statement panels)

## Shell / MDI / chrome

- **Help → About Tawala Designer** — **Done Jul 24** (`AboutDialog` + Help menu). Two separate © lines (Tawala Systems 2005–2009; Douglas G. Carlston 2026); Beta Version; third-party acknowledgments; no OS/.NET/File Versions. Spec: `DESIGNER_MENU_SPEC.md` § Help. Owner review queue **#13**.
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
| 2 | **Home-page control audit — menus, tabs, and toolbars** | **Jul 17:** File/Edit/Insert/View/Project/Windows/Help. **View toggles wired.** Format removed (palette); Tables skipped; Project Tabs/Styles wired; **Page Header wired Jul 24**; Themes = local CSS. **Help → About** split out to queue **#13** (legal). |
| 3 | **Review remaining gated items** (3-browser smoke; look-and-feel parity) | Still **gated** until Designer is basically finished — owner asked to keep them visible on the review queue; discuss before starting. **Do not start during #9 smoke.** |
| 4 | **MCQ dynamic choice source** (“from stored data” + Configure Function) | **Done Jul 23** — Design Configure + Deploy XML. Owner smoke SignupSheets-class apps still useful. |
| — | **Design-canvas Style paint** | **Owner Jul 18:** **Text** Instructional/Error shown on Forms → Text (already implemented). **FIB/MCQ layout paint = won't do** — interferes with editing; Preview immediate. See `DESIGNER_FORM_FORMAT_TOOLBAR.md`. |
| 5 | **HTML→XML export for functions we already Configure** | **Done Jul 13–16** for 13 of 17. **Jul 24:** remaining 4 **Deferred stub** (parked) — see matrix below. |
| 6 | **Move Up / Down** for form items (process statements if cheap) | **Done Jul 12** — Form + Process: arrows and drag-reorder. **Document blocks — owner smoke Jul 15: pass.** |
| 7 | **Sample / template review (first pass)** | **Done Jul 12** (owner). **Re-review after #9 and #10** — functions + Deploy must work before a second full pass. |
| 8 | **Other structured Form Text tables** (e.g. choice tally) | Same click-to-Configure / rich-edit path as MQL + correlation when a template needs them. Part of #9. |
| 9 | **Wire the rest of the functions** | See **#9 function status** table below. Core ladder **Done**; four stubs **Deferred**; template re-smoke optional. |
| 10 | **Get Deploy working** | **Done (usable)** Jul 12–21. Potluck + DirtBowl legacy **Passed** Jul 20; `.tawala` Open landed Jul 21. Remaining = unmarked templates in `DESIGNER_TEMPLATE_MATRIX.md` (Form+Document, Sign-up w Email). |
| 11 | **Implement MCQ-aware Function Where** | **Done Jul 19** — `mcConditionOperators` + `FunctionConditionsEditor` field-kind switch; XML emits `mc*`. **Owner Passed Jul 20.** |
| 12 | **RESPONSE TOTALS multi-select undercount** | **Done Jul 19 (investigation)** — no Totals-specific bug; same tally as Bar Graph; regression tests added. **Owner Passed Jul 20** (side-by-side Totals vs Bar Graph on multi MCQ — both pick up all choices). |
| 13 | **Help → About Tawala Designer** | **Done Jul 24** — `AboutDialog` wired; attorney-guided copy locked (two © lines; Beta Version; third-party note). Spec: `DESIGNER_MENU_SPEC.md` § Help. Owner smoke Help → About. |
| 14 | **Device output sizing** (computer / tablet / phone + optional autoswitch) | **Parked Jul 24** — not legacy parity; too large for this track. Spec: § **After the other two project branches**. Revisit with generic payment / other parked stubs after those branches. |
| 15 | **Project Themes — hide themes without local CSS** | **CSS installed Jul 27** — all **28** official themes copied from Tony archive `web.zip` into `docker/tomcat/css/project/`; `LOCAL_CSS_PATHS` in `projectThemes.ts` expanded 7→28 (all menu entries blue). **Hide-unavailable** change likely obsolete unless we drop themes again; optional smoke on 8080 after `docker cp`/Tomcat refresh. Theme-maker UI still out of scope. |
| 16 | **`.tawala` reconvert quality (Library)** | **Owner Jul 25:** Early conversions incomplete. Re-run `files to reconvert to JSON/*.tawala` with current `tawala-to-json.mjs`. Fix/improve: (1) unwanted `Record:` on function fields; (2) structured Text → placeholder instead of canvas HTML; (3) tables/functions missing on Open. Hold full Library vet until Designer **variables-as-text** bug is fixed (`DESIGNER_OPEN_BUGS.md`). Detail: `LIBRARY_PROJECTS_TRIAGE_JUL22.md` § Jul 25. |

**Cleanup plan Jul 24:** Home-page menu audit (#2) and gated items (#3) stay **parked**. Function #9 core is Done; optional next is unmarked template Deploy smokes (matrix), not a home-page menu pass.

### #9 function status (Jul 24 — Done / Smoke-needed / Deferred stub)

Source: `documentHtmlToXml.mjs` + `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § Function status matrix.

| Status | Functions |
|--------|-----------|
| **Done** (XML emit + owner Passed) | DISPLAY IMAGE, DISPLAY MCQ, FORM RECORD COUNT, MULTIPLE QUESTION LIST, PROJECT EMAIL COUNT, QUESTION CORRELATION, RANKED MULTIQUESTION LIST, RANKED RESPONSE COUNTS, RANKED RESPONSE NAME, RESPONSE BAR GRAPH, RESPONSE TOTALS, SINGLE QUESTION LIST, SUM |
| **Smoke-needed** | None for the 13 emitters — template ladder Passed (Simple Survey, Multi Survey, Signup MQL, Get Together w/ caveats, Potluck SUM). Optional re-smoke only if a regression is reported. |
| **Deferred stub** (parked Jul 24; hidden from Insert picker) | CATEGORIZER, EXPORT TEAM ROSTER, LINK TO PROJECT DETAILS, PAYPAL BUTTON → generic payment later |

**Template smoke ladder (when picking up #9 again):** (1) Multiple Question Survey — **Passed**; (2) Get Together / Potluck / Simple Survey — already Passed; (3) only then ad-hoc DISPLAY IMAGE / DISPLAY MCQ / record-count / simple-list if a new sample needs them. Leave the four Deferred stubs alone.

---

## After Designer is basically finished (gated)

Owner (July 12, 2026): park these until the browser Designer is considered **basically finished**. **Do not start any of these without prior discussion with the owner.**

**Jul 16 cleanup:** Explicitly deferred until after #9 remaining function smokes (and owner says Designer is basically finished).

1. **Big smoke test on three different browsers** — full walkthrough of Designer (and critical Preview/Deploy paths as agreed) on three browsers; capture browser-specific defects. *(Also listed in Owner review queue #3.)*
2. **Conform Look and Feel** of the Designer shell and its windows to the legacy Designer application **without breaking** underlying behavior already shipped (layout, chrome, typography/colors — visual parity pass only after functional freeze). *(Also listed in Owner review queue #3.)*
3. **Main Page menus and tabs — no duplicates; identical behavior** — audit every main menu and tab for duplicate entries; on selection, each must operate exactly the same as its counterpart (no divergent handlers or stale duplicates). **Owner Jul 12:** schedule **after** Main icon toolbar is wired (Owner review queue #1–2); toolbar duplicates File/Edit and is part of the same audit. **Jul 17:** View menu stubs restored (all five); **wire View chrome toggles after menu review completes** (Owner review queue #2). Also parks Page Header/Themes, File↔toolbar parity. **Help → About is not parked here** — see Owner review queue **#13** (legal).

---

## After the other two project branches (parked product epics)

Owner (Jul 24, 2026): these are **not** 2011 Designer parity gaps and are **too large** to start on the current Designer track. Keep them visible in the spec; **do not design or implement** until the other two AI-Tawala branches are finished and the owner reopens them. (Same gate as the four parked Insert → Function stubs — payment/generic checkout, categorizer, roster, My Tawala link.)

### Device output sizing (computer / tablet / phone)

**Intent:** Let a designer author **separate outputs** (or variants) depending on whether the end user is viewing on a **computer**, **tablet**, or **phone**, with the option that Tawala **autoswitches** based on what it can detect at runtime (viewport / user-agent / similar — mechanism TBD).

**Why park:** Did not exist in the legacy Designer fifteen years ago; touches Design authoring, Deploy/runtime layout, and possibly Process/Show paths. Far larger than a single Insert → Function stub.

**Not this epic:** Today’s Document **margin-based reflow** (narrow the Design window → wrap/pack) remains the interim way to approximate smaller widths. Device output sizing is an explicit multi-surface authoring + delivery model on top of (or instead of) “one layout squeezed.”

**When reopened:** Capture product decisions first — how many breakpoints; whether layouts are fully separate projects/forms/documents or conditional blocks; autoswitch vs manual “view as…”; Preview/Deploy parity.

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
