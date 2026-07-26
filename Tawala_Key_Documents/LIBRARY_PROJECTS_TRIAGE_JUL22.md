# Library Projects triage — Jul 22, 2026

Source folder (historical Jul 22–23): `Projects/Tawala Projects/Library Projects`  
(Jul 22 scan used `Triage group/Best/Library Projects`; files were moved up Jul 23.)

**Scanner (saved Jul 24):** `scripts/triage-library-json.mjs` — Jul 22 hazards plus Jul 23–24 checks (Heading Main/Sub, Page Header, rich FIB/MCQ nested font). Report-only by default; `--amend` only qualifies SUM `Record:` prefixes.

```bash
cd ~/Projects/AI-Tawala
node scripts/triage-library-json.mjs
node scripts/triage-library-json.mjs "/path/to/other/folder"
```

---

## Jul 25 — Library sort paused (owner)

**Sort status:** Finished for now. Working tree under `~/Projects/Tawala Projects/`:

| Folder | Role |
|--------|------|
| `JSON Library Sort/` | Settled Main Menu / 1b / unique copies + deploy smoke reports |
| `files to reconvert to JSON/` | **`.tawala` sources** to re-run through current converter (`scripts/tawala-to-json.mjs`) |
| `Being Reconverted/` | Incomplete / early JSON — **do not fully evaluate** until matching `.tawala` is reconverted |
| `XLibrary Projects/` | Parked Library candidates (leading `X` / `x`) pending reconvert or Designer fixes |
| Desktop `Deep Backup Tawala` | Large projects not yet fully Deploy-smoked — out of active sort |

### `.tawala` waiting reconvert (`files to reconvert to JSON/`)

BB Potluck · CFF · CFF Revised · Get Together · Horses and Penguins · Poll or Survey · Potluck · Sign-up Sheet · SportsDashboards Template.jdfFix  
(+ `XPoll or Survey.json` parked beside them)

### Three known early-conversion problems (owner Jul 25)

1. **`Record:` on function field names** — some function Configure / chips get an unwanted `Record:` prefix on fields (or the wrong shape of prefix). Triage scanner’s `--amend` only touches SUM `Record:` qualification; do **not** blanket-rewrite other `Record:` usages.
2. **Structured Text placeholder** — many Form/Document text items were replaced with: *“This text item has structured content that cannot be edited on the canvas yet…”* instead of editable canvas content. **Jul 25 evening:** converter now emits HTML tokens for fields / invitations / hyperlinks (structured arrays only for MQL tables); SportsDashboardsTemplateVersion3 reconvert confirms **0** placeholders and **−774** warn lines vs prior pass.
3. **Tables / functions not displaying** — additional unknown loss (tables, function chips) after Open — investigate after (1)–(2) with fresh reconverts; may mix converter gaps and Designer render gaps.

### Designer blockers for Library vetting (owner Jul 25)

- **Main:** browser Designer often treats **all variables as text** in situations where legacy typed variables matter — blocks full vetting of larger/process-heavy projects until fixed (see `DESIGNER_OPEN_BUGS.md`).
- Other Designer bugs found during Library sort — collect for a dedicated Designer session; do not mix into 8080 CSS work.

### Next when resuming this track

1. ~~Batch-reconvert `files to reconvert to JSON/*.tawala`~~ — **Done Jul 25** → outputs in `Being Reconverted/Reconverted-Jul25/` (9/9 OK; **0** structured-text placeholders in this batch; see that folder’s `README.txt`).
1b. **SportsDashboardsTemplateVersion3.tawala** (larger sibling) — **reconverted Jul 25 evening** after invitation/hyperlink/field HTML fixes; warns **1768 → 994**; **0** invitation-empty / hyperlink-empty warns; see `Being Reconverted/SportsDashboardsTemplateVersion3-CONVERT-NOTES.txt`.
2. Diff / smoke against parked `X*` JSONs; promote keepers back toward Library.
3. Only after Designer variable-typing (+ other blocking bugs) — full vet of big projects still in Deep Backup.

---

## Jul 22 scan (historical)

Projects scanned (converted from `.tawala` Jul 22):

| File | Notes |
|------|--------|
| `Dirtbowl.json` | Large DirtBowl Registration library |
| `Poll or Survey.json` | Survey / questionnaire library |
| `SignupSheets.json` | SignupSheets library |

## What we looked for (Jul 22 Designer hazards)

- Nested `<font><font>` / font→font wrappers that Java drops around display components (Potluck Details SUM class of bug)
- SUM / MQL chips inside HTML tables with double font wrap on Redeploy
- SUM `field` missing `Record:` prefix
- Orphan punctuation (`Your >:`)
- `font color="rgb(...)"` in XML-ish markup
- `Record:FIBn:a` without a form name

## Added Jul 24 (re-run scanner)

- **Heading Main/Sub** — mixed `heading-size-main` / `heading-size-sub` spans with no `<br>` (glue / Redeploy smoke); sticky `level` with size spans
- **Page Header** — empty `pageHeader`; image taller than ~160px (Deploy CSS cap)
- **Rich Form text** — nested `<font>` in FIB/MCQ/Text; info flags when FIB/MCQ prompts have B/I/U markup (smoke Deploy)

## Results

**Jul 22:** None of the “blank SUM / nested font envelope” failure modes showed up in these three JSONs.

**Jul 24 re-scan** (`node scripts/triage-library-json.mjs` against `Library Projects/`): still **no hazard-class findings**. Info-only: bare / `Form:Field` names, structured MQL nodes, many whole-box headings (`level=main|sub` without mixed size spans).

They use **structured Document content** (paragraph / font / bold / field / `itemizationTable` nodes), not HTML `function-token` chips. There are **zero** `data-function-id="sum"` tokens and **zero** bare nested `font→font` wrappers.

### Observed (usually OK — do not auto-rewrite)

| Pattern | Dirtbowl | Poll or Survey | SignupSheets | Interpretation |
|---------|---------:|---------------:|-------------:|----------------|
| Bare field names (`FromName`, `AdminAdrss`, `QNumber`, …) | 83 | 12 | 10 | Process/email variables & custom fields — normal |
| `Form:Field` without `Record:` (`Registration:FirstName`, …) | 19 | 1 | 2 | Common Document display refs to form fields — often correct |
| Structured `itemizationTable` nodes | 20 | 3 | 6 | Expected MQL embeds |

**No `Amended/` copies** were written — mechanical rewrite of field prefixes would risk breaking Document/email semantics.

## Owner smoke

1. File → Open each Library JSON → Redeploy key Documents/Forms.
2. Confirm MQL tables and any totals still look right under today’s `form-layout-core` / exporter.
3. Finish **Safari + Edge** (+ Chrome) Main Menu starter smoke (separate from this triage).

## Related fix (already in Designer track)

Potluck Details blank SUM on Redeploy was fixed in `documentHtmlToXml.mjs` (no nested font around table-cell SUM; `Record:Form:Field` on SUM). That matters when a project uses **HTML table + function chips**; these Library JSONs did not use that shape.
