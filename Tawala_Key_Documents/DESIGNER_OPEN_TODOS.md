# Designer open TODOs

Unfinished features, polish, and out-of-scope runtime gaps — **not** confirmed product bugs. Split out from chat bug inventories (July 10, 2026). See also **`DESIGNER_OPEN_BUGS.md`**.

Items marked **Deferred** were consciously postponed. Verify in the app before scheduling; later commits may have partially addressed some.

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

## Form items & Fields

- **Move Up / Move Down** for form items and process statements — **Done Jul 12** (↑/↓ + Alt+arrows + select-then-drag reorder; compact lists, caret only while dragging). **Document blocks — owner smoke Jul 15: pass.**
- **FIB hint-text styling** (smaller italic secondary font for parentheticals). **Deferred** → `docs/DESIGNER_BACKLOG_FORMS_FIBS.md`. (Source: Designer Sign-up DirtBowl)
- **Heading per-run Main/Sub size spans stripped** on export/runtime (legacy single-`type` heading can’t express mixed sizes). **Deferred.** (Source: Designer MDI and Heading)
- **FIB fine-grained Fields drop map** (question vs blank vs capture label). **Deferred** / unfinished. (Sources: Designer MDI and Heading; Forms canvas & Skip)
- **Per-item Properties popups** not migrated — permanent Properties panel still used for non-canvas-inline items. **Deferred.** (Source: Designer MDI and Heading)
- **Properties: Individual Items stay fully expanded** when not selected (should compress to a single line). UX polish — **superseded July 10:** right-column Properties panel removed; Fields owns the column. Per-item Properties popups remain a separate deferred item.
- **File Uploader always greyed / not insertable** — visual parity only; not implemented. **Deferred.** (Sources: Designer Forms foundation; Forms canvas & Skip)
- **Items palette icons** are Unicode/CSS placeholders, not legacy assets. **Deferred.** (Source: Designer Forms foundation)
- **MCQ dynamic choice source** (“from stored data” + Configure Function). **Deferred.** (Source: Forms canvas & Skip)
- **Rich text HTML → legacy XML export incomplete** for Heading/Text/FIB prompt/MCQ question formatting (MCQ question still stripped to plain text). Unfinished. (Source: Forms canvas & Skip)

## Skip Instructions

- **Nested If inside then/else `( )` not supported.** **Deferred** (Skip parked). (Source: Forms canvas & Skip)
- **Modify on existing Skip script line missing** (legacy Add/Modify). **Deferred.** (Source: Forms canvas & Skip)
- **Skip toolbar Cut/Copy/Paste/Undo stubs** — icons present, not functional. **Deferred.** (Source: Forms canvas & Skip)

## Process editor

- **Edit → Connect / Disconnect Pre/Post-Process** menu actions missing — yellow banner + connection dialog work; menu parity does not. **Deferred.** (Sources: Designer MDI and Heading; Process editor If/Set)
- **Send does nothing at runtime** (`runtimeEngine.mjs` no-op; no Resend/SES). Designer panel layout OK (smoke July 10); **email validation incomplete**; mail API out of scope. **Deferred.** (Source: Process statement panels)
- **Get `where` filter not applied in browser preview.** Designer Where UI works; runtime filter not wired. **Deferred.** (Source: Process statement panels)
- **Append / Show document merge not in browser preview.** Designer panels work; runtime merge not in this track. **Deferred.** (Source: Process statement panels)
- **Fields palette: no Record List / RecordSet branch after Get** (legacy when Get selected). ForEach record branches landed; Get RecordSet left as polish. **Deferred.** (Source: Process statement panels)

## Shell / MDI / chrome

- **MDI Pass 2** — no Windows menu; Design/Preview and selected item still global across form windows; no layout persistence; no maximize/tile/snap. **Deferred.** (Sources: Designer MDI and Heading; Document WYSIWYG & palette)
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
| 2 | **Home-page control audit — menus, tabs, and toolbars** | Draft canvas delivered Jul 12. **Insert menu context-sensitivity fixed Jul 12.** **Deferred until after #9 function smoke** (cleanup plan Jul 16) — File/Edit/View/Project/Help vs toolbar duplicates; View/Help/Page Header/Themes still stubs. |
| 3 | **Review remaining gated items** (3-browser smoke; look-and-feel parity) | Still **gated** until Designer is basically finished — owner asked to keep them visible on the review queue; discuss before starting. **Do not start during #9 smoke.** |
| 4 | **MCQ dynamic choice source** (“from stored data” + Configure Function) | Priority for SignupSheets-class apps; still **Deferred** in Form items until scheduled. |
| 5 | **HTML→XML export for functions we already Configure** | **Mostly done Jul 13–16** — 13 of 17 emit real XML; 4 deferred stubs. Remaining work is **owner smoke**, not emit. See function status matrix in `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`. |
| 6 | **Move Up / Down** for form items (process statements if cheap) | **Done Jul 12** — Form + Process: arrows and drag-reorder. **Document blocks — owner smoke Jul 15: pass.** |
| 7 | **Sample / template review (first pass)** | **Done Jul 12** (owner). **Re-review after #9 and #10** — functions + Deploy must work before a second full pass. |
| 8 | **Other structured Form Text tables** (e.g. choice tally) | Same click-to-Configure / rich-edit path as MQL + correlation when a template needs them. Part of #9. |
| 9 | **Wire the rest of the functions** | **Active — smoke ladder (Jul 16).** Configure for all 17. **Deferred stubs:** categorizer, export-team-roster, link-to-project-details, paypal. **Template smoke done:** itemization (SignupSheet), choice-tally (Simple + Multi Survey), correlation (Get Together caveats), sum (Potluck caveats). **Emit locked by tests** (`documentHtmlToXml.test.mjs` function emit matrix). **Still smoke-needed (no template owner pass):** display-image, display-mcq-label, record-count, project-email-count, response-totals, simple-list, ranked* (3). Full matrix: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`. |
| 10 | **Get Deploy working** | **Usable Jul 12–16** on 5173/3001 and 8080 when Tomcat up (`dev`/`dev`). Still: email Send, theme polish, AdminDash→Thank you navigation bug. |

**Sequencing note (owner Jul 12):** Finish **#9 (functions)** and **#10 (Deploy)**, then **re-review sample projects** (#7 second pass).

**Cleanup plan Jul 16:** Home-page menu audit (#2) and gated items (#3 / After Designer finished) stay **parked** until the remaining #9 smoke-needed functions above are cleared or explicitly deferred. Do not start menu look-and-feel or 3-browser smoke without owner discussion.

---

## After Designer is basically finished (gated)

Owner (July 12, 2026): park these until the browser Designer is considered **basically finished**. **Do not start any of these without prior discussion with the owner.**

**Jul 16 cleanup:** Explicitly deferred until after #9 remaining function smokes (and owner says Designer is basically finished).

1. **Big smoke test on three different browsers** — full walkthrough of Designer (and critical Preview/Deploy paths as agreed) on three browsers; capture browser-specific defects. *(Also listed in Owner review queue #3.)*
2. **Conform Look and Feel** of the Designer shell and its windows to the legacy Designer application **without breaking** underlying behavior already shipped (layout, chrome, typography/colors — visual parity pass only after functional freeze). *(Also listed in Owner review queue #3.)*
3. **Main Page menus and tabs — no duplicates; identical behavior** — audit every main menu and tab for duplicate entries; on selection, each must operate exactly the same as its counterpart (no divergent handlers or stale duplicates). **Owner Jul 12:** schedule **after** Main icon toolbar is wired (Owner review queue #1–2); toolbar duplicates File/Edit and is part of the same audit. **Also parks owner review #2** remaining home-page control audit (View/Help stubs, Page Header/Themes, File↔toolbar parity).

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
