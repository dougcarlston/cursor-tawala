# Designer open TODOs

Unfinished features, polish, and out-of-scope runtime gaps — **not** confirmed product bugs. Split out from chat bug inventories (July 10, 2026). See also **`DESIGNER_OPEN_BUGS.md`**.

Items marked **Deferred** were consciously postponed. Verify in the app before scheduling; later commits may have partially addressed some.

---

## Document editor & export

- **Document HTML → XML export incomplete** — only some function types emit real XML; others become comments. Tables/placed text improved in `documentHtmlToXml.mjs` but export is still partial. (Sources: Document palette & typewriter; Document WYSIWYG & palette)
- **Field-token drag polish** — optional leftover; not confirmed broken. (Source: Document palette & typewriter)
- **Default Font / Default Size greyed rules** — legacy: Reset greyed on fresh doc until mixed formatting; Reset is currently always greyed (see bugs). Remaining label/enable polish if needed after Reset is fixed. (Source: Document WYSIWYG & palette)

## Form items & Fields

- **Move Up / Move Down** for form items, process statements, and document blocks — insertion-point arrows exist; reorder commands do not. **Deferred** (Process-editing blocker). (Source: Designer Sign-up DirtBowl)
- **FIB hint-text styling** (smaller italic secondary font for parentheticals). **Deferred** → `docs/DESIGNER_BACKLOG_FORMS_FIBS.md`. (Source: Designer Sign-up DirtBowl)
- **Heading per-run Main/Sub size spans stripped** on export/runtime (legacy single-`type` heading can’t express mixed sizes). **Deferred.** (Source: Designer MDI and Heading)
- **FIB fine-grained Fields drop map** (question vs blank vs capture label). **Deferred** / unfinished. (Sources: Designer MDI and Heading; Forms canvas & Skip)
- **Per-item Properties popups** not migrated — permanent Properties panel still used for non-canvas-inline items. **Deferred.** (Source: Designer MDI and Heading)
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
- **Send does nothing at runtime** (`runtimeEngine.mjs` no-op; no Resend/SES). Designer UI done; mail API out of scope. **Deferred.** (Source: Process statement panels)
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
