# Field-reference audit — Jul 28, 2026

**Repo:** `/Users/DougC1/Projects/Tawala`  
**Scanner:** `scripts/audit-field-refs.mjs` (report-only; no FIB alternate-label invention)  
**Corpus:** `~/Projects/Tawala Projects/Library Projects/`  
**Amended copies:** `Library Projects/Amended/`

```bash
cd ~/Projects/Tawala
node scripts/audit-field-refs.mjs
node scripts/audit-field-refs.mjs "/path/to/folder-or.json"
```

Related Jul 22–24 hazards: `scripts/triage-library-json.mjs`.

## Rules (do not simplify)

| Shape | Meaning |
|-------|---------|
| `<<Name>>` | Process **variable** |
| `<<Form:Field>>` / `Form:Field` | Current submission blank |
| `<<Record:Form:Field>>` | Stored row (MQL, many Wheres) |
| `<<RecordName:Form:Field>>` | ForEach loop row |

**Out of scope this pass:** inventing / rewriting FIB alternate labels.

## Hazard vs info (refined Jul 28 afternoon)

| Kind | Severity |
|------|----------|
| `bare_token_matches_form_blank` | **Hazard** — bare `<<Email>>` matches a form blank |
| `record_scoped_missing_Record_prefix` | **Hazard** — MQL/SUM cell `Form:Field` without `Record:` |
| `record_missing_form` | **Hazard** — `Record:FIBn:a` without form |
| `foreach_missing_recordName` | **Hazard** — empty ForEach record name |
| `dup_blank_label_across_forms` | **Hazard** only if a bare `<<label>>` also exists |
| `info_dup_blank_label_qualified_refs` | Info — shared alternate label; refs already `Form:`-qualified |
| `info_dup_item_label_across_forms` | Info — shared `Q1` / `Q1:a` item labels |
| `info_foreach_body_current_form_field` | Info — `<<Form:Field>>` inside ForEach (often current submission on purpose) |
| `info_get_where_form_field` | Info — Get/Delete Where uses current-form `Form:Field` (common multi-select filter) |

## Library batch — Amended + Deploy (all clean)

Order: mid-size first; **DirtBowl last**. Only mechanical Record: fix was Multiple Question Survey MQL columns.

| Source | Amended copy | uniqueId | Startpoint smoke |
|--------|--------------|----------|------------------|
| `xSophisticated SignupSheets.json` | `xSophisticated-SignupSheets-field-audit.json` | `myfjra8kynhbh94` | SignUp, ShowSignups, Administrator — 200 ok |
| `XMultiple Question Survey.json` | `XMultiple-Question-Survey-field-audit.json` | `n3z79nyexvmvrkj` | Survey, Report — 200 ok (**MQL Record: fix**) |
| `XSign-up Sheet.json` | `XSign-up-Sheet-field-audit.json` | `1lyt8pokpjf5wc5` | Customize, Questionnaire, Administration — 200 ok |
| `XSophisticated Sign-up Sheet Variant.json` | `XSophisticated-Sign-up-Sheet-Variant-field-audit.json` | `4ncf4jlcl110xhu` | Administrator, SignUp, ShowSignups — 200 ok |
| `XSophisticatedPoll or Survey.json` | `XSophisticatedPoll-or-Survey-field-audit.json` | `o75v2k3zom5y8is` | Questionnaire, Administration, CustomizationPreview, Setup — 200 ok |
| `XDirtbowl.json` (last) | `XDirtbowl-field-audit.json` | `dbtrxb6f8ms22qq` | Registration, AdminDash — 200 ok (no mechanical rewrites) |

All Deploy via `POST :3001/api/deploy` → Java `:8080`. No `No class registered` in startpoint HTML.

### Only mechanical fix (MQ Survey)

Report MQL columns: `<<Survey:Name|Hand|Hair|Age|Status>>` → `<<Record:Survey:…>>`.  
RESPONSE TOTALS `field: Survey:Hand` left unchanged (names the MCQ to tally).

### DirtBowl note

Shared labels (`FirstName`, etc.) and Form: refs inside ForEach / Get Where are **info**, not auto-fixed — rewriting would break current-submission semantics. Copy is rename-only for Deploy isolation.

---

## Batch — JSON Library Sort + Being Reconverted (Jul 28 afternoon)

**Script:** `scripts/batch-field-audit-trees.mjs`  
**Machine log:** `~/Projects/Tawala Projects/FIELD-AUDIT-BATCH-JUL28.json`  
**Amended:** `JSON Library Sort/Amended/`, `Being Reconverted/Amended/` (see each `README.txt`)

Skipped: `00*` leaves, `_warn-analysis.json`, `DEPLOY-SMOKE-REPORT.json`.

| Tree | Files | Deployed OK | HOLD |
|------|------:|------------:|------|
| JSON Library Sort | 14 | **14** | 0 (Potluck fixed: MQL/SUM/Where → `Record:`) |
| Being Reconverted | 10 | **9** | 1 SportsDashboards |

**Extra mechanical fixes this batch:** Sign-up Sheet w Email MQL `Record:Form 1:*`; Potluck Main Menu + XBB Potluck variant SUM/Where `Record:`.

**HOLD — SportsDashboardsTemplateVersion3:** 10× shared blanks also used bare (`<<CustomQuestion10–15>>`, `<<Action>>`, `<<Role>>`, …). Amended copy kept; **not Deployed**. Needs Form: qualification later.

## Still out of scope

- SportsDashboards Form: qualification / Deep Backup full vet  
- Inventing FIB alternate labels  
- Website / Library product rebuild  
