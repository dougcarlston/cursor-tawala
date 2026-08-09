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

Entries below use a **stacked** layout (not wide tables) for Preview readability.

### A. Runtime / data model / Process (Designer + :8080)

#### 1. Fields and Variables
- **Path:** `…/Fields and Variables.doc`
- **Date:** ~2006-03-16
- **Tags:** Designer, Runtime, Process
- **Rel.:** High
- **Why:** Canonical definitions: record types, fields vs variables, CRUD scenarios, validation, persistence — still the mental model for Forms/Processes.
- **Integrate into:** `DESIGNER_VARIABLES_TYPING_HANDOFF.md`, `TAWALA_PROJECT_SUMMARY.md`; cite from process specs

#### 2. Dynamic MCQs
- **Path:** `…/Dynamic MCQs.doc`
- **Date:** ~2007 (no footer date)
- **Tags:** Designer, Runtime, Process
- **Rel.:** High
- **Why:** Data providers, store value vs letter, reference vs user data; Exam Builder / shopping / Coffee Schedule patterns.
- **Integrate into:** `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`; park unimplemented providers on ROADMAP

#### 3. Additional Flow Control Elements in Forms
- **Path:** `…/Additional Flow Control Elements in Forms.doc`
- **Date:** ~2007
- **Tags:** Designer, Process, Runtime
- **Rel.:** High
- **Why:** Form-level FOR EACH / IF, `DisplayMCQ`, Exam Builder + Sign-up Sheet structures — documents intended form control flow beyond Skip.
- **Integrate into:** `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` + process specs; ROADMAP if not shipped

#### 4. DirtBowl Notes
- **Path:** `…/DirtBowlNotes.doc`
- **Date:** post-DirtBowl (~2007–08)
- **Tags:** Designer, Runtime, Process, MyTawala
- **Rel.:** High
- **Why:** Lessons from real large project: structure pain, validation+documents, MCQ values, process reuse, PayPal, Project Manager gaps — maps to current DirtBowl/:8080 work.
- **Owner evidence (Aug 1, 2026 — not from this memo):** *“Every year the sports leagues asked how to move over rosters from previous years so they wouldn't have to re-enter all that data.”* **Refinement:** end-of-season **Excel Export → archive**; reuse was **player data only** (strip graduates, add new kids) — season handoff ≈ selective Export/Import subset, not full Backup/Restore, not Library re-pull. Aligns with EXPORT/IMPORT (Excel) spine; soft open for “roll season” = export archive + import filtered roster. See `website-mock/README.md` § Library Actions / Use framing + Parked / backlog.
- **Related open (owner Aug 7, 2026 — park):** **Selective Purge by form** (keep Player/Coach, purge statistics for a new year) — ask commissioners; may or may not have been legacy. Documented under B7 + `website-mock/README.md` Open questions / Parked. Distinct from season Export/Import roster carry-forward.
- **Integrate into:** `TAWALA_PROJECT_SUMMARY.md`, open bugs / ROADMAP; do not treat as UI pixel spec

#### 5. Improvements to Project Structure
- **Path:** `…/Improvements to Project Structure.doc`
- **Date:** ~2008-01-04
- **Tags:** Designer, Website, Runtime
- **Rel.:** High
- **Why:** Proposes **Pages**, de-emphasize processes, nav/menus, validation messages — explains many Designer quirks and “submitless page” workarounds still visible.
- **Integrate into:** Architecture note / `docs/ROADMAP.md`; mostly **park** for browser Designer unless owner wants page model

#### 6. Web Application Performance
- **Path:** `…/Database/WebAppPerformance.doc`
- **Date:** 2006-10-02
- **Tags:** Runtime, Library, MyTawala
- **Rel.:** High
- **Why:** GET/WHERE cost, CLOB submissions, Library indexing/cache, Project Manager export limits — explains runtime/Library scaling assumptions.
- **Integrate into:** Ops / `docs/COMPARING_RUNTIMES.md` / park performance backlog

### B. Website / Library / MyTawala

#### 7. Project Versioning
- **Path:** `…/Project Versioning.doc` (+ `Project Version Web UI.GIF`)
- **Date:** ~2006-08-22
- **Tags:** MyTawala, Library, Designer, Website
- **Rel.:** High
- **Why:** Deployed vs non-deployed versions, Library submit rules, test-drive, upload metadata — core MyTawala/Library product contract.
- **Integrate into:** `website-mock` fidelity notes; `docs/ROADMAP.md` Phase 3; Deploy dialog behavior
- **Tenancy (owner Jul 31, 2026):** One public Library; each account’s My Tawala is private (other accounts cannot see it). **Publish** is the deliberate bridge into Library — until then, projects stay in that account’s My Tawala. Product truth in `website-mock/README.md` § Tenancy / Save·Deploy·Publish glossary (mock remains single-browser `localStorage`).
- **Library quality / community (owner Aug 1, 2026 — open questions; L&F / later product):** Pro vs amateur/community-shared ideas hard to mix with complex professionally designed projects; reviews/ratings as possible promotion mechanism (reputation gameable); community Publish may require much easier Designer (YouTube “easy to load” analogy). Framing only — not initial wiring; do not build ratings now. See `website-mock/README.md` § Library quality / community.
- **Sequencing (owner Jul 31, 2026; first slice Aug 5, 2026):** My Tawala **listing** stays **flat** (one row per project) until Delete / Purge and ops (**EXPORT / IMPORT**, **BACKUP / RESTORE**) are trustworthy enough for listing clutter; **audit trail**, deploy-switch, and delete-version stay with that later lifecycle package. **Safe first slice (owner green light):** Deploy may mint a monotonic `versionNumber` + optional description onto the My Tawala overlay / receipt, and Project Details **Versions** may **show history** (number, description, date, current/deployed mark) + optional minimal download — **not** listing version piles. B7 remains the target model. See `website-mock/README.md` glossary **Sequencing / hold**.
- **Owner smoke Aug 7, 2026 — Versions / Deploy-switch (updated after wire):** Early gap was radios that looked like Deploy-switch but did nothing. **Resolved same day:** **Deploy this version** wired for snapshot-backed rows (Deploy → Show in My Tawala saves definition snapshots). Delete-version still grey. **Restore ≠ make earlier definition version current** — mock Backup/Restore stays **data-oriented**; use Deploy this version for definition switch. **Import** field-mismatch on newer schema = **correct**. **Version Download** = minimal metadata JSON only. Honesty table: `website-mock/README.md` glossary **Owner smoke Aug 7**.
- **Owner decisions Aug 9, 2026 (lists 1+4 walkthrough):** Canonical plain-English agreements + Task List in `website-mock/README.md` § **Aug 9 decisions (lists 1+4)** (stills under `website-mock/legacy-reference/stills/`). Highlights for B7: **Publish** = new Library entry; **update existing Library app** = new version onto that entry (drop third “Update library with this version?” unless evidence returns); Library CTAs = Test Drive + Save a copy; **Active/De-activate** = temporary take-down (hidden from public Library, stays on author’s My Tawala); no ACL invite — Start-point help + Copy link + optional labels + Include-in-Web-Page embed; **Records** = project-wide submissions on list + Details; clones = Library copies into My Tawala; **Edit project in Designer** on Details only; **Make a Copy** = own fork with new uniqueId; Backup held pending package decision (proposal: deployed definition + current responses); Selective Purge = same Project Data toolbar as whole-project Purge (whole-project first).
- **Selective Purge by form (updated Aug 9, 2026):** Same UI as whole-project Purge (select form → Export / Import / Purge toolbar). Build whole-project first, then per-form; sports stats-only nuance may refine later. No longer a pure “do not implement” park — see README Aug 9 Task List item 4.
- **My Tawala listing column groups — DONE (Aug 5, 2026):** Flat listing visual groups (Project info · Data transfer · Backup · Destructive red Purge/Delete · Library transfer). Implemented in `website-mock` (`project-ops.js` + `tawala-chrome.css` + `mytawala.html`). Spec + screenshot path retained in `website-mock/README.md` § Parked / backlog item 1 (marked done). Screenshot: `/Users/DougC1/.cursor/projects/Users-DougC1-Projects-Tawala/assets/Possible_improvements-3481b37c-4d2c-46b9-a970-f886b30f8754.png`.
- **Ops verb split (Java build1700 + memos; owner Jul 31, 2026 reconciled):** **EXPORT / IMPORT** = Excel **response data** only (Import = restore messed-up data into **current** project; field mismatch fails; does **not** roll back definition). **BACKUP / RESTORE** = `.backup` ZIP = **paired project definition + data** (plus properties / links) — Restore re-applies matching definition then data (why restore held up across later field changes). **Deploy** mints My Tawala **definition versions** (shipped Java auto-deploys the new version) — separate from Backup. Designer **File → Save** = local definition only. Shipped PM UI had **no “Save” for submissions** — verbs were EXPORT / IMPORT / BACKUP / RESTORE; owner colloquial “Save” may have meant Backup. Product framing: `website-mock/README.md` glossary. **Open (park — do not redesign):** exact product meanings of Export / Import / Backup / Restore still TBD with owner; some **functions** may have export limited to the fields they incorporate (possible **standalone** vs project-level E/I/B/R). **Aug 7 clarification:** mock Restore today is narrower / data-oriented — do not equate it with Deploy-switch to an earlier definition version.
- **Paired snapshot (reconciled Jul 31, 2026):** The earlier “perhaps Save must also preserve project” tension is resolved against Java: **Backup / Restore already was** the paired definition + data path; data-alone was **Export / Import**. Not a My Tawala version-pile feature (sequencing hold stands). See glossary + **Project Backup** note below.

#### 8. Private Invitations
- **Path:** `…/Private Invitations.doc`
- **Date:** ~2007-04-18
- **Tags:** Runtime, Process, Designer, MyTawala
- **Rel.:** High
- **Why:** Memo title emphasizes invite-only / InvitationToken. **Owner Jul 31:** everyday use is **in-project Form links** (DirtBowl AdminDash); conflate with Insert→Hyperlink so **Form link = primary**, external URL = secondary special case, private InviteeID = tertiary. Not two equal menu peers.
- **Integrate into:** `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § unified Link/Invitation framing (Jul 31, 2026)

#### 9. Hiding Form Names
- **Path:** `…/Hiding Form Names.doc`
- **Date:** ~2007-09-30
- **Tags:** Runtime, MyTawala
- **Rel.:** High
- **Why:** Form name → random URL tokens on deploy to My Tawala; security of admin URLs.
- **Integrate into:** Runtime/deploy URL docs; website test-drive link design

#### 10. Web Application Themes
- **Path:** `…/Web Application Themes.doc`
- **Date:** ~2006-09-11
- **Tags:** Runtime, Designer, Website
- **Rel.:** High
- **Why:** Default/standard/custom CSS themes, per-user vs per-project storage, `tawala.` namespaces — matches theme CSS under Tomcat/docker.
- **Integrate into:** Runtime CSS / Styles UI; `DESIGNER_MENU_SPEC` Styles section

#### 11. Customizing Web App Appearance
- **Path:** `…/Customizing Web App Appearance.doc`
- **Date:** ~2007-05-17
- **Tags:** Website, Library, Designer
- **Rel.:** High
- **Why:** Replaceable images/logo, theme pick, preview, preserve in project — Library customizer appearance step.
- **Integrate into:** `website-mock` customize flow; park until site customize UI

#### 12. Emailing from Tawala Apps
- **Path:** `…/Emailing from Tawala Apps.doc`
- **Date:** ~2007-05-14
- **Tags:** Process, Runtime, MyTawala
- **Rel.:** High
- **Why:** Async send queue, quotas, bounce, delayed send — explains why SEND ≠ SMTP inline; Email Delivery product.
- **Integrate into:** `docs/EMAIL_DELIVERY_OPS.md`; `DESIGNER_PROCESS_STATEMENTS_SEND.md`

#### 13. UI Outline (customizer)
- **Path:** `…/Customizable Web apps/UI Outline.doc`
- **Date:** ~2007
- **Tags:** Website, Library, MyTawala
- **Rel.:** High
- **Why:** End-to-end customizer path: Appearance → Content → Save → Publish → Send; Signup Sheet as lead app; drop-off lessons.
- **Integrate into:** `website-mock/README.md`, Library mock UX
- **Note:** Save / Publish here are the customizer verbs; account tenancy (private My Tawala vs one public Library) is clarified under B7 + `website-mock/README.md` § Tenancy.

#### 14. Essentials of a simple and great Customization UI
- **Path:** `…/Customizable Web apps/Essentials of a simple and great Customization UI.doc`
- **Date:** ~2007
- **Tags:** Website, Library
- **Rel.:** High
- **Why:** Customizer UX principles (WYSIWYG, play vs live, logo/theme, one lead app).
- **Integrate into:** Website mock acceptance criteria

#### 15. Customization Goals / Evolution / Form Customization
- **Path:** `…/Customizable Web apps/*.doc` (Bockman, Apr–Jun 2007)
- **Date:** 2007-04–06
- **Tags:** Website, Designer, Library
- **Rel.:** High
- **Why:** Why setup-form customization doesn’t scale; browser Form Customizer; eliminate special setup forms.
- **Integrate into:** ROADMAP / Library product narrative; **park** full Form Customizer

### C. Strong Medium (still Phase 1 useful)

#### Project Backup
- **Path:** `…/Project Backup.doc`
- **Date:** ~2007-11-28
- **Tags:** MyTawala
- **Rel.:** Med
- **Why:** Project Manager backup/restore on Project Details — paired with the EXPORT/IMPORT vs BACKUP/RESTORE split (see notes).
- **Integrate into:** MyTawala project ops mock; park if not shipping soon
- **Note (Java build1700 + owner Jul 31, 2026 reconciled):** Do **not** conflate Excel data tools with Backup. **EXPORT / IMPORT** = Excel **response data** only; **Import** restores messed-up **data** into the current project (field mismatch fails; does **not** roll back definition). **BACKUP / RESTORE** = `.backup` ZIP = **paired project definition + data** (plus properties / links); Restore re-applies matching definition then data. Separate from B7 **Deploy** definition versions and from Designer File→Save. Shipped PM had no submissions “Save” — those four verbs. See `website-mock/README.md` glossary **Ops verb split**.
- **Owner smoke Aug 7, 2026:** Expected Restore ≈ switch to earlier **definition version**; mock Restore is **data-oriented** (properties + submissions). Definition switch = **Deploy this version** (wired Aug 7 for snapshot-backed rows). Import schema-mismatch failure = correct. See B7 Aug 7 bullets + `website-mock/README.md` **Owner smoke Aug 7**.
- **Paired snapshot (reconciled Jul 31, 2026):** Backup/Restore **already was** the paired definition + data path that earlier notes speculated Save/Import might need; Export/Import stayed data-alone. Not a version-listing feature. See B7 note + glossary.

#### Web Application Error Handling
- **Path:** `…/Web Application Error Handling.doc`
- **Date:** ~2006-02-14
- **Tags:** Runtime
- **Rel.:** Med
- **Why:** Error classes, logging, prevention at startup.
- **Integrate into:** Runtime ops notes; low priority for mock

#### Updates to Project Format
- **Path:** `…/Updates to  Project Format.doc`
- **Date:** ~2007-12-26
- **Tags:** Designer, Runtime
- **Rel.:** Med
- **Why:** Proposal: separate `<fields>` from HTML `<ui>` segments for WYSIWYG — may be aspirational vs shipped XML.
- **Integrate into:** Compare to actual project XML before integrating; likely **park**

#### Customization Improvement
- **Path:** `…/Customization Improvement.doc`
- **Date:** ~2007-05-24
- **Tags:** Website
- **Rel.:** Med
- **Why:** SurveyMonkey / Wufoo / CircleUp competitive notes.
- **Integrate into:** Website UX inspiration only

#### Desirable features for customization
- **Path:** `…/Customizable Web apps/Desirable features….doc`
- **Date:** 2007-05-24
- **Tags:** Website
- **Rel.:** Med
- **Why:** Workshop framing + feature wishlist.
- **Integrate into:** Fold into website criteria with Essentials/UI Outline

#### Style Guidelines / Nomenclature
- **Path:** `…/Customizable Web apps/`
- **Date:** ~2007
- **Tags:** Website, Designer
- **Rel.:** Med–Low
- **Why:** Naming/style conventions for customizable apps.
- **Integrate into:** Sample if Styles/Library copy needs consistency

#### Database Initial Production Model.jpg
- **Path:** `…/Database/`
- **Date:** ~2006
- **Tags:** Runtime
- **Rel.:** Med
- **Why:** Visual schema companion to performance memo.
- **Integrate into:** Keep with WebAppPerformance; no text extract

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
- SportsDashboards: development list spreadsheet only. **Owner Aug 1, 2026:** annual roster carry-forward ask from sports leagues (see DirtBowl Notes §4) — historical path was Excel Export archive + selective player Import (not Backup/Restore-all); product need for SDT / season handoff on that spine; still ignore the spreadsheet until that backlog opens.  
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
- **Aug 9 decisions (canonical; grew from Aug 7–8 triage):** `website-mock/README.md` § Aug 9 decisions (lists 1+4) — plain-English product agreements + Task List; stills in `website-mock/legacy-reference/stills/`
