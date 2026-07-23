# Designer session EOD — Jul 22, 2026

Branch: `cursor/forms-canvas-wysiwyg`

## Done today

### Deploy / runtime layout
- **MQL tables:** stopped auto `dtFixTableWidth` empty right “blank box”; content-sized by default; column drag-resize kept (`ItemizationTable.java`, `default.js`, Preview).
- **List/MQL tables:** content-fit + `white-space: nowrap` + **6in** max (`--tawala-list-table-max-width`) in `form-layout-core.css` + Preview CSS + `fitTableToContent()` in `default.js`.
- **FIB layout lock:** `form-layout-core.css` loaded last via `CommonTheme` / `UserDefinedTheme`; multi-row Align padding; freeform `div.fib` soft-row spacing.

### FIB same-line blanks
- Stopped exporter from splitting `Email ____ Email (again) ____` / `First ____ Last ____` into separate Align rows (`fibToXml` — preserve Design soft-rows; Java remainder keeps them one line).

### Potluck Details SUM
- Redeploy blank Adults/Kids totals: Document **table cells** double-wrapped `<font><font><sum>` → Java dropped sum. Fixed `documentHtmlToXml` cell wrap + SUM `Record:Form:Field`.

### Other (same working tree / recent)
- Instructional Text unbold / image styling Deploy path; embedded image resize mins; theme wiring tests; FIB prompt / rich export hardening; sample template JSON stubs removed from `public/samples/templates/` (prefer `.tawala` import / Library).

### Specs / tests
- Updated `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md`, `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`, `DESIGNER_FORM_FORMAT_TOOLBAR.md`.
- Unit tests: `formLayoutCore`, `documentHtmlToXml` (Potluck SUM table), `fibToXml` same-line, theme CSS.

## Still to do (owner)

### Finish 3-browser smoke (in progress)
- Owner smoked Main Menu / Start projects on primary browser.
- **Still open:** Safari + Edge (and confirm Chrome) for the same Main Menu starter set — layout, Deploy, forms/Documents, MQL tables, FIB same-line, Potluck SUM totals after Redeploy.

### Immediate Redeploy checks
- Potluck Details/Report: Redeploy with today’s API → Adults/Kids SUM numbers.
- Hard-refresh Deploy CSS (`form-layout-core.css`) so 6in table fit + FIB row gaps apply (cache-buster `?x=local-…` can stick).

### Library Projects triage (Jul 22 evening)
- Folder: `Projects/Tawala Projects/Triage group/Best/Library Projects` — `Dirtbowl.json`, `Poll or Survey.json`, `SignupSheets.json` (converted from `.tawala` today).
- Automated scan for Jul-22 hazards (nested font, table-hosted SUM without Record:, orphan `Your >:`, etc.): **no hits** on those three.
- Notes: `LIBRARY_PROJECTS_TRIAGE_JUL22.md` (repo) and same file beside the Library folder. No `Amended/` copies needed from auto-fix.
- Still smoke Open → Redeploy for any Document totals tables.

## Designer track — remains (park / backlog)

Priority themes from scopes + open todos (not all blocked on Jul 22):

1. **Keep Deploy contracts solid** per form item (FIB / MQL / Document functions) — continue Redeploy smoke after export/CSS changes.
2. **MCQ “choices from stored data”** (owner queue).
3. **One solid `.tawala` Open → Deploy** path for Library/vetted projects; swap Main Menu / New Project starters when vetting finishes.
4. **Document export gaps** — remaining deferred function stubs; Document invent/chip polish.
5. **Park until “basically finished”:** look-and-feel, full menu audit, Font/Size mixed-run honesty leftovers, long chip / MDI chrome.

**Ops reminder:** Designer API `:3001` with `TAWALA_JAVA_URL=http://localhost:8080`; Tomcat `:8080`; hard-refresh after CSS `docker cp`.
