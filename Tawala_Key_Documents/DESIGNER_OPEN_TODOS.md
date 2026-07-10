# Designer open TODOs

Unfinished features, polish, and out-of-scope runtime gaps — **not** confirmed product bugs. Split out from chat bug inventories (July 10, 2026). See also **`DESIGNER_OPEN_BUGS.md`**.

Items marked **Deferred** were consciously postponed. Verify in the app before scheduling; later commits may have partially addressed some.

---

## Document editor & export

- **Document HTML → XML export incomplete** — only some function types emit real XML; others become comments. Tables/placed text improved in `documentHtmlToXml.mjs` but export is still partial. (Sources: Document palette & typewriter; Document WYSIWYG & palette)
- **fx / Insert Function not fully implemented** — picker popup works (smoke July 10); many functions lack full configure UI and/or real XML export. (Source: Document palette & typewriter; smoke item 14)
- **Field-token drag polish** — optional leftover; not confirmed broken. (Source: Document palette & typewriter)
- **Default Font / Default Size greyed rules** — legacy: Reset greyed on fresh doc until mixed formatting; Reset is currently always greyed (see bugs). Remaining label/enable polish if needed after Reset is fixed or removed. (Source: Document WYSIWYG & palette)

### Document line model / reflow (epic — owner Jul 10)

**Not** legacy parity: vintage Designer used a hard-coded ~7″ text box with horizontal scroll. Owner prefers **margin-based reflow** so SportsDashboards (and similar) can target mobile/tablet by constraining overall form/window size instead of hand-rebuilding every layout.

Planned pieces (after single-line margin align):

- Invisible **line slots** (height from font size) between left/right margins  
- **Field snap-to-line** on drop so tokens join that line’s content — **done July 10**  
- **Move field token** after drop — **done July 10**  
- **Wrap on type** at right margin; push following lines down/aside — **verified July 10**  
- **Wrap on resize** when the MDI/window narrows — **verified July 10**  
- **Multi-line highlight** then **multi-line align** — **verified July 10**  

Single-line left/center/right/justify to margins shipped first; selection-only size + grow/shrink packing also landed July 10. Wrap-on-type soft-wraps within the margin box (from line `left` to right margin) and packs lines below when height changes. ResizeObserver re-wraps/packs on editor width change. Drag-select can span placed lines; align applies to all intersecting lines.

## Form items & Fields

- **Move Up / Move Down** for form items, process statements, and document blocks — insertion-point arrows exist; reorder commands do not. **Deferred** (Process-editing blocker). (Source: Designer Sign-up DirtBowl)
- **FIB hint-text styling** (smaller italic secondary font for parentheticals). **Deferred** → `docs/DESIGNER_BACKLOG_FORMS_FIBS.md`. (Source: Designer Sign-up DirtBowl)
- **Heading per-run Main/Sub size spans stripped** on export/runtime (legacy single-`type` heading can’t express mixed sizes). **Deferred.** (Source: Designer MDI and Heading)
- **FIB fine-grained Fields drop map** (question vs blank vs capture label). **Deferred** / unfinished. (Sources: Designer MDI and Heading; Forms canvas & Skip)
- **Per-item Properties popups** not migrated — permanent Properties panel still used for non-canvas-inline items. **Deferred.** (Source: Designer MDI and Heading)
- **Properties: Individual Items stay fully expanded** when not selected (should compress to a single line). UX polish — moved from bugs list July 10 review. (Source: Designer Sign-up DirtBowl)
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
- **Row-1 tools palette shell** above Project Explorer — left indent reserves space; legacy icon bar not built. **Deferred.** (Source: Document WYSIWYG & palette)
- **Processes palette, Project Explorer chrome, canvas item windows** not restyled to legacy look-and-feel. **Deferred** (Items palette only was Choice A). (Source: Designer Forms foundation)

## Architectural / DirtBowl (not Designer UI bugs)

- **DirtBowl Preview vs Deploy data/seed mismatch** (local Preview defaults vs Java persisted admin/division data). **Deferred** as architectural distinction. (Source: Designer Sign-up DirtBowl)

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
