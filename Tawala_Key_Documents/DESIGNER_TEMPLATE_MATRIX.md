# Designer — New Project template matrix

Maps **File → New Project** templates to repo `.tawala` files and deploy smoke expectations.

**Source:** `TawalaDesigner/Code/TAWALA/Setup/Templates/templates.xml` + template XML.  
**Deploy:** `node scripts/deploy-tawala-template.mjs "Template Label"` → `http://localhost:8080/client`.

**Browser Designer JSON (Jul 23):** `designer-web/public/samples/templates/*.json` — smoked starters + **Multiple Question Survey** (owner smoked `XMultiple Question Survey.json` → installed as `multiple-question-survey.json`, project name without leading `X`). New Project dialog is a **compact** legacy-style icon grid (no left Project-type tree; ~380×300); tile icon is the exact 32×32 bitmap from legacy `NewProjectDialog` ImageList (`designer-web/public/icons/new-project-template.png` — pale blue paper, blue dog-ear, lavender gear). Descriptions are hover tooltips. Order: Basic → Activities → Meetings → Polls. Sign-up with E-mail omitted (mail backlog).

---

## Template catalog

| Category | UI label | File | Theme (typical) |
|----------|----------|------|-----------------|
| Basic | Empty | `Empty Project.tawala` | default |
| Basic | Form with Process | `Form with process.tawala` | default |
| Basic | Form, Process and Document | `Form with process connecting a document.tawala` | default |
| Activities | Sign-up Sheet | `Signup Sheet Template.tawala` | baseball |
| Activities | Sign-up Sheet with E-mail | `Signup Sheet Template w Email.tawala` | redrays |
| Meetings | Get Together | `Get Together Template.tawala` | default |
| Meetings | Potluck | `Potluck Template.tawala` | greentea |
| Polls | Multiple Question Survey | `Multiple Question Survey Template.tawala` | default |
| Polls | Simple Survey | `Simple Survey Template.tawala` | style2 |

---

## Structure summary

| Template | Forms | Processes | Documents | Post-submit (process) |
|----------|-------|-----------|-----------|------------------------|
| **Empty** | — | — | — | — |
| **Form with Process** | Form 1 (start) | Process 1 (empty) | — | — |
| **Form + Process + Document** | Form 1 (start) | Process 1 | Document 1 | `Show Document Document 1` |
| **Sign-up Sheet** | Form 1 (start) | — | — | Inline **itemization-table** on form |
| **Sign-up Sheet w Email** | Form 1 (start) | Process 1 | NewSignup | Process + send (see XML) |
| **Simple Survey** | Survey (start), Report (start) | Process 1 | — | `Show form Report` |
| **Multiple Question Survey** | Survey, Report | Process 1 | — | `Show form Report` (multi tally tables) |
| **Get Together** | Survey, Report | Process 1 | — | `Show form Report` + **question-correlation-table** |
| **Potluck** | Potluck Organizer, Report | Send Thanks, Show Results, Delete Name | Details, Thank you ×2 | Pre/post processes, delete, show docs |

---

## Deploy smoke tests (Phase 2)

| Priority | Template | Status | Minimal pass criteria |
|----------|----------|--------|---------------------|
| 1 | **Simple Survey Template** | **Passed** | Survey → Report tally on 8080 |
| 2 | **Signup Sheet Template** | **Passed** (Jul 16 refresh) | Submit → table rows; Form MQL path; FIB Deploy formatting |
| 3 | **Form with process** | **Passed** | Empty Form 1 + Process 1 (Designer demo) |
| 4 | **Form with process connecting a document** | **Unmarked** | Submit → **Document 1** HTML visible |
| 5 | **Sign-up Sheet w Email** | **Unmarked** | FIB submit + table; Send may need mail config |
| 6 | **Get Together** | **Passed w/ caveats** | Correlation data OK; silk icons + table CSS patched |
| 7 | **Multiple Question Survey** | **Passed** (owner Jul 2026 — see reference below) | Multi tally sections + itemization on Report |
| 8 | **Potluck (legacy direct deploy)** | **Owner Passed Jul 20** | `node scripts/deploy-tawala-template.mjs "Potluck"` → full app on 8080: forms, MQL, theme CSS; looked good and worked. Still **not** Browser Designer New Project JSON (that remains a stub). |
| — | **DirtBowl (legacy `.tawala`)** | **Owner Passed Jul 20** | Full project (dozens of Forms/Processes/Documents) via `designer-web/public/samples/legacy/DirtBowl.tawala` — worked flawlessly. Corrupted JSON copies are not a smoke target. |

Update this table when each passes owner click-test. Function-level status: `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § Function status matrix.

---

## Simple Survey (reference)

- **Survey** form: instructional Text + one MCQ (`Q1`), `startPoint`, `process="Process 1"`.
- **Report** form: **choice-tally-table** on `Record:Survey:Q1`; **`startPoint`** so admins can open results directly.
- **Process 1:** `Show form Report`.

**Data lifecycle:** tally counts every Survey submission stored for that deployment. For a fresh poll, deploy under a **new project name**, or use **Project Manager → Purge** (all responses) / **Erase** (per form) on the existing deployment.

Good first deploy target — small XML, no documents, one process statement.

**Deploy (June 2026):** `node scripts/deploy-tawala-template.mjs "Simple Survey Template"`

| Form | URL |
|------|-----|
| Survey | http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey |
| Report | http://localhost:8080/p/gy1zssbrwm4fgfm/d6ceolx.Report |

*(URLs change on redeploy; re-run script for current links.)*

**Owner confirmed:** Survey submit → Report tally works on 8080. Legacy Designer **Deploy** button still targets **www.tawala.com** (unreachable); use the script above for local deploy instead.

---

## Sign-up Sheet (reference)

- **Form 1:** FIB fields (FirstName, LastName, Email, Tel, Address) + **itemization-table** on same form — no separate process.
- Theme: `baseball`, `startPoint` on Form 1.

**Deploy (June 2026):** `node scripts/deploy-tawala-template.mjs "Sign-up Sheet"`

| Form | URL |
|------|-----|
| Form 1 | http://localhost:8080/p/cicw55xxhvwrrh7/l2u4sdg.Form+1 |

**Smoke test:** Fill FIB fields and submit → runtime shows a **Thank You** page. Use the browser **Back** button (or reopen Form 1) to return to the start form; the **itemization-table** lists every submission (two submits → two rows). Table and FIBs are on the same form — there is no separate report form.

**Owner confirmed (July 2026):** Two test submissions; table expanded correctly with all FIB columns. Legacy CSS/theme (`baseball`) not fully present in local stack — acceptable for functional testing.

---

## Form with process (reference)

- **Form 1:** empty form, `startPoint`, `process="Process 1"`.
- **Process 1:** empty (no statements).

**Designer demo only** — teaches linking a form to a process; not a featured sample app.

**Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Form with Process"`

| Form | URL |
|------|-----|
| Form 1 | http://localhost:8080/p/5u8ql7u0nvla3co/3w6olnt.Form+1 |

**Owner confirmed (July 2026):** Empty form loads as expected.

---

## Potluck (reference)

- **Potluck Organizer** (start): FIB + MCQ — RSVP, headcount, dish contribution.
- **Report:** organizer view of responses.
- Processes: Send Thanks, Show Results, Delete Name; documents Details, Thank you.

**Legacy direct Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Potluck"`. This deployed `Potluck Template.tawala` directly to Java. It did **not** test Browser Designer import, Configure, or export; the New Project Potluck JSON is only a starter stub.

**Owner Passed Jul 20 (legacy `.tawala` path):** Deployed Potluck looked good and worked end-to-end, including **MULTIPLE QUESTION LIST** and a **CSS theme**. Confirms Java Deploy + theme for the full template; does **not** upgrade the Browser Designer New Project Potluck stub.

**Browser Designer SUM smoke (owner passed Jul 19):** The separate `public/samples/sum-smoke-test.json` project passed Configure, export, and live Deploy total. This validates Browser Designer SUM independently; it does not make the Potluck starter stub a full template.

| Form | URL |
|------|-----|
| Potluck Organizer | http://localhost:8080/p/t03vtb1poh34kkn/2bpec4j.Potluck+Organizer |
| Report | http://localhost:8080/p/t03vtb1poh34kkn/3i70frf.Report |

**Smoke test:** Submit organizer form → **Details** document (headcount + dish list) via `preProcess`; post-submit **Send Thanks** shows Coming vs NotComing doc from Q2.

**Owner confirmed (July 2026):** Five of six runs correct — totals and name list update. **Caveats:**
1. **First run only:** record did not save; showed **“Sorry you won't be able to attend!”** (NotComing document). Likely Q2 (Yes/No) not evaluated as `a` on that submit — intermittent / cold-start; not reproduced on retries.
2. **Q5 confirm not enforced:** second-page MCQ “Yes, save this information” is optional in template XML — submit saves even if Q5 left blank (only “No, change” skips back to T2).

---

## Get Together (reference)

- **Survey** (start): multi-select dates + single top preference + name FIB.
- **Report:** **question-correlation-table** for date overlap.
- **Process 1:** `Show form Report`.

**Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Get Together"`

| Form | URL |
|------|-----|
| Survey | http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey |
| Report | http://localhost:8080/p/b6do4s50iq64vl8/ejeypox.Report |

**Smoke test:** Submit Survey with date choices → Report shows **question-correlation-table** (who picked which dates; totals with preferred counts in brackets; best date in **bold**).

**Owner confirmed (July 2026):** Doug + Larry submissions; totals and best date (June 10 **2 (1)**) correct.

**Visual assets (July 2026):** Docker image now patches `docker/tomcat/images/silk/` (tick.gif, star.png) and legacy `default.css` + theme CSS (`greentea`, `style2`, `baseball` stub). Rebuild: `docker compose build tawala && docker compose up -d tawala`. The star on the best-option totals cell is intentional.

Prior caveats before patch:
1. Selected vs preferred icons looked identical — missing `/images/silk/`.
2. Erratic table alignment — incomplete `default.css`.
3. “Extra” graphic on Totals row — best-option star (`star.png`); not a layout bug.

---

## Form with process connecting a document (reference)

- **Form 1:** empty form, `startPoint`, `process="Process 1"`.
- **Process 1:** `Show Document Document 1`.
- **Document 1:** sample HTML document.

**Designer demo only** — teaches form → process → document; not a featured sample app.

**Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Form with process connecting a document"`

| Form | URL |
|------|-----|
| Form 1 | http://localhost:8080/p/x6oovxu7g8tjelp/ullhihn.Form+1 |

**Smoke test:** Open Form 1 → submit (no fields) → runtime shows **Document 1**. In the shipped template, Document 1 is intentionally empty (one blank paragraph → `&nbsp;` on the page). That is correct for this Designer pedagogy demo (Form + Process + Document windows), not a user-facing sample app.

**Owner confirmed (July 2026):** Saw **“Processing, please wait…”** modal with a broken image icon. That modal is legacy submit UX (appears if the round-trip takes more than ~5 seconds); the broken icon was a missing `/images/submit-progress.gif` in our Docker image (patched in `docker/tomcat/images/`). After submit, the result page is nearly blank — as expected for an empty document.

---

## Sign-up Sheet w Email (reference)

- Same FIB + **itemization-table** as Sign-up Sheet template.
- **Process 1:** Send email with **NewSignup** document (placeholder recipient in XML).
- Theme: `redrays`.

**Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Sign-up Sheet Template w Email"`

| Form | URL |
|------|-----|
| Form 1 | http://localhost:8080/p/onszvng2ec776jt/uwh7ift.Form+1 |

**Smoke test:** Same as Sign-up Sheet (submit → Thank You → back → table grows). **Send** process enqueues email via `EmailService` → Spring `JavaMailSender` (`mail.host=localhost` by default). Without SMTP, signup may save but notification mail fails.

**Owner confirmed (July 2026):** Does not work end-to-end — no working email on local 8080. Form/table behavior not separately retested. Outbound mail tracked in `docs/ROADMAP.md` **Backlog** (JavaMailSender vs provider — deferred).

---

## Multiple Question Survey (reference)

- **Survey** (start): name FIB + several MCQs + optional “see results?” MCQ.
- **Report:** **choice-tally-table** per MCQ + **itemization-table** of all responses.
- **Process 1:** `Show form Report` when Q6 = Yes.

**Deploy (July 2026):** `node scripts/deploy-tawala-template.mjs "Multiple Question Survey Template"`

| Form | URL |
|------|-----|
| Survey | http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey |
| Report | http://localhost:8080/p/grniytf6dvmobqe/w2licdd.Report |

**Smoke test:** Submit Survey with MCQ answers → Report shows tally sections for Q2, Q3, Q5 and response table with all columns.

**Owner confirmed (July 2026):** Worked perfectly. **Status: Passed** (template Deploy smoke; listed in Phase 2 table).

---

*Last updated: July 2026.*
