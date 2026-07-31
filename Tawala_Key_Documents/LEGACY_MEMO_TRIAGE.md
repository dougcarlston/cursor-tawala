# Legacy Word memo triage (Phase 1)

**Corpus:** `/Users/DougC1/Projects/Tawala Projects/Zipped Word Documentation`  
(Note: folder name is `Zipped Word Documentation`, not `ZippedWordDocumentation`.)

**Index created:** 2026-07-30  
**Scope:** Phase 1 only — everything **except** the owner-deferred folders listed below.  
**Method:** Folder inventory + `textutil -convert txt` sampling of Phase 1 `.doc` files. No full-corpus conversion. **Do not commit** this index unless the owner asks.

---

## Top-level layout

| Entry | Phase | Notes |
|-------|-------|-------|
| Root `.doc` memos (15) + GIF + `tawala.EAP` + `tawala-passwords.txt` | **1** | Core engineering / product memos (~2006–2008; zip mtime 2014-03-16) |
| `Customizable Web apps/` | **1** | Customizer UX / website product thinking (2007) |
| `Database/` | **1** | Performance + ER diagram |
| `Designer Help and Tips/` | **Ignore first** | 1× `.docx` CLI deploy switches |
| `Early Tawala Docs/` | **Phase 2** | Large (~1981 files / ~205 `.doc`); owner says useful despite name |
| `SportsDashboards/` | **Ignore first** | 1× `.xls` SDT list (product spelling SportsDashboards) |
| `VersionOne Exported Data/` | **Ignore first** | Story exports (`.xls` / `.csv`) |
| `Harry Chomsky's Docs/` | **Phase 3** | Evolution / design thinking |

---

## File counts by folder

Counts exclude `.DS_Store`. Many folders still contain SVN metadata (`.svn/`); “content” counts below exclude `.svn` where noted.

| Location | Content files (approx) | `.doc` | `.docx` | Other notable |
|----------|------------------------|--------|---------|---------------|
| **Root** (files only) | 18 | 15 | 0 | 1 GIF, 1 EAP (Access DB), 1 passwords `.txt` |
| `Customizable Web apps/` | 9 (+ SVN) | 8 | 0 | 1 `.tawala` sample |
| `Database/` | 3 (+ SVN) | 1 | 0 | 1 JPG ER model, Thumbs.db |
| `Designer Help and Tips/` | 1 (+ SVN) | 0 | 1 | — |
| `Early Tawala Docs/` | ~685 content / ~1981 w/ SVN | ~205 | 0 | Many images/screenshots under Design, Specs, etc. |
| `Harry Chomsky's Docs/` | 7 (+ SVN) | 7 | 0 | — |
| `SportsDashboards/` | 1 (+ SVN) | 0 | 0 | 1 `.xls` |
| `VersionOne Exported Data/` | 7 (+ SVN) | 0 | 0 | `.xls` / `.csv` |

### Early Tawala Docs subfolders (Phase 2 peek — names/counts only)

| Subfolder | `.doc` | Files (non-SVN approx) |
|-----------|--------|-------------------------|
| Design | 136 | 364 |
| Original Specifications | 39 | 191 |
| User Documentation | 9 | 45 |
| Developer Notes | 10 | 10 |
| Schedule | 4 | 27 |
| Planning | 5 | 6 |
| Meeting Notes | 2 | 2 |
| Misc / Retrospectives | 0 | 2 |

---

## Formats and conversion barriers

| Format | Prevalence | Extraction |
|--------|------------|------------|
| **`.doc`** (OLE Compound / Word 97–2004) | Almost all Phase 1 memos | **`textutil -convert txt`** works well on this Mac. `antiword` / `pandoc` not installed. |
| **`.docx`** | Only in deferred `Designer Help and Tips/` | `textutil` should handle when Phase resumes. |
| **`.xls` / `.csv`** | VersionOne, SportsDashboards | Spreadsheet tools; not sampled in Phase 1. |
| **`.GIF`** | `Project Version Web UI.GIF` | Visual companion to Project Versioning. |
| **`.EAP`** | `tawala.EAP` | Enterprise Architect / Access DB — needs EA or Access; not text-extractable via textutil. |
| **`.tawala`** | Exam Template sample | Binary project package; use Designer/import paths if needed. |
| **SVN `.svn-base`** | Many folders | Duplicates of real files; ignore for reading. |

**Barriers:** TOC field junk in textutil output; embedded diagrams missing from text; `.EAP` opaque; Early corpus volume (~200 docs + images) needs a Phase 2 pass by theme, not wholesale convert.

**Security:** `tawala-passwords.txt` at corpus root — treat as secrets; do not copy into git or this index body.

---

## Phase 1 — High relevance (top items)

Rough dates from in-document SAVE/DATE fields where present (filesystem dates are mostly 2014 zip unpack).

### A. Runtime / data model / Process (Designer + :8080)

| # | Title | Path | Date | Tags | Rel. | Why | Integrate into |
|---|-------|------|------|------|------|-----|----------------|
| 1 | Fields and Variables | `…/Fields and Variables.doc` | ~2006-03-16 | Designer, Runtime, Process | **High** | Canonical definitions: record types, fields vs variables, CRUD scenarios, validation, persistence — still the mental model for Forms/Processes. | `DESIGNER_VARIABLES_TYPING_HANDOFF.md`, `TAWALA_PROJECT_SUMMARY.md`; cite from process specs |
| 2 | Dynamic MCQs | `…/Dynamic MCQs.doc` | ~2007 (no footer date) | Designer, Runtime, Process | **High** | Data providers, store value vs letter, reference vs user data; Exam Builder / shopping / Coffee Schedule patterns. | `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`; park unimplemented providers on ROADMAP |
| 3 | Additional Flow Control Elements in Forms | `…/Additional Flow Control Elements in Forms.doc` | ~2007 | Designer, Process, Runtime | **High** | Form-level FOR EACH / IF, `DisplayMCQ`, Exam Builder + Sign-up Sheet structures — documents intended form control flow beyond Skip. | `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` + process specs; ROADMAP if not shipped |
| 4 | DirtBowl Notes | `…/DirtBowlNotes.doc` | post-DirtBowl (~2007–08) | Designer, Runtime, Process, MyTawala | **High** | Lessons from real large project: structure pain, validation+documents, MCQ values, process reuse, PayPal, Project Manager gaps — maps to current DirtBowl/:8080 work. | `TAWALA_PROJECT_SUMMARY.md`, open bugs / ROADMAP; do not treat as UI pixel spec |
| 5 | Improvements to Project Structure | `…/Improvements to Project Structure.doc` | ~2008-01-04 | Designer, Website, Runtime | **High** | Proposes **Pages**, de-emphasize processes, nav/menus, validation messages — explains many Designer quirks and “submitless page” workarounds still visible. | Architecture note / `docs/ROADMAP.md`; mostly **park** for browser Designer unless owner wants page model |
| 6 | Web Application Performance | `…/Database/WebAppPerformance.doc` | 2006-10-02 | Runtime, Library, MyTawala | **High** | GET/WHERE cost, CLOB submissions, Library indexing/cache, Project Manager export limits — explains runtime/Library scaling assumptions. | Ops / `docs/COMPARING_RUNTIMES.md` / park performance backlog |

### B. Website / Library / MyTawala

| # | Title | Path | Date | Tags | Rel. | Why | Integrate into |
|---|-------|------|------|------|------|-----|----------------|
| 7 | Project Versioning | `…/Project Versioning.doc` (+ `Project Version Web UI.GIF`) | ~2006-08-22 | MyTawala, Library, Designer, Website | **High** | Deployed vs non-deployed versions, Library submit rules, test-drive, upload metadata — core MyTawala/Library product contract. | `website-mock` fidelity notes; `docs/ROADMAP.md` Phase 3; Deploy dialog behavior |
| 8 | Private Invitations | `…/Private Invitations.doc` | ~2007-04-18 | Runtime, Process, Designer, MyTawala | **High** | `PRIVATE INVITATION` / invitation-only forms / InvitationToken — under-documented subsystem vs Insert→Invitation in Designer. | `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`; runtime invite URL behavior |
| 9 | Hiding Form Names | `…/Hiding Form Names.doc` | ~2007-09-30 | Runtime, MyTawala | **High** | Form name → random URL tokens on deploy to My Tawala; security of admin URLs. | Runtime/deploy URL docs; website test-drive link design |
| 10 | Web Application Themes | `…/Web Application Themes.doc` | ~2006-09-11 | Runtime, Designer, Website | **High** | Default/standard/custom CSS themes, per-user vs per-project storage, `tawala.` namespaces — matches theme CSS under Tomcat/docker. | Runtime CSS / Styles UI; `DESIGNER_MENU_SPEC` Styles section |
| 11 | Customizing Web App Appearance | `…/Customizing Web App Appearance.doc` | ~2007-05-17 | Website, Library, Designer | **High** | Replaceable images/logo, theme pick, preview, preserve in project — Library customizer appearance step. | `website-mock` customize flow; park until site customize UI |
| 12 | Emailing from Tawala Apps | `…/Emailing from Tawala Apps.doc` | ~2007-05-14 | Process, Runtime, MyTawala | **High** | Async send queue, quotas, bounce, delayed send — explains why SEND ≠ SMTP inline; Email Delivery product. | `docs/EMAIL_DELIVERY_OPS.md`; `DESIGNER_PROCESS_STATEMENTS_SEND.md` |
| 13 | UI Outline (customizer) | `…/Customizable Web apps/UI Outline.doc` | ~2007 | Website, Library, MyTawala | **High** | End-to-end customizer path: Appearance → Content → Save → Publish → Send; Signup Sheet as lead app; drop-off lessons. | `website-mock/README.md`, Library mock UX |
| 14 | Essentials of a simple and great Customization UI | `…/Customizable Web apps/Essentials of a simple and great Customization UI.doc` | ~2007 | Website, Library | **High** | Customizer UX principles (WYSIWYG, play vs live, logo/theme, one lead app). | Website mock acceptance criteria |
| 15 | Customization Goals / Evolution / Form Customization | `…/Customizable Web apps/*.doc` (Bockman, Apr–Jun 2007) | 2007-04–06 | Website, Designer, Library | **High** | Why setup-form customization doesn’t scale; browser Form Customizer; eliminate special setup forms. | ROADMAP / Library product narrative; **park** full Form Customizer |

### C. Strong Medium (still Phase 1 useful)

| Title | Path | Date | Tags | Rel. | Why | Integrate into |
|-------|------|------|------|------|-----|----------------|
| Project Backup | `…/Project Backup.doc` | ~2007-11-28 | MyTawala | **Med** | Excel workbook backup/restore on Project Details — Project Manager feature. | MyTawala project ops mock; park if not shipping soon |
| Web Application Error Handling | `…/Web Application Error Handling.doc` | ~2006-02-14 | Runtime | **Med** | Error classes, logging, prevention at startup. | Runtime ops notes; low priority for mock |
| Updates to Project Format | `…/Updates to  Project Format.doc` | ~2007-12-26 | Designer, Runtime | **Med** | Proposal: separate `<fields>` from HTML `<ui>` segments for WYSIWYG — may be aspirational vs shipped XML. | Compare to actual project XML before integrating; likely **park** |
| Customization Improvement | `…/Customization Improvement.doc` | ~2007-05-24 | Website | **Med** | SurveyMonkey / Wufoo / CircleUp competitive notes. | Website UX inspiration only |
| Desirable features for customization | `…/Customizable Web apps/Desirable features….doc` | 2007-05-24 | Website | **Med** | Workshop framing + feature wishlist. | Fold into website criteria with Essentials/UI Outline |
| Style Guidelines / Nomenclature | `…/Customizable Web apps/` | ~2007 | Website, Designer | **Med–Low** | Naming/style conventions for customizable apps. | Sample if Styles/Library copy needs consistency |
| Database Initial Production Model.jpg | `…/Database/` | ~2006 | Runtime | **Med** | Visual schema companion to performance memo. | Keep with WebAppPerformance; no text extract |

### D. Phase 1 Low / skip / caution

| Item | Rel. | Note |
|------|------|------|
| `Exam Template 02.tawala` | Low | Sample project binary; open only if reconstructing Exam Builder. |
| `tawala.EAP` | Low (until needed) | Architecture model — needs Enterprise Architect; Phase 2/3 if schema archaeology required. |
| `tawala-passwords.txt` | **Skip (secrets)** | Do not index contents; rotate if still valid anywhere. |
| SVN trees under all folders | Skip | Duplicates. |

---

## Phase 1 summary — suggested reading order (High)

1. **Fields and Variables** — data model foundation  
2. **DirtBowl Notes** — what broke at real scale (ties to :8080)  
3. **Project Versioning** + GIF — Library / MyTawala deploy model  
4. **Web Application Themes** + **Customizing Web App Appearance** — look/feel & customize  
5. **UI Outline** + **Essentials** + **Customization Goals** — website customizer story  
6. **Private Invitations** + **Hiding Form Names** — access / URL subsystem  
7. **Dynamic MCQs** + **Additional Flow Control…** — form power features  
8. **Emailing from Tawala Apps** — SEND / delivery architecture  
9. **WebAppPerformance** — Library + runtime storage assumptions  
10. **Improvements to Project Structure** — “page” vision (mostly park, but explains Designer pain)

---

## Deferred folders (not triaged in depth)

### Designer Help and Tips — ignore first
- `Tawala Designer Command Line Switches for Novalidation and Deployment to Servers.docx`  
- Likely useful later for offline deploy / novalidation; single file; easy Phase 1.5 if Deploy CLI needed.

### Early Tawala Docs — Phase 2 (**recommended**)
Owner note: looked useful despite the name. Highest-value targets for Phase 2:

- **Original Specifications** — Form/Process/Document/Invitation/Deployment UI specs (align with `DESIGNER_*.md`)  
- **User Documentation** — Alpha step-by-step / features  
- **Developer Notes** — e.g. `Web Site Requirements.doc`, `Tawala Client Server XML.doc`  
- **Design** — large screenshot/doc set (sample by topic, don’t convert all)

**Recommendation:** Run Phase 2 after website Library mock is stable enough to know which UX questions remain open. Prioritize Original Specifications + Web Site Requirements over Design image dumps.

### SportsDashboards / VersionOne — ignore first
- SportsDashboards: development list spreadsheet only.  
- VersionOne: historical story trackers; use only if tracing “why was X built.”

### Harry Chomsky's Docs — Phase 3 (**recommended later**)
Titles (for later): `Chomsky Tawala comments(.JDF).doc`, `Points taken from Chomsky Memo(.JDF).doc`, `Tawala database ideas.doc`, `Tawala next level.doc`, `Tawala types.doc`.  
**Recommendation:** Yes for Phase 3 when product/architecture narrative matters more than mock fidelity — evolution and engineering design thinking, not pixel specs.

---

## Phase 2 / 3 recommendation (verdict)

| Phase | Recommended? | When |
|-------|--------------|------|
| **Phase 2 — Early Tawala Docs** | **Yes** | After Phase 1 High items are skimmed into specs/ROADMAP; especially if Designer UI gaps or website IA need original UI specs / Web Site Requirements. |
| **Phase 3 — Harry Chomsky** | **Yes, later** | After website + core Designer parity; treat as architecture/product philosophy, not immediate implement backlog. |
| Designer Help / SportsDashboards / VersionOne | Only on demand | CLI deploy, SportsDashboards feature archaeology, or story archaeology. |

---

## Conversion cheat sheet (owner / next agent)

```bash
CORPUS="/Users/DougC1/Projects/Tawala Projects/Zipped Word Documentation"
textutil -convert txt -stdout "$CORPUS/Some Memo.doc" | less
# Optional: write one High memo to workspace (avoid bulk)
# textutil -convert txt -output /tmp/memo.txt "$CORPUS/Fields and Variables.doc"
```

Do **not** bulk-convert Early Tawala Docs until a Phase 2 theme list is chosen.

---

## Related current docs

- `Tawala_Key_Documents/TAWALA_PROJECT_SUMMARY.md`  
- `Tawala_Key_Documents/DESIGNER_*.md`  
- `docs/ROADMAP.md`, `docs/CHAT_HANDOFF.md`, `website-mock/README.md`  
- `docs/EMAIL_DELIVERY_OPS.md`
