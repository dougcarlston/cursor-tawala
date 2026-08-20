# Designer open bugs

Confirmed or likely **broken / incorrect behavior** in the browser Designer (`designer-web/`). Reviewed with owner July 10, 2026.

Unfinished features and polish: **`DESIGNER_OPEN_TODOS.md`**.  
Skipped chats (not Designer track): Website library mock; 8080 templates/Docker/Tomcat CSS; Findme Assistant.

---

## Smoke test July 10 (`b48656b`) — pass summary

| # | Area | Result |
|---|------|--------|
| 1–4 | Shell / MDI / palettes | Pass |
| 5–6 | Form insert/edit, field drag | Pass |
| 7 | Skip dialog | Pass (solid) |
| 8 | Process If/Set/Show | Pass |
| 9 | Send/Get panels | Layout OK; email validation incomplete; mail backend unimplemented → **TODOs** |
| 10–12 | Document typewriter Enter, font stick, blank click | Pass |
| 13 | Drag-select highlight | **Pass** (cross-block + multi-line align, July 10 tip) |
| 14 | fx popup | Opens; catalog/config not fully implemented → **TODOs** |
| 15 | Reset | **Removed** from palette July 10 |
| 16 | Save / reload | Pass |
| 17 | `npm run build` | Optional — see owner note |

---

## Active / deferred bugs

### Parked Jul 30 / reconfirmed Aug 11 (Not blocking for Live Library) — **fix after return ~Aug 20**

Also listed in `.cursor/rules/tawala-designer-parked-post-website.mdc` and catchup `docs/CATCHUP_MEMO_RETURN_AUG20.md`.

**Owner policy:** will not deploy projects to **Library Live** that cannot be fixed until **blocking** Designer bugs are removed. Items below are **Not blocking** for Live Library (ugly / polish), except treat #5 as **Design authoring quality** for Online Exam Setup (still not Live-Library-blocking unless Setup is the only start users hit).

**Owner Aug 11:** Recreated a lost-stash set before travel; write-ups below. Do **not** require owner on the machine to start these — screenshots are enough.

#### 1) FIB Styles — “Align right side” control almost gone (Not blocking)

- **Path:** Main Menu → Project → Styles → FIBs → dialog **“Fill in the Blank Styles”**.
- **Symptom:** Under **Blanks**, the control for **“Align right side”** is squashed — a narrow vertical blue sliver instead of a usable radio/checkbox. **Labels** radios (Above / Left justified / Right justified / Freeform) look OK.
- **Screenshots:** `Tawala_Key_Documents/assets/Bug_-_FIB-Styles-AlignRight-squashed-radio.png` (Jul 30); **reconfirm Aug 11** `…/Bug_-_FIB-Styles-AlignRight-squashed-radio-Aug11.png` (owner “Styles BUG”).
- **Fixed Aug 20:** `.form-item-styles-dialog` was overriding checkbox `width` to `auto`, which collapses `appearance:none` Win32-style boxes to a sliver. Restored 13×13 sizing + slightly wider Labels/Blanks column.

#### 2) Form Text — paragraph separation lost on Deploy + image breaks highlighting (Not blocking but ugly)

- **Path:** Forms → Text (repro on Multiple Question Survey template instructional text with inline Deploy/globe icon).
- **Symptom A:** Paragraphs created with a couple of Return key presses show blank-line separation in Design, but on Deploy/runtime the paragraphs pack together (blank-line separation lost).
- **Symptom B:** Inserting an image breaks highlighting — selection will not include any paragraphs that include the image or text beyond it.
- **Screenshots:** Design (separated) `Tawala_Key_Documents/assets/Bug_-_Text-paragraph-spacing-Design.png`; Deploy/runtime (packed) `…/Bug_-_Text-paragraph-spacing-Deploy.png` (source chat assets: `LossofParSpacing-…png`, `ParSpacing2-…png`).
- **Note:** Not in the Aug 11 recreations; keep from Jul 30 stash.

#### 3) Form canvas badges — uneven widths (Not blocking) — **NEW / reconfirmed Aug 11**

- **Path:** Form Design canvas — left Qn / `{Qn}` / SKIP / Tn badges (e.g. Campaign Dashboards or any form with mixed conditional braces + Skip).
- **Symptom:** Badge chips are different widths (`Q1` vs `{Q2}` vs orange selected `Q4` vs long `SKIP`). Owner wants a **straight vertical line down the right edge** of the normal question badges so the column reads clean.
- **Exception (owner):** **SKIP** may use a different color and may stick out past that line — Skips interrupt the flow.
- **Screenshot:** `Tawala_Key_Documents/assets/Bug_-_Form-badge-uneven-widths-Aug11.png` (owner “Uneven Labels”).
- **Fixed Aug 20:** Badge stack always **88px** (braces no longer widen the column). SKIP uses a light red chip and may grow past 88px.

#### 4) Sign-up Template — Right justified FIB labels bounce vertically (Not blocking) — **NEW / reconfirmed Aug 11**

- **Path:** Sign-up Sheet Template (or similar) with FIB **Right justified** (+ Align right side as used). Preview / Push / `:8080` form.
- **Symptom:** Labels **bounce** between vertical middle of the blank and vertical bottom of the blank (e.g. “First Name:” centered vs “Last name:” bottom-aligned on the same row; same for Email vs Telephone). Horizontal right-edge of left-column labels may also be slightly uneven.
- **Contract reminder:** Spec wants label **bottoms** aligned with field **bottoms** (`DESIGNER_FORM_FORMAT_TOOLBAR.md` / form-layout-core) — not mixed mid/bottom.
- **Screenshot:** `Tawala_Key_Documents/assets/Bug_-_Signup-FIB-right-justified-vertical-bounce-Aug11.png` (owner “Bug in Alignment”).
- **Fixed Aug 20:** `form-layout-core.css` FIB table cells use `vertical-align: bottom` (was `middle`, which mixed with remainder `flex-end`). Hard-refresh / redeploy CSS on `:8080` after `docker cp` or image rebuild.

#### 5) Online Exam Builder Setup — expanded Text font size chaotic / cross-talk (Not blocking but serious UX) — **NEW / reconfirmed Aug 11**

- **Path:** Project **Online Exam Builder** (`~/Website Staging/3-Ready-for-Public-Library` or Library JSON). Run start form **Setup**. Two large rich **Text** / instruction boxes: “Pre-test instructions…” and “Comments or instructions… upon completion…”.
- **Symptom A:** Font size control appears random — toolbar may show `4 (14pt)` while some lines are huge and others tiny; trying larger/smaller does not reliably apply.
- **Symptom B:** Formatting / content effects seem to **bleed between the two expanded Text boxes** (same phrases/sizes appearing in both after edits).
- **Screenshot:** `Tawala_Key_Documents/assets/Bug_-_OnlineExam-Setup-expanded-Text-font-size-chaos-Aug11.png` (owner “Expanded text boxes”).
- **Likely area:** Form Text contenteditable / shared format toolbar state — isolate per-editor document + selection; do not share one `document.execCommand` target across two open Text items.

### Legacy `.tawala` → JSON conversion (batch fix queue) — **Aug 2, 2026 morning**

**Owner strategy:** Systematically find conversion bugs while opening / reconverting Library apps (before another large trove of `.tawala` imports). **Accumulate a solid list → fix them all at once with tests** — not manual one-off rewrites per project. Owner will stop hand-debugging big programs once this inventory is complete enough to batch.

**Do not implement code fixes in inventory-only sessions.** When the batch is scheduled: one converter pass + unit fixtures per bug below (and any new rows the owner adds).

**Primary converter paths (cite when fixing):**

| Path | Role |
|------|------|
| `designer-web/src/lib/tawalaXmlToJson.mjs` | Shared core (`convertTawalaXmlToProject`) — CLI + File → Open |
| `designer-web/src/lib/tawalaXmlToJson.test.ts` | Focused conversion smokes |
| `scripts/tawala-to-json.mjs` | CLI: `node scripts/tawala-to-json.mjs <in.tawala\|xml> [out.json]` |
| `scripts/convert-signupsheets-xml-to-json.mjs` | Batch wrapper around CLI |
| Designer **File → Open** | Via `shellCommands.ts` → same `tawalaXmlToJson` (accepts `.json` / `.tawala` / `.tawala.xml`) |
| Mapping reference | `TAWALA_XML_TO_JSON_MAPPING.md` |
| Library reconvert ops | `LIBRARY_PROJECTS_TRIAGE_JUL22.md` · folders under `~/Projects/Tawala Projects/` (`files to reconvert to JSON/`, `Being Reconverted/`, Library sort) |
| Related scanners (do not invent labels) | `scripts/audit-field-refs.mjs`, `scripts/triage-library-json.mjs` |

**Repro / stress sources (owner):** **Online Exam Builder** (sequential FIBs; Heading `<<Customize_Title>>`; Form **Answer** MCQ/FIB `<<QNumber>>` / `<<Q>>` / `<<A>>`… tokens — C1 + C5); Library file `website-mock/projects/library/Online Exam Builder.json` (Priority Library / demo stub per `website-mock` MANIFEST); **Priority Library Projects** and Deep Backup reconverts; Jul 25 SportsDashboards / Signup-family reconverts for function-table follow-ups.

**Ops note (not conversion):** Online Exam Builder **Customize_Title Deploy** worked after a **stale `:3001` API restart** (fresh process required after `headingExport` / server edits — not a product regression).

---

#### Batch queue (open)

| # | Bug | Symptom | Notes / separation | Status |
|---|-----|---------|--------------------|--------|
| **C1** | **Sequential / contiguous FIB blanks elided** | Multiple adjacent `_` runs / `<blank>`s collapse into **one** field after convert | **Destroys connection with multiple alternate labels** — each blank should keep its own `blank.name` / `alternateLabel` (legacy multi-blank FIB). Process/Fields/MQL refs that keyed off distinct alts break. **Fix Aug 3:** `convertFib` separates adjacent underscore runs (paragraph breaks + no glued `_`+`_`); preserves FIB `alternateLabel` as `name` (e.g. MCQ choices). Regression: Online Exam-style 6 Choice blanks in `tawalaXmlToJson.test.ts`. | **Fixed Aug 3** |
| **C2** | **Form MCQ alternate labels not converted** | Owner alternate field names lost on import; MCQs appear as **Q1, Q2, …** (default `label`) instead of preserved alts | **NEW Aug 2.** Distinct from C1 (FIB blanks). Converter has `convertMc` + `alternateLabel` → `item.name` — verify XML attr / casing / placement vs canvas Fields panel use of `name` vs `label`. Add fixture from a Library MCQ with real alt. | **Open — batch** |
| **C3** | **Function tables fail convert** | Several function / itemization tables did not convert successfully (chips missing, wrong shape, or unusable after Open) | **NEW Aug 2 — open.** Owner still collecting examples. May overlap Jul 25 note “tables / functions not displaying” (`LIBRARY_PROJECTS_TRIAGE_JUL22.md`). Paths: `convertItemizationTable` / `convertSumFunction` in `tawalaXmlToJson.mjs`; do not confuse with Deploy-time MQL Where (fixed Jul 30). | **Open — examples pending** |
| **C4** | **Field tokens in Headings (and some Text)** — conversion-time residue | Deploy of literal `<<Variable>>` in Headings was **fixed** (`headingExport.mjs`: strip/lookbehind so `<<Name>>` survives; emit `<field name="…"/>`) | **Deploy path closed** (Online Exam Builder → Administration / `Customize_Title`). **Still watch conversion:** if tokens are already mangled or plain text in JSON **on import** (not on Redeploy of correct JSON), that is a `convertHeading` / `richNodesToFormHtml` / Text field-token issue — batch only if owner finds mangled JSON after `.tawala` Open. Design still shows literal `<<…>>` (legacy; not a convert bug). Spec: `DESIGNER_FORM_ITEMS_HEADING.md` § Process-variable token. | **Deploy fixed; convert residual = watch / owner evidence** |
| **C5** | **MCQ question field tokens mangled** (Design after convert / insert) | After `.tawala` → JSON Open, Form **Answer** Q2 **question** shows HTML residue like `<>) <">"` instead of chip pair `<<QNumber>> ) <<Q>>`. **Choices still show** `<<A>>`…`<<F>>` chips. Q1 **FIB** with same `<<QNumber>>` / `<<Q>>` often OK (FIB embeds tokens). Owner Aug 4: Online Exam **Question** form — FIB Q5 shows `<<Question:Question>>`; MCQ Q6 shows broken `<<Question>>`. | **Fixed Aug 4.** JSON already had plain `"question": "<<Question:Question>>"`. Bug: `McqCanvasRow` idle/edit injected question via raw `innerHTML` **without** `embedPlainFieldTokensAsHtml` (choices + FIB already used it). Browser tag-parses `<<Form:Field>>`. Now both idle and edit entry embed chips first. Regression: `fieldTokens.dom.test.ts` Form:Field + colon. | **Fixed Aug 4** |
| **C6** | **Flat form items lose all wording** | Convert OK but Form looks **empty**: Text/FIB/MC prompts stripped | **NEW Aug 3.** Older format 1.3–1.x puts `#text` and `<blank>` **directly under** `<text>` / `<fib>` / `<question>` with **no** `<paragraph>`. Converter only walked paragraph children (`children()` drops `#text`). **Public / red-list examples:** Living Will v1.0, Wildcat Week; also cs v95, Printing troubleshooter. **Fix Aug 3:** `flatItemBodyToBlocks` + flat `convertFib` body walk; test “flat (non-paragraph)…”. Reconverted under `Tawala-JSON-Conversions/converted/`. File → Open `.tawala` uses the same fix. | **Fixed Aug 3** |

#### Related (not necessarily same batch)

- Jul 25 reconvert quality rows still partly live: unwanted `Record:` on function fields; early structured-Text placeholders **largely fixed** Jul 25 evening — see `LIBRARY_PROJECTS_TRIAGE_JUL22.md` and TODO #16 in `DESIGNER_OPEN_TODOS.md`.
- Variables-as-text / Set expression typing: **Deploy export largely fixed Jul 27**; remaining Library gaps are separate from this convert queue.
- Prefer **reconvert from `.tawala`** after the batch fix over hand-editing broken JSON for big apps.

---

### Fixed Jul 30 — MQL Where clause not wired on Deploy (blocking) — **re-opened then fixed with evidence**

- **Symptom:** Configure Function → MULTIPLE QUESTION LIST → **Limit output to records where** (e.g. `Form 1:lastName equals Carlston`) showed correctly in Design, but Java Deploy/runtime listed **all** signups (or nearly all).
- **Root cause (1st pass):** Structured `itemizationTable` Deploy only read legacy `where`, not Configure `conditions` / `conditionsRows`.
- **Root cause (2nd pass):** Present-day chips are HTML `<<MULTIPLE QUESTION LIST(...)>>` + `data-function-config`; Design can also show a legacy `{ MULTIPLE QUESTION LIST }` brace chip in the **same** Text item (owner dual-chip screenshots).
- **Root cause (3rd pass / DB evidence):** Emission of `equals Carlston` **works** when `conditionsRows` is in the project JSON (proof deploy `MqlWhereProof*` in Postgres has `<equals field="Record:Form 1:lastName"><string value="Carlston"/>`, and Java runtime filtered to Carlston-only). Owner’s latest `Signup-sheet` deploy (`duj5twj0tohr94q`) stored **`<isNotBlank field="Record:Form 1:lastName"/>`** instead — looks like a full table. Design could show Carlston on the live chip while Redeploy used the **store** (Configure OK sometimes no-op’d `commit` when the palette handle didn’t match the editor).
- **Fix:** (1) Never no-op Configure commit — pass explicit `commit` from TextCanvasRow / RichTextEditor. (2) After MQL Configure, remove sibling legacy brace chips. (3) On Open / New Project, migrate structured / brace MQL → modern function-token HTML. (4) Deploy: legacy structured-node chips emit Where; if modern + legacy coexist, drop the legacy table. Proof script: `node designer-web/scripts/dump-mql-where-xml.mjs`.
- **Verify:** Restart `:3001` → **New Project → Sign-up Sheet** → chip is `<<MULTIPLE QUESTION LIST(...)>>` (not `{ }`) → Configure Where `lastName equals Carlston` → confirm chip text includes Carlston → **exit the Text row / click another item** so store commits → Redeploy → table shows only Carlston rows. Or: `node designer-web/scripts/dump-mql-where-xml.mjs` must print the equals snippet.

---

### Fixed Jul 30 — FIB prompt bold lost on Deploy (MQS `topLabels`)

- **Symptom:** On Multiple Question Survey, MCQ labels stayed bold after Deploy; FIB “Name:” / “Age:” did not — even when bolded in Design. (Related to parked Text-spacing item only as same template; separate root cause.)
- **Root cause:** `fibToXml` mirrored Design B/I/U only on the **freeform** path. MQS FIBs use `style="topLabels"` (and left/right-align also stripped HTML via plain `parseFibPrompt` segments). MCQ plain questions auto-get `<b>` in `mcToXml`.
- **Fix:** `topLabels` / leftAlign / rightAlignJustified now use the same rich walker as freeform when the prompt has formatting. Unit: `server/fibToXml.test.mjs` (topLabels + leftAlign bold cases).
- **Verify:** Bold FIB Name:/Age: in Design → Redeploy (restart `:3001` after server edits) → Java form shows bold FIB labels.

---

### Variables treated as text (Library vetting blocker — owner Jul 25) — **Set-statement math/concat fixed Jul 27**

- **Symptom:** Browser Designer treats **variables as text** in most situations where legacy Designer honored typed / non-text variables (process, conditions, function Where, etc.).
- **Root cause (Process `Set` — narrowed Jul 27):** the value/expression is never actually a stored "type" — Java's runtime `Value`/`SmartNumber` duck-types everything as a string and only coerces to a number when an `<add>/<sub>/<mul>/<div>` operator reads it (see `TawalaWebapp-build1700/src/com/tawala/project/commands/{SetCommand,StringConcatenationExpression,SmartNumber}.java`). The bug was entirely in **Deploy export**: `designer-web/server/jsonToXml.mjs`'s `valueToXml()` only special-cased the single pattern `<<field>> + literalDigits` as `<add>`; every other expression with operators (`field + field`, `-`, `*`, `/`, parens, quoted text) fell through to a flat `<string>`/`<string field>` concatenation that **baked the literal operator character into the exported text** (e.g. `SET c = <<a>>+<<b>>` deployed as the string `"1+2"`, never `3`). The existing "Treat arithmetic expression as text" checkbox (`arithmeticAsText` on the `set` command) was **already wired into the UI and round-trips through XML**, but had **no effect on the exported XML body** — it only toggled an XML attribute Java doesn't even read.
- **Fixed (Jul 27, Deploy export only — no Design canvas / `*CanvasRow` changes):** `designer-web/server/jsonToXml.mjs` now has a real expression compiler for Set/condition values (`compileSetExpression` + a shunting-yard parser), applied only when the value contains `+ - * / ^` operator tokens (values with none are untouched — same output as before):
  - Numeric operands (bare digits, or `<<field>>` — whose actual type follows the field's own runtime value, e.g. a numeric-validated FIB blank) compile to nested `<add>/<sub>/<mul>/<div>` XML with proper precedence and `()` grouping — matching the Java `Operator`/`ContainingOperator` factory exactly (verified against `TawalaWebapp-build1700/test/.../SetCommandTest.java`).
  - `^` (exponent): Java has **no** pow operator. A literal non-negative integer exponent (e.g. `<<a>> ^ 3`) is expanded into repeated `<mul>`; a field or fractional/negative exponent has no Java equivalent and safely falls back to literal text export (documented gap, not silently wrong).
  - A **quoted literal is always text** (owner rule: `"123"` → text, never numeric), even though it isn't part of the legacy C# grammar — this is a new rule the owner asked to implement, not legacy parity.
  - Text operands joined only by `+` compile to sibling `<string value>/<string field>` nodes (dropping the `+`) — Java's `StringConcatenationExpression` already concatenates multiple `<string>` siblings, so `SET dessert = "ice"+"cream"` deploys as `icecream` with **zero Java changes**.
  - Text operands joined by any operator **other than** `+` (undefined by the owner's rules, e.g. `"ice" - "cream"`) fall back to the original literal string, unchanged — same safe behavior as an unparseable expression.
  - **Heads up for content authors:** any Set value containing a bare `+ - * / ^` character is now interpreted as an operator (matches legacy `Expression.isArithmetic()`'s same "contains any of + - * /" heuristic, plus new `^` support) instead of being passed through literally. Literal text that happens to contain those characters (phone numbers, score lines like `3-0`, fractions typed as `3/4`) needs the **existing** "Treat arithmetic expression as text" checkbox — exactly the legacy escape hatch, now functional.
  - Also improves Process **If** condition values for free, since `conditionValueXml` shares the same `valueToXml`; not separately tested this session (owner asked to scope to Set).
- **Tests:** `designer-web/server/jsonToXml.test.mjs` → `describe("Process Set expression typing (numeric vs text)")`, 19 cases covering the owner's rules 1–6 plus precedence/parens/`^`/fallback cases, **plus 3 cases (Jul 27 follow-up) using the exact real "Horses and Penguins Test" project patterns**: self-referencing `Set Score to <<Score>> + 1`, reversed-operand `Set Wrong to 17 - <<Score>>` (number first, field second), and chained `<<Score>> * 100 / 17`. `npm test` in `designer-web/` passes (2 pre-existing unrelated failures: heading Main/Sub split, `form-layout-core.css` comment-strip assertion — both predate this change).
- **Jul 27 follow-up — owner reported the fix "didn't work" on the real Horses and Penguins Test project.** Root cause was **not** a code bug: `compileSetExpression` already handled the project's exact Sets correctly (verified by running `projectToXml` directly against `Tawala Projects/Being Reconverted/XHorses and Penguins__suggested-primary.json` — `<<Score>> + 1` → `<add>`, `17 - <<Score>>` → `<sub>`, `<<Score>> * 100 / 17` → `<div><mul>…`). The real cause: **the local dev API (`designer-web/server/index.mjs`, port 3001) was a plain `node` process with no file-watch/reload**, started *before* the `jsonToXml.mjs` fix was written to disk, so the owner's Deploy/redeploy was still hitting the pre-fix code in memory. `npm run dev`/`dev:api` has no nodemon/tsx-watch — editing server files never takes effect until the API process is restarted. Fixed by killing the stale PID and restarting via `designer-web/scripts/ensure-dev-api.sh` (that script only restarts when the health check is *down* — it will silently keep serving a stale-but-healthy process, so after any server-side Deploy/XML fix, kill the port-3001 process explicitly before running it, don't just call the script). **Takeaway: after any `designer-web/server/*` change, always restart the dev API before telling the owner to redeploy and verify.**
- **Smoke test:** New Process with `SET a = 1`, `SET b = 2`, `SET c = <<a>>+<<b>>` (drag `a`/`b` from Fields panel so they're real `<<field>>` tokens, not bare typed names) should Deploy `c` as `3`, not `"1+2"`; `SET dessert = "ice"+"cream"` should Deploy `dessert` as `icecream`. Verify via **Deploy** (`node scripts/deploy-dirtbowl-java.mjs`-style flow or the Designer's own Deploy dialog) + a Document/Show that prints the variable, since the Design canvas UI itself was already correct (only Deploy XML was wrong).
- **Impact:** Blocks full vetting of larger Library / Deep Backup projects even after `.tawala` reconvert. Remaining scope (conditions, function Where, deeper Library examples) still open — collect concrete examples in a Designer chat if more gaps surface.
- **Related Library note:** `LIBRARY_PROJECTS_TRIAGE_JUL22.md` § Jul 25 — reconvert queue; **Jul 25 evening** invitation/hyperlink/field HTML converter fixes landed (SportsDashboards warns 1768→994). Structured-content / `Record:` converter gaps for MQL `where` are separate from the variables-as-text bug.

### Deploy data isolation (Jul 22 Safari smoke)

- **Symptom:** Safari multi-app smoke looked like “projects sharing data” (signups / form UI from one app showing up in another).
- **Findings:**
  1. **DB rows are per `Project` id** — emails did **not** cross uniqueIds in a scrape of Signup / Survey / Potluck lists.
  2. **`Simple Survey` Deploy (`qyzju5cyuagbidj`) is contaminated** — page heading / form is **Sign-up Sheet Template**, while the MQL still lists older Form 1 survey-style rows. Classic **Redeploy-by-name**: Java `ProjectsHibernateImpl.put` reuses `UserProject` when **name** matches and **keeps submissions**.
  3. **Shared `JSESSIONID` on `localhost:8080`** — session `StoredContextInfo` key omitted `userProjectId`, so **concurrent Deploy tabs** could thrash in-progress edits across apps (Safari multi-tab is an easy trigger).
  4. **Main Menu template JSON was deleted** in the Jul 22 commit — Vite returned SPA HTML for `/samples/templates/*.json` (broken New Project starters). **Restored** under `designer-web/public/samples/templates/`.
- **Fixed (session):** `ExecutionContext.getStorageAttribute()` now appends `userProjectId`; class hot-copied into Tomcat Jul 22 evening.
- **Ops clean-up (Jul 22 evening):** Deleted contaminated Tomcat Deploy **`Simple Survey`** (`qyzju5cyuagbidj`) + its 7 submissions; see `CONTAMINATED_SIMPLE_SURVEY_JUL22.md`. Prefer **New Project** (not overwrite) between featured apps; Redeploy under a fresh name if lists look wrong.

### New Project / distinct uniqueId must never inherit old submissions (Aug 20) — **OPEN**

- **Owner smoke Aug 20:** MAX / MIN **Passed**. New Project with a new FIB blank (`Form1:FIB1:a`) showed a **pre-filled value `24` on live `:8080` after Push** — not in Design, not in Preview. Remove Duplicates **Passed** the same day.
- **Invariant to enforce:** Projects with **different uniqueIds** must never share or inherit each other’s submission values (including blank defaults painted from prior rows). A **New Project** first Push must land on a **fresh** uniqueId with **empty** response data for that id.
- **Suspected causes (investigate when scheduled):**
  1. Push / Redeploy-by-**name** reuses an existing `UserProject` and keeps old submissions (Jul 22 Redeploy-by-name pattern).
  2. First Push fails to mint a new uniqueId when `_freshFromTemplate` / File→New should have forced one.
  3. Less likely given “only on `:8080`”: blank **name** collision across projects under one reused id (not a true cross-uniqueId leak).
- **Ops workaround until fixed:** `?reset=1` on the start URL; Purge responses for that uniqueId; or Push under a **unique project name** so Java cannot match an old row by name.
- **Related:** Jul 22 Deploy data isolation notes above; Jul 16 “Session junk rows” mitigated-ops row; website-mock Make a Copy already clears `uniqueId` for forks — Library Save-to-MyTawala mock still shares Library uniqueId by design until production mint.
- **Do not close** until New Project → Push → `:8080` blank fields are empty when no submissions exist for **that** uniqueId, and two live projects with distinct uniqueIds cannot paint each other’s field values.

### Online Exam Builder — Admin → Scores first-hit “session expired” (Aug 3)

- **Symptom:** Java Deploy (`:8080`, MADE WITH TAWALA chrome). From **Administration**, select **Scores** → first POST yields legacy error *“An error occured while running this application…”* (BackButton / session page). **This page** recovery often opens **Setup** (not Admin). Second Scores works and shows the Student Scores table.
- **Not a Designer/JSON bug:** Deploy XML for `Post-Administration` is correct (`show document="Student Scores"` + incomplete-exam get + `show form="Administration"`). Student Scores MQL Where `Record:Exam:status equals completed` emits correctly. **Node** runtime Admin→Scores works on the **first** submit (no PRG store).
- **Root cause (Java PRG):** After form POST, Tomcat does redirect-after-post (`?__`) and reloads a page stashed on `HttpSession`. `storeLastPage` / `getStoredPage` used **one global** session key (`com.tawala.project.last.stored.page`) shared by **all** projects on `localhost:8080` (same `JSESSIONID`). Concurrent Deploy tabs (Exam setup, another app, Admin elsewhere) clobber the stored page → first Admin Scores GET `?__` sees null → exact error page. Jul 22 only scoped `StoredContextInfo` by `userProjectId`, **not** the PRG last-page slot.
- **Also expected (not fixed here):**
  - **Stale Admin form after Tomcat restart** — cookie/session wiped; open form still has old post-token → same error. Refresh Admin URL, then Scores again.
  - **Browser BACK / double-submit** of the menu form after a successful Scores POST → intentional back-button token reject (same copy).
  - **“This page” → Setup** — `originalLink` freezes to the **first** startpoint touched in that session. Setup before Admin means recovery returns Setup. Not a Scores link bug.
- **Fix (source):** `ExecutionContext.storeLastPage` / `getStoredPage` key by `userProjectId` + form name (`TawalaWebapp-build1700/.../ExecutionContext.java`). Rebuild/hot-copy class into Tomcat WEB-INF (same pattern as Jul 22) and restart the webapp so the new class loads.
- **Smoke (clean):** Redeploy Online Exam → open **only** Administration start URL in a fresh private window → Scores → expect **Completed Tests** + Admin menu stacked (or Admin again). Repeat with two Deploy tabs of different apps and POST both around the same time — Scores must still land, not the error page.
- **`:3001` restart** does **not** wipe Tomcat session (Java and Node sessions are separate). Wiping/restarting **Tomcat** or multi-tab PRG thrash do.
- **Park (product):** Setup form has **no** exit path to Administration — owner: add when done; see `DESIGNER_OPEN_TODOS.md` deferred smoke follow-ups. Student Scores table column clip / horizontal scrollbar is a separate Deploy CSS item (other agent).

### Preview / Deploy font size vs Design (Jul 23)

- **Symptom:** In Chrome / Safari / Edge, Form **Preview** and **Deploy** body text often looked larger than Design canvas (toolbar default **12 pt**; canvas unstyled text is **13px**).
- **Cause:** Preview `BASE_FORM_CSS` had **no** body `font-size` (browser default ~16px); Deploy `default.css` used **11pt** (~14.7px); Safari could also inflate via text-size-adjust. Starter templates often have **no** inline `font-size`, so they inherit the page base.
- **Fixed:** Preview + Deploy body/html **13px** + `text-size-adjust: 100%`; Preview inputs inherit. DirtBowl theme still uses its own 16px registration CSS. **Smoke:** hard-refresh Deploy (`default.css`); reopen Form Preview — instructional text / MQL should match Design size.
### Document functions (Jul 20)

- **Two identical MQL on Document 2 — different Deploy results** — One block showed `<>` (no table); the other rendered a table but the multi-select MCQ column (“All possibilities”) was blank. **Root cause (1):** formatting toolbar / styled wrapper `<span style="font-size…">` around a function token exported as nested `<font><font><itemization-table>…` — Java Font FACTORY drops the inner table. **Fixed:** `documentHtmlToXml.mjs` detects font-wrapped display components and skips the outer wrap. **Root cause (2):** multi-select MCQ values sometimes stored as one comma-separated string; Java `displayLabelsOnly` did not split them. **Fixed:** `DisplayMultipleChoiceLabel.java` splits comma-separated selections; Preview MQL uses `formatMcqCellValue` in `itemizationPreview.mjs`. **Owner OK Jul 20** after Tomcat WAR rebuild + Redeploy (re-entered one MQL manually).

- **Configure Function field target: Fields palette double-click does not replace** — **Fixed Jul 20.** Two causes: (1) Configure inputs live outside `.mdi-window.active`, so Fields double-click was refused as a “stale MDI target”; (2) column Contents / expression boxes appended at caret instead of replacing. Fix: `configureDialog` targets stay usable outside MDI + replace whole value on drop/double-click. **Smoke:** focus Contents or Where field → double-click a Fields leaf → prior value replaced (not appended / not no-op).

### File / Save

- **Save / Save As ignore last-loaded file name** — **Owner Passed Jul 20** (no longer seeing project-naming bugs on Save). After **Open…** / load, Save and Save As default from the file leaf / aligned `project.name`; **Untitled** only for **New Project**. Explorer root tracks the file name after Save As.

- **Many `.json` files greyed out in Open / Save As pickers** — **Fixed Jul 20:** Chromium + macOS often typed fresh saves as `text/plain`, so strict `application/json` filters greyed valid `.json` on the **first** Open after Save; second Open worked after Launch Services caught up. Open picker now shows all files (validate `.json` after pick); Save picker accepts `application/json`, `text/json`, and `text/plain`. Quiet-Save handle is released during Open so the file we just saved is not greyed as “already open.”

### Palette

- **Reset Formatting broken** — **Removed July 10** (control deleted from Formatting Palette; was always greyed and unreliable).

- **Font/size dropdown on mixed-format runs** — **Owner smoking Jul 20** (especially after adding or deleting function/field chips in a run). May still show Default Font/Size when the selection mixes faces/sizes, or go false-Mixed / false-default after chip insert/delete. Single-run caret sync and Document typing persistence verified OK July 10. **Smoke focus:** `DESIGNER_DOCUMENT_EDITOR.md` § 7c, 7j (uniform + chip runs; Face→Size with chips; re-drag after click-away).

- **Function placeholder jumps after partial Size change (Jul 20 owner, intermittent)** — Changing font size on **some words** in a paragraph (mixed-size run) can occasionally move a function chip to the wrong place in the same line/paragraph. Easy workaround: drag the chip back (§ 22h). Likely related to Size wrapper split around `contenteditable=false` tokens. Not blocking; track until a reliable repro for fix.

### Document canvas & palette (smoke test July 10)

Owner could not fully test overnight (hooks-order / “too many hooks” error); retested on tip `b48656b` after hard refresh. Font face/size for plain typing OK; remaining Document issues:

- **Backspace deletes function chip then caret vanishes / arrows die** — **Fixed Jul 20.** After Backspace removed a trailing `<<…>>` chip (esp. chip-only placed line), focus often sat after `contenteditable=false` with no ZWSP landing (or selection cleared) → no blinking caret; ArrowLeft appeared dead. Fix: explicit chip Backspace/Delete keeps a live caret landing; `focusPlacedBlock` / `focusPlacedBlockEnd` ensure chip ZWSP pads. **Smoke:** `DESIGNER_DOCUMENT_EDITOR.md` § 22e.

- **Space delete → mid-word wrap; Backspace between paragraphs eats many lines** — **Fixed Jul 20.** (1) `overflow-wrap: break-word` mid-broke glued words after deleting a space (“fashioned” / “names” with room on the line); switched to wrap-at-spaces only. (2) Backspace at start with a blank above now removes **only that blank**; content+content still merges (with a joining space); onInput prune no longer clears every trailing blank invent in one keystroke. **Smoke:** § 22f.

- **✥ drag moves only the first of several selected paragraphs** — **Fixed Jul 20 (superseded):** ✥ anchors **removed** — they snapped back, blocked highlight sizing, and were redundant. Move is **highlight + hand cursor**: select line(s) → grab/grabbing drag moves all selected `.doc-placed-text` together (no pack-to-tight snap-back). **Also Jul 20:** drag into a blank gap above used to bounce back (reflow mid-drag + blank husk collision); now skip pack while `.is-placed-moving` and `finalizePlacedBlocksMove` removes overlapping same-column blanks (parity with Backspace on the blank). **Also Jul 20:** drag-select/move no longer skips a visually middle line that is later in the DOM (fill same-column span + expand Range). **Owner Passed Jul 20.** **Also Jul 20:** double-click / partial word highlight must **not** move the whole paragraph — only whole-line or multi-line selections start highlight-move (`listPlacedBlocksForHighlightMove`). **Smoke:** § 22g.

- **Dragging a placeholder moves the whole paragraph** — **Fixed Jul 20.** Highlight-drag treated a selected field/function chip as a placed-line move (`preventDefault` blocked HTML5 chip drag). Mousedown on a chip no longer starts paragraph move; function chips are relocatable like field tokens. **Owner Passed Jul 20.** **Smoke:** § 22h.

- **Multiline / drag-select highlighting buggy** — **Verified July 10** for plain text: cross-block drag-select works; multi-line align applies to all intersecting placed lines. **Reopened Jul 19 (owner):** drag highlighting still does **not** work properly when the selection includes **Field tokens** and/or **function labels** (`<<…>>`). **Jul 20:** folded into Document caret-model epic (live caret + drag of highlighted content) — see `DESIGNER_OPEN_TODOS.md` § Document caret model. **Related Jul 20:** middle-line skip on plain multi-line select/move fixed (DOM order ≠ visual tops) — **Owner Passed**; chip-inclusive drag-select still open.

- **Cannot drag Function label onto same line as text** — **Owner Passed Jul 20.** Function chips HTML5-relocate like field tokens; drop onto a text line joins it. **Smoke:** § 22h.

- **Long function chips + unreachable MDI chrome (Jul 20 owner)** — Function labels use `white-space: nowrap`, so a long `<<DISPLAY…>>` / MQL chip may **look** wrapped to the next visual line until the Document/Form window is widened (then it snaps back onto its line). Widening (or leaving a wide window) can push **minimize / close** so far right that they are hard or impossible to reach: MDI children **cannot** slide behind **Project Explorer** or **Items/Statements**, but **can** still slide behind **Fields**. Workarounds today: **View** → hide Explorer / Items / Fields, or **Windows → Cascade**. Prior Jul 19 clamp (“keep frame inside `.mdi-surface`”) did not fully solve Fields underlap or title-bar reach after wide chip layout.

- **Cannot rename Form / Process / Document in Explorer** — **Fixed Jul 19:** rename was only a 500ms long-press on an already-selected row (easy to miss, and HTML5 drag canceled it). Now: **click a selected** Form/Process/Document name to edit, or press **F2**. Enter commits, Escape cancels.

- **Cannot rename Process in Explorer (Forms/Documents OK)** — **Fixed Jul 20:** a linked process appears twice (under the form and under **Processes**). Inline rename matched `kind + name`, so both rows entered edit mode; the second input stole focus and blur-cancelled the first. Now matches the unique render `key` so only one `RenameInput` mounts. **Smoke:** select a Post/Pre (or Processes-folder leaf) → click again or F2 → type new name → Enter; Explorer + open window title update.

- **Document rename does not update Process Show / Send / Append** — **Fixed Jul 19:** renaming a Document in Explorer now cascades into Process command refs (`documentRenameCascade.ts` via `renameDocument`), including nested If / ForEach. **Owner Passed Jul 20.**

- **Form rename resizes Document / Form function chips** — **Fixed Jul 20.** Renaming a Form remounts its MDI window; open Documents’ `ResizeObserver` packed layout and **committed**, and commit stripped chip `font-size: 12pt` so chips inherited a larger parent line. Fix: (1) never strip font-size from `.function-token` / `.field-token`; (2) only persist Document reflow when that surface’s **width** changed. Also added **form rename cascade** (`formRenameCascade.ts`) so `Form:Field` refs / function chips update like legacy. **Smoke:** open Document + Form with MQL chips → rename another Form in Explorer → chip sizes and Document layout stay put; rename a referenced Form → chip labels update without growing.

- **Process Connect dialog blocked multi-form Pre/Post** — **Fixed Jul 19:** legacy allows one process on many forms (checklist; Potluck `Show Results`). Dialog was a single-form dropdown/Attach flow. Now Pre and Post **form checklists** (check several forms); banner uses plural “N Forms” when appropriate. **Also:** drag a Process onto a Form in Explorer attaches as **Post-process** when that form’s Post slot is empty (legacy drop).

- **Field rename does not update Functions / function labels** — **Fixed Jul 19:** renaming a Hidden Field, FIB blank alt label, or MCQ field name cascades into Document/Form Text function chips (`data-function-config` + visible `<<NAME(…)>>`), field tokens, and Process command field refs (`fieldRenameCascade.ts` via `updateFormItem`).

- **Function chips insert at random sizes / resize nearby text** — **Fixed Jul 19 (regression):** Form Text still used badge `10px` (Document already inherited). Insert also painted sticky size onto the parent line. Now Form Text chips inherit like Document; insert no longer resizes the paragraph; default insert leaves chip size unset so it matches surrounding text.

- **Ghost / deleted Document text still visible** — **Jul 19 false alarm (owner):** the unexpected question phrasing on Deploy came from **Form Item** MCQ text (Response Totals injects the Form question), not from deleted Document prose. Separately, Design still got a small hardening Jul 19 (discard orphan glyphs after delete; prune stacked duplicate placed lines; missing `reflowPlacedLinesBelow` import) — keep as regression prevention, not as confirmation of that screenshot.

- **Fields and variables font face/size** — **Face/size match verified July 10** after fix. Placement still broken (below).

- **Field token drop/placement on Document** — **Snap-to-line + move-after-drop verified July 10:** drop joins the nearest line; drag relocates an existing token; mid-line drop, typing after token, and joint highlight/reformat with text all work.

- **Font Color selector** — **Verified July 10:** highlight recolors only the selection; **A** applies current color; **▾** chooses a new color; icon swatch tracks color; new typing keeps the color.

- **Alignment tools** — **Verified July 10:** single- and multi-line left/center/right/justify to margins; justify wraps at content width; last line left-aligned.

- **Font size / line packing** — **Verified July 10:** selection-only size; one enlarged word can push lines down; pull-up only when the line box shrinks; reset to default works (mixed highlight shows Mixed, not false default).

- **Can still overwrite existing text without deleting it** — typewriter Return now **pushes** lines below instead of deleting/stacking (**verified July 10**). Owner recheck July 10 afternoon: **no residual overwrite** observed.

- **Empty placed-line husks after delete** — select-all + Delete left an invisible `.doc-placed-text` snap target. **Fixed July 10:** delete/cut/backspace-to-empty prunes husks (Return blank lines kept until deleted).

- **Arrow keys leave Document line / spawn nearby lines** — **Verified July 10:** confine arrows/Home/End in placed lines; Up/Down move between lines (including within soft-wrapped blocks); click on same row snaps into existing line.

### Form canvas (UX backlog)

- **Del/× on selected function chip deletes whole Text row** — **Fixed Jul 20:** highlighting a `<<…>>` function (or field) chip and pressing Del/Backspace, or clicking the row/toolbar **×**, used to remove the entire Text item. Now removes only the selected chip(s); whole-row delete still applies when no chip is highlighted.

- **Design-mode FIB blanks are editable** — should be placeholders / length lines only while editing? **UX bug; deferred.** (Idle Design correctly keeps literal `_` — Batch 2 hold-list Jul 18; boxes only in Preview/Deploy.)

- **Design-mode checkboxes and radios change state** on the canvas. **UX bug; deferred.**

### Hierarchical convert / preserved-warning cues (Aug 6, 2026) — **implemented (partial)**

**Owner design request.** Lead the author quickly from outer → inner to the warning target so they can decide **workaround vs Designer edit needed**. Applies to uneditable / preserved import gaps (e.g. `displayCondition`, Configure Function gaps) and similar convert warnings — not a substitute for eventually editing those properties in UI.

**Cue hierarchy (outer → inner) — wired in `designer-web`:**

1. **Project Explorer** — Form or Document node shows an amber warning cue if **anything inside** still has an **uneditable** gap (itemization **column** `displayCondition` / Document HTML embed). Item-level Form Item conditions are editable and no longer flag Explorer.
2. **Form / Document canvas** — **`{Qn}` braces** on the badge when `item.displayCondition` is set (product state, not a warning). Orange left-edge gap stripe only for rows that still embed **column** conditions. **Function chip** turns amber when columns carry `displayCondition`.
3. **Configure Function dialog** — column-level **`cond` badge** next to “Column N” when that column has `displayCondition` (round-trip preserves the marker; edit UI still deferred).
4. **Status bar** — count of **uneditable** preserved gap markers (column conditions). Item-level conditions are not counted as gaps.

**Still not visually flagged** (honest remainder): Send/email path limits, dropped `<styles>`, multi-source-form itemization primary-only, Dynamic MCQ nested record-selector `where`, skipped `<file>` uploaders, empty `<show/>`, and other convert messages that leave no durable column-`displayCondition`-like marker (or leave data that Design already edits via Where rows).

**Related:** Legacy `.tawala` → JSON batch queue; import lossy notes in `DESIGNER_OPEN_TODOS.md`. Helpers: `designer-web/src/lib/preservedImportGaps.ts`. **Edit UI for item `displayCondition`:** Done Aug 10 — right-click badge → **Display conditionally…**; canvas **`{Qn}` braces** (`DESIGNER_FORM_ITEMS_CONDITIONAL_DISPLAY.md`). Column-level Configure cues remain read-only. When an item condition is cleared or only braces remain, the orange “convert error” stripe goes away — gaps clear when cured.

### Hold-list (Jul 18 gated pass)

| Batch | Item | Status |
|-------|------|--------|
| 1 | Form DnD vs text selection (#1 + #10) | **Done** — badge-only reorder (`formItemReorder.ts`); Alt Label `select()` on focus |
| 2 | Design FIB idle keeps `_` (#3) | **Done** — idle paints prompt HTML with underscores |
| 3 | Preview FIB (#4 + #9) | **Done** — `fibPrompt` + runtime; leftAlign keeps interstitial text order |
| 4 | FIB insert highlight (#2) | **Done** — placeholder selected; trailing `_` not |
| 5 | Text indent face/size (#7, #8) | **Done** — `margin-left` indent (no `execCommand("indent")`) |
| 6 | Tables multi-select + overflow (#5, #6) | **Done** — cell selection center; Text wrap `max-height` + scroll |

### Functions (final Designer run-through)

- **Function Where on MCQ fields** — **Implemented Jul 19 (TODO #11).** Function Conditions switches to legacy `mcEquals` / `mcContains` / `mcIsBlank` / … when the left field is an MCQ (`onlyone` → MCOne vs MCMany). Value placeholder = choice letter. FIB Where unchanged. **Owner Passed Jul 20** (non-numeric Where + 2 numeric tests OK).

- **RESPONSE TOTALS multi-select undercount** — **Investigated Jul 19 (TODO #12): no code bug found.** Java + Preview tally both loop all `getValues` / array choices (same as Bar Graph). Added regression tests. **Owner Passed Jul 20** — Totals and Bar Graph both pick up all choices on the same multi-select MCQ.

### Edit / Undo — **closed policy (owner Jul 20)**

**Contract (do not reopen without discussion):** Undo/Redo is **best-effort browser `execCommand`** on the focused contenteditable only. We accept declining returns and will **not** build project-level or full-canvas history now.

| In scope (best effort) | Out of scope (will not undo) |
|------------------------|------------------------------|
| Glyph / format edits while focus stays in **one** Form rich region (Text / FIB / MCQ) or **one** Document surface, if the browser still has a stack | Field name / FIB blank / MCQ name renames |
| | Form item **Label** renames |
| | Document highlight-drag **Moves** of placed lines |
| | Skip / Process statement Add · Modify · delete · reorder |
| | Explorer Form / Process / Document rename; insert/delete items; Save |
| | Cross-item or cross-window Undo; single-MDI-window history stacks |

**Known limits (not bugs to chase):**
- **Document:** placed-line remount / commit churn often leaves Undo empty or useless even for typing.
- **Form:** Undo is **per-item**; leaving that row clears its stack.
- **Skip toolbar** Cut/Copy/Paste/Undo — legacy icons only; **never implemented and not needed** inside Skip (owner Jul 23). Not a TODO.
- Legacy Jan 2011 build often had Undo/Redo **always greyed** anyway.

**Past notes (superseded by policy above):** Document Undo “does not work”; field/Label/Moves not on the stack — expected under this contract, not open fix items.

- **Paste required a one-time browser permission prompt** — expected, not a bug: toolbar/menu Paste reads the system clipboard via `navigator.clipboard` (`clipboard-read`), which browsers gate; Cut/Copy write the current selection and are allowed silently. After **Allow**, Paste and ⌘V work without re-prompting. (Fixed Jul 18: Paste now uses the Clipboard API since `execCommand("paste")` is blocked from button clicks.)

### Full build / TypeScript (`npm run build`) — **green Jul 21**

`cd designer-web && npm run build` (`tsc -b` + `vite build`) **passes** as of Jul 21. Cleared unused locals, null/`Element` casts, rename-cascade `FormItem`/`FunctionConfig` typing, and Save-chord `shiftKey` mismatch. No product behavior change intended — typing/cleanup only.

*(Prior inventory Jul 17 had ~14–25 errors across live code + tests; day-to-day Vite still worked throughout.)*

### `.tawala` File → Open — **landed Jul 21**

Legacy projects open via shared `tawalaXmlToJson` (CLI + Designer). Accepts `.json` / `.tawala` / `.tawala.xml`. Import is lossy-with-warnings (pageHeader, styles, rich FIB, invitations, etc.) — see `DESIGNER_OPEN_TODOS.md` week banner and `TAWALA_XML_TO_JSON_MAPPING.md`. After `.tawala` Open, Save As writes format 2.0 `.json` (does not overwrite the legacy file). Prefer imported DirtBowl over corrupted `dirtbowl_definition_v3.json`.

### Skip Instructions

- **Skip dialog: click / edit / delete statement ignored** — **Fixed Jul 19:** Edit Skip Instructions had no select / Modify / line delete (toolbar Delete disabled; Add always appended). Now Process-parity: click a line to edit (Modify), × / toolbar Delete, ↑↓, insert at the arrow (not always append). Also: SkipTo no longer resets the dropdown to the first FIB on open (that made Skip-after-FIB1 look like a no-op).

- **Skip re-edit stuck in Modify-only / cannot add inside If (Jul 20)** — **Fixed Jul 20.** After Close and **Edit** again, selecting a line entered edit mode but the dialog never passed `showAllInsertionGaps`, so the single legacy insert arrow hid and there were **no** clickable gaps — could only Modify existing lines (e.g. could not place a Set inside the If `then` without rebuilding). Now: all insert gaps stay available; clicking a gap clears selection (insert mode); choosing a different Statements palette tool also leaves Modify when it does not match the selected line. **Smoke:** `DESIGNER_FORM_ITEMS_HIDDEN_SKIP_BREAK.md` § Skip re-edit.

- **Skip dialog re-open does not restore insertion point.** Soft leftover; session now also stores `insertIndex` + selection. Re-smoke when convenient.

- **Skip modal overlay quirks** (positioning / full-screen dim). Soft / polish; Close-only dismiss is intentional.

### Process / runtime navigation

- **Process statement: edit vs insert mode (legacy arrow)** — **Fixed Jul 16 (v2); hardened Jul 24.** Selecting a script row enters **edit mode** — solid blue highlight and left **▶** on that statement; insert-gap ▶ chrome is **hidden** (hit-only targets remain so a gap click returns to insert mode). Clicking an insert gap / setting insert point clears selection (**insert mode** — ▶ on the separator). Row labels do not keep a text caret beside the block highlight (`caret-color: transparent` + blur on click). Smoke: (1) click “Show Form …” → ▶ on statement + blue row, **no** insert-gap ▶; (2) click between lines → ▶ on gap only, no statement highlight; (3) click selected row again / Modify panel — still no text caret in the statement text.

- **AdminDash start point → empty Thank you; Coach Contact hard to reach after deploy** — **Owner Passed Jul 20 (legacy `.tawala`).** Not a Designer navigation bug. The JSON / open DirtBowl copy was **corrupted** (bare `Show`, blank AdminDashboard). Full legacy `designer-web/public/samples/legacy/DirtBowl.tawala` (dozens of Forms / Processes / Documents) **deployed and worked flawlessly** for the owner Jul 20 — including AdminDash-scale navigation. Truncated `dirtbowl_definition_v3.json` / New Project stubs remain unreliable; use legacy `.tawala` + `deploy-tawala-template.mjs` for DirtBowl smoke.

### Fixed Jul 15 (Signup Sheet smoke — held bugs)

- **MQL Configure column toolbar blank + no tips** — footer +/−/↑/↓ used SVG + native `title` (invisible under modal CSS; tips clipped / absent on disabled). Fixed: glyph buttons + upward `win-tip` tooltips.
- **Signup Sheet submit showed “Registration step complete”** — `renderSubmitAck` DirtBowl copy. Fixed: generic “Thank you” / “Back to {form}” unless Registration.
- **Return to Form 1 kept prior values** — session formFields not cleared. Fixed: append record + `clearFormAnswers` on complete; back link `?fresh=1`.
- **Preview/Deploy lost Form Item spacing** — runtime CSS had no inter-item margins. Fixed: `.tawala-form > …` spacing in `runtime.mjs` page shell (Design canvas unchanged).
- **Signup MQL headers with empty thin rows** — `topLabels` Preview inputs were `readonly` (empty submits → blank records). Fixed: live `blankInput`; skip all-empty `appendFormRecord`.
- **Document injected `Continue →`** — Show Document then Show Form used a separate Continue page. Fixed: stack blank Form under Document on the same response (no Continue artifact).
- **Document MQL config with `<<Field>>` broke parse** — span regex used `[^>]*`. Fixed: quote-aware `htmlSpanReplace.mjs`.
- **Email/Phone validators ignored in Node Preview** — only Registration had checks. Fixed: `fibBlankValidation.mjs` on submit when blank.validation is set.
- **Horizontal FIB layout missing on default/baseball** — vertical spacing only. Fixed: flex rules in `BASE_FORM_CSS`.
- **MQL columns blank despite stored signups** — blanks named `a`/`b`/`c`/`d` with alternateLabel `First`/`Last`/…; MQL asked for `<<Form 1:First>>`. Fixed: `blankAliasesFromForm` at render + append. Also force-restarted API (stale process still injected Continue →).
- **Show Form after Document left prior answers** — `clearFormAnswers` cleared `a`/`b` but not alternateLabel keys (`First`/`Last`), so `blankInput` refilled. Fixed: clear aliases + Form stacks blank.
- **Email/Phone accepted garbage on Signup** — Node Preview skipped validators unless `blank.validation` set. Fixed: infer Email/Tel from name/alternateLabel; reject `..` emails.

---

## DirtBowl / Registration leftover watchlist (Jul 21 sweep)

Structured pass over `designer-web/` for hard-coded DirtBowl / Registration behavior that can leak into other projects (Potluck, Signup, templates).

### Buckets

| | Meaning |
|---|---|
| **A** | Intentional product support (theme, samples, Registration-only paths) — keep |
| **B** | Registration helpers correctly gated by `formName === "Registration"` / `project.name === "DirtBowl"` / `theme === "dirtbowl2"` — keep; spot-check gates |
| **C** | Runs (or defaults) for **every** project — fix or tighten |

### A — keep

| Location | Notes |
|----------|--------|
| `server/themes/dirtbowl2.css` | Theme CSS |
| `sessionStore.seedDefaultRecords` | Gated `project.name === "DirtBowl"` |
| `registrationLayout.mjs` / `registrationTextToXml.mjs` / `registrationFibToXml.mjs` / `registrationReview*.mjs` | Deploy/Preview Registration chrome; text/FIB XML gated on Registration |
| `jsonToXml` FIB/text paths calling those modules | `formName === "Registration"` |
| Samples / tests / comments mentioning DirtBowl | Docs & fixtures only |
| UI copy (“not DirtBowl participant login”) | Harmless |

### B — keep (gated); light review only

| Location | Gate |
|----------|------|
| Most `runtime.mjs` Registration branches | `isRegistrationForm` (= name `"Registration"`) |
| `Submit →` label | `theme === "dirtbowl2"` |
| `RegStep2` fee rewrite | Form name `RegStep2` |
| Injected `__page2footer__` | Registration + segment 1 |
| `SEND_DOC_DEFAULTS` in `jsonToXml.mjs` | Only when Send body document is a DirtBowl letter name (e.g. `AdminRegNotification`) |

### C — worth fixing (recommended order)

| Pri | Issue | Why | Suggested fix | Status |
|-----|--------|-----|----------------|--------|
| **1** | `applyLegacyPlainTextLeaguePlaceholder` still replaces `""` → `League \|\| "Dirt Bowl"` on **all** plain-text Form items | Any template with literal `""` in Normal text shows “Dirt Bowl”; already burned us on rich HTML | Gate: only Registration **or** only when `League` field exists; else leave `""` alone. Prefer empty/`«League»` over hard-coded Dirt Bowl | **Done Jul 21** — Registration or non-empty `League` only; no hard-coded Dirt Bowl fallback (`runtime.mjs` + Preview tests) |
| **2** | `scrubRegistrationBlankCollisions(session)` runs on **every** `getOrCreateSession` with default form `"Registration"`, and **always deletes** `session.fields.a/b/c` | Signup / any FIB using blank labels `a`/`b`/`c` can lose aliases on session touch | Call scrub only when project has a Registration form (or `project.name === "DirtBowl"`); never wipe bare `a/b/c` for other projects | **Done Jul 21** — `getOrCreateSession` gates on Registration / DirtBowl (`sessionStore.mjs` + scrub tests) |
| **3** | `registrationLayout` fallback `\|\| "Dirt Bowl"` / hard-coded **April 28, 2008** | Only hit for Registration Preview — wrong defaults for a renamed league | Use field value or neutral placeholder (`League` / blank date); leave dated copy in template XML only | Open |
| **4** | `SEND_DOC_DEFAULTS.AdminRegNotification` alias `"Dirt Bowl Automated Email Server"` | Only if that document name is used — still brand-locked | Prefer `aliasField: "AdminName"` or project/League field; or omit aliasLiteral | Open |
| **5** | Error string “still see DirtBowl URLs” in `index.mjs` | Confusing for Potluck deploy failures | Generic “stale Java start URLs” wording | Open |

### Not C (do not “fix” away)

- Comment-only / shape notes (`documentHtmlToXml`, FIB tab stops, Where `Record:Form:Field`).
- Tomcat CSS under `docker/tomcat/css/project/` for dirtbowl2 — Deploy theme, not Designer global.

### Regression rule

Any future Registration-only helper must be gated. Add a **non-DirtBowl** test (Potluck or Signup) when touching Preview/session/XML defaults.

---


### Pickup Jul 16 — MULTIPLE QUESTION LIST / SignupSheet (owner resume)

**Passed Jul 15 (owner):** MQL table shows stored rows; Document stacks above blank Form 1 (no Continue →); Delete on form items asks “Are you sure?” (blur/Del mishap mitigated).

**Passed / fixed Jul 16 (code + owner smoke where noted):**

| # | Item | Notes |
|---|------|--------|
| 1 | **Horizontal FIB layout** | **Fixed:** Preview/Deploy blank widths from underscore `blank.length` (`size` + `ch`); stacked rows inline (not flex-stretch). Theme CSS in `themes/index.mjs`. |
| 2 | **FIB Required per blank** | **Owner OK.** First click selects item only; second click places caret under the click (`FibCanvasRow`). |
| 3 | **Required / empty submit** | **Owner OK.** `blank.required` blocks empty Submit; validators on Node + Java CSS (`.validateError` in `docker/tomcat/css/project/default.css`). |
| 4 | **MQL Where clause** | **Owner OK.** |
| 5 | **Print / Excel export links** | **Owner OK on Deploy.** Configure toggles persist; Preview emits Print / CSV export (`itemizationPreview.mjs`). |
| 7 | **Java Deploy (8080) parity** | **Owner OK** for Signup Form+MQL path (Tomcat up, `dev`/`dev`). Also: Document MQL field tokens + no nested font/division; FIB multi-blank soft-rows; Design B/I/U→Deploy (`fibRichPromptToXml`). **Re-smoke:** Redeploy after `fcebcfa` to confirm latest FIB formatting on 8080. |
| 8 | **Dev API on 3001 dies** | **Mitigated:** `designer-web/scripts/ensure-dev-api.sh` + README; check `/api/health` when Preview/Deploy fails. |
| 9 | **Session junk / cross-project values** | **OPEN Aug 20 (raised):** New Project showed leftover blank value **only on `:8080` after Push**. Track: distinct uniqueIds must never inherit each other’s submissions; New Project first Push must mint empty data. Detail: § **New Project / distinct uniqueId must never inherit old submissions**. Ops: `?reset=1` / Purge / unique Push name until fixed. |
| — | **FIB Design formatting → Deploy** | **Fixed Jul 16:** freeform prompts mirror B/I/U / face / size / color into Java font XML (`fibToXml` + tests). |

**Still open (leave MQL when these are done or explicitly deferred):**

| # | Item | Notes |
|---|------|--------|
| 6 | **`baseball` theme CSS** | Stub / fallback still weak vs full theme; optional — not blocking Signup default theme. |
| — | **Process caret + row highlight** | **Fixed Jul 16 (v2); hardened Jul 24** — edit mode hides insert-gap ▶; no text caret on selected row (see Active / Process). |

**Then:** MQL/SignupSheet core is done enough to leave — continue owner review **#9** other untested functions that already emit Document XML (skip four deferred stubs: Categorizer / Roster / Link / PayPal).

- **Form item Delete had no confirm** — Del/× removed canvas items immediately (easy after FIB Required focus). Fixed Jul 15: `confirmAndDeleteFormItem` + strip/button Del ignore.

---

## Removed from bugs list (July 10 review)

| Item | Disposition |
|------|-------------|
| Properties: Individual Items stay fully expanded | Moved to **TODOs** (UX polish, not broken) |
| Dev server restart blanks Designer tab | Not a product bug (HMR); dropped |
| Empty MDI until form clicked in Explorer | By design; dropped |
| MDI windows slide under Fields (unreachable close/minimize) | **Partial Jul 19** — drag/resize clamp inside `.mdi-surface`; **reopened Jul 20** — long nowrap chips force wide windows; chrome can still sit under **Fields** / off-reach while Explorer/Items block sliding (see open bugs) |

---

## Earlier prune (fixed / superseded)

| Item | Why removed |
|------|-------------|
| Formatting Palette table tools not wired | Landed in later Document/palette work |
| Formatting Palette fx / Insert → Function not wired | Landed in Document palette & typewriter (`3f49995`) |
| Process UX bugs (If/Show panels, connection overlay, etc.) | Fixed in Process chats |
| Yellow Connect banner / connection dialog | Landed; menu parity → TODOs |
| Document HTML→XML “paragraph-only” | Partial fix; remainder → TODOs |
| Field-token drag polish | → TODOs |
