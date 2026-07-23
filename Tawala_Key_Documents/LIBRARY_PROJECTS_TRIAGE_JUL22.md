# Library Projects triage — Jul 22, 2026

Source folder: `Projects/Tawala Projects/Triage group/Best/Library Projects`

Projects scanned (converted from `.tawala` today):

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

## Results

**None of the Jul-22 “blank SUM / nested font envelope” failure modes showed up** in these three JSONs.

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
