# SignupSheets XML → JSON conversion gaps

One-off conversion of legacy `SignupSheets.tawala.xml` (Designer build 228, format 1.13) into Open Project–ready JSON format 2.0.

| Artifact | Path |
|----------|------|
| Source XML | `designer-web/public/samples/legacy/SignupSheets.tawala.xml` |
| Converter | `scripts/convert-signupsheets-xml-to-json.mjs` |
| Output JSON | `designer-web/public/samples/signup-sheets.json` |
| Mapping reference | `TAWALA_XML_TO_JSON_MAPPING.md` |

Re-run: `node scripts/convert-signupsheets-xml-to-json.mjs` from the repo root.

This is **not** a general `.tawala` importer (File → Open still does not load `.tawala`).

---

## What converted cleanly

- **Project shell** — name `SignupSheets`, `format: "2.0"`, `_originalFormat: "1.13"`, themePath preserved as `mvsc`
- **9 forms** with `startPoint`, `process` / `preProcess`, `themePath`, ordered items:
  - Setup, ManageSheets, Administrator, Sheet, SignUp, SignupSheet, AlreadySignedup, DeleteRecord, ShowSignups
- **Form items** — heading (main/sub + `<<field>>` content), text (plain + rich blocks), FIB (prompt, blanks, alternateLabel, height), MC (static + dynamic), hidden fields, page breaks, skip instructions
- **12 processes** / **147 commands** — `comment`, `set` (including arithmetic `<<Count>> + 1`), `get`/`where`, `foreach`/`do`, `if`/`then`/`else`, `skip`, `show` / `showDocument`, `edit` (+ condition), `delete`, `send` (to/from/subject/body)
- **8 documents** — rich paragraph trees with field nodes (`Confirmation`, `Message`, …)
- **Structured itemization** on Form Text → `itemizationTable` with `form` + `columns[{header,field}]` (same shape as `templates/signup-sheet.json`)
- **Dynamic MCQ** → `choiceSource: "stored"` + `choices: [{ type: "dynamic", sourceForm, displayExpr, valueExpr, where? }]` (DirtBowl shape)

---

## Dropped / approximated / stubbed

| Legacy feature | Conversion behavior |
|----------------|---------------------|
| `<pageHeader>` | Dropped (not in JSON schema; theme absorbs styling) |
| Global `<styles>` | Absent in this project; would be dropped if present |
| `<imagedef>` | None in this project; not in JSON schema |
| `<rawHtmlData>` on documents | Would be stripped if present |
| Theme `mvsc` | Kept as string; local Designer/Tomcat may not ship that theme CSS |
| MC `paddingBottom="false"` | Stored on item as `paddingBottom: false` (Designer UI may ignore) |
| FIB prompt with italics / multi-font | Collapsed to plain prompt string |
| Heading / text field refs | Emitted as `<<Name>>` or rich `{ type: "field", name }` |
| Document invitations | Preserved as `{ type: "invitation", form, text, project }` nodes |
| Itemization multi-form sources | Primary form kept; extra forms logged (e.g. PlayerData: SignupSheet + Setup) |
| Itemization / column `display-conditions` | Preserved on column as `displayCondition` + table `where` when present |
| Item displayConditions on FIB | Preserved as `displayCondition` on the item |
| Send email | Full JSON `send` command; runtime/deploy email path still limited |

Warnings printed by the script at convert time (42 on last run) cover the same points.

---

## Browser Designer holes this project surfaces

Concrete gaps vs editing / round-tripping this project in `designer-web`:

1. **No `.tawala` import product** — only this one-off script; README still notes File → Open does not load `.tawala` (`designer-web/README.md`).

2. **Item `displayCondition` not editable in UI** — SignupSheet Q2–Q10 hide fields based on setup flags. JSON keeps `displayCondition`, but there is no property-panel control (`designer-web/src` has no `displayCondition` usage). Export via `jsonToXml.mjs` also does not emit `<displayConditions>` today.

3. **Dynamic MCQ “Configure Function” deferred** — five stored choice lists (filters on SignUp Q1, Administrator Q4, ShowSignups Q3, etc.). Canvas supports `choiceSource: "stored"` but the configure dialog is stubbed (`McqCanvasRow.tsx`). Filters on `where` are not editable.

4. **Itemization column display-conditions & table `where`** — ViewSheet / PlayerData columns gate Parent/Email/… and filter by sheet/player. Form Text badges edit header/field/form (`FunctionTableBadge.tsx`, `structuredItemizationEdit.ts`) but not per-column conditions or record filters in the UI. **Jul 13:** `jsonToXml.mjs` now emits nested `<conditions>` filters and column `<display-conditions>` (fixes “name appears in every sheet” on Deploy). UI editing of those filters still deferred.

5. **Document invitations** — ViewHeader / PlayerHeader / ViewSignupReturn use `<invitation>`. Nodes are stored; Document WYSIWYG (`DocumentEditor.tsx` → `blocksToHtml`) drops invitation / itemization / font wrappers when loading rich blocks into HTML, so re-saving from the editor can lose them. Prefer leaving these docs as structured JSON until invitation insert lands.

6. **Document itemization tables** — Same lossy HTML path as above; DirtBowl-style rich `content` arrays survive Open Project but not a full Document editor round-trip.

7. **Theme `mvsc`** — Project/form `themePath: "mvsc"`. **Jul 12:** local stub added at `docker/tomcat/css/project/mvsc/project.css` (also copied into running Tomcat) so Deploy/Send no longer 404s missing CSS. Full visual parity still deferred. Confirmation **Send** still needs local SMTP if Email is collected (known mail backlog).

8. **Send / email** — Post-SignupSheet and Post-AlreadySignedUp `send` commands convert cleanly; browser Designer + local runtime still do not fully author or deliver confirmation mail (known backlog; README “Sign-up Sheet w Email”).

9. **Multiple start points** — Administrator, SignUp, and ShowSignups are all `startPoint: true` (legacy multi-entry). Explorer can toggle start point per form; confirm UX matches owner expectation for multi-entry projects.

10. **Export fidelity** — `server/jsonToXml.mjs` is best-effort. **Fixed Jul 12:** itemization-table v2 column headers now emit `<string value="…"/>` (plain-text headers caused Java `/client` to return the public Error HTML page). Remaining gaps: displayConditions, dynamic MCQ `where`, itemization filters/column conditions, invitation auth, rich heading field tokens, and `paddingBottom`. Use the generated JSON for Designer inspection; spot-check XML on Deploy for advanced filters.

---

## Suggested smoke test (owner)

1. Designer → **Open Project…** → `designer-web/public/samples/signup-sheets.json`
2. Open forms Setup, SignUp, SignupSheet — confirm item lists and itemization badge on Setup T2
3. Open process Pre-Signup — confirm nested get/foreach/set arithmetic
4. Open document Confirmation — expect limited WYSIWYG if loaded as rich blocks; ViewSheet may look empty in the HTML editor even though JSON still has the itemization table
5. Optionally Deploy and compare against legacy behavior on 8080 (theme `mvsc` may need attention)
