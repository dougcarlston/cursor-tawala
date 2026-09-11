# Where we are / where we’re going — Sep 8, 2026 (Library / Samples resume)

**Branch:** `cursor/forms-canvas-wysiwyg`  
**Staging (NOT in git):** `~/Projects/ Current Tawala Projects/Website Staging/`  
**Backup (Sep 8, before this pass):** `~/Projects/ Current Tawala Projects/Website Staging backups/Website-Staging-2026-09-08.zip`

Companions: this file; tray READMEs under `2-Ready-for-MyTawala/` and `3-Ready-for-Public-Library/Samples/`. UniqueId sidebar: `.cursor` canvases `library-project-ids.canvas.tsx` (open beside chat).

**Owner travel:** Fri Sep 11 afternoon + Sat Sep 12. Resume **Monday Sep 14** (California) from the list below.

---

## Monday Sep 14 — ToDo (owner left Fri Sep 11)

Do **not** start Designer C14/C15/F1. Do **not** implement “projects of projects” until the planning session below is done. Isolation restoration stays the product spine.

### A. First — OEB Setup first-pass (small, unfinished smoke)

Setup start should open **SetupVariables** (title / date / Q3) when there are **no Question rows**. Hub “Your exam is now set up!” + Update Setup is **original v.8** once any question exists — not a single-door rewrite.

**Bug:** Library JSON Pre-Setup (Aug 28 `e02b507`) set `FirstViewOfSetup` to two quote characters (`"\"\""`), so `isBlank` always failed and Setup skipped first-pass. Session leftover `FirstViewOfSetup=false` or a Test Drive snapshot of live `/p/` questions does the same.

**Done in JSON (not live until Push):** real empty reset at the top of Pre-Setup, then the original “any Question → hub” test. Files:

- `website-mock/projects/library/Online Exam Builder.json`
- `website-mock/projects/mytawala/Online Exam Builder.json`
- Keeper: `~/Projects/ Current Tawala Projects/Current Deployed MyTawala/Online Exam Builder.json`  
  (Open this. Live My Tawala `un7qtlf0tcc0rqq` / Tomcat name **Online Exam Builder e3c56ca6**. Public Library `u3hkqgwtrepjlur`.)

**Monday smoke:** File → Open keeper → confirm uniqueId `un7q…` → Push → fresh Test Drive / Use **Setup** with **no questions** → SetupVariables. If questions already exist on that uniqueId, hub is correct; use a new TD session or Purge Question rows first. Then Publish/replace Library in place (`u3hk…`) if public TD should match.

Also confirm examinee **Exam** still shows Exam Start (title + Q3) — that Push/Publish already smoked Sep 11 (`Pre-Exam`).

### B. Finish Test Drive isolation restoration

**Wired Sep 11 (in this commit):** Library Test Drive / Copy link → `http://localhost:8080/projectmanager/testdrive?id={uniqueId}&form={Form}` → 302 `/t/{uniqueId}/{token}.{Form}` + `JSESSIONID`. Private in-memory World. Two browsers = two copies. My Tawala **Use** stays `/p/`. Start picker stays open after pick (hop starts, same session). Honesty copy: private try-out, not “same uniqueId for every visitor.”

**Still open (do in order):**

1. **Stop copying live `/p/` into a new TD World** — `TestDriveSupport.copyExistingSubmissionsToTestWorld` still snapshots the public pile at session start. That is why a “fresh” Setup can skip first-pass (questions already there) and why isolation is incomplete. Prefer empty World (or copy **definition only**). Confirm two visitors never see each other’s entries **and** a new drive does not inherit author/library submissions.
2. **Idle TTL / leave-wipe** — today discard = Tomcat session expiry, not listing `onunload`. Product still wants wipe when the visitor **leaves**. Spec TTL; do not fake it in `:5500`.
3. **Smoke dual-use via Library TD** (not only My Tawala Use): Sign-up Customize-first; Shared To-Do Setup; OEB Administration or Setup; Poll only after leak-fix. Default door remains **single start**; picker is admin opt-in. Do not implement two-door (AdminStart vs UserStart) unless asked.
4. **Then Publish dual-use as they already are** — no JSON reshape for a single `/p/` door:
   - Sign-up Sheet operate `gnqp5qfd06ktec3`
   - ALB after **Free a live :8080 name** `ceihmyxlssxn6yn` (do not share `vrjayw9lwun85i1` as public TD)
   - Poll — wait for leak-fix Push `b7ttti35vdo58pu`
5. Sync catalog JSON on disk after each successful in-place Publish so Copy to MyTawala matches `:8080`.

### C. Reversions / alterations tied to isolation (do not undo the sandbox)

| Do | Do not |
|----|--------|
| Keep apps multi-start in JSON; TD isolation is the `/t/` World | Rewrite Sign-up / ALB / GT so Customize is the only start |
| Jump-tracks in **published invite URLs** = definition rule (Poll leak-fix) | Jump-track tricks in Test Drive |
| Copy to MyTawala = empty private clone of the **listing definition** | Same uniqueId as Library TD |
| Publish = empty Library uniqueId (reuse listing id on **replace**) | Put author’s My Tawala data on public TD |
| Start picker = rare opt-in; list stays open while hopping | Close picker on pick (felt like the drive died) |
| | Revive Aug 26 same-uniqueId “keep Question/SetupVariables, purge Exam” |

**Ancillary (same spine):**

- Honesty / tests: `js/testDriveHonesty.test.mjs`, `js/testDriveSandbox.test.mjs` — keep green when touching TD URLs.
- Cache-bust Library scripts after TD/JSON changes.
- Two hard walls still: unregistered visitors never see each other; clients never jump into Administration (invitees on a private My Tawala uniqueId are the exception).
- GT/Potluck Sample JSONs stay parked in `_set-aside/`. Keepers: `Current Deployed MyTawala`.
- Duplicate My Tawala rows with the **same** uniqueId: Delete **one listing** only (overlay). Does not retire `:8080`.
- Designer C14/C15/F1 still parked.

### D. Planning session — “Projects of projects” (do not code yet)

**When:** After isolation **B1** (empty TD World) is real, and OEB first-pass **A** is live. Isolation is a dependency: a Library try-out of a filled exam must be a sandbox with the author’s **questions**, not the author’s live uniqueId and not other visitors’ scores.

**What the product is:** Library holds **tools** (blank Online Exam Builder) **and** **content products** (e.g. “American History Final Exam”) built *inside* OEB, not in Designer. A teacher Publishes a standard exam: keep authoring/subject content, strip student/respondent data. Other teachers Copy to My Tawala and run it with their classes.

**Why start-point names are not enough:** OEB subject matter lives on `Question` and `SetupVariables` (and Customize_* derived from setup). Student data lives on `Exam` / `Answer` (and scores, incomplete tests, email confirmations). Administration is a **door**, not a data class. Aug 26 “keep Question/SetupVariables, purge Exam” on the **same** uniqueId scrambled OEB — do not revive it.

**Scoping questions for the session (coding implications):**

1. **Data roles, not form names** — per-project (or per-template) map: authoring forms vs respondent forms vs derived/admin reports. OEB: keep `Question` + `SetupVariables`; strip `Exam` + `Answer`; decide `ShowExamineeDetail` / score documents (empty after strip). Other builders (Poll, Sign-up) need their own maps — do not hard-code OEB names into Publish.
2. **Identity** — content product is a **new Library uniqueId** (empty of students, full of questions). Never Publish onto the teacher’s My Tawala uniqueId. Duplicate-product refuse (same `jsonFile` as catalog OEB) must gain an exception or a “derived from tool X” flag so American History is not blocked as “the same as Online Exam Builder.”
3. **Authoring vs listing definition** — Copy to MyTawala of a content product should clone **questions + setup**, not a blank tool. Publish from OEB must snapshot those submissions into the Library clone, then wipe respondent rows. Two-step clone (definition XML + selected forms’ submissions) vs one special purge.
4. **Who may Publish content** — listed author of the *tool* vs any teacher who used Copy to MyTawala. Library categories: Tools vs Curriculum / User Contributed.
5. **Test Drive of a filled exam** — sandbox `/t/` with questions present, scores empty, no write-back to `u3hk…` or the teacher’s `un7q…`. Depends on **B1** (do not snapshot *student* rows from live `/p/`; *do* include authoring rows for content products).
6. **Versioning** — teacher updates questions → Publish replace in place (like OEB tool replace on `u3hk…`) vs mint a new exam version. Occupancy: content titles (“American History Final Exam”) must not collide with tool title “Online Exam Builder.”
7. **Out of scope for v1** — nested “projects of projects of projects”; billing; Designer changes to make OEB. Teachers stay in the runtime app.

**Session output:** a one-page role table for OEB (keep/strip/report), identity rules, and a sequenced build list. Code only after that page is agreed.

---

Sep 4 evening close (pre-travel) is preserved below the Sep 8 delta.

---

## Sep 8 — what changed

1. **Backup Staging** — zip written outside git (path above).
2. **Horses and Penguins Test** — copied from Live Library into Samples tray.
3. **Get Together Sample** + **Potluck Sample** — looping demos were built Sep 8; **parked Sep 9** (`_set-aside/`). Owner: those two are not Samples this pass. Full apps restored to multi-start.
4. **Automated List Builder** — private Push identity stamped; public occupant left in place. Option A AfterSave on Setup; Questionnaire + Administration restored as starts. **Owner Push from Designer** (File→Open tray JSON → Push) then TD-smoke Setup.

Did **not** this pass: public Library Push; Designer C14/C15/F1.

**Sep 9 — Library admin Delete (replaces Retire):** listings are removed with no My Tawala copy; the name is free for a later Publish. Occupant uniqueIds (ALB `ceihmyxlssxn6yn`) can be vacated from library-admin **Free a live :8080 name** — no agent hatch. Tomcat still cannot destroy uniqueIds (rename only).

**Sep 9 — Test Drive doors:** Library admin **Test Drive doors** sets per listing **Single door** (default) or **Start picker**. Single door also picks which start Test Drive / Copy link open. Copy to MyTawala still clones every start. Stored in `tawala.mock.libraryTestDriveDoors`. Missing overlay uses the old heuristics (OEB→Administration, email/Shared To-Do→Setup).

**Sep 11 — Start picker stays open:** choosing a start opens the try-out in another tab and leaves the list up so you can hop. Close / Escape still dismisses it. Copy-link still closes after copy.

**Sep 11 — OEB examinee Exam Start:** Exam form preProcess is **Pre-Exam** (copy SetupVariables → Customize_* , then `showDocument Exam Start`). Name page shows title, date, and Q3 pre-instructions. Administration / CustomizationPreview still use SetupCustomizationVariables. Owner: Push from My Tawala, then update public Library.

---

## Exact tray state (Sep 8)

### Samples — `3-Ready-for-Public-Library/Samples/`

| File | Status |
|------|--------|
| `Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Survey) |
| `Multiple Question Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Multiple Question Survey). Drop Report as a **start** (keep Report form; loop still reaches it). TD door = Survey. |
| `Sign-up Sheet Sample.json` | **Live Library Sep 9** |
| `Horses and Penguins Test.json` | **Live Library — leave as-is** (owner Sep 9; do not re-Publish or rewrite the blurb) |
| `Get Together Sample.json` | **Parked Sep 9** → `_set-aside/Get Together Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |
| `Potluck Sample.json` | **Parked Sep 9** → `_set-aside/Potluck Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |

### Smoked real apps — `2-Ready-for-MyTawala/Smoked-Ready-to-Push/`

Public names (drop Sophisticated). Library **Test Drive OK** for these four:

| File | TD door | Notes |
|------|---------|--------|
| `Single Question Poll or Survey.json` | **AdminStart + Start Questionnaire** | Live My Tawala uniqueId `b7ttti35vdo58pu`. **Library TD OK** (AdminStart). **Not Library until owner Pushes the Sep 9 leak fix:** Survey-link respondents get thank-you only (no Administration invitation). Organizer still uses AdminStart / Administration. Email notify still links Administration for the organizer. |
| `Sign-up Sheet.json` | **Customize** (also Questionnaire + Administration) | Keeper uniqueId `gnqp5qfd06ktec3`. Leftover My Tawala row deleted Sep 9. Dual-use like Poll: Customize is the TD hub. |
| `Shared To-Do Test Drive.json` | **Setup** only | Smoke artifact — **not** the public listing. Live Library is the three-start app (`shared-to-do`, uniqueId `bx44wpdnspbsi3k`). Website Test Drive opens Setup only. |
| `Automated List Builder.json` | **Setup** | Typo pass + private Push Sep 8. Display name **Automated List Builder**. Live copy is uniqueId `vrjayw9lwun85i1` (Tomcat still parked as Smoke so we do not overwrite public occupant `ceihmyxlssxn6yn`). Email warning = Library listing honesty, not in-form. |

### Still in prep — `2-Ready-for-MyTawala/`

| File | Status |
|------|--------|
| `Sophisticated Get Together.json` | **Multi-start restored Sep 9:** Customize + Questionnaire + Administration. AfterSave hub removed. **No Library TD / no Sample JSON.** My Tawala only for the full app. File→New Simple Get Together ≠ this file. |
| `Sophisticated Shared To-Do.json` | **Owner smoked + Pushed + Save As Sep 9.** Setup + Signup + Administration. uniqueId `qggyhqoy8m4td23`. TD copy smoke passed. |

Also: older junk at `3-Ready` root (CYO, Dirtbowl Communicator, MVSC, etc.) — **not** this startup-app pass.

---

## Product rules locked this week

| Rule | Meaning |
|------|---------|
| Sample vs Real vs Builder | **Sample** = that is all it is (looping Library demo). **Real** = Customize/Setup/Admin (drop Sophisticated on publish). **Builder** = OEB-class only. Do not call a real/TD-sibling app a Sample. |
| Library-bound | Everything that works as a Test Drive is **on the plan to move to public Library** — Samples, dual-use real apps, Builders, and TD siblings. Operate-only this week (GT / Potluck) still on that plan once a TD door exists. |
| Double smoke | If the Library TD shape is not the same JSON as the My Tawala working copy, smoke **both**: TD door (one in-session hub) **and** the multi-start deploy. Poll / Sign-up = one JSON, both roles. **Shared To-Do Library = one JSON** (three starts); website Test Drive opens Setup only. The Setup-only `Shared To-Do Test Drive.json` is a smoke artifact, not the listing. |
| Test Drive wipe | Leaving a start **wipes** TD data. Multi-start is for **My Tawala Use**, not hopping doors in one TD. |
| Library TD door | One **in-session** hub that can reach the product (Poll AdminStart; Sign-up Customize; Shared To-Do Test Drive Setup). |
| **GT + Potluck** | Full apps = My Tawala now. **Library still on the plan** (TD sibling or looping demo later — parked Sample JSONs are not the operate apps). Not this week's Samples folder. |
| Sign-up Sheet w Email | New Project only — never Library |

### Option A pattern (Sign-up / Potluck / GT Customize·Setup)

After-save MCQ with alternate name `AfterSave`. Condition must be `Form:AfterSave` (not `MCQ1`) — Sign-up first fail was wrong field ref. Choices: try guest path / stay editing. Thin Admin not used as TD destination.

List Builder Sep 8: Setup hub is **MenuChoice** (a = customizer, b = list/sign-up, c = stay). Not AfterSave.

---

## Automated List Builder — identity (Sep 8)

**Still true:** live Tomcat name `Automated List Builder` = uniqueId `ceihmyxlssxn6yn` (Library/public occupant). Do not Push the tray file under that bare name.

**Canonical file (Sep 8, after typo pass):** `2-Ready-for-MyTawala/Smoked-Ready-to-Push/Automated List Builder.json`  
Display name **Automated List Builder**. Same live copy: `deployUniqueId` `vrjayw9lwun85i1`. Tomcat is still parked as Smoke so Push does not overwrite public occupant `ceihmyxlssxn6yn`.  
Setup: `http://localhost:8080/p/vrjayw9lwun85i1/r2s6sqe.Setup`

**Email warning (Sep 8, option 2):** Library listing Test Drive / Copy-link honesty (`tooltipSingleEmail`) + **Sends real email.** badge. In-form `TWarn` **removed**. **Version 2 (B9):** runtime TD flag so a form item can show only during Test Drive.

**Not done:** retire/hatch `ceihmyxlssxn6yn`. Extra JSON copies (Sophisticated / Smoke filenames) **deleted Sep 8** — only `Smoked-Ready-to-Push/Automated List Builder.json` remains. Recovery path is the old `.tawala`, not a JSON twin.

---

## Designer (git) — parked

- Form Text blank/Style — done
- **C14** FIB Above + `*`; **C15** Pre-populate / Block Back; **F1** 3-browser — later
- **Process script opening-quote wrap** (Sep 4 polish) — noted in `DESIGNER_OPEN_TODOS.md` + shot in assets

---

## Next — Sep 11

**Locked (owner Sep 11):** Stop reshaping Library JSON so Test Drive can be a single `/p/{libraryUniqueId}` door. That warps apps (Sign-up TD skipped Customize), risks visitor-vs-visitor data mixing, and risks client/admin jump-tracks. Legacy TD was a **per-drive temporary instantiation** (session world on `/t/…`, not the live `/p/` uniqueId). Restore that isolation **before** more dual-use Publishes. Admin **Start picker** may stay as a rare opt-in; it is not how the catalog stays honest.

Smoke of Customize-first apps via My Tawala **Use** does **not** count as Library TD smoke.

**First group is in Live Library (owner Sep 9):** Survey Sample, Multiple Question Survey Sample, Sign-up Sheet Sample, Horses and Penguins Test. Horses stays as published. **Shared To-Do** is also Live (`bx44wpdnspbsi3k`).

1. **First: Test Drive sandbox** — **wired Sep 11.** Library Test Drive / Copy link → `/projectmanager/testdrive?id=&form=` (session World → `/t/…`). Default door still a single start (Customize/Setup/Admin heuristics; picker opt-in). Do not rewrite Sign-up / ALB / GT JSON for a single `/p/` door. Then Publish dual-use apps **as they already are**.
2. **Then** Publish dual-use apps **as they already are** (Sign-up Sheet operate `gnqp5qfd06ktec3`, ALB after **Free a live :8080 name** `ceihmyxlssxn6yn`, etc.).
3. **Poll** — wait for owner leak-fix Push on `b7ttti35vdo58pu`. Poll v.8 still Defer-rewrite. Jump-tracks in **published invite URLs** stays a definition rule (not a TD trick).
4. **GT + Potluck** — parked Sample JSONs in `_set-aside/`. Not this Samples group.
5. Later: Designer C14/C15/F1. Keepers in `Current Deployed MyTawala`.

### Parked — User Contributed “projects of projects” (owner Sep 11)

A teacher could Publish **American History Final Exam** from OEB: same Designer definition as Online Exam Builder, **keep administrator/authoring data** (questions, setup), **strip student/respondent data**. Library would hold both **tools** (blank OEB) and **content products** (filled OEB instances). Teachers build in OEB, not Designer.

Not this pass. Needs a first-class **authoring vs respondent** data role (start-point names are not enough — OEB questions live on `Question` / `SetupVariables`, not on Administration). Do not revive the Aug 26 same-uniqueId “keep Question / SetupVariables, purge Exam” path — it scrambled OEB. Duplicate-product refuse (same `jsonFile` as catalog OEB) would also have to change for contributed content. Depends on per-drive TD isolation so a Library try-out of a filled exam is a sandbox with questions, not the author’s live uniqueId.

---

## New-chat opener

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md **Monday Sep 14 ToDo**.
Branch cursor/forms-canvas-wysiwyg. Staging outside git: Website Staging.
**First:** smoke OEB Pre-Setup first-pass (keeper JSON, Push un7q…, then Library replace u3hk…).
**Then:** finish TD isolation — stop copying live /p/ into new /t/ Worlds; then dual-use Publishes as they are.
Parked: User Contributed “projects of projects” planning session (section D) — do not code yet.
No Designer C14 this pass.
```

---

## Fri Sep 4 evening close (pre-travel) — archived

**Owner travel:** Saturday → ~1 week. Left mid-smoke Fri Sep 4 ~11:15.

Samples then: Survey / Multi-Q / Sign-up smoked; Horses not copied; GT/Potluck Samples not built.  
List Builder Push blocked on occupant `ceihmyxlssxn6yn` vs tray `6jpbj1hn2yzghh9`.
